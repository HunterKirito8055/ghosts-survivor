using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public  class OnTriggerRegister : MonoBehaviour
{
    public float TakeDamage { get; set; }
    [SerializeField]
    protected bool hit;

    public virtual void Start()
    {
        hit = false;
    }
    public virtual void OnTriggerEnter(Collider col)
    {
        IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
        if (damageable != null && damageable.Tag.Equals("Enemy")/*&& col.isTrigger*//* && col.GetType() == typeof(BoxCollider)*/)
        {
            damageable.OnHit(TakeDamage);
            hit = true;
        }
    }
    public virtual void OnCollisionEnter(Collision col)
    {
        IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
        if (damageable != null && damageable.Tag.Equals("Enemy"))
        {
            damageable.OnHit(TakeDamage);
            hit = true;
        }
    }

}
