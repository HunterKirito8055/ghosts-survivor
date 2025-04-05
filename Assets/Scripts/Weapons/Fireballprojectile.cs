using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireballprojectile : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    private float speed;
    private float lifeSpan;
    private Vector3 moveVector;
    private int damage;

    private Rigidbody selfBody;

    [SerializeField]
    private ScriptableReference firewandreference;

    private FireWand _fireWand;
    private void Awake()
    {
        selfBody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        if (_fireWand == null)
        {
            _fireWand = firewandreference.Reference.GetComponent<FireWand>();
        }
    }
    private void Update()
    {
        lifeSpan -= Time.deltaTime;
        if (lifeSpan <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    private WeaponStats stats;
    public void Spawn(WeaponStats stat, Vector3 _movedirection)
    {
        stats = stat;
        speed = stat.Speed;
        lifeSpan = stat.Duration;
        damage = stat.Damage;
        moveVector = _movedirection;
        gameObject.SetActive(true);
    }
    private void FixedUpdate()
    {
        moveVector.y = 0;
        selfBody.MovePosition(selfBody.position + (moveVector.normalized * speed * Time.fixedDeltaTime));
    }
    public void OnHit()
    {
        gameObject.SetActive(false);
        Collider[] e = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Enemy"));
        WeaponBase.ApplyDamage(e, damage, selfBody.velocity.normalized * stats.Knockback);
    }
}
