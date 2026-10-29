using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UI-based Tower of Hanoi manager. Create three HanoiRod objects and assign them here.
/// Disks should be UI Images with HanoiDisk attached.
/// </summary>
public class HanoiGameManager : MonoBehaviour
{
    [Header("Puzzle")]
    [Min(3)] public int diskCount = 5;
    public HanoiRod[] rods = new HanoiRod[3];
    public HanoiDisk diskPrefab;
    public RectTransform diskParent;
    public float diskHeight = 42f;
    public float smallestDiskWidth = 100f;
    public float diskWidthStep = 28f;
    public float bottomOffset = 24f;

    [Header("UI (optional)")]
    public TMP_Text movesText;
    public TMP_Text statusText;
    public GameObject winPanel;
    public Button restartButton;
    public Button menuButton;
    public string menuSceneName = "StartMenu";

    [Header("Rewarded bonuses (optional)")]
    public Button hintButton;
    public Button undoButton;

    private readonly List<HanoiDisk> disks = new List<HanoiDisk>();
    private readonly Stack<MoveRecord> moveHistory = new Stack<MoveRecord>();
    private int moves;
    private bool gameWon;
    private bool awaitingReward;
    private RewardType pendingReward;

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

    private void Start()
    {
        if (rods == null || rods.Length != 3 || rods[0] == null || rods[1] == null || rods[2] == null)
        {
            Debug.LogError("HanoiGameManager: Assign exactly three HanoiRod objects in the Inspector.");
            enabled = false;
            return;
        }

        foreach (HanoiRod rod in rods)
            rod.Initialize(this);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartPuzzle);
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);
        if (hintButton != null)
            hintButton.onClick.AddListener(RequestRewardedHint);
        if (undoButton != null)
            undoButton.onClick.AddListener(RequestRewardedUndo);

        RestartPuzzle();
    }

    public bool CanPickUp(HanoiDisk disk)
    {
        if (disk == null || gameWon || awaitingReward)
            return false;

        HanoiRod rod = disk.CurrentRod;
        return rod != null && rod.TopDisk == disk;
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
            SetStatus("You cannot place a larger disk on a smaller disk.");
            RestoreDiskPosition(disk);
            return false;
        }

        int fromIndex = System.Array.IndexOf(rods, source);
        int toIndex = System.Array.IndexOf(rods, destination);

        source.RemoveDisk(disk);
        destination.AddDisk(disk);
        moveHistory.Push(new MoveRecord(disk, fromIndex, toIndex));
        moves++;

        RefreshAllRodPositions();
        UpdateMovesUI();
        SetStatus("");

        if (destination == rods[2] && destination.DiskCount == diskCount)
        {
            gameWon = true;
            if (winPanel != null)
                winPanel.SetActive(true);
            SetStatus("Puzzle complete!");
        }

        return true;
    }

    public void RestartPuzzle()
    {
        gameWon = false;
        awaitingReward = false;
        pendingReward = RewardType.None;
        moves = 0;
        moveHistory.Clear();

        if (winPanel != null)
            winPanel.SetActive(false);

        foreach (HanoiRod rod in rods)
            rod.ClearDisks();

        foreach (HanoiDisk disk in disks)
            if (disk != null)
                Destroy(disk.gameObject);
        disks.Clear();

        if (diskPrefab == null || diskParent == null)
        {
            Debug.LogError("HanoiGameManager: Assign Disk Prefab and Disk Parent.");
            return;
        }

        for (int size = diskCount; size >= 1; size--)
        {
            HanoiDisk disk = Instantiate(diskPrefab, diskParent);
            disk.name = "Disk_" + size;
            disk.Initialize(this, size, smallestDiskWidth + (size - 1) * diskWidthStep);
            rods[0].AddDisk(disk);
            disks.Add(disk);
        }

        RefreshAllRodPositions();
        UpdateMovesUI();
        SetStatus("Move all disks to the right-hand rod.");
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void RequestRewardedHint()
    {
        if (gameWon || awaitingReward)
            return;

        RewardedAds ads = AdManager.Instance != null ? AdManager.Instance.rewardedAds : FindFirstObjectByType<RewardedAds>();
        if (ads == null)
        {
            SetStatus("Rewarded ads are not available right now.");
            return;
        }

        awaitingReward = true;
        pendingReward = RewardType.Hint;
// ads.ShowForHanoiReward(GrantRewardAfterAd);
    }

    public void RequestRewardedUndo()
    {
        if (gameWon || awaitingReward || moveHistory.Count == 0)
            return;

        RewardedAds ads = AdManager.Instance != null ? AdManager.Instance.rewardedAds : FindFirstObjectByType<RewardedAds>();
        if (ads == null)
        {
            SetStatus("Rewarded ads are not available right now.");
            return;
        }

        awaitingReward = true;
        pendingReward = RewardType.Undo;
// ads.ShowForHanoiReward(GrantRewardAfterAd);
    }

    // Called by RewardedAds only after Unity Ads reports COMPLETED.
    private void GrantRewardAfterAd(bool completed)
    {
        awaitingReward = false;
        if (!completed)
        {
            pendingReward = RewardType.None;
            SetStatus("Ad wasn't completed. No bonus was granted.");
            return;
        }

        if (pendingReward == RewardType.Hint)
            ShowHint();
        else if (pendingReward == RewardType.Undo)
            UndoLastMove();

        pendingReward = RewardType.None;
    }

    private void ShowHint()
    {
        // For the current legal state, compute the first legal move that advances
        // the canonical recursive solution from rod 0 to rod 2.
        List<int> solution = new List<int>();
        BuildSolution(diskCount, 0, 2, 1, solution);

        foreach (int encoded in solution)
        {
            int from = encoded / 10;
            int to = encoded % 10;
            HanoiDisk top = rods[from].TopDisk;
            if (top != null && (rods[to].TopDisk == null || rods[to].TopDisk.DiskSize > top.DiskSize))
            {
                SetStatus("Hint: move the top disk from rod " + (from + 1) + " to rod " + (to + 1) + ".");
                return;
            }
        }

        SetStatus("Try moving the smallest available top disk.");
    }

    private void BuildSolution(int count, int from, int to, int spare, List<int> output)
    {
        if (count <= 0) return;
        BuildSolution(count - 1, from, spare, to, output);
        output.Add(from * 10 + to);
        BuildSolution(count - 1, spare, to, from, output);
    }

    private void UndoLastMove()
    {
        if (moveHistory.Count == 0)
        {
            SetStatus("There are no moves to undo.");
            return;
        }

        MoveRecord record = moveHistory.Pop();
        HanoiRod current = rods[record.toRod];
        HanoiRod previous = rods[record.fromRod];

        // Undo only if the moved disk is still the top disk on its current rod.
        if (record.disk == null || current.TopDisk != record.disk)
        {
            SetStatus("Undo isn't available for this move anymore.");
            return;
        }

        current.RemoveDisk(record.disk);
        previous.AddDisk(record.disk);
        moves = Mathf.Max(0, moves - 1);
        gameWon = false;

        if (winPanel != null)
            winPanel.SetActive(false);

        RefreshAllRodPositions();
        UpdateMovesUI();
        SetStatus("Last move undone.");
    }

    public void RefreshAllRodPositions()
    {
        foreach (HanoiRod rod in rods)
            rod.LayoutDisks(diskHeight, bottomOffset);
    }

    private void RestoreDiskPosition(HanoiDisk disk)
    {
        if (disk != null && disk.CurrentRod != null)
            disk.CurrentRod.LayoutDisks(diskHeight, bottomOffset);
    }

    private void UpdateMovesUI()
    {
        if (movesText != null)
            movesText.text = "Moves: " + moves + "  |  Minimum: " + (Mathf.Pow(2, diskCount) - 1);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}
