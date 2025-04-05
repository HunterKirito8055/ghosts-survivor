using MathHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockLancet : WeaponBase
{
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
        StartCoroutine(TimeFire());
    }
    private IEnumerator TimeFire()
    {
        pauseTimer = true;
        randAngle = 0;
        Vector3 shootDirection;
        for (int i = 0; i < 12; i++)
        {
            shootDirection = VectorHelper.GetDirectionAtAngle(randAngle);
            GameObject freezeBall = fireballs.Retrieve(transform.position);
            freezeBall.GetComponent<FreezeBall>().Spawn(stats.Speed, stats.Damage, stats.Duration, shootDirection);
            randAngle += (360 / 12);
            yield return new WaitForSeconds(0.15f);
        }
        pauseTimer = false;
    }

}
