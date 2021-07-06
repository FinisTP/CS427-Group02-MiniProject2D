using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;


public class GameManager_ : MonoBehaviour
{
    public GameObject Player;
    public CinemachineVirtualCamera MainCamera;
    public SoundManager SoundPlayer;
    public ParticleManager ParticlePlayer;
    public UIManager UIPlayer;

    public const float NORTH_LIMIT = 20f;
    public const float SOUTH_LIMIT = -20f;
    public const float WEST_LIMIT = -25f;
    public const float EAST_LIMIT = 25f;

    public bool IsRunningGame = false;

    public int Stage = 1;
    public float _progress = 0f;

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

    public void ScaleUp()
    {
        MainCamera.m_Lens.OrthographicSize += 0.5f;
        Player.transform.localScale *= 2;
        Stage++;
        _progress = 0f;
    }

    private void Update()
    {
        
    }

    

}
