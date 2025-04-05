using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhieraDerProjectile : MonoBehaviour
{
    private float speed;
    private float lifeSpan;
    private Vector3 moveVector;
    private int damage;

    private Rigidbody selfBody;
    [SerializeField]
    private float disableAfter = 10f;
    private float timer;
    private void Awake()
    {
        selfBody = GetComponent<Rigidbody>();
    }
    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    private WeaponStats stats;
    public void Spawn(WeaponStats stat, Vector3 randomDirection)
    {
        stats = stat;
        speed = stats.Speed;
        lifeSpan = stats.Duration;
        damage = stats.Damage;
        moveVector = randomDirection;
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
        Collider[] e = Physics.OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Enemy"));
        WeaponBase.ApplyDamage(e, damage, knockBack: stats.Knockback);
    }
}
