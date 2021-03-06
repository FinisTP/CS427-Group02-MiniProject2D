using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

[System.Serializable]
public class PlayerProgression
{
    public float RequiredFood;
    public float Scale;
}
public class GameManager_ : MonoBehaviour
{
    public GameObject Player;
    public CinemachineVirtualCamera MainCamera;
    public SoundManager SoundPlayer;
    public ParticleManager ParticlePlayer;
    public UIManager UIPlayer;
    public Volume GlobalVolume;

    private ShadowsMidtonesHighlights SMH;
    

    public AnimatorOverrideController CharacterAnimation;

    public List<IPower> Powers = new List<IPower>();

    public LevelStatistics[] LevelStats;

    public int Coin => coin;

    public int Score => score;
    public int Live => live;

    private int score = 0;
    private int live = 9;
    private int coin = 2000;

    public float NORTH_LIMIT = 20f;
    public float SOUTH_LIMIT = -20f;
    public float WEST_LIMIT = -25f;
    public float EAST_LIMIT = 25f;

    public float HungerRate = 5f;
    public float TimeBeforeHunger = 5f;
    private float hungerCounter = 0f;

    public PlayerProgression[] Progress;

    public bool IsRunningGame = false;

    public int Stage = 0;
    private float currentProgress = 0;

    public float MinZoom = 4.5f;
    public float MaxZoom = 10f;
    public float ZoomSpeed = 10f;

    private float currentZoom = 4.5f;
    private float maxProgress = 0f;
    public int MaxLive = 7;

    private float comboTime = 2f;
    private float currentComboTime = 0f;
    private float multiplier = 1f;
    public int comboState = 0;

    public float boost = 0;
    public float boostUnit = 15;
    public float boostLimit = 100;
    private bool boosting = false;
    public float tranceBoost = 10f;

    public bool Won = false;
    public bool Lost = false;
    public bool PlayableScene = true;

    public float speedBoost = 0;
    public float dashTimeBoost = 0;
    public int shield = 0;
    private int currentShield = 0;
    public float scoreMultiplier = 1f;
    public float comboTimeAmplifer = 0f;

    public float TranceValue = 20f;
    public bool IsTrance = false;
    private float currentTranceValue = 0f;

    private LevelStatistics currentLevel;

    public RecordTracker tracker;

    private static GameManager_ instance = null;
    public static GameManager_ Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager_>();
                if (instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = "GameManager_";
                    instance = go.AddComponent<GameManager_>();

                    // DontDestroyOnLoad(go);
                }

            }
            return instance;
        }
    }

    private void Start()
    {
        coin = tracker.CurrentMoney;
        speedBoost = 0.5f;
        dashTimeBoost = 0.5f;
        scoreMultiplier = 0.5f;
        comboTimeAmplifer = 0.5f;
        foreach (KeyValuePair<int,int> kvp in tracker.BoughtItemStates)
        {
            switch(kvp.Key)
            {
                case 1:
                    // speed
                    speedBoost = 0.5f + 0.5f * kvp.Value;
                    break;
                case 2:
                    dashTimeBoost = 0.5f + 0.5f * kvp.Value;
                    break;
                //dash
                case 3:
                    scoreMultiplier = 0.5f + 0.5f * kvp.Value;
                    break;
                //score
                case 4:
                    comboTimeAmplifer = 0.5f + 0.5f * kvp.Value;
                    break;
                    // combo
            }
        }
    }


    private void OnLevelWasLoaded(int level)
    {
        print("level" + level);
        for (int i = 0; i < LevelStats.Length; ++i)
        {
            if (LevelStats[i].sceneId == level)
            {
                Player = FindObjectOfType<PlayerController>().gameObject;
                Invoke("SetPlayerAnimation", 0.5f);
                MainCamera = FindObjectOfType<CinemachineVirtualCamera>();
                LoadLevelStat(LevelStats[i]);
                ResetPlayerStatus();
                UIPlayer.ToggleHUD(true);
                PlayableScene = true;
                IsRunningGame = true;
                SoundPlayer.PlayBGM(LevelStats[i].BGM, LevelStats[i].Ambience1, LevelStats[i].Ambience2);
                return;
            }
        }
        // not a playable scene
        UIPlayer.ToggleHUD(false);
        IsRunningGame = false;
        PlayableScene = false;

        if (level == 0)
        {
            SoundPlayer.PlayTitleClip();
        }

    }

    private void SetPlayerAnimation()
    {
        Player.GetComponent<PlayerController>().SetAnimation(CharacterAnimation);
    }

    private void LoadLevelStat(LevelStatistics ls)
    {
        currentLevel = ls;
        Progress = ls.Progression;
        MaxLive = ls.MaxLive;
        UIPlayer.SetProgressSprite(ls.StageSprites);
        UIPlayer.SetHeartSprite(ls.HeartIcon);
        NORTH_LIMIT = ls.LevelTopLeft.y;
        SOUTH_LIMIT = ls.LevelBottomRight.y;
        WEST_LIMIT = ls.LevelTopLeft.x;
        EAST_LIMIT = ls.LevelBottomRight.x;
    }

    private void ResetPlayerStatus()
    {
        Player.transform.localScale = new Vector3(1, 1, 1) * Progress[0].Scale;
        maxProgress = Progress[Progress.Length - 1].RequiredFood;

        if (MaxLive >= 4) live = MaxLive - 2;
        else live = MaxLive;
        score = 0;
        currentProgress = 0;
        currentZoom = 4.5f;
        currentShield = shield;
        Stage = 0;
        Won = false;
        Lost = false;

        UIPlayer.UpdateEnergy(0, boostLimit);
        UIPlayer.UpdateScore(0);
        UIPlayer.UpdateLives(live);
        UIPlayer.UpdateProgress(currentProgress, 0, Progress[Stage].RequiredFood, Stage);
        Player.GetComponent<PlayerController>().Vortex.SetActive(false);

        SoundPlayer.PlayBGM(currentLevel.BGM, currentLevel.Ambience1, currentLevel.Ambience2);
    }

    private void Update()
    {
        if (!IsRunningGame || Won || Lost) return;

        UIPlayer.UpdateTrance(currentTranceValue, TranceValue);
        if (IsTrance)
        {
            tranceBoost = 10f;
            currentTranceValue -= 2f * Time.deltaTime * (1/Time.timeScale);
            if (currentTranceValue < 0)
            {
                ClearTrance();
            }
        }

        CameraZoom();

        currentComboTime += Time.deltaTime;
        if (currentComboTime >= comboTime + comboTimeAmplifer)
        {
            Player.GetComponent<PlayerController>().Vortex.SetActive(false);
        }

        if (!boosting)
        {
            boost += (boostUnit + dashTimeBoost * 5) * Time.deltaTime;
            if (boost >= boostLimit)
            {
                boost = boostLimit;
            }
        }
        Decay();

        UIPlayer.UpdateEnergy(boost, boostLimit);

        for (int i = 0; i < Powers.Count; ++i)
        {
            Powers[i].UpdateEffect();
        }
        
    }

    public void ActivateTrance()
    {
        UIPlayer.PlayTrance();
        SoundPlayer.PlayTrance(true);
        currentTranceValue = TranceValue;
        
        Time.timeScale = 0.5f;
        SMH.midtones.value = new Vector4(0, 0, 0, -1f);
        IsTrance = true;
        
    }

    public void ClearTrance()
    {
        currentTranceValue = 0f;
        Time.timeScale = 1f;
        SMH.midtones.value = new Vector4(1, 1, 1, 0f);
        IsTrance = false;
        tranceBoost = 0f;
        SoundPlayer.PlayTrance(false);
    }
    
    public void Decay()
    {
        if (!IsTrance)
        {
            hungerCounter += Time.deltaTime;
            //if (hungerCounter >= TimeBeforeHunger && !Won && !Lost && ((Stage == 0 && currentProgress - HungerRate * Time.deltaTime > 0)
            //    || (Stage > 0 && currentProgress - HungerRate * Time.deltaTime > Progress[Stage - 1].RequiredFood)))
            //{
            //    AddProgress(-HungerRate * Time.deltaTime);
            //}
            if (hungerCounter >= TimeBeforeHunger && !Won && !Lost)
            {
                currentTranceValue -= Time.deltaTime;
                if (currentTranceValue < 0) currentTranceValue = 0;
            }
        }
    }

    public void LoadLevel(int levelId)
    {
        StartCoroutine(LoadAsynchronously(levelId));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            // print(operation.progress);
            yield return null;
        }
        // yield return new WaitForSeconds(2f);
    }

    public void CameraZoom()
    {
        float tempZoom = currentZoom;
        if (Input.mouseScrollDelta.y > 0)
        {
            currentZoom = Mathf.Lerp(currentZoom, tempZoom - ZoomSpeed * Time.deltaTime, 1);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            currentZoom = Mathf.Lerp(currentZoom, tempZoom + ZoomSpeed * Time.deltaTime, 1);
        }
        currentZoom = Mathf.Clamp(currentZoom, MinZoom, MaxZoom);
        MainCamera.m_Lens.OrthographicSize = currentZoom;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        UIPlayer.ToggleHUD(false);
        IsRunningGame = false;
        GlobalVolume.profile.TryGet<ShadowsMidtonesHighlights>(out SMH);
        ClearTrance();
    }

    public void ContinueChargingBoost()
    {
        boosting = false;
    }

    public void StopChargingBoost()
    {
        boosting = true;
    }
    public bool UseBoost(float used)
    {
        if (boost > 0)
        {
            float reduced = used;
            boost -= reduced;
            if (boost < 0) boost = 0;
            return true;
        }
        return false;
    }

    public void ResetStatus()
    {
        coin = 0;
        speedBoost = 0;
        dashTimeBoost = 0;
        shield = 0;
        currentShield = 0;
        scoreMultiplier = 1f;
        comboTimeAmplifer = 0f;
    }

    public void AddProgress(float prog)
    {
        if (Won || Lost) return;
        if (currentProgress + prog < 0) currentProgress = 0;
        else if (currentProgress + prog > maxProgress)
        {
            currentProgress = maxProgress;
            Won = true;
            IsRunningGame = false;
            StartCoroutine(TriggerWinGame());
            // win game
        }
        currentProgress += prog;
        if (currentProgress >= Progress[Stage].RequiredFood) ScaleUp();
        float baseProg = (Stage == 0 ? 0 : Progress[Stage - 1].RequiredFood);
        UIPlayer.UpdateProgress(currentProgress, baseProg, Progress[Stage].RequiredFood, Stage);
        if (prog > 0) hungerCounter = 0;
    }

    public int AddScore(int score)
    {
        if (Won || Lost) return 0;
        if (!IsTrance)
        {
            currentTranceValue += 1f;
            if (currentTranceValue > TranceValue)
            {
                ActivateTrance();
            }
        }
        
        if (this.score + score < int.MaxValue)
        {
            if (currentComboTime < (comboTime + comboTimeAmplifer))
            {
                Player.GetComponent<PlayerController>().Vortex.SetActive(true);
                multiplier += 0.5f;
            } else
            {
                multiplier = 0.5f;
                comboState = 0;
            }
            currentComboTime = 0;
        }
        int weightedScore = (int)((float)(score) * multiplier * scoreMultiplier);
        print(score);
        print(weightedScore);
        UIPlayer.UpdateScore(this.score);
        this.score += weightedScore;
        return weightedScore;
        
    }

    public bool AddLive(int live)
    {
        if (shield > 0)
        {
            shield--;
            return false;
        }

        if (Won || Lost) return true;
        if (this.live + live <= MaxLive && this.live + live > 0) this.live += live;
        else if (live > 0) this.live = MaxLive;
        else if (live < 0)
        {
            this.live = 0;
            Lost = true;
            IsRunningGame = false;
            //UIPlayer.ShowLoseMenu(true);
            StartCoroutine(TriggerGameOver());
            // lose game
        }
        UIPlayer.UpdateLives(this.live);
        return true;
    }

    public void ResetProgress()
    {
        if (Stage == 0) currentProgress = 0f;
        else currentProgress = Progress[Stage-1].RequiredFood;
        float baseProg = (Stage == 0 ? 0 : Progress[Stage - 1].RequiredFood);
        UIPlayer.UpdateProgress(currentProgress, baseProg, Progress[Stage].RequiredFood, Stage);
    }

    public void ScaleUp()
    {
        if (Stage + 1 < Progress.Length)
        {
            Player.transform.localScale = new Vector3(Mathf.Sign(Player.transform.localScale.x) * 1, 1, 1) * Progress[++Stage].Scale;
            currentProgress = Progress[Stage-1].RequiredFood;
            Player.GetComponent<PlayerController>().GrowParticle.Play();
            currentZoom = 0.5f * Stage + 4.5f;
            SoundPlayer.PlayClip("LevelUp");
            // ParticlePlayer.PlayEffect("Grow", Player.transform.position);
        } 
    }

    IEnumerator TriggerGameOver()
    {
        AddCoin((int)Mathf.Round(score / 2000));
        tracker.UpdateHighScore(currentLevel.sceneOrder, score);
        yield return new WaitForSeconds(2f);
        ClearTrance();
        SoundPlayer.StopAllTrack();
        SoundPlayer.PlayClip("Defeat");
        UIPlayer.ShowLoseMenu(true);
        
    }

    IEnumerator TriggerWinGame()
    {
        tracker.CurrentLevel = currentLevel.sceneOrder;
        AddCoin((int)Mathf.Round(score / 1000));
        tracker.UpdateHighScore(currentLevel.sceneOrder, score);
        yield return new WaitForSeconds(2f);
        ClearTrance();
        SoundPlayer.StopAllTrack();
        SoundPlayer.PlayClip("Victory");
        print((int)Mathf.Round(score / 200000 * 5f));
        UIPlayer.ShowWinMenu(true, (int)Mathf.Round(score/200000 * 5f));
    }

    public void Boost(bool state)
    {
        Player.GetComponent<PlayerController>().SetBoosting(state);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddPower(IPower power)
    {
        Powers.Add(power);
        power.InitializeEffect();
    }

    public void RemovePower(IPower power)
    {
        Powers.Remove(power);
        power.EndEffect();
    }

    public void AddCoin(int num)
    {
        if (coin + num >= 0 && coin + num <= 999999)
        {
            coin += num;
            tracker.CurrentMoney = coin;
        }
    }

    private void OnApplicationQuit()
    {
        tracker.SaveProgress();
    }

}
