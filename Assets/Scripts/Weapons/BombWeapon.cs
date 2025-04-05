using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWeapon : WeaponBase
{

    [SerializeField] private PoolContainer bombPool;

    public void Awake()
    {
        bombPool = GetComponent<PoolContainer>();
        base.Awake();
        AttackAction += Fire;
    }
    private void Fire()
    {
        GameObject bomb = bombPool.Retrieve(transform.position);
        bomb.GetComponent<Bomb>().Spawn(stats.Damage, this);
    }
}
