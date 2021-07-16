using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Record
{
    public int CurrentLevel;
    public List<int> BoughtSkinIds;
    public Dictionary<int, int> BoughtItemStates;
    public Dictionary<int, int> HighScore;
    public int CurrentMoney;

    public Record(RecordTracker tracker) {
        CurrentLevel = tracker.CurrentLevel;
        BoughtSkinIds = tracker.BoughtSkinIds;
        BoughtItemStates = tracker.BoughtItemStates;
        HighScore = tracker.HighScore;
        CurrentMoney = tracker.CurrentMoney;
    }
}

public class RecordTracker : MonoBehaviour
{
    public int CurrentLevel = 0;
    public List<int> BoughtSkinIds;
    public Dictionary<int, int> BoughtItemStates;
    public Dictionary<int, int> HighScore;
    public int CurrentMoney = 200;

    private void Awake()
    {
        BoughtSkinIds = new List<int>();
        HighScore = new Dictionary<int, int>();
        BoughtItemStates = new Dictionary<int, int>();
        LoadProgress();
    }

    public void SaveProgress()
    {
        SaveSystem.SaveProgress(this);
    }

    public void LoadProgress()
    {
        Record data = SaveSystem.LoadProgress();

        if (data != null)
        {
            CurrentLevel = data.CurrentLevel;
            BoughtSkinIds = data.BoughtSkinIds;
            BoughtItemStates = data.BoughtItemStates;
            HighScore = data.HighScore;
            CurrentMoney = data.CurrentMoney;
        }

    }

    public void UpdateHighScore(int levelId, int score)
    {
        if (HighScore.ContainsKey(levelId))
        {
            HighScore[levelId] = Mathf.Max(HighScore[levelId], score);
        }
        else HighScore[levelId] = score;
    }

    public void ClearProgress()
    {
        BoughtSkinIds.Clear();
        HighScore.Clear();
        BoughtItemStates.Clear();
        CurrentLevel = 0;
        CurrentMoney = 0;
        GameManager_.Instance.ResetStatus();
    }

}
