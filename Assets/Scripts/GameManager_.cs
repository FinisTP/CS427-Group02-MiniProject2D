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

    public int Score = 0;

    public const float NORTH_LIMIT = 20f;
    public const float SOUTH_LIMIT = -20f;
    public const float WEST_LIMIT = -25f;
    public const float EAST_LIMIT = 25f;

    public PlayerProgression[] Progress;

    public bool IsRunningGame = false;

    public int Stage = 0;
    public float currentProgress = 0;

    public float MinZoom = 4.5f;
    public float MaxZoom = 10f;
    public float ZoomSpeed = 10f;

    private float CurrentZoom = 4.5f;

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
        Player.transform.localScale = new Vector3(1, 1, 1) * Progress[0].Scale;
        currentProgress = 0;
    }

    private void Update()
    {
        CameraZoom();
    }

    public void CameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            CurrentZoom -= ZoomSpeed * Time.deltaTime;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            CurrentZoom += ZoomSpeed * Time.deltaTime;
        }
        CurrentZoom = Mathf.Clamp(CurrentZoom, MinZoom, MaxZoom);
        MainCamera.m_Lens.OrthographicSize = CurrentZoom;
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
    }

    public void ResetProgress()
    {
        currentProgress = 0;
    }

    public void ScaleUp()
    {
        if (Stage + 1 < Progress.Length)
        {
            Player.transform.localScale = new Vector3(1, 1, 1) * Progress[++Stage].Scale;
            currentProgress = 0;
        } else
        {
            // win game
        }
        
    }

    

    

}
