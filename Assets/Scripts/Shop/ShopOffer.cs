using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>One thing for sale in the shop popup.</summary>
public class ShopOffer : MonoBehaviour
{
    public enum OfferType { Permanent, DiskSkin, TokenPack }

    public OfferType type;
    public string itemId;
    public int price;
    [Tooltip("Token packs only: how many tokens one purchase gives.")]
    public int amount = 3;

    public Button button;
    public TMP_Text buttonLabel;
    public GameObject priceIcon;
    [Tooltip("Token packs only: shows how many the player already has.")]
    public TMP_Text ownedText;

    public AudioSource audioSource;
    public AudioClip buySound;

    private static readonly Color buyColor = Color.white;
    private static readonly Color useColor = new Color(0.62f, 0.9f, 0.55f);
    private static readonly Color doneColor = new Color(0.6f, 0.6f, 0.6f);

    private void OnEnable()
    {
        StarBank.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        StarBank.Changed -= Refresh;
    }

    public void Refresh()
    {
        bool owned = type != OfferType.TokenPack && StarBank.HasItem(itemId);
        Image buttonImage = button.GetComponent<Image>();

        // The price sits right of the star icon; other labels are centred.
        priceIcon.SetActive(!owned);
        buttonLabel.margin = new Vector4(owned ? 0f : 40f, 0f, 0f, 0f);

        if (!owned)
        {
            buttonLabel.text = price.ToString();
            buttonImage.color = buyColor;
            button.interactable = StarBank.Balance >= price;
        }
        else if (type == OfferType.DiskSkin)
        {
            bool selected = StarBank.DiskSkin == itemId;
            buttonLabel.text = selected ? "IZVELETS" : "IZVELETIES";
            buttonImage.color = selected ? doneColor : useColor;
            button.interactable = !selected;
        }
        else
        {
            buttonLabel.text = "NOPIRKTS";
            buttonImage.color = doneColor;
            button.interactable = false;
        }

        if (ownedText != null)
            ownedText.text = "Tev ir: " + StarBank.Tokens(itemId);
    }

    public void OnButtonClicked()
    {
        bool owned = type != OfferType.TokenPack && StarBank.HasItem(itemId);

        if (!owned)
        {
            if (!StarBank.Spend(price))
                return;

            if (type == OfferType.TokenPack)
                StarBank.AddTokens(itemId, amount);
            else
                StarBank.GiveItem(itemId);

            if (audioSource != null && buySound != null)
                audioSource.PlayOneShot(buySound);
        }

        if (type == OfferType.DiskSkin)
            StarBank.DiskSkin = itemId;
    }
}
