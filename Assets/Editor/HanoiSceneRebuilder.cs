using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public static class HanoiSceneRebuilder
{
    private const string ScenePath = "Assets/Scenes/TowerOfHanoi.unity";
    private const string DiskPrefabPath = "Assets/Prefabs/HanoiDisk.prefab";
    private const string AdsPrefabPath = "Assets/Prefabs/AdsScriptHolder.prefab";
    private const string ArtPath = "Assets/Sprites/Hanoi/";
    private const string SoundPath = "Assets/Sounds/";
    private const string FontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/Chewy SDF.asset";

    private static readonly string[] SkinIds = { "krasainie", "koks", "marmors", "zelts" };
    private static readonly Color DarkPanel = new Color(0f, 0f, 0f, 0.72f);
    private static readonly Color Dim = new Color(0f, 0f, 0f, 0.55f);
    private static readonly Color ButtonText = new Color(0.15f, 0.12f, 0.1f);
    private static readonly Color HelperYellow = new Color(1f, 0.86f, 0.42f);

    private static TMP_FontAsset font;
    private static Sprite rounded;

    [MenuItem("Tools/Tower of Hanoi/Rebuild Scene (Backup Existing)")]
    public static void Rebuild()
    {
        if (!EditorUtility.DisplayDialog(
            "Rebuild Tower of Hanoi scene",
            "This creates a backup of the current TowerOfHanoi scene, then replaces it with a fully wired UI layout. Continue?",
            "Rebuild scene", "Cancel"))
            return;

        if (System.IO.File.Exists(ScenePath))
        {
            string backup = "Assets/Scenes/TowerOfHanoi_Backup_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".unity";
            System.IO.File.Copy(ScenePath, backup, true);
            AssetDatabase.Refresh();
        }

        Build();
        EditorUtility.DisplayDialog("Scene rebuilt", "TowerOfHanoi.unity was rebuilt and saved. A timestamped backup was saved beside it.", "OK");
    }

    /// <summary>Batch mode: -executeMethod HanoiSceneRebuilder.RebuildWithoutBackup (git keeps the history).</summary>
    public static void RebuildWithoutBackup()
    {
        Build();
    }

    private static void Build()
    {
        ImportArt();
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.12f, 0.16f);
        cameraObject.AddComponent<AudioListener>();

        GameObject canvasObject = new GameObject("HanoiCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        // Landscape only, so the height decides the scale: the table and rods
        // always sit at the same height, wider phones just see more lake.
        scaler.matchWidthOrHeight = 1f;

        GameObject es = new GameObject("EventSystem", typeof(EventSystem));
        // Input System UI module is preferred if the project has the new Input System.
        System.Type inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputModuleType != null)
            es.AddComponent(inputModuleType);
        else
            es.AddComponent<StandaloneInputModule>();

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();

        // ---------------- background and board
        Image bg = CreateImage("Background", canvasRect, LoadSprite("hanoi_lake_bg.png"), Vector2.zero, new Vector2(2560, 1080));
        bg.raycastTarget = false;
        bg.preserveAspect = false;
        AspectRatioFitter bgFit = bg.gameObject.AddComponent<AspectRatioFitter>();
        bgFit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        bgFit.aspectRatio = 2560f / 1080f;

        RectTransform board = CreateImage("Board", canvasRect, null, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one).rectTransform;
        Object.DestroyImmediate(board.GetComponent<Image>());

        // The banner ad covers the bottom ~150 units, so the table stands above it.
        Image table = CreateImage("Table", board, LoadSprite("hanoi_table.png"), new Vector2(0, -300), new Vector2(1800, 182));
        table.raycastTarget = false;

        const float restY = -222f;   // top face of the rod bases
        HanoiRod[] rods = new HanoiRod[3];
        string[] rodNames = { "RodLeft", "RodMiddle", "RodRight" };
        float[] x = { -520f, 0f, 520f };
        for (int i = 0; i < 3; i++)
        {
            Image pole = CreateImage(rodNames[i] + "_Pole", board, LoadSprite("hanoi_rod.png"), new Vector2(x[i], restY - 4), new Vector2(30, 300));
            pole.rectTransform.pivot = new Vector2(0.5f, 0f);
            pole.preserveAspect = false;
            pole.raycastTarget = false;

            Image baseImage = CreateImage(rodNames[i] + "_Base", board, LoadSprite("hanoi_rod_base.png"), new Vector2(x[i], restY), new Vector2(400, 92));
            baseImage.rectTransform.pivot = new Vector2(0.5f, 59f / 92f);
            baseImage.raycastTarget = false;
        }

        // Invisible drop targets in front of the rods. Their pivot is where the bottom disk rests.
        for (int i = 0; i < 3; i++)
        {
            Image rodImage = CreateImage(rodNames[i], board, null, new Vector2(x[i], restY), new Vector2(420, 440));
            rodImage.rectTransform.pivot = new Vector2(0.5f, 0f);
            rodImage.color = Color.clear;
            rodImage.raycastTarget = true;
            rods[i] = rodImage.gameObject.AddComponent<HanoiRod>();
        }

        Image hintArrow = CreateImage("HintArrow", board, LoadSprite("hanoi_hint_arrow.png"), Vector2.zero, new Vector2(90, 110));
        hintArrow.rectTransform.pivot = new Vector2(0.5f, 0f);
        hintArrow.raycastTarget = false;

        // ---------------- HUD: [MENU] [HINT] [LEVEL / MOVES] [UNDO] [RESTART]
        Vector2 top = new Vector2(0.5f, 1f);
        Button menuButton = CreateButton(canvasRect, "MenuButton", "IZVELNE", new Vector2(140, -62), new Vector2(220, 84), new Vector2(0f, 1f));
        Button restartButton = CreateButton(canvasRect, "RestartButton", "NO JAUNA", new Vector2(-140, -62), new Vector2(210, 84), new Vector2(1f, 1f));

        Button hintButton = CreateButton(canvasRect, "HintButton", "PADOMS", new Vector2(-350, -66), new Vector2(220, 100), top);
        hintButton.GetComponent<Image>().color = HelperYellow;
        TMP_Text hintLabel = hintButton.GetComponentInChildren<TMP_Text>();
        Button undoButton = CreateButton(canvasRect, "UndoButton", "ATSAUKT", new Vector2(350, -66), new Vector2(220, 100), top);
        undoButton.GetComponent<Image>().color = HelperYellow;
        TMP_Text undoLabel = undoButton.GetComponentInChildren<TMP_Text>();

        Image info = CreateImage("InfoPanel", canvasRect, rounded, new Vector2(0, -72), new Vector2(440, 124), top, top);
        info.type = Image.Type.Sliced;
        info.color = DarkPanel;
        info.raycastTarget = false;
        TMP_Text levelText = CreateText("LevelText", info.rectTransform, "1. LIMENIS", new Vector2(0, 26), new Vector2(420, 60), 46, FontStyles.Normal, TextAlignmentOptions.Center);
        TMP_Text movesText = CreateText("MovesText", info.rectTransform, "Gajieni: 0 / 7", new Vector2(0, -30), new Vector2(420, 50), 34, FontStyles.Normal, TextAlignmentOptions.Center);
        movesText.color = HelperYellow;

        Image statusBox = CreateImage("StatusBox", canvasRect, rounded, new Vector2(0, -180), new Vector2(1000, 64), top, top);
        statusBox.type = Image.Type.Sliced;
        statusBox.color = new Color(0f, 0f, 0f, 0.5f);
        statusBox.raycastTarget = false;
        TMP_Text statusText = CreateText("StatusText", statusBox.rectTransform, "", Vector2.zero, new Vector2(980, 60), 34, FontStyles.Normal, TextAlignmentOptions.Center);
        statusBox.gameObject.SetActive(false);

        // ---------------- level select
        Sprite star = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/star.png");
        Image levelPanel = CreateImage("LevelPanel", canvasRect, null, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
        levelPanel.color = Dim;
        CreateText("Title", levelPanel.rectTransform, "IZVELIES LIMENI", new Vector2(0, 330), new Vector2(1000, 110), 84, FontStyles.Normal, TextAlignmentOptions.Center);

        HanoiGameManager.LevelButton[] levelButtons = new HanoiGameManager.LevelButton[3];
        int[] diskCounts = { 3, 4, 5 };
        for (int i = 0; i < 3; i++)
        {
            Button card = CreateButton(levelPanel.rectTransform, "Level" + (i + 1), "", new Vector2((i - 1) * 380, 20), new Vector2(330, 400), new Vector2(0.5f, 0.5f));
            Object.DestroyImmediate(card.GetComponentInChildren<TMP_Text>().gameObject);
            RectTransform cardRect = card.GetComponent<RectTransform>();
            TMP_Text name = CreateText("Name", cardRect, (i + 1) + ". LIMENIS", new Vector2(0, 150), new Vector2(310, 70), 52, FontStyles.Normal, TextAlignmentOptions.Center);
            name.color = ButtonText;
            TMP_Text count = CreateText("Disks", cardRect, diskCounts[i] + " diski", new Vector2(0, 98), new Vector2(310, 50), 34, FontStyles.Normal, TextAlignmentOptions.Center);
            count.color = new Color(0.4f, 0.36f, 0.32f);
            Image preview = CreateImage("Preview", cardRect, LoadSprite("Disks/krasainie_preview.png"), new Vector2(0, -8), new Vector2(270, 180));
            preview.raycastTarget = false;

            Image[] stars = new Image[3];
            for (int s = 0; s < 3; s++)
            {
                stars[s] = CreateImage("Star" + (s + 1), cardRect, star, new Vector2((s - 1) * 92, -140), new Vector2(84, 63));
                stars[s].raycastTarget = false;
            }

            Image lockImage = CreateImage("Lock", cardRect, null, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
            lockImage.color = new Color(0f, 0f, 0f, 0.55f);
            lockImage.raycastTarget = false;
            Image lockIcon = CreateImage("Icon", lockImage.rectTransform, LoadSprite("lock.png"), Vector2.zero, new Vector2(130, 130));
            lockIcon.raycastTarget = false;

            levelButtons[i] = new HanoiGameManager.LevelButton { button = card, lockIcon = lockImage.gameObject, stars = stars };
        }
        Button levelMenuButton = CreateButton(levelPanel.rectTransform, "MenuButton", "IZVELNE", new Vector2(0, -305), new Vector2(300, 84), new Vector2(0.5f, 0.5f));

        // ---------------- win panel (same look as CityScene's end panel)
        Image winPanel = CreateImage("WinPanel", canvasRect, null, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
        winPanel.color = Dim;
        Image winCard = CreateImage("Card", winPanel.rectTransform, rounded, new Vector2(0, 30), new Vector2(1100, 640));
        winCard.type = Image.Type.Sliced;
        winCard.color = DarkPanel;
        RectTransform winRect = winCard.rectTransform;

        CreateText("WinText", winRect, "UZVARA!", new Vector2(0, 240), new Vector2(900, 120), 100, FontStyles.Normal, TextAlignmentOptions.Center);
        Image[] winStars = new Image[3];
        for (int s = 0; s < 3; s++)
        {
            winStars[s] = CreateImage("Star" + (s + 1), winRect, star, new Vector2((s - 1) * 200, 100), new Vector2(170, 127));
            winStars[s].raycastTarget = false;
        }
        TMP_Text winMoves = CreateText("MovesText", winRect, "Gajieni: 7   (minimums 7)", new Vector2(0, -15), new Vector2(900, 60), 42, FontStyles.Normal, TextAlignmentOptions.Center);

        Image earnedIcon = CreateImage("EarnedIcon", winRect, star, new Vector2(-170, -88), new Vector2(80, 60));
        earnedIcon.raycastTarget = false;
        TMP_Text earned = CreateText("Earned", winRect, "+3", new Vector2(-70, -88), new Vector2(120, 70), 56, FontStyles.Normal, TextAlignmentOptions.Left);
        earned.color = HelperYellow;
        TMP_Text total = CreateText("Total", winRect, "kopa: 0", new Vector2(120, -88), new Vector2(260, 60), 38, FontStyles.Normal, TextAlignmentOptions.Left);
        StarBalanceLabel totalLabel = total.gameObject.AddComponent<StarBalanceLabel>();
        totalLabel.label = total;
        totalLabel.format = "kopa: {0}";

        Vector2 middle = new Vector2(0.5f, 0.5f);
        Button nextButton = CreateButton(winRect, "NextLevelButton", "NAKAMAIS LIMENIS", new Vector2(-355, -215), new Vector2(330, 90), middle);
        nextButton.GetComponent<Image>().color = HelperYellow;
        Button againButton = CreateButton(winRect, "PlayAgainButton", "VELREIZ", new Vector2(-70, -215), new Vector2(200, 90), middle);
        Button levelsButton = CreateButton(winRect, "LevelsButton", "LIMENI", new Vector2(150, -215), new Vector2(200, 90), middle);
        Button winMenuButton = CreateButton(winRect, "MenuButton", "IZVELNE", new Vector2(370, -215), new Vector2(200, 90), middle);

        // ---------------- disk prefab: plain image; sprite and size come from the skin
        GameObject prefabObject = PrefabUtility.LoadPrefabContents(DiskPrefabPath);
        Image diskImage = prefabObject.GetComponent<Image>();
        if (diskImage == null) diskImage = prefabObject.AddComponent<Image>();
        if (prefabObject.GetComponent<CanvasGroup>() == null) prefabObject.AddComponent<CanvasGroup>();
        if (prefabObject.GetComponent<HanoiDisk>() == null) prefabObject.AddComponent<HanoiDisk>();
        diskImage.sprite = LoadSprite("Disks/krasainie_3.png");
        diskImage.preserveAspect = false;
        diskImage.type = Image.Type.Simple;
        PrefabUtility.SaveAsPrefabAsset(prefabObject, DiskPrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabObject);

        HanoiDisk diskPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DiskPrefabPath).GetComponent<HanoiDisk>();

        // ---------------- manager
        GameObject managerObject = new GameObject("HanoiGameManager");
        HanoiGameManager manager = managerObject.AddComponent<HanoiGameManager>();
        manager.diskCount = 3;
        manager.rods = rods;
        manager.diskPrefab = diskPrefab;
        manager.diskParent = board;
        manager.diskHeight = 40f;
        manager.bottomOffset = 2f;
        manager.levelDiskCounts = diskCounts;
        manager.levelPanel = levelPanel.gameObject;
        manager.levelButtons = levelButtons;
        manager.levelText = levelText;
        manager.movesText = movesText;
        manager.statusText = statusText;
        manager.statusBox = statusBox.gameObject;
        manager.winPanel = winPanel.gameObject;
        manager.winStars = winStars;
        manager.winMovesText = winMoves;
        manager.winEarnedText = earned;
        manager.nextLevelButton = nextButton;
        manager.restartButton = restartButton;
        manager.menuButton = menuButton;
        manager.menuSceneName = "StartMenu";
        manager.hintButton = hintButton;
        manager.undoButton = undoButton;
        manager.hintLabel = hintLabel;
        manager.undoLabel = undoLabel;
        manager.hintArrow = hintArrow.rectTransform;

        manager.diskSkins = new HanoiGameManager.DiskSkin[SkinIds.Length];
        for (int s = 0; s < SkinIds.Length; s++)
        {
            Sprite[] sprites = new Sprite[HanoiGameManager.MAX_DISKS];
            for (int size = 1; size <= sprites.Length; size++)
                sprites[size - 1] = LoadSprite("Disks/" + SkinIds[s] + "_" + size + ".png");
            manager.diskSkins[s] = new HanoiGameManager.DiskSkin { id = SkinIds[s], sprites = sprites, tintBySize = s == 0 };
        }

        // Music and sound effects
        AudioSource music = managerObject.AddComponent<AudioSource>();
        music.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(SoundPath + "theTower.mp3");
        music.loop = true;
        music.playOnAwake = true;
        music.volume = 0.35f;
        AudioSource effects = managerObject.AddComponent<AudioSource>();
        effects.playOnAwake = false;
        manager.audioSource = effects;
        manager.pickUpSound = AssetDatabase.LoadAssetAtPath<AudioClip>(SoundPath + "click.wav");
        manager.dropSound = AssetDatabase.LoadAssetAtPath<AudioClip>(SoundPath + "drop.wav");
        manager.wrongSound = AssetDatabase.LoadAssetAtPath<AudioClip>(SoundPath + "wrong-buzzer-sound-effect_fGbnqNlA (mp3cut.net).mp3");
        manager.winSound = AssetDatabase.LoadAssetAtPath<AudioClip>(SoundPath + "victory.mp3");

        // HanoiGameManager.Start() wires restart/menu/hint/undo buttons; the rest is saved with the scene.
        for (int i = 0; i < 3; i++)
            UnityEventTools.AddIntPersistentListener(levelButtons[i].button.onClick, manager.StartLevel, i + 1);
        UnityEventTools.AddPersistentListener(levelMenuButton.onClick, manager.ReturnToMenu);
        UnityEventTools.AddPersistentListener(nextButton.onClick, manager.NextLevel);
        UnityEventTools.AddPersistentListener(againButton.onClick, manager.RestartPuzzle);
        UnityEventTools.AddPersistentListener(levelsButton.onClick, manager.ShowLevelSelect);
        UnityEventTools.AddPersistentListener(winMenuButton.onClick, manager.ReturnToMenu);

        winPanel.gameObject.SetActive(false);
        hintArrow.gameObject.SetActive(false);

        // Ads live in a prefab that is in every scene (see AdManager).
        GameObject adsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AdsPrefabPath);
        if (adsPrefab != null)
            PrefabUtility.InstantiatePrefab(adsPrefab, scene);
        else
            Debug.LogWarning("AdsScriptHolder prefab not found: run Tools > Kur mans vags > Setup Menu and Ads first.");

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log("Tower of Hanoi scene rebuilt at " + ScenePath);
    }

    private static void ImportArt()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites/Hanoi" }))
            EnsureSpriteImport(AssetDatabase.GUIDToAssetPath(guid));
    }

    private static Sprite LoadSprite(string file)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + file);
        if (sprite == null)
            Debug.LogError("Sprite not found: " + ArtPath + file);
        return sprite;
    }

    public static void EnsureSpriteImport(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        if (importer.textureType != TextureImporterType.Sprite || importer.mipmapEnabled)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 4096;
            importer.SaveAndReimport();
        }
    }

    private static Image CreateImage(string name, RectTransform parent, Sprite sprite, Vector2 position, Vector2 size)
    {
        return CreateImage(name, parent, sprite, position, size, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
    }

    private static Image CreateImage(string name, RectTransform parent, Sprite sprite, Vector2 position, Vector2 size, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = sprite != null;
        return image;
    }

    private static TMP_Text CreateText(string name, RectTransform parent, string value, Vector2 position, Vector2 size, float fontSize, FontStyles style, TextAlignmentOptions alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.enableAutoSizing = true;
        text.fontSizeMax = fontSize;
        text.fontSizeMin = fontSize * 0.5f;
        text.raycastTarget = false;
        text.font = font != null ? font : TMP_Settings.defaultFontAsset;
        return text;
    }

    // White button with dark Chewy text, like the buttons in CityScene.
    private static Button CreateButton(RectTransform parent, string name, string label, Vector2 position, Vector2 size, Vector2 anchor)
    {
        Image image = CreateImage(name, parent, rounded, position, size, anchor, anchor);
        image.type = Image.Type.Sliced;
        image.color = Color.white;
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        image.gameObject.AddComponent<ButtonHoverAnimation>();

        TMP_Text text = CreateText(name + "_Label", image.rectTransform, label, Vector2.zero, size - new Vector2(16, 8), 40, FontStyles.Normal, TextAlignmentOptions.Center);
        text.color = ButtonText;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.lineSpacing = -20f;
        return button;
    }
}
