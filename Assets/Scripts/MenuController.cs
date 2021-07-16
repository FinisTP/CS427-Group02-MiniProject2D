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
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        GameObject.FindObjectOfType<GameManager_>().LoadLevel(levelId);
    }

    public void ToNextPanel()
    {
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        if (currentPanel < LevelPanels.Length - 1)
        {
            LevelPanels[currentPanel++].SetActive(false);
            LevelPanels[currentPanel].SetActive(true);
        }
    }

    public void ToPreviousPanel()
    {
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        if (currentPanel > 0)
        {
            LevelPanels[currentPanel--].SetActive(false);
            LevelPanels[currentPanel].SetActive(true);
        }
    }

    public void QuitGame()
    {
        GameObject.FindObjectOfType<GameManager_>().QuitGame();
    }

    public void Continue()
    {
        int currentLevel = GameManager_.Instance.tracker.CurrentLevel;
        if (currentLevel <= 5)
        GameManager_.Instance.LoadLevel(currentLevel * 2 + 2);
        else GameManager_.Instance.LoadLevel(1);
    }


}
