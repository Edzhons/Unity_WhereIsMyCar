using System;
using UnityEngine;

/// <summary>
/// Stars ("zvaigznes") earned by winning in both games and spent in the shop.
/// Everything is kept in PlayerPrefs.
/// </summary>
public static class StarBank
{
    // Shop items
    public const string NO_ADS = "bez_reklamam";
    public const string DEFAULT_SKIN = "krasainie";

    // Hanoi helpers that can be bought instead of watching an ad
    public const string HINT_TOKENS = "padomi";
    public const string UNDO_TOKENS = "atsauksanas";

    public const int CHEAT_STARS = 50;

    private const string BALANCE_KEY = "StarBank_Balance";
    private const string ITEM_KEY = "StarBank_Item_";
    private const string TOKEN_KEY = "StarBank_Tokens_";
    private const string SKIN_KEY = "StarBank_DiskSkin";

    /// <summary>Raised whenever the balance, items or tokens change.</summary>
    public static event Action Changed;

    public static int Balance => PlayerPrefs.GetInt(BALANCE_KEY, 0);

    public static void Deposit(int stars)
    {
        if (stars <= 0)
            return;

        PlayerPrefs.SetInt(BALANCE_KEY, Balance + stars);
        Save();
    }

    public static bool Spend(int stars)
    {
        if (stars > Balance)
            return false;

        PlayerPrefs.SetInt(BALANCE_KEY, Balance - stars);
        Save();
        return true;
    }

    // =========================
    // ITEMS (bought once)
    // =========================

    public static bool HasItem(string id)
    {
        return id == DEFAULT_SKIN || PlayerPrefs.GetInt(ITEM_KEY + id, 0) == 1;
    }

    public static void GiveItem(string id)
    {
        PlayerPrefs.SetInt(ITEM_KEY + id, 1);
        Save();
    }

    public static string DiskSkin
    {
        get
        {
            string skin = PlayerPrefs.GetString(SKIN_KEY, DEFAULT_SKIN);
            return HasItem(skin) ? skin : DEFAULT_SKIN;
        }
        set
        {
            PlayerPrefs.SetString(SKIN_KEY, value);
            Save();
        }
    }

    // =========================
    // TOKENS (used up one by one)
    // =========================

    public static int Tokens(string kind)
    {
        return PlayerPrefs.GetInt(TOKEN_KEY + kind, 0);
    }

    public static void AddTokens(string kind, int amount)
    {
        PlayerPrefs.SetInt(TOKEN_KEY + kind, Tokens(kind) + amount);
        Save();
    }

    public static bool UseToken(string kind)
    {
        int left = Tokens(kind);
        if (left <= 0)
            return false;

        PlayerPrefs.SetInt(TOKEN_KEY + kind, left - 1);
        Save();
        return true;
    }

    // TEST KEY - F2 gives stars (works in builds too, with a keyboard)
    public static void CheckCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Deposit(CHEAT_STARS);
            Debug.Log("TEST: +" + CHEAT_STARS + " stars, now " + Balance);
        }
    }

    private static void Save()
    {
        PlayerPrefs.Save();
        Changed?.Invoke();
    }
}
