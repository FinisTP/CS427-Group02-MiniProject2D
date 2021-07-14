using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ItemType
{
    Speed,
    Shield,
    Dash,
    Score,
    Combo
}

[System.Serializable]
public class ItemClass
{
    public int price;
    public float effectLevel;
}

public class TabItem : MonoBehaviour
{
    private int currentLevel = 1;
    public ItemClass[] itemProgression;
    public string description;
    public TMP_Text priceTag;
    public string transactionMessage;
    public ItemType itemType;

    public Image[] levelCubes;

    private void Start()
    {
        if (currentLevel < itemProgression.Length - 1)
        {
            priceTag.text = itemProgression[currentLevel].price.ToString();
        }
        else priceTag.text = "MAXED";
    }

    public void Buy()
    {
        if (currentLevel < itemProgression.Length && GameManager_.Instance.Coin >= itemProgression[currentLevel].price)
        {
            GameManager_.Instance.Coin -= itemProgression[currentLevel].price;
            switch (itemType)
            {
                case ItemType.Combo:
                    ModifyCombo(itemProgression[currentLevel].effectLevel);
                    break;
                case ItemType.Score:
                    ModifyScore(itemProgression[currentLevel].effectLevel);
                    break;
                case ItemType.Shield:
                    ModifyShield((int)itemProgression[currentLevel].effectLevel);
                    break;
                case ItemType.Dash:
                    ModifyDash(itemProgression[currentLevel].effectLevel);
                    break;
                case ItemType.Speed:
                    ModifySpeed(itemProgression[currentLevel].effectLevel);
                    break;

            }


            currentLevel++;
            ShopManager.Instance.ShowMessage(transactionMessage, 2);
            ShopManager.Instance.UpdateCoin();
            if (currentLevel < itemProgression.Length - 1)
            {
                priceTag.text = itemProgression[currentLevel].price.ToString();
            }
            else priceTag.text = "MAXED";
            
           
            UpdateLevelCube();
        } else
        {
            ShopManager.Instance.ShowMessage("You don't have enough leaves to purchase this item!", 1);
        }
    }

    public void UpdateLevelCube()
    {
        for (int i = 0; i < currentLevel; ++i)
        {
            levelCubes[i].color = Color.white;
        }
        for (int i = currentLevel; i < levelCubes.Length; ++i)
        {
            levelCubes[i].color = new Color(0, 0, 0, 0);
        }
    }

    public void DisplayInfo()
    {
        ShopManager.Instance.ShowMessage(description, 3);
    }

    public void ModifySpeed(float modifier)
    {
        GameManager_.Instance.speedBoost = modifier;
    }

    public void ModifyDash(float modifier)
    {
        GameManager_.Instance.dashTimeBoost = modifier;
    }

    public void ModifyShield(int modifier)
    {
        GameManager_.Instance.shield = modifier;
    }

    public void ModifyCombo(float modifier)
    {
        GameManager_.Instance.comboTimeAmplifer = modifier;
    }

    public void ModifyScore(float modifier)
    {
        GameManager_.Instance.scoreMultiplier = modifier;
    }

}
