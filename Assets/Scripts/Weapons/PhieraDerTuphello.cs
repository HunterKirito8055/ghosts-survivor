using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathHelper;
public class PhieraDerTuphello : WeaponBase
{

    [SerializeField] private PoolContainer firePool;

    private void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        AttackAction += Fire;
    }
    public void Fire()
    {
        float randAngle = 0f;
        for (int i = 0; i < stats.SimulataneousAmount; i++)
        {
            Vector3 randomDirection = VectorHelper.GetDirectionAtAngle(randAngle);
            GameObject fire = firePool.Retrieve(transform.position);
            fire.GetComponent<PhieraDerProjectile>().Spawn(stats, randomDirection);
            randAngle += 90;
        }
    }
    public float randAngle = 45;

    private void OnDisable()
    {
        AttackAction -= Fire;
    }
}
