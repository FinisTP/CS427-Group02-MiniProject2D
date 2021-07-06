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

    public int Score => score;
    public int Live => live;

    private int score = 0;
    private int live = 9;

    public const float NORTH_LIMIT = 20f;
    public const float SOUTH_LIMIT = -20f;
    public const float WEST_LIMIT = -25f;
    public const float EAST_LIMIT = 25f;

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
        
    }

    private void Start()
    {
        Player.transform.localScale = new Vector3(1, 1, 1) * Progress[0].Scale;
        currentProgress = 0;
        live = MaxLive;
        maxProgress = Progress[Progress.Length - 1].RequiredFood;
        UIPlayer.UpdateLives(live);
        UIPlayer.UpdateProgress(currentProgress, 0, Progress[Stage].RequiredFood, Stage);
    }

    private void Update()
    {
        CameraZoom();
        
    }

    public void CameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            currentZoom -= ZoomSpeed * Time.deltaTime;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            currentZoom += ZoomSpeed * Time.deltaTime;
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
        
    }

    public void AddProgress(float prog)
    {
        currentProgress += prog;
        if (currentProgress >= Progress[Stage].RequiredFood) ScaleUp();
        float baseProg = (Stage == 0 ? 0 : Progress[Stage - 1].RequiredFood);
        UIPlayer.UpdateProgress(currentProgress, baseProg, Progress[Stage].RequiredFood, Stage);
    }

    public void AddScore(int score)
    {
        if (this.score + score < int.MaxValue)
        this.score += score;
        UIPlayer.UpdateScore(this.score);
    }

    public void AddLive(int live)
    {
        if (this.live + live <= MaxLive) this.live += live;
        else this.live = MaxLive;
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
            Player.transform.localScale = new Vector3(1, 1, 1) * Progress[++Stage].Scale;
            currentProgress = Progress[Stage-1].RequiredFood;
            Player.GetComponent<PlayerController_Temp>().GrowParticle.Play();
            // ParticlePlayer.PlayEffect("Grow", Player.transform.position);
        } else
        {
            // win game
        }
        
    }

    

    

}
