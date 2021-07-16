using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TMP_Text toastText;
    public TMP_Text money;
    public static ShopManager Instance => _instance;
    private static ShopManager _instance = null;

    public List<Skin> skinList;
    public List<int> BoughtSkinIds;
    public Dictionary<int, int> BoughtItemStates;
    public GameObject[] Tabs;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else Destroy(gameObject);
        BoughtSkinIds = GameManager_.Instance.tracker.BoughtSkinIds;
        BoughtItemStates = GameManager_.Instance.tracker.BoughtItemStates;
        UpdateCoin();
    }

    public void UpdateCoin()
    {
        money.text = $"{GameManager_.Instance.Coin.ToString("N0")}";
    }

    public void ShowMessage(string message, int messageType)
    {
        // 1: warning
        // 2: result
        // 3: description
        toastText.SetText(message);

        switch (messageType)
        {
            case 1:
                toastText.color = Color.red;
                break;
            case 2:
                toastText.color = Color.blue;
                break;
            case 3:
                toastText.color = Color.black;
                break;
            default:
                break;
        }
    }

    public void SwitchTab(int tabId)
    {
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        for (int i = 0; i < Tabs.Length; ++i)
        {
            if (i != tabId) Tabs[i].SetActive(false);
            else Tabs[i].SetActive(true);
        }
    }

    public void HighlightSkin(Skin s)
    {
        for (int i = 0; i < skinList.Count; ++i)
        {
            if (skinList[i] != s)
            {
                skinList[i].HighlightSprite(false);
                skinList[i].isSelected = false;
            }
            else
            {
                skinList[i].HighlightSprite(true);
                skinList[i].isSelected = true;
            }
        }
    }

    private void OnDestroy()
    {
        // GameManager_.Instance.tracker.BoughtSkinIds = BoughtSkinIds;
        // GameManager_.Instance.tracker.BoughtItemStates = BoughtItemStates;
    }

}
