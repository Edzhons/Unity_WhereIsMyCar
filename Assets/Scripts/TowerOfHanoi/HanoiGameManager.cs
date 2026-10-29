using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UI-based Tower of Hanoi manager. Create three HanoiRod objects and assign them here.
/// Disks should be UI Images with HanoiDisk attached.
/// Three levels (3, 4 and 5 disks), unlocked one after another. Stars depend
/// on the number of moves. Hint and undo cost a token from the shop or a
/// rewarded ad.
/// </summary>
public class HanoiGameManager : MonoBehaviour
{
    public const int MAX_DISKS = 5;

    [System.Serializable]
    public class LevelButton
    {
        public Button button;
        public GameObject lockIcon;
        public Image[] stars = new Image[3];
    }

    [System.Serializable]
    public class DiskSkin
    {
        public string id;
        [Tooltip("Disk sprites, smallest first (5).")]
        public Sprite[] sprites;
        [Tooltip("Colour each size differently (the default skin).")]
        public bool tintBySize;
    }

    [Header("Puzzle")]
    [Min(3)] public int diskCount = 5;
    public HanoiRod[] rods = new HanoiRod[3];
    public HanoiDisk diskPrefab;
    public RectTransform diskParent;
    [Tooltip("Thickness of a disk: how far apart stacked disks are.")]
    public float diskHeight = 40f;
    public float bottomOffset = 0f;

    [Header("Levels")]
    public int[] levelDiskCounts = { 3, 4, 5 };
    public GameObject levelPanel;
    public LevelButton[] levelButtons;

    [Header("Disk skins (bought in the shop)")]
    public DiskSkin[] diskSkins;

    [Header("UI (optional)")]
    public TMP_Text levelText;
    public TMP_Text movesText;
    public TMP_Text statusText;
    [Tooltip("Backdrop of the status text, hidden while there is no message.")]
    public GameObject statusBox;
    public GameObject winPanel;
    public Image[] winStars = new Image[3];
    public TMP_Text winMovesText;
    public TMP_Text winEarnedText;
    public Button nextLevelButton;
    public Button restartButton;
    public Button menuButton;
    public string menuSceneName = "StartMenu";

    [Header("Rewarded bonuses (optional)")]
    public Button hintButton;
    public Button undoButton;
    public TMP_Text hintLabel;
    public TMP_Text undoLabel;
    public RectTransform hintArrow;
    [Tooltip("How high above a rod's base the hint arrow points down.")]
    public float hintArrowHeight = 320f;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip pickUpSound;
    public AudioClip dropSound;
    public AudioClip wrongSound;
    public AudioClip winSound;

    private const string UNLOCKED_KEY = "Hanoi_UnlockedLevel";
    private const string BEST_STARS_KEY = "Hanoi_BestStars_";
    private static readonly Color starOff = new Color(0.25f, 0.25f, 0.25f, 0.45f);

    private readonly List<HanoiDisk> disks = new List<HanoiDisk>();
    private readonly Stack<MoveRecord> moveHistory = new Stack<MoveRecord>();
    private int currentLevel = 1;
    private int moves;
    private bool gameWon;
    private bool playing;
    private bool awaitingReward;
    private RewardType pendingReward;
    private float hintArrowTime;
    private int hintTargetRod = -1;

    private enum RewardType { None, Hint, Undo }

    private struct MoveRecord
    {
        public HanoiDisk disk;
        public int fromRod;
        public int toRod;

        public MoveRecord(HanoiDisk disk, int fromRod, int toRod)
        {
            this.disk = disk;
            this.fromRod = fromRod;
            this.toRod = toRod;
        }
    }

    private int MinimumMoves => (1 << diskCount) - 1;

    public int UnlockedLevel
    {
        get { return Mathf.Clamp(PlayerPrefs.GetInt(UNLOCKED_KEY, 1), 1, levelDiskCounts.Length); }
        private set { PlayerPrefs.SetInt(UNLOCKED_KEY, value); PlayerPrefs.Save(); }
    }

    private void Start()
    {
        if (rods == null || rods.Length != 3 || rods[0] == null || rods[1] == null || rods[2] == null)
        {
            Debug.LogError("HanoiGameManager: Assign exactly three HanoiRod objects in the Inspector.");
            enabled = false;
            return;
        }

        // The puzzle is laid out for landscape.
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Time.timeScale = 1f;

        InitializeRods();

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartPuzzle);
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);
        if (hintButton != null)
            hintButton.onClick.AddListener(RequestRewardedHint);
        if (undoButton != null)
            undoButton.onClick.AddListener(RequestRewardedUndo);

        if (winPanel != null)
            winPanel.SetActive(false);
        if (hintArrow != null)
            hintArrow.gameObject.SetActive(false);

        ShowLevelSelect();
    }

    private void OnDestroy()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    private void Update()
    {
        // TEST KEY - instantly win the level
        if (Input.GetKeyDown(KeyCode.F1))
            ForceWin();

        StarBank.CheckCheatKey();

        UpdateHelperButtons();
        AnimateHintArrow();
    }

    public void InitializeRods()
    {
        foreach (HanoiRod rod in rods)
            rod.Initialize(this);
    }

    // =========================
    // LEVELS
    // =========================

    public void ShowLevelSelect()
    {
        playing = false;
        HideHint();
        SetStatus("");

        if (winPanel != null)
            winPanel.SetActive(false);

        int unlocked = UnlockedLevel;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool open = i + 1 <= unlocked;
            levelButtons[i].button.interactable = open;
            levelButtons[i].lockIcon.SetActive(!open);
            SetStars(levelButtons[i].stars, PlayerPrefs.GetInt(BEST_STARS_KEY + (i + 1), 0));
        }

        if (levelPanel != null)
            levelPanel.SetActive(true);
    }

    public void StartLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, levelDiskCounts.Length);
        diskCount = levelDiskCounts[currentLevel - 1];

        if (levelPanel != null)
            levelPanel.SetActive(false);

        RestartPuzzle();
    }

    public void NextLevel()
    {
        StartLevel(currentLevel + 1);
    }

    // =========================
    // MOVES
    // =========================

    public bool CanPickUp(HanoiDisk disk)
    {
        if (disk == null || !playing || gameWon || awaitingReward)
            return false;

        HanoiRod rod = disk.CurrentRod;
        return rod != null && rod.TopDisk == disk;
    }

    public void OnDiskPickedUp()
    {
        PlaySound(pickUpSound);
    }

    public void OnDiskLanded()
    {
        PlaySound(dropSound);
    }

    public bool TryMove(HanoiDisk disk, HanoiRod destination)
    {
        if (disk == null || destination == null || gameWon || awaitingReward)
            return false;

        HanoiRod source = disk.CurrentRod;
        if (source == null || source == destination || source.TopDisk != disk)
        {
            RestoreDiskPosition(disk);
            return false;
        }

        HanoiDisk targetTop = destination.TopDisk;
        if (targetTop != null && targetTop.DiskSize < disk.DiskSize)
        {
            SetStatus("Lielaku disku nevar likt uz mazaka!");
            PlaySound(wrongSound);
            RestoreDiskPosition(disk);
            return false;
        }

        int fromIndex = System.Array.IndexOf(rods, source);
        int toIndex = System.Array.IndexOf(rods, destination);

        source.RemoveDisk(disk);
        destination.AddDisk(disk);
        moveHistory.Push(new MoveRecord(disk, fromIndex, toIndex));
        moves++;

        HideHint();
        RefreshAllRodPositions();
        UpdateMovesUI();
        SetStatus("");

        if (destination == rods[2] && destination.DiskCount == diskCount)
            WinPuzzle();

        return true;
    }

    public void RestartPuzzle()
    {
        gameWon = false;
        playing = true;
        awaitingReward = false;
        pendingReward = RewardType.None;
        moves = 0;
        moveHistory.Clear();
        HideHint();

        if (winPanel != null)
            winPanel.SetActive(false);

        foreach (HanoiRod rod in rods)
            rod.ClearDisks();

        foreach (HanoiDisk disk in disks)
        {
            if (disk == null)
                continue;
            if (Application.isPlaying)
                Destroy(disk.gameObject);
            else
                DestroyImmediate(disk.gameObject);
        }
        disks.Clear();

        if (diskPrefab == null || diskParent == null)
        {
            Debug.LogError("HanoiGameManager: Assign Disk Prefab and Disk Parent.");
            return;
        }

        DiskSkin skin = CurrentSkin();
        for (int size = diskCount; size >= 1; size--)
        {
            HanoiDisk disk = Instantiate(diskPrefab, diskParent);
            disk.name = "Disk_" + size;
            disk.Initialize(this, size, skin.sprites[size - 1], skin.tintBySize);
            rods[0].AddDisk(disk);
            disks.Add(disk);
        }

        RefreshAllRodPositions(false);
        UpdateMovesUI();
        if (levelText != null)
            levelText.text = currentLevel + ". LIMENIS";
        SetStatus("Parvieto visu torni uz labo stieni!");
    }

    DiskSkin CurrentSkin()
    {
        string selected = StarBank.DiskSkin;
        foreach (DiskSkin skin in diskSkins)
        {
            if (skin.id == selected)
                return skin;
        }
        return diskSkins[0];
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    // =========================
    // WIN
    // =========================

    void ForceWin()
    {
        if (!playing || gameWon)
            return;

        Debug.Log("TEST: Force Win");

        // Whole tower onto the right rod, as if solved perfectly.
        disks.Sort((a, b) => b.DiskSize.CompareTo(a.DiskSize));
        foreach (HanoiRod rod in rods)
            rod.ClearDisks();
        foreach (HanoiDisk disk in disks)
            rods[2].AddDisk(disk);
        moves = MinimumMoves;
        moveHistory.Clear();

        RefreshAllRodPositions();
        UpdateMovesUI();
        WinPuzzle();
    }

    void WinPuzzle()
    {
        gameWon = true;
        playing = false;
        HideHint();

        int stars = GetStarCount();
        string bestKey = BEST_STARS_KEY + currentLevel;
        if (stars > PlayerPrefs.GetInt(bestKey, 0))
            PlayerPrefs.SetInt(bestKey, stars);
        if (currentLevel < levelDiskCounts.Length && UnlockedLevel <= currentLevel)
            UnlockedLevel = currentLevel + 1;
        PlayerPrefs.Save();

        StarBank.Deposit(stars);

        SetStars(winStars, stars);
        if (winMovesText != null)
            winMovesText.text = "Gajieni: " + moves + "   (minimums " + MinimumMoves + ")";
        if (winEarnedText != null)
            winEarnedText.text = "+" + stars;
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(currentLevel < levelDiskCounts.Length);

        if (winPanel != null)
            winPanel.SetActive(true);
        SetStatus("");
        PlaySound(winSound);
    }

    // 3 stars for the minimum number of moves, 2 for up to half again as many.
    int GetStarCount()
    {
        if (moves <= MinimumMoves)
            return 3;

        if (moves <= Mathf.FloorToInt(MinimumMoves * 1.5f))
            return 2;

        return 1;
    }

    void SetStars(Image[] stars, int count)
    {
        for (int i = 0; i < stars.Length; i++)
            stars[i].color = i < count ? Color.white : starOff;
    }

    // =========================
    // HINT AND UNDO (token or rewarded ad)
    // =========================

    public void RequestRewardedHint()
    {
        if (!playing || gameWon || awaitingReward)
            return;

        if (StarBank.UseToken(StarBank.HINT_TOKENS))
        {
            ShowHint();
            return;
        }

        RequestAd(RewardType.Hint);
    }

    public void RequestRewardedUndo()
    {
        if (!playing || gameWon || awaitingReward || moveHistory.Count == 0)
            return;

        if (StarBank.UseToken(StarBank.UNDO_TOKENS))
        {
            UndoLastMove();
            return;
        }

        RequestAd(RewardType.Undo);
    }

    void RequestAd(RewardType reward)
    {
        RewardedAds ads = AdManager.Instance != null ? AdManager.Instance.rewardedAds : null;
        if (ads == null || !ads.IsReady)
        {
            SetStatus("Reklama vel nav gatava. Megini velreiz!");
            return;
        }

        awaitingReward = true;
        pendingReward = reward;
        if (!ads.ShowForHanoiReward(GrantRewardAfterAd))
            awaitingReward = false;
    }

    // Called by RewardedAds only after Unity Ads reports COMPLETED.
    private void GrantRewardAfterAd(bool completed)
    {
        awaitingReward = false;
        if (!completed)
        {
            pendingReward = RewardType.None;
            SetStatus("Reklama netika noskatita lidz galam.");
            return;
        }

        if (pendingReward == RewardType.Hint)
            ShowHint();
        else if (pendingReward == RewardType.Undo)
            UndoLastMove();

        pendingReward = RewardType.None;
    }

    void UpdateHelperButtons()
    {
        bool adReady = AdManager.Instance != null && AdManager.Instance.rewardedAds.IsReady;
        bool canUse = playing && !gameWon && !awaitingReward;

        int hints = StarBank.Tokens(StarBank.HINT_TOKENS);
        if (hintButton != null)
            hintButton.interactable = canUse && (hints > 0 || adReady);
        if (hintLabel != null)
            hintLabel.text = "PADOMS\n<size=65%>" + (hints > 0 ? "x" + hints : "REKLAMA") + "</size>";

        int undos = StarBank.Tokens(StarBank.UNDO_TOKENS);
        if (undoButton != null)
            undoButton.interactable = canUse && moveHistory.Count > 0 && (undos > 0 || adReady);
        if (undoLabel != null)
            undoLabel.text = "ATSAUKT\n<size=65%>" + (undos > 0 ? "x" + undos : "REKLAMA") + "</size>";
    }

    private void ShowHint()
    {
        int from, to;
        if (!FindNextMove(out from, out to))
        {
            SetStatus("Tornis jau ir galamerki!");
            return;
        }

        SetStatus("Padoms: parvieto disku no " + (from + 1) + ". uz " + (to + 1) + ". stieni");
        rods[from].TopDisk.Flash(3f);
        hintTargetRod = to;
        hintArrowTime = 4f;
        if (hintArrow != null)
            hintArrow.gameObject.SetActive(true);
    }

    // The best next move from any position (not only from the start): to get
    // disks 1..n onto a rod, the biggest one that is elsewhere must move there,
    // after all the smaller ones are out of its way.
    bool FindNextMove(out int from, out int to)
    {
        int[] rodOf = new int[diskCount + 1];
        for (int r = 0; r < rods.Length; r++)
        {
            foreach (HanoiDisk disk in disks)
            {
                if (disk.CurrentRod == rods[r])
                    rodOf[disk.DiskSize] = r;
            }
        }

        from = to = -1;
        int target = 2;
        for (int size = diskCount; size >= 1; size--)
        {
            if (rodOf[size] == target)
                continue;

            from = rodOf[size];
            to = target;
            target = 3 - rodOf[size] - target;   // smaller disks go to the spare rod
        }
        return from >= 0;
    }

    void AnimateHintArrow()
    {
        if (hintArrow == null || hintTargetRod < 0)
            return;

        hintArrowTime -= Time.unscaledDeltaTime;
        if (hintArrowTime <= 0f)
        {
            HideHint();
            return;
        }

        RectTransform rod = (RectTransform)rods[hintTargetRod].transform;
        float bob = Mathf.Abs(Mathf.Sin(Time.unscaledTime * 5f)) * 25f;
        hintArrow.anchoredPosition = rod.anchoredPosition + new Vector2(0f, hintArrowHeight + bob);
    }

    void HideHint()
    {
        hintTargetRod = -1;
        if (hintArrow != null)
            hintArrow.gameObject.SetActive(false);
        foreach (HanoiDisk disk in disks)
        {
            if (disk != null)
                disk.StopFlash();
        }
    }

    private void UndoLastMove()
    {
        if (moveHistory.Count == 0)
        {
            SetStatus("Nav ko atsaukt.");
            return;
        }

        MoveRecord record = moveHistory.Pop();
        HanoiRod current = rods[record.toRod];
        HanoiRod previous = rods[record.fromRod];

        // Undo only if the moved disk is still the top disk on its current rod.
        if (record.disk == null || current.TopDisk != record.disk)
        {
            SetStatus("So gajienu vairs nevar atsaukt.");
            return;
        }

        current.RemoveDisk(record.disk);
        previous.AddDisk(record.disk);
        moves = Mathf.Max(0, moves - 1);
        gameWon = false;

        HideHint();
        RefreshAllRodPositions();
        UpdateMovesUI();
        SetStatus("Pedejais gajiens atsaukts.");
    }

    // =========================
    // HELPERS
    // =========================

    public void RefreshAllRodPositions()
    {
        RefreshAllRodPositions(true);
    }

    public void RefreshAllRodPositions(bool animate)
    {
        foreach (HanoiRod rod in rods)
            rod.LayoutDisks(diskHeight, bottomOffset, animate);
    }

    private void RestoreDiskPosition(HanoiDisk disk)
    {
        if (disk != null && disk.CurrentRod != null)
            disk.CurrentRod.LayoutDisks(diskHeight, bottomOffset, true);
    }

    private void UpdateMovesUI()
    {
        if (movesText != null)
            movesText.text = "Gajieni: " + moves + " / " + MinimumMoves;
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        if (statusBox != null)
            statusBox.SetActive(!string.IsNullOrEmpty(message));
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
