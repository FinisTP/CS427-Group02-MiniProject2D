using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Image EnergyBar;
    public GameObject HUD;
    public Joystick JoyStick;
    public RectTransform FxHolder;

    public TMP_Text ScoreText;
    public Image[] Hearts;
    public Image[] StageImages;
    public Image ProgressBar;

    public GameObject PauseMenuObject;
    public GameObject WonMenuObject;
    public GameObject LostMenuObject;

    public Sprite Flag;

    float lerpSpeed;

    private void Start()
    {
        ScoreText.SetText(GameManager_.Instance.Score.ToString("000,000,000"));
        lerpSpeed = 3f * Time.deltaTime;
        if (!Application.isMobilePlatform)
        {
            JoyStick.gameObject.SetActive(false);
        }
        ShowPauseMenu(false);
    }

    public void SetProgressSprite(Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; ++i)
        {
            StageImages[i].sprite = sprites[i];
        }
        for (int i = sprites.Length; i < StageImages.Length; ++i)
        {
            StageImages[i].sprite = Flag;
        }
    }

    public void SetHeartSprite(Sprite sprite)
    {
        for (int i = 0; i < Hearts.Length; ++i)
        {
            Hearts[i].sprite = sprite;
        }
    }

    public void ToggleHUD(bool val)
    {
        HUD.SetActive(val);
    }

    public void UpdateScore(int score)
    {
        ScoreText.SetText(score.ToString("000,000,000"));
    }
    // progress bar is split in 4 parts, equally 25% each, passing each part will unlock new prey
    public void UpdateProgress(float currentProgress, float baseProgress, float maxProgress, int stage)
    {
        ProgressBar.fillAmount = (1/3f) * stage + (1/3f) * (currentProgress - baseProgress) / (maxProgress - baseProgress);
        for (int i = 0; i <= stage; ++i)
        {
            StageImages[i].color = new Color(1, 1, 1, 1);
        }
        for (int i = stage+1; i < StageImages.Length-1; ++i)
        {
            StageImages[i].color = new Color(0, 0, 0, 1);
        }
        if (ProgressBar.fillAmount >= 0.99f) StageImages[StageImages.Length - 1].color = new Color(1, 1, 1, 1);
        else StageImages[StageImages.Length - 1].color = new Color(0, 0, 0, 1);
        // print($"Current: {currentProgress},Base: {baseProgress}, Max: {maxProgress}");

    }

    public void UpdateEnergy(float currentEnergy, float maxEnergy)
    {
        float progress = currentEnergy / maxEnergy;
        EnergyBar.fillAmount = Mathf.Lerp(EnergyBar.fillAmount, progress, lerpSpeed);
        Color healthColor = Color.Lerp(Color.red, Color.green, (progress));
        EnergyBar.color = healthColor;
        // FxHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, -progress * 360));
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

    public void Resume()
    {
        ShowPauseMenu(false);
    }

    void Update()
    {
        if (GameManager_.Instance.PlayableScene && Input.GetKeyDown(KeyCode.Escape))
            ShowPauseMenu(!PauseMenuObject.activeSelf);
    }

    public void ShowPauseMenu(bool b)
    {
        if (PauseMenuObject.activeSelf == b || WonMenuObject.activeSelf == true || LostMenuObject.activeSelf == true)
            return;

        // GameManager_.Instance.IsRunningGame = !b;
        PauseMenuObject.SetActive(b);
        Time.timeScale = b ? 0f : 1f;
    }

    public void ShowWinMenu(bool b, int star)
    {
        if (WonMenuObject.activeSelf == b)
            return;

        // GameManager_.Instance.IsRunningGame = !b;
        WonMenuObject.SetActive(b);
        // Time.timeScale = b ? 0f : 1f;
        if (!b)
        {
            Time.timeScale = 1f;
            return;
        }
         
        ToggleHUD(false);
        WonMenuObject.GetComponent<WinMenu>().PlayWinMenu(star, GameManager_.Instance.Score, GameManager_.Instance.Score / 1000);
        
    }

    public void ShowLoseMenu(bool b)
    {
        if (LostMenuObject.activeSelf == b)
            return;

        // GameManager_.Instance.IsRunningGame = !b;
        LostMenuObject.SetActive(b);
        if (!b)
        {
            Time.timeScale = 1f;
            return;
        }
        ToggleHUD(false);
        LostMenuObject.GetComponent<LoseMenu>().PlayLoseMenu(GameManager_.Instance.Score, GameManager_.Instance.Score / 2000);
        // Time.timeScale = b ? 0f : 1f;
    }

    public void ToLevelSelect()
    {
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        HideInterface();
    }

    public void ToMenu()
    {
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        HideInterface();
    }

    public void Retry()
    {
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        HideInterface();
    }

    public void NextLevel()
    {
        GameManager_.Instance.SoundPlayer.PlayClip("Button");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        Time.timeScale = 1f;
        HideInterface();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void HideInterface()
    {
        ShowPauseMenu(false);
        ShowLoseMenu(false);
        ShowWinMenu(false, 0);
    }


}
