using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

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

    public LevelStatistics[] LevelStats;

    public int Score => score;
    public int Live => live;

    private int score = 0;
    private int live = 9;

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
    public string CharacterName = "Simba";

    private float currentZoom = 4.5f;
    private float maxProgress = 0f;
    public int MaxLive = 7;

    public float boost = 0;
    public float boostUnit = 15;
    public float boostLimit = 100;
    private bool boosting = false;

    public bool PlayableScene = false;
    public bool Won = false;
    public bool Lost = false;

    public bool isNight = false;

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



    private void OnLevelWasLoaded(int level)
    {
        for (int i = 0; i < LevelStats.Length; ++i)
        {
            if (LevelStats[i].sceneId == level)
            {
                Player = FindObjectOfType<PlayerController>().gameObject;
                MainCamera = FindObjectOfType<CinemachineVirtualCamera>();
                LoadLevelStat(LevelStats[i]);
                ResetPlayerStatus();
                UIPlayer.ToggleHUD(true);
                PlayableScene = true;
                IsRunningGame = true;
                return;
            }
        }
        // not a playable scene
        UIPlayer.ToggleHUD(false);
        IsRunningGame = false;
        PlayableScene = false;
    }

    private void LoadLevelStat(LevelStatistics ls)
    {
        Progress = ls.Progression;
        MaxLive = ls.MaxLive;
        UIPlayer.SetProgressSprite(ls.StageSprites);
        UIPlayer.SetHeartSprite(ls.HeartIcon);
        CharacterName = ls.CharacterName;
        NORTH_LIMIT = ls.LevelTopLeft.y;
        SOUTH_LIMIT = ls.LevelBottomRight.y;
        WEST_LIMIT = ls.LevelTopLeft.x;
        EAST_LIMIT = ls.LevelBottomRight.x;
    }

    private void ResetPlayerStatus()
    {
        Player.transform.localScale = new Vector3(1, 1, 1) * Progress[0].Scale;
        maxProgress = Progress[Progress.Length - 1].RequiredFood;

        live = MaxLive;
        score = 0;
        currentProgress = 0;
        currentZoom = 4.5f;
        Stage = 0;
        Won = false;
        Lost = false;

        UIPlayer.UpdateLives(live);
        UIPlayer.UpdateProgress(currentProgress, 0, Progress[Stage].RequiredFood, Stage);
    }

    private void Update()
    {
        if (!IsRunningGame || Won || Lost) return;
        CameraZoom();

        if (!boosting)
        {
            boost += boostUnit * Time.deltaTime;
            if (boost >= boostLimit)
            {
                boost = boostLimit;
            }
        }
        Decay();

        UIPlayer.UpdateEnergy(boost, boostLimit);
        
    }
    
    public void Decay()
    {
        hungerCounter += Time.deltaTime;
        if (hungerCounter >= TimeBeforeHunger && !Won && !Lost && ((Stage == 0 && currentProgress - HungerRate * Time.deltaTime > 0)
            || (Stage > 0 && currentProgress - HungerRate * Time.deltaTime > Progress[Stage - 1].RequiredFood)))
        {
            AddProgress(-HungerRate * Time.deltaTime);
        }
    }

    public void LoadLevel(int levelId)
    {
        SceneManager.LoadScene(levelId);
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
            boost -= used;
            if (boost < 0) boost = 0;
            return true;
        }
        return false;
    }

    public void AddProgress(float prog)
    {
        if (currentProgress + prog < 0) currentProgress = 0;
        else if (currentProgress + prog > maxProgress)
        {
            currentProgress = maxProgress;
            Won = true;
            UIPlayer.ShowWinMenu(true);
            // win game
        }
        currentProgress += prog;
        if (currentProgress >= Progress[Stage].RequiredFood) ScaleUp();
        float baseProg = (Stage == 0 ? 0 : Progress[Stage - 1].RequiredFood);
        UIPlayer.UpdateProgress(currentProgress, baseProg, Progress[Stage].RequiredFood, Stage);
        if (prog > 0) hungerCounter = 0;
    }

    public void AddScore(int score)
    {
        if (this.score + score < int.MaxValue)
        this.score += score;
        UIPlayer.UpdateScore(this.score);
    }

    public void AddLive(int live)
    {
        if (this.live + live <= MaxLive && this.live + live > 0) this.live += live;
        else if (live > 0) this.live = MaxLive;
        else if (live < 0)
        {
            this.live = 0;
            Lost = true;
            UIPlayer.ShowLoseMenu(true);
        }
        UIPlayer.UpdateLives(this.live);
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
            // ParticlePlayer.PlayEffect("Grow", Player.transform.position);
        } 
    }
}
