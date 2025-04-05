using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathHelper;
using UnityEngine.Serialization;

public class AxeProjectile : MonoBehaviour
{
    private float AnimationTime;
    public float duration = 5f;
    private Vector3 start;
    private Vector3 end;
    public float height;
    public ParabolaPlane parabolaPlane;
    private int damage;
    public Vector3 travel;
    private float timer;
    private Vector3 travelDirection;
    private WeaponStats stats;
    private void Update()
    {
        AnimationTime += Time.deltaTime;
        AnimationTime = AnimationTime % duration;
        if ((timer -= Time.deltaTime) <= 0.012)
        {
            gameObject.SetActive(false);
        }
        travel = VectorHelper.Parabola(start, end, height, AnimationTime / duration, parabolaPlane);
        travelDirection = (transform.position - travel).normalized;
        transform.position = travel;
    }
    public void Spawn(Vector3 spawnAt, Vector3 endAt, WeaponStats stat, ParabolaPlane plane)
    {
        stats = stat;
        parabolaPlane = plane;
        timer = duration = stat.Duration;
        start = spawnAt;
        end = endAt;
        hitCounter = stat.Pierce;
        damage = stat.Damage;
        height = Vector3.Distance(spawnAt, endAt) / 3;
        gameObject.SetActive(true);
    }
    [SerializeField] private int hitCounter = 0;
    public void OnHit()
    {
        Collider[] e = Physics.OverlapSphere(transform.position, GetComponent<BoxCollider>().bounds.extents.y, LayerMask.GetMask("Enemy"));
        WeaponBase.ApplyDamage(e, damage, travelDirection * stats.Knockback);
        hitCounter--;
        if (hitCounter <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
