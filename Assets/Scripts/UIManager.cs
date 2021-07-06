using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Canvas MenuCanvas;
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject PauseMenu;

    public Canvas HUD;

    public TMP_Text ScoreText;
    public Image[] Hearts;
    public Image[] StageImages;
    public Image ProgressBar;

    private void Start()
    {
        ScoreText.SetText(GameManager_.Instance.Score.ToString("000,000,000"));
    }

    public void ToggleMenuCanvas(bool val)
    {
        MenuCanvas.gameObject.SetActive(val);
    }

    public void ToggleMainMenu(bool val)
    {
        MainMenu.SetActive(val);
    }

    public void ToggleSettingsMenu(bool val)
    {
        SettingsMenu.SetActive(val);
    }

    public void TogglePauseMenu(bool val)
    {
        PauseMenu.SetActive(val);
    }


    public void UpdateScore(int score)
    {
        ScoreText.SetText(score.ToString("000,000,000"));
    }
    // progress bar is split in 4 parts, equally 25% each, passing each part will unlock new prey
    public void UpdateProgress(float currentProgress, float baseProgress, float maxProgress, int stage)
    {
        ProgressBar.fillAmount = 0.33f * stage + 0.33f * (currentProgress - baseProgress) / maxProgress;
        for (int i = 0; i <= stage; ++i)
        {
            StageImages[i].color = new Color(1, 1, 1, 1);
        }
        for (int i = stage+1; i < StageImages.Length; ++i)
        {
            StageImages[i].color = new Color(0, 0, 0, 1);
        }
            
    }

    public void UpdateLives(int live)
    {
        for (int i = 0; i < live; ++i)
        {
            Hearts[i].color = new Color(1, 1, 1, 1);
        }
        for (int i = live; i < Hearts.Length; ++i)
        {
            Hearts[i].color = new Color(1, 1, 1, 0);
        }
    }
}
