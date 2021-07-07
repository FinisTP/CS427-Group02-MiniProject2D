using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnProgression
{
    public int MinSpawnCount;
    public int MaxSpawnCount;
}

[System.Serializable]
public class EnemyStatistics
{
    public GameObject EnemyObject;
    public string name;
    public int MaxCount;
    public SpawnProgression[] Progress;
    public int CurrentCount;
} 

public class ObjectSpawner : MonoBehaviour
{
    public float SpawnDelay = 5f;
    private Transform Holder;

    public EnemyStatistics[] EnemyPrefabs;
    private Dictionary<string, List<GameObject>> enemyPool = new Dictionary<string, List<GameObject>>();

    private void Start()
    {
        GameObject spawned;
        for (int i = 0; i < EnemyPrefabs.Length; ++i)
        {
            enemyPool[EnemyPrefabs[i].name] = new List<GameObject>();
            for (int j = 0; j < EnemyPrefabs[i].MaxCount; ++j)
            {
                spawned = Instantiate(EnemyPrefabs[i].EnemyObject);
                spawned.SetActive(false);
                spawned.transform.parent = Holder;
                enemyPool[EnemyPrefabs[i].name].Add(spawned);
            }
            EnemyPrefabs[i].CurrentCount = 0;
            
        }
        Holder = transform;
        InvokeRepeating("SpawnWave", 2f, SpawnDelay);
    }

    private EnemyStatistics GetEnemyFromString(string enemyName)
    {
        for (int i = 0; i < EnemyPrefabs.Length; ++i)
        {
            if (EnemyPrefabs[i].name.Equals(enemyName))
            {
                return EnemyPrefabs[i];
            }
        }
        return null;
    }

    public void UpdateStatistics()
    {
        for (int i = 0; i < EnemyPrefabs.Length; ++i)
        {
            int currentCount = 0;
            EnemyStatistics stat = EnemyPrefabs[i];
            for (int j = 0; j < enemyPool[stat.name].Count; ++j)
            {
                if (enemyPool[stat.name][j].activeInHierarchy) currentCount++;
            }
            stat.CurrentCount = currentCount;
        }
    }

    public GameObject SpawnEnemy(string enemyName, Vector2 spawnPos)
    {

        if (enemyPool.ContainsKey(enemyName))
        {
            EnemyStatistics es = GetEnemyFromString(enemyName);
            for (int i = 0; i < enemyPool[enemyName].Count; ++i)
            {
                if (enemyPool[enemyName][i].activeInHierarchy == false)
                {
                    enemyPool[enemyName][i].SetActive(true);
                    enemyPool[enemyName][i].transform.position = spawnPos;
                    // es.CurrentCount++;
                    return enemyPool[enemyName][i];
                }
            }
        }
        return null;
    }

    public void SpawnWave()
    {
        UpdateStatistics();
        for (int i = 0; i < EnemyPrefabs.Length; ++i)
        {
            EnemyStatistics es = EnemyPrefabs[i];
            int countToSpawn = Random.Range(es.Progress[GameManager_.Instance.Stage].MinSpawnCount, es.Progress[GameManager_.Instance.Stage].MaxSpawnCount);
            if (es.CurrentCount <= countToSpawn)
            {
                for (int j = 0; j < countToSpawn - es.CurrentCount; ++j)
                {
                    int r = Random.Range(0, 4);
                    Vector2 spawnPos = Vector2.zero;
                    switch (r)
                    {
                        case 0: // North
                            spawnPos = new Vector2(Random.Range(GameManager_.WEST_LIMIT, GameManager_.EAST_LIMIT), GameManager_.NORTH_LIMIT); 
                            break;
                        case 1: // South
                            spawnPos = new Vector2(Random.Range(GameManager_.WEST_LIMIT, GameManager_.EAST_LIMIT), GameManager_.SOUTH_LIMIT);
                            break;
                        case 2: // West
                            spawnPos = new Vector2(GameManager_.WEST_LIMIT, Random.Range(GameManager_.SOUTH_LIMIT, GameManager_.NORTH_LIMIT));
                            break;
                        case 3: // East
                            spawnPos = new Vector2(GameManager_.EAST_LIMIT, Random.Range(GameManager_.SOUTH_LIMIT, GameManager_.NORTH_LIMIT));
                            break;
                        default:
                            break;
                    }
                    SpawnEnemy(es.name, spawnPos);
                }
            }
            
        }
    }
}
