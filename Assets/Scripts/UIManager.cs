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

    public string Level;
    public int MoveCount;
    public int MinMoveRequired;

    public TMP_Text LevelText;
    public TMP_Text MoveCountText;

    private void Start()
    {
        LevelText.text = "Level " + Level;
        MoveCountText.text = $"Move Count: {0}/{MinMoveRequired}";
    }

    public void UpdateLevel(string name)
    {
        Level = name;
        LevelText.text = "Level " + Level;
    }
    public void UpdateMove(int count = -1)
    {
        if (count == -1) MoveCount++;
        else  MoveCount = count;
        MoveCountText.text = $"Move Count: {MoveCount}/{MinMoveRequired}";
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
}
