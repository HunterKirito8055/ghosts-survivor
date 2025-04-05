using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using AarquieSolutions.DevelopmentConsole;
using UnityEngine.Rendering.Universal;
using ARandom = AarquieSolutions.Utility.RandomExtension;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private List<EntityStatsBase> enemyEntities;
    [SerializeField] private List<PoolContainer> enemyPool;
    [SerializeField] private List<Transform> spawnPointsList = new List<Transform>();

    [SerializeField] private Vector2 spawnCount = new Vector2(1, 15);
    [SerializeField] private Vector2 spawnTime = new Vector2(0.3f, 5f);

    [SerializeField] private float spawnFrequency = 1;

    [SerializeField] private Player player;


    [Range(0, 1f)]
    private float verticalRange, horizontalRange;

    private Vector3 viewportToWorldPoint;

    /// <summary>
    /// distance away from camera view
    /// </summary>
    private float borderOffset;

    [SerializeField]
    private Vector3 spawnPoint;

    [SerializeField] private FloatReferencer stageTimer;

    private float counter = 0;
    private List<GameObject> activeObjects = new List<GameObject>();
    public List<GameObject> ActiveGameObjects
    {
        get
        {
            activeObjects = new List<GameObject>();
            foreach (PoolContainer poolContainer in enemyPool)
            {
                if (poolContainer.poolList.Count > 0)
                {
                    activeObjects.AddRange(poolContainer.ActiveObjects);
                }
            }
            return activeObjects;
        }
    }
    private void Start()
    {
        DevLogConsole.AddCommand<int>("monster", "enter 1-13 monster number", OnlyMonster);
        DevLogConsole.AddCommand("all", "all monsters", AllMonsters);
        StartCoroutine(SpawnMonsters());
        enemies = new List<Enemy>();
    }
    private void Update()
    {
        counter += Time.deltaTime;
        if (counter >= spawnFrequency)
        {
            counter = 0;
            StartCoroutine(SpawnMonsters());
        }
    }
    [Range(0, 15)]
    private int monsterIndex = -1;
    public void AllMonsters()
    {
        monsterIndex = -1;
    }
    public void OnlyMonster(int n)
    {
        monsterIndex = n - 1;
    }
    /// <summary>
    /// Spawns Enemies with number of count and spawn time differences
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnMonsters()
    {
        yield return null;

        int numOfMonsters = (int)Random.Range(1, enemyPool.Capacity);
        Dictionary<int, int> monstersIndexes = new Dictionary<int, int>();
        for (int i = 0; i < numOfMonsters; i++)
        {
            int random = (int)Random.Range(1, enemyPool.Capacity);
            while (monstersIndexes.ContainsValue(random))
            {
                random = (int)Random.Range(1, enemyPool.Capacity);
            }
            monstersIndexes.Add(i, random);
        }
        foreach ((int key, int value) in monstersIndexes)
        {
            int spawnNumbers = (int)Random.Range(spawnCount.x, spawnCount.y);
            int timeIntervals = (int)Random.Range(spawnTime.x, spawnTime.y);


            for (int i = 0; i < spawnNumbers; i++)
            {
                Vector3 spawnpoint = GetSpawnPointOnGround;

                GameObject enemy = null;
              
                if (monsterIndex != -1)
                {
                    enemy = enemyPool[monsterIndex].Retrieve(spawnpoint, Quaternion.identity);
                }
                else
                {
                    if (enemyPool[value].Prefab.GetComponent<Enemy>().Type == EnemyType.BOSS)
                    {
                        if (stageTimer.Reference <= 180 && enemies.FindAll(x => x.Type == EnemyType.BOSS).Count >= 2)
                        {
                            goto Skip;
                        }
                    }
                    enemy = enemyPool[value].Retrieve(spawnpoint, Quaternion.identity);
                }

                Enemy enem = enemy.GetComponent<Enemy>();
                if (!enemies.Contains(enem))
                {
                    enemies.Add(enem);
                }
                enemy.SetActive(true);
                yield return new WaitForSeconds(timeIntervals);
            }
        Skip: ;
        }

    }
    public List<Enemy> enemies;
    private Transform GetSpawnPoint()
    {
        return spawnPointsList[Random.Range(0, spawnPointsList.Capacity)];
    }
    /// <summary>
    /// Gets world position of spawn point on ground that is out of view from Camera
    /// </summary>
    private Vector3 GetSpawnPointOnGround
    {
        get
        {
            if (ARandom.Chance(0.5f))
            {
                horizontalRange = ARandom.Chance(0.5f) ? 0 : 1;
                borderOffset = 5f;
                verticalRange = Random.Range(0, 1f);
            }
            else
            {
                verticalRange = ARandom.Chance(0.5f) ? 0 : 1;
                borderOffset = verticalRange == 0 ? 5f : 1f;
                horizontalRange = Random.Range(0, 1f);
            }
            viewportToWorldPoint = camera.ViewportToWorldPoint(new Vector3(horizontalRange, verticalRange, 1));
            RaycastHit hit;
            Physics.Raycast(camera.transform.position, (viewportToWorldPoint - camera.transform.position).normalized, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"));
            viewportToWorldPoint = (hit.point + player.transform.position).normalized;
            viewportToWorldPoint.y = 0;
            spawnPoint = hit.point + viewportToWorldPoint * borderOffset;
            spawnPoint.y = 0;
            return spawnPoint;
        }
    }

}
