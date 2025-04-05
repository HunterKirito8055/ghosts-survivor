using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagicWand : WeaponBase
{
    [SerializeField] private PrefabContainer fireObject;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<MagicballProjectile> magicBallsPoolList = new List<MagicballProjectile>();
    private void Awake()
    {
        base.Awake();
    }
    private void OnEnable()
    {
        AttackAction += Fire;
    }
    private void OnDisable()
    {
        AttackAction -= Fire;
    }
 

    private GameObject enemy;
    public void Fire()
    {
        enemy = SelectEnemyInRange(spawnPoint.position);
        if (enemy != null)
        {
            MagicballProjectile magicBall = GetMagicBall();
            magicBall.LoadBall(spawnPoint.position, stats.Damage, stats.Speed, (enemy.transform.position - spawnPoint.position));
        }
    }
    private MagicballProjectile GetMagicBall()
    {
        MagicballProjectile magicBall = null;
        if (magicBallsPoolList != null)
        {
            if (magicBallsPoolList.Count > 0)
            {
                magicBall = magicBallsPoolList.FirstOrDefault(x => !x.isActiveAndEnabled || !x.gameObject.activeInHierarchy);
            }
        }
        if (magicBall == null)
        {
            if (fireObject.prefabObject != null)
            {
                magicBall = Instantiate(fireObject.prefabObject, spawnPoint.position, Quaternion.identity).GetComponent<MagicballProjectile>();
                if (magicBallsPoolList == null)
                {
                    magicBallsPoolList = new List<MagicballProjectile>();
                }
                magicBall.transform.SetParent(transform);
                magicBallsPoolList.Add(magicBall);
            }
        }
        magicBall.gameObject.SetActive(true);
        return magicBall;
    }
 
}
