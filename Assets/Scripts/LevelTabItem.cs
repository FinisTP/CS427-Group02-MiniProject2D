using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelTabItem : MonoBehaviour
{
    public int levelId;
    public TMP_Text levelScore;
    private void Start()
    {
        if (levelScore == null) return;
        if (GameManager_.Instance.tracker.HighScore.ContainsKey(levelId))
        {
            levelScore.text = GameManager_.Instance.tracker.HighScore[levelId].ToString("000,000,000");
        } else
        {
            levelScore.text = "Uncleared";
        }
    }
}
