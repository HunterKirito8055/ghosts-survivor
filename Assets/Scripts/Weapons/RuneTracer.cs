using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RuneTracer : WeaponBase
{
    [SerializeField] private PoolContainer tracers;

    private void Awake()
    {
        tracers = GetComponent<PoolContainer>();
        base.Awake();
        AttackAction += OnAttack;
    }
    private void OnEnable()
    {
        amount = stats.Amount;
    }
    private void OnCool()
    {
        
    }

    private void OnAttack()
    {
        RuneTracerProjectile tracer = tracers.Retrieve(transform.position).GetComponent<RuneTracerProjectile>();
        Vector3 randDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        tracer.Spawn((Direct)(Random.Range(0, Enum.GetValues(typeof(Direct)).Length)), stats.Speed, stats.Damage, stats.Duration,randDirection);
    }
}
