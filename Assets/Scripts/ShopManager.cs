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

    public GameObject[] Tabs;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else Destroy(gameObject);
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
        for (int i = 0; i < Tabs.Length; ++i)
        {
            if (i != tabId) Tabs[i].SetActive(false);
            else Tabs[i].SetActive(true);
        }
    }

}
