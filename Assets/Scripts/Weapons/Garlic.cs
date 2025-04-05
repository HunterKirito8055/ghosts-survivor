using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garlic : WeaponBase
{
    [SerializeField] private float radius = 12f;
    public void Awake()
    {
        base.Awake();
        AttackAction += Attack;
    }
    private void Attack()
    {
        Collider[] e = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Enemy"));
        ApplyDamage(e, stats.Damage, knockBack: stats.Knockback);
    }
}
