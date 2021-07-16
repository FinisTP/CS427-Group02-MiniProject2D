using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skin : MonoBehaviour
{
    public int itemId;
    public AnimatorOverrideController Animation;
    public int price;
    public TMP_Text priceTag;
    public string transactionMessage;
    public Image CharacterSprite;

    public bool isSelected = false;

    public bool bought = false;

    private void Start()
    {
        if (ShopManager.Instance.BoughtSkinIds.Contains(itemId))
        {
            bought = true;
        }

        priceTag.text = price.ToString("N0");
        if (bought) priceTag.text = "Bought";
    }

    public void Buy()
    {
        if (!bought)
        {
            if (GameManager_.Instance.Coin >= price)
            {
                GameManager_.Instance.AddCoin(-price);
                priceTag.text = "Bought";
                ShopManager.Instance.ShowMessage(transactionMessage, 2);
                ShopManager.Instance.UpdateCoin();
                bought = true;
                GameManager_.Instance.tracker.BoughtSkinIds.Add(itemId);
            }
            else ShopManager.Instance.ShowMessage("You don't have enough leaves to buy this skin!", 1);
        } else
        {
            
            ShopManager.Instance.ShowMessage(transactionMessage, 2);
        }
        
    }

    public void HighlightSprite(bool state)
    {
        if (state)
        {
            CharacterSprite.color = new Color(1, 1, 1, 1);
        } else
        {
            CharacterSprite.color = new Color(0.3f, 0.3f, 0.3f, 1);
        }
    }

    public void SelectSkin()
    {
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        if (bought)
        {
            ShopManager.Instance.HighlightSkin(this);
            GameManager_.Instance.CharacterAnimation = Animation;
        }
        else ShopManager.Instance.ShowMessage("You have to buy this skin before selecting it!", 1);
    }

}
