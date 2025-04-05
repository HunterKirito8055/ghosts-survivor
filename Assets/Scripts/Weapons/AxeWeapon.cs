using MathHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : WeaponBase
{
    [SerializeField] private PoolContainer axes;
    public void Awake()
    {
        axes = GetComponent<PoolContainer>();
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
            randomDirection = VectorHelper.GetDirectionAtAngle(randAngle) * 90;
            randomDirection += transform.position;
            GameObject axe = axes.Retrieve(transform.position);
            axe.GetComponent<AxeProjectile>().Spawn(transform.position, randomDirection, stats, (ParabolaPlane)Random.Range(1, 3));
        }
    }
}
