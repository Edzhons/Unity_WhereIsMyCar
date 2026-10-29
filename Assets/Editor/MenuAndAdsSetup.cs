using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Tools > Kur mans vags > Setup Menu and Ads:
///  - CityScene's AdsScriptHolder becomes a prefab that every scene contains,
///    and the banner on/off button is removed (the banner is always on);
///  - the dashboard menu gets the "HANOJAS TORNIS" sign, the star count on the
///    radio, the shop on the phone and the secret duck;
///  - TowerOfHanoi is rebuilt and added to the build.
/// Batch mode: -executeMethod MenuAndAdsSetup.SetupAll
/// </summary>
public static class MenuAndAdsSetup
{
    const string MENU_SCENE = "Assets/Scenes/StartMenu.unity";
    const string CITY_SCENE = "Assets/Scenes/CityScene.unity";
    const string HANOI_SCENE = "Assets/Scenes/TowerOfHanoi.unity";
    const string ADS_PREFAB = "Assets/Prefabs/AdsScriptHolder.prefab";
    const string MENU_ART = "Assets/Sprites/Menu/";
    const string HANOI_ART = "Assets/Sprites/Hanoi/";
    const string SOUNDS = "Assets/Sounds/";
    const string FONT = "Assets/TextMesh Pro/Resources/Fonts & Materials/Chewy SDF.asset";

    static readonly Color DarkPanel = new Color(0.07f, 0.08f, 0.1f, 1f);
    static readonly Color RowColor = new Color(1f, 1f, 1f, 0.09f);
    static readonly Color ButtonText = new Color(0.15f, 0.12f, 0.1f);
    static readonly Color Yellow = new Color(1f, 0.86f, 0.42f);
    static readonly Color LcdText = new Color(156 / 255f, 237 / 255f, 132 / 255f);   // like "CITY FM"

    static TMP_FontAsset font;
    static Sprite rounded;

    [MenuItem("Tools/Kur mans vags/Setup Menu and Ads")]
    public static void SetupAll()
    {
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT);
        rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites/Menu", "Assets/Sprites/Hanoi" }))
            HanoiSceneRebuilder.EnsureSpriteImport(AssetDatabase.GUIDToAssetPath(guid));

        SetupCityScene();
        SetupMenu();
        HanoiSceneRebuilder.RebuildWithoutBackup();
        AddHanoiToBuild();
        AssetDatabase.SaveAssets();
        Debug.Log("MenuAndAdsSetup: done");
    }

    // ---------------------------------------------------------------- CityScene

    static void SetupCityScene()
    {
        Scene scene = EditorSceneManager.OpenScene(CITY_SCENE, OpenSceneMode.Single);

        GameObject holder = GameObject.Find("AdsScriptHolder");
        if (holder != null && !PrefabUtility.IsPartOfPrefabInstance(holder))
            PrefabUtility.SaveAsPrefabAssetAndConnect(holder, ADS_PREFAB, InteractionMode.AutomatedAction);

        // The banner is always visible now, so its on/off button goes.
        GameObject bannerButton = GameObject.FindGameObjectsWithTag("BannerAdButton").FirstOrDefault();
        if (bannerButton != null)
            UnityEngine.Object.DestroyImmediate(bannerButton);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    // ---------------------------------------------------------------- menu

    static void SetupMenu()
    {
        Scene scene = EditorSceneManager.OpenScene(MENU_SCENE, OpenSceneMode.Single);
        RectTransform canvas = (RectTransform)GameObject.Find("Canvas").transform;
        StartMenuScript menu = UnityEngine.Object.FindFirstObjectByType<StartMenuScript>();

        foreach (string name in new[] { "HanoiSign", "RadioDisplay", "Duck", "PhoneShop", "ShopPopup" })
        {
            Transform old = canvas.Find(name);
            if (old != null)
                UnityEngine.Object.DestroyImmediate(old.gameObject);
        }

        if (!scene.GetRootGameObjects().Any(g => g.name == "AdsScriptHolder"))
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(ADS_PREFAB), scene);

        AudioClip click = AssetDatabase.LoadAssetAtPath<AudioClip>(SOUNDS + "click.wav");
        AudioClip kachow = AssetDatabase.LoadAssetAtPath<AudioClip>(SOUNDS + "kachow_r22ZKsu.mp3");
        Sprite star = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/star.png");

        // The painted "LIMENIS 1" sign in blue, standing on top of it (art made
        // from the picture itself, so it lines up with it exactly).
        Image sign = Img("HanoiSign", canvas, Load(MENU_ART + "sign_hanoi.png"), new Vector2(-83.8f, 388.5f), new Vector2(472.7f, 142.3f));
        sign.preserveAspect = false;
        Button signButton = sign.gameObject.AddComponent<Button>();
        signButton.transition = Selectable.Transition.None;
        sign.gameObject.AddComponent<ButtonHoverAnimation>();
        UnityEventTools.AddPersistentListener(signButton.onClick, menu.StartHanoi);

        // The radio display shows the star balance where "CITY FM" was
        // (the painted note icon and signal bars stay).
        Image radio = Img("RadioDisplay", canvas, Load(MENU_ART + "radio_lcd.png"), new Vector2(382.4f, -339.7f), new Vector2(147f, 90.7f));
        radio.preserveAspect = false;
        radio.raycastTarget = false;
        Image lcdStar = Img("Star", radio.rectTransform, Load(HANOI_ART + "lcd_star.png"), new Vector2(-36, 2), new Vector2(40, 40));
        lcdStar.color = LcdText;
        lcdStar.raycastTarget = false;
        TMP_Text radioText = Text("Stars", radio.rectTransform, "0", new Vector2(20, 0), new Vector2(84, 76), 56, LcdText);
        RadioStarDisplay radioDisplay = radio.gameObject.AddComponent<RadioStarDisplay>();
        radioDisplay.displayText = radioText;

        // Rubber duck, cut out of the picture so it can wiggle (5 taps = test stars).
        Image duck = Img("Duck", canvas, Load(MENU_ART + "duck.png"), new Vector2(250, -157.5f), new Vector2(170, 174.5f));
        duck.rectTransform.pivot = new Vector2(0.5f, 0.1f);
        duck.preserveAspect = false;
        DuckSecret secret = duck.gameObject.AddComponent<DuckSecret>();
        secret.radio = radioDisplay;
        secret.audioSource = menu.audioSource;
        secret.tapSound = AssetDatabase.LoadAssetAtPath<AudioClip>(SOUNDS + "goofy-bonk.mp3");
        secret.rewardSound = kachow;

        // Shop popup
        ShopPanel shop = BuildShop(canvas, menu, star, click, kachow);

        // The phone on the dashboard shows a shop app and opens the shop.
        // A soft light around its screen pulses so it looks tappable.
        Image phone = Img("PhoneShop", canvas, Load(MENU_ART + "phone_shop.png"), new Vector2(866.4f, -131.4f), new Vector2(173.4f, 264.1f));
        phone.preserveAspect = false;
        Button phoneButton = phone.gameObject.AddComponent<Button>();
        phoneButton.transition = Selectable.Transition.None;
        Image glow = Img("Glow", phone.rectTransform, Load(MENU_ART + "phone_glow.png"), Vector2.zero, new Vector2(173.4f, 264.1f));
        glow.preserveAspect = false;
        glow.raycastTarget = false;
        phone.gameObject.AddComponent<GentlePulse>().fadeTarget = glow;
        PlayOnClick(phoneButton, menu.audioSource, click);
        UnityEventTools.AddPersistentListener(phoneButton.onClick, shop.Open);

        shop.panel.transform.SetAsLastSibling();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static ShopPanel BuildShop(RectTransform canvas, StartMenuScript menu, Sprite star, AudioClip click, AudioClip buySound)
    {
        Image popup = Img("ShopPopup", canvas, null, Vector2.zero, Vector2.zero);
        Stretch(popup.rectTransform);
        popup.color = new Color(0f, 0f, 0f, 0.6f);

        ShopPanel shop = popup.gameObject.AddComponent<ShopPanel>();
        shop.panel = popup.gameObject;

        // A big phone-like card
        Image card = Img("Card", popup.rectTransform, rounded, Vector2.zero, new Vector2(1300, 820));
        card.type = Image.Type.Sliced;
        card.color = DarkPanel;
        Image notch = Img("Notch", card.rectTransform, rounded, new Vector2(0, 392), new Vector2(180, 18));
        notch.type = Image.Type.Sliced;
        notch.color = Color.black;
        RectTransform c = card.rectTransform;

        Text("Title", c, "VEIKALS", new Vector2(-300, 340), new Vector2(600, 100), 84, Color.white);
        Image balance = Img("Balance", c, rounded, new Vector2(470, 340), new Vector2(260, 90));
        balance.type = Image.Type.Sliced;
        balance.color = RowColor;
        Img("Icon", balance.rectTransform, star, new Vector2(-70, 0), new Vector2(84, 63)).raycastTarget = false;
        shop.balanceText = Text("Stars", balance.rectTransform, "0", new Vector2(40, 0), new Vector2(150, 80), 60, Yellow);

        Text("HelpersTitle", c, "PALIGI UN REKLAMAS", new Vector2(-330, 268), new Vector2(560, 50), 38, Yellow);
        Text("SkinsTitle", c, "DISKU IZSKATS", new Vector2(330, 268), new Vector2(560, 50), 38, Yellow);

        AudioSource audio = menu.audioSource;
        Sprite adsIcon = Load("Assets/Sprites/Ads.png");
        OfferRow(c, new Vector2(-330, 175), "BEZ REKLAMAM", "Nav reklamas, mainot ainu", adsIcon, true,
            ShopOffer.OfferType.Permanent, StarBank.NO_ADS, 30, star, audio, buySound);
        OfferRow(c, new Vector2(-330, 30), "3 PADOMI", null, Load(HANOI_ART + "hanoi_hint_arrow.png"), false,
            ShopOffer.OfferType.TokenPack, StarBank.HINT_TOKENS, 5, star, audio, buySound);
        OfferRow(c, new Vector2(-330, -115), "3 ATSAUKSANAS", null, Load(HANOI_ART + "undo_icon.png"), false,
            ShopOffer.OfferType.TokenPack, StarBank.UNDO_TOKENS, 5, star, audio, buySound);

        string[] skins = { "krasainie", "koks", "marmors", "zelts" };
        string[] names = { "KRASAINIE", "KOKS", "MARMORS", "ZELTS" };
        int[] prices = { 0, 8, 15, 25 };
        for (int i = 0; i < skins.Length; i++)
        {
            Vector2 pos = new Vector2(330 + (i % 2 == 0 ? -140 : 140), i < 2 ? 105 : -165);
            SkinTile(c, pos, names[i], skins[i], prices[i], star, audio, buySound);
        }

        Button close = TextButton("CloseButton", c, "AIZVERT", new Vector2(0, -360), new Vector2(300, 80), Color.white);
        PlayOnClick(close, audio, click);
        UnityEventTools.AddPersistentListener(close.onClick, shop.Close);

        popup.gameObject.SetActive(false);
        return shop;
    }

    static void OfferRow(RectTransform parent, Vector2 pos, string title, string description, Sprite icon, bool crossedOut,
        ShopOffer.OfferType type, string id, int price, Sprite star, AudioSource audio, AudioClip buySound)
    {
        Image row = Img("Offer_" + id, parent, rounded, pos, new Vector2(560, 130));
        row.type = Image.Type.Sliced;
        row.color = RowColor;
        row.raycastTarget = false;
        RectTransform r = row.rectTransform;

        Image iconImage = Img("Icon", r, icon, new Vector2(-215, 0), new Vector2(96, 96));
        iconImage.raycastTarget = false;
        if (crossedOut)
        {
            Image slash = Img("Slash", iconImage.rectTransform, null, Vector2.zero, new Vector2(110, 12));
            slash.rectTransform.localRotation = Quaternion.Euler(0, 0, 40);
            slash.color = new Color(0.86f, 0.15f, 0.15f);
            slash.raycastTarget = false;
        }

        // Between the icon (left) and the price button (right).
        Text("Title", r, title, new Vector2(-25, 24), new Vector2(250, 50), 38, Color.white, TextAlignmentOptions.Left);
        TMP_Text second = Text("Description", r, description ?? "Tev ir: 0", new Vector2(-25, -24), new Vector2(250, 40), 28,
            new Color(0.78f, 0.78f, 0.78f), TextAlignmentOptions.Left);

        ShopOffer offer = row.gameObject.AddComponent<ShopOffer>();
        offer.type = type;
        offer.itemId = id;
        offer.price = price;
        offer.amount = 3;
        offer.ownedText = type == ShopOffer.OfferType.TokenPack ? second : null;
        PriceButton(offer, r, new Vector2(190, 0), new Vector2(160, 80), star, audio, buySound);
    }

    static void SkinTile(RectTransform parent, Vector2 pos, string title, string id, int price, Sprite star, AudioSource audio, AudioClip buySound)
    {
        Image tile = Img("Skin_" + id, parent, rounded, pos, new Vector2(270, 260));
        tile.type = Image.Type.Sliced;
        tile.color = RowColor;
        tile.raycastTarget = false;
        RectTransform t = tile.rectTransform;

        Img("Preview", t, Load(HANOI_ART + "Disks/" + id + "_preview.png"), new Vector2(0, 50), new Vector2(225, 150)).raycastTarget = false;
        Text("Name", t, title, new Vector2(0, -42), new Vector2(250, 44), 34, Color.white);

        ShopOffer offer = tile.gameObject.AddComponent<ShopOffer>();
        offer.type = ShopOffer.OfferType.DiskSkin;
        offer.itemId = id;
        offer.price = price;
        PriceButton(offer, t, new Vector2(0, -95), new Vector2(210, 60), star, audio, buySound);
    }

    // Shows the price with a star (car) icon, or "IZVELETIES"/"NOPIRKTS" once owned.
    static void PriceButton(ShopOffer offer, RectTransform parent, Vector2 pos, Vector2 size, Sprite star, AudioSource audio, AudioClip buySound)
    {
        Button button = TextButton("BuyButton", parent, "", pos, size, Color.white);
        Image icon = Img("PriceIcon", button.GetComponent<RectTransform>(), star, new Vector2(-size.x / 2 + 38, 0), new Vector2(54, 40));
        icon.raycastTarget = false;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
        label.margin = new Vector4(40, 0, 0, 0);

        offer.button = button;
        offer.buttonLabel = label;
        offer.priceIcon = icon.gameObject;
        offer.audioSource = audio;
        offer.buySound = buySound;
        UnityEventTools.AddPersistentListener(button.onClick, offer.OnButtonClicked);
    }

    // ---------------------------------------------------------------- build settings

    static void AddHanoiToBuild()
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (!scenes.Any(s => s.path == HANOI_SCENE))
            scenes.Add(new EditorBuildSettingsScene(HANOI_SCENE, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    // ---------------------------------------------------------------- UI helpers

    static Sprite Load(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
            throw new Exception("Sprite not found: " + path);
        return sprite;
    }

    static Image Img(string name, RectTransform parent, Sprite sprite, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.layer = LayerMask.NameToLayer("UI");
        RectTransform rt = (RectTransform)go.transform;
        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = sprite != null && sprite != rounded;
        return image;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    static TMP_Text Text(string name, RectTransform parent, string value, Vector2 pos, Vector2 size, float fontSize, Color color,
        TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.layer = LayerMask.NameToLayer("UI");
        RectTransform rt = (RectTransform)go.transform;
        rt.SetParent(parent, false);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = align;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.enableAutoSizing = true;
        text.fontSizeMax = fontSize;
        text.fontSizeMin = fontSize * 0.45f;
        text.raycastTarget = false;
        return text;
    }

    static Button TextButton(string name, RectTransform parent, string label, Vector2 pos, Vector2 size, Color color)
    {
        Image image = Img(name, parent, rounded, pos, size);
        image.type = Image.Type.Sliced;
        image.color = color;
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        Text("Label", image.rectTransform, label, Vector2.zero, size - new Vector2(16, 8), 40, ButtonText);
        return button;
    }

    static void PlayOnClick(Button button, AudioSource source, AudioClip clip)
    {
        if (source != null && clip != null)
            UnityEventTools.AddObjectPersistentListener<AudioClip>(button.onClick, source.PlayOneShot, clip);
    }

    // ---------------------------------------------------------------- screenshots

    /// <summary>Renders scenes to PNGs to check layouts (folder from KMV_CAPTURE_DIR).</summary>
    public static void CaptureAll()
    {
        string dir = Environment.GetEnvironmentVariable("KMV_CAPTURE_DIR") ?? "Temp/Captures";
        Directory.CreateDirectory(dir);

        Capture(MENU_SCENE, 2340, 1080, dir + "/menu.png", null);
        Capture(MENU_SCENE, 2340, 1080, dir + "/shop.png", () =>
        {
            GameObject.Find("Canvas").transform.Find("ShopPopup").gameObject.SetActive(true);
            foreach (ShopOffer offer in UnityEngine.Object.FindObjectsByType<ShopOffer>(FindObjectsSortMode.None))
                offer.Refresh();
        });
        Capture(HANOI_SCENE, 2340, 1080, dir + "/hanoi_levels.png", () => Hanoi().ShowLevelSelect());
        Capture(HANOI_SCENE, 2340, 1080, dir + "/hanoi_play.png", () =>
        {
            HanoiGameManager game = Hanoi();
            game.StartLevel(3);
            game.TryMove(game.rods[0].TopDisk, game.rods[2]);
            game.TryMove(game.rods[0].TopDisk, game.rods[1]);
            game.hintArrow.gameObject.SetActive(true);
            game.hintArrow.anchoredPosition = ((RectTransform)game.rods[1].transform).anchoredPosition + new Vector2(0, game.hintArrowHeight);
            game.statusText.text = "Padoms: parvieto disku no 3. uz 2. stieni";
        });
        Capture(HANOI_SCENE, 1440, 1080, dir + "/hanoi_tablet.png", () => { Hanoi().StartLevel(3); });
        Capture(HANOI_SCENE, 2340, 1080, dir + "/hanoi_win.png", () =>
        {
            HanoiGameManager game = Hanoi();
            game.StartLevel(2);
            game.winPanel.SetActive(true);
            for (int i = 0; i < 3; i++)
                game.winStars[i].color = i < 2 ? Color.white : new Color(0.25f, 0.25f, 0.25f, 0.45f);
            game.winMovesText.text = "Gajieni: 19   (minimums 15)";
            game.winEarnedText.text = "+2";
        });
        Debug.Log("MenuAndAdsSetup: captures in " + dir);
    }

    static HanoiGameManager Hanoi()
    {
        HanoiGameManager game = UnityEngine.Object.FindFirstObjectByType<HanoiGameManager>();
        game.InitializeRods();
        return game;
    }

    static void Capture(string scenePath, int width, int height, string outPath, Action setup)
    {
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        GameObject camObj = new GameObject("CaptureCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.orthographic = true;
        cam.targetTexture = target;
        camObj.transform.position = new Vector3(0, 0, -10);
        foreach (Camera other in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            if (other != cam) other.enabled = false;

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None).Where(c => c.isRootCanvas).ToArray();
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
            canvases[i].worldCamera = cam;
            canvases[i].planeDistance = 5f + i;
        }

        // Batch mode never runs Update: size the canvases by hand first.
        for (int pass = 0; pass < 2; pass++)
        {
            foreach (CanvasScaler scaler in UnityEngine.Object.FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None))
                typeof(CanvasScaler).GetMethod("Handle", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(scaler, null);
            Canvas.ForceUpdateCanvases();
            foreach (AspectRatioFitter fitter in UnityEngine.Object.FindObjectsByType<AspectRatioFitter>(FindObjectsSortMode.None))
                typeof(AspectRatioFitter).GetMethod("UpdateRect", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(fitter, null);
        }

        setup?.Invoke();
        Canvas.ForceUpdateCanvases();
        cam.Render();

        RenderTexture.active = target;
        Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();
        RenderTexture.active = null;
        File.WriteAllBytes(outPath, image.EncodeToPNG());
    }
}
