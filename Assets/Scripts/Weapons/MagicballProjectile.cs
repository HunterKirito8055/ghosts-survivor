using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AarquieSolutions.DependencyInjection.ComponentField;

public class MagicballProjectile : OnTriggerRegister
{
    [SerializeField] private ScriptableReference reference;
    public float Speed { get; set; }
    private Vector3 direction;
    [GetComponent] public Rigidbody rigidbody;

    [SerializeField] private bool canKnockback;
    public override void Start()
    {
        base.Start();
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody>();
        }
    }
    public void LoadBall(Vector3 spawnPoint, float damage, float speed, Vector3 direction)
    {
        transform.position = spawnPoint;
        TakeDamage = damage;
        Speed = speed;
        this.direction = direction;
    }
    private Vector3 moveVector;
    private void FixedUpdate()
    {
        if (hit)
        {
            gameObject.SetActive(false);
            hit = false;
        }
        moveVector = (direction).normalized;
        moveVector.y = 0;
        rigidbody.MovePosition(rigidbody.position + moveVector * (Speed * Time.fixedDeltaTime));
    }

    public override void OnTriggerEnter(Collider col)
    {
        IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
        if (damageable != null && damageable.Tag.Equals("Enemy")/*&& col.isTrigger*//* && col.GetType() == typeof(BoxCollider)*/)
        {
            if (canKnockback)
            {
                damageable.OnHit(TakeDamage, GetComponent<Rigidbody>().velocity.normalized);
            }
            else
            {
                damageable.OnHit(TakeDamage);
            }
            hit = true;
        }
    }
}
