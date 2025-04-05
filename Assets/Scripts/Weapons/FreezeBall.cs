using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBall : MonoBehaviour
{
    public float speed;
    public int damage;
    public float duration;
    public Vector3 direction;
    
    private Rigidbody selfBody;
    private void Awake()
    {
        selfBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        direction.y = 0;
        selfBody.MovePosition(selfBody.position + (direction.normalized * speed * Time.fixedDeltaTime));
    }
    public void Spawn(float statsSpeed, int statsDamage, float statsDuration, Vector3 shootDirection)
    {
        speed = statsSpeed;
        damage = statsDamage;
        duration = statsDuration;
        direction = shootDirection;
        gameObject.SetActive(true);
    }
    public void OnHit()
    {
        gameObject.SetActive(false);
        Collider[] e = Physics.OverlapSphere(transform.position, GetComponent<Collider>().bounds.extents.y, LayerMask.GetMask("Enemy"));
        damage = -1;
        WeaponBase.ApplyDamage(e, damage);
    }
}
