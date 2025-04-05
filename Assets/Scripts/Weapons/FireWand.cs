using MathHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireWand : WeaponBase
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PoolContainer fireballs;
    [SerializeField]
    private float angleOffsets;
    public void Awake()
    {
        fireballs = GetComponent<PoolContainer>();
        base.Awake();
        AttackAction += Fire;
    }
    private float randAngle;
    private void Fire()
    {
        randAngle = Random.Range(0, 360f);
        Vector3 randomDirection;
        // spawnPoint.forward = randomDirection;
        for (int i = 0; i < stats.SimulataneousAmount; i++)
        {
            randomDirection = VectorHelper.GetDirectionAtAngle(randAngle);
            GameObject fire = fireballs.Retrieve(spawnPoint.position, spawnPoint.rotation);
            fire.GetComponent<Fireballprojectile>().Spawn(stats, randomDirection);
            randAngle += angleOffsets;
        }
    }


}
