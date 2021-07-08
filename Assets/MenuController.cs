using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject[] LevelPanels;
    private int currentPanel = 0;

    private void Start()
    {
        if (LevelPanels.Length > 0)
        {
            for (int i = 0; i < LevelPanels.Length; ++i)
            {
                LevelPanels[i].SetActive(false);
            }
            LevelPanels[0].SetActive(true);
        }
        
    }
    public void LoadLevel (int levelId)
    {
        GameObject.FindObjectOfType<GameManager_>().LoadLevel(levelId);
    }

    public void ToNextPanel()
    {
        if (currentPanel < LevelPanels.Length - 1)
        {
            LevelPanels[currentPanel++].SetActive(false);
            LevelPanels[currentPanel].SetActive(true);
        }
    }

    public void ToPreviousPanel()
    {
        if (currentPanel > 0)
        {
            LevelPanels[currentPanel--].SetActive(false);
            LevelPanels[currentPanel].SetActive(true);
        }
    }
}
