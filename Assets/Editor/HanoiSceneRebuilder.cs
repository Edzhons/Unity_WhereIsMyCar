using UnityEditor;
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
    private const string BackgroundPath = "Assets/Sprites/Lakeside Tower Puzzle UI Scene.png";
    private const string DiskSpritePath = "Assets/Sprites/Polished Wooden Tower Disk.png";

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

        // Ensure the generated disk sprite is imported as a UI sprite.
        EnsureSpriteImport(DiskSpritePath);
        EnsureSpriteImport(BackgroundPath);

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
        scaler.matchWidthOrHeight = 0.5f;

        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(EventSystem));
            // Input System UI module is preferred if the project has the new Input System.
            System.Type inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputModuleType != null)
                es.AddComponent(inputModuleType);
            else
                es.AddComponent<StandaloneInputModule>();
        }

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();

        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        if (backgroundSprite != null)
        {
            Image bg = CreateImage("Background", canvasRect, backgroundSprite, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
            bg.raycastTarget = false;
        }
        else
        {
            Image bg = CreateImage("Background", canvasRect, null, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
            bg.color = new Color(0.55f, 0.76f, 0.83f);
            bg.raycastTarget = false;
            Debug.LogWarning("Background sprite was not found/imported as Sprite. Created a solid-color background instead.");
        }

        // Soft translucent panel behind gameplay area.
        Image board = CreateImage("GameplayBoard", canvasRect, null, new Vector2(0, -65), new Vector2(1500, 650));
        board.color = new Color(0.10f, 0.18f, 0.16f, 0.26f);
        board.raycastTarget = false;

        TMP_Text title = CreateText("Title", canvasRect, "TOWER OF HANOI", new Vector2(0, 445), new Vector2(700, 70), 42, FontStyles.Bold, TextAlignmentOptions.Center);
        title.color = new Color(0.22f, 0.12f, 0.06f);

        TMP_Text movesText = CreateText("MovesText", canvasRect, "Moves: 0  |  Minimum: 31", new Vector2(0, 365), new Vector2(600, 50), 28, FontStyles.Bold, TextAlignmentOptions.Center);
        movesText.color = Color.white;
        TMP_Text statusText = CreateText("StatusText", canvasRect, "Move all disks to the right-hand rod.", new Vector2(0, 315), new Vector2(1000, 45), 22, FontStyles.Normal, TextAlignmentOptions.Center);
        statusText.color = Color.white;

        // Rod targets are siblings under the same Canvas parent, matching HanoiRod.LayoutDisks().
        HanoiRod[] rods = new HanoiRod[3];
        string[] rodNames = { "RodLeft", "RodMiddle", "RodRight" };
        float[] x = { -520f, 0f, 520f };
        for (int i = 0; i < 3; i++)
        {
            Image rodImage = CreateImage(rodNames[i], canvasRect, null, new Vector2(x[i], -105), new Vector2(390, 390));
            rodImage.color = new Color(0.25f, 0.18f, 0.10f, 0.16f);
            rodImage.raycastTarget = true;
            rods[i] = rodImage.gameObject.AddComponent<HanoiRod>();

            // Decorative pole and base.
            Image pole = CreateImage(rodNames[i] + "_Pole", canvasRect, null, new Vector2(x[i], -105), new Vector2(20, 300));
            pole.color = new Color(0.38f, 0.20f, 0.09f, 0.92f);
            pole.raycastTarget = false;
            pole.transform.SetAsLastSibling();

            Image baseImage = CreateImage(rodNames[i] + "_Base", canvasRect, null, new Vector2(x[i], -300), new Vector2(420, 24));
            baseImage.color = new Color(0.32f, 0.16f, 0.07f, 1f);
            baseImage.raycastTarget = false;
            baseImage.transform.SetAsLastSibling();
        }

        // Put drop targets above decorative objects but below spawned disks.
        for (int i = 0; i < rods.Length; i++)
            rods[i].transform.SetAsLastSibling();

        // Buttons and controls.
        Button restartButton = CreateButton(canvasRect, "RestartButton", "RESTART", new Vector2(610, 435), new Vector2(190, 62));
        Button menuButton = CreateButton(canvasRect, "MenuButton", "MENU", new Vector2(-800, 435), new Vector2(170, 62));
        Button hintButton = CreateButton(canvasRect, "HintButton", "HINT · AD", new Vector2(390, 270), new Vector2(220, 62));
        Button undoButton = CreateButton(canvasRect, "UndoButton", "UNDO · AD", new Vector2(640, 270), new Vector2(220, 62));

        GameObject winPanel = new GameObject("WinPanel", typeof(RectTransform), typeof(Image));
        winPanel.transform.SetParent(canvasRect, false);
        RectTransform winRect = winPanel.GetComponent<RectTransform>();
        winRect.anchorMin = winRect.anchorMax = new Vector2(0.5f, 0.5f);
        winRect.anchoredPosition = Vector2.zero;
        winRect.sizeDelta = new Vector2(620, 300);
        Image winImage = winPanel.GetComponent<Image>();
        winImage.color = new Color(0.04f, 0.08f, 0.06f, 0.95f);

        TMP_Text winText = CreateText("WinText", winRect, "PUZZLE COMPLETE!\nWell done!", new Vector2(0, 40), new Vector2(560, 150), 36, FontStyles.Bold, TextAlignmentOptions.Center);
        winText.color = Color.white;
        Button playAgain = CreateButton(winRect, "PlayAgainButton", "PLAY AGAIN", new Vector2(0, -95), new Vector2(230, 58));
        winPanel.SetActive(false);

        // Dedicated bottom strip left clear for the real Unity banner ad.
        Image bannerGuide = CreateImage("BannerSafeArea", canvasRect, null, new Vector2(0, -505), new Vector2(1000, 65));
        bannerGuide.color = new Color(0, 0, 0, 0.22f);
        bannerGuide.raycastTarget = false;
        CreateText("BannerLabel", bannerGuide.rectTransform, "BANNER AD AREA", Vector2.zero, new Vector2(700, 50), 18, FontStyles.Bold, TextAlignmentOptions.Center);

        // Create/repair disk prefab.
        GameObject prefabObject = PrefabUtility.LoadPrefabContents(DiskPrefabPath);
        if (prefabObject == null)
        {
            prefabObject = new GameObject("HanoiDisk", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            prefabObject.AddComponent<HanoiDisk>();
            Image diskImage = prefabObject.GetComponent<Image>();
            diskImage.preserveAspect = true;
            if (AssetDatabase.LoadAssetAtPath<Sprite>(DiskSpritePath) != null)
                diskImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(DiskSpritePath);
            PrefabUtility.SaveAsPrefabAsset(prefabObject, DiskPrefabPath);
            Object.DestroyImmediate(prefabObject);
        }
        else
        {
            Image diskImage = prefabObject.GetComponent<Image>();
            if (diskImage == null) diskImage = prefabObject.AddComponent<Image>();
            if (prefabObject.GetComponent<CanvasGroup>() == null) prefabObject.AddComponent<CanvasGroup>();
            if (prefabObject.GetComponent<HanoiDisk>() == null) prefabObject.AddComponent<HanoiDisk>();
            if (AssetDatabase.LoadAssetAtPath<Sprite>(DiskSpritePath) != null)
                diskImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(DiskSpritePath);
            diskImage.preserveAspect = true;
            PrefabUtility.SaveAsPrefabAsset(prefabObject, DiskPrefabPath);
            PrefabUtility.UnloadPrefabContents(prefabObject);
        }

        HanoiDisk diskPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DiskPrefabPath).GetComponent<HanoiDisk>();

        GameObject managerObject = new GameObject("HanoiGameManager");
        HanoiGameManager manager = managerObject.AddComponent<HanoiGameManager>();
        manager.diskCount = 5;
        manager.rods = rods;
        manager.diskPrefab = diskPrefab;
        manager.diskParent = canvasRect;
        manager.diskHeight = 48f;
        manager.smallestDiskWidth = 130f;
        manager.diskWidthStep = 38f;
        manager.bottomOffset = -170f;
        manager.movesText = movesText;
        manager.statusText = statusText;
        manager.winPanel = winPanel;
        manager.restartButton = restartButton;
        manager.menuButton = menuButton;
        manager.menuSceneName = "StartMenu";
        manager.hintButton = hintButton;
        manager.undoButton = undoButton;

        // HanoiGameManager.Start() wires restart/menu/hint/undo buttons.
        playAgain.onClick.AddListener(manager.RestartPuzzle);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Scene rebuilt", "TowerOfHanoi.unity was rebuilt and saved. A timestamped backup was saved beside it.", "OK");
        Debug.Log("Tower of Hanoi scene rebuilt at " + ScenePath);
    }

    private static void EnsureSpriteImport(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
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
        text.enableWordWrapping = true;
        if (TMP_Settings.defaultFontAsset != null)
            text.font = TMP_Settings.defaultFontAsset;
        return text;
    }

    private static Button CreateButton(RectTransform parent, string name, string label, Vector2 position, Vector2 size)
    {
        Image image = CreateImage(name, parent, null, position, size);
        image.color = new Color(0.35f, 0.19f, 0.08f, 0.96f);
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.88f, 0.65f, 0.31f);
        colors.highlightedColor = new Color(1f, 0.78f, 0.43f);
        colors.pressedColor = new Color(0.68f, 0.43f, 0.18f);
        button.colors = colors;

        TMP_Text text = CreateText(name + "_Label", image.rectTransform, label, Vector2.zero, size - new Vector2(12, 8), 22, FontStyles.Bold, TextAlignmentOptions.Center);
        text.color = new Color(0.12f, 0.08f, 0.04f);
        text.raycastTarget = false;
        return button;
    }
}
