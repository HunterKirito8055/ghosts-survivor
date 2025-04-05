using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float secondsToExplodeAfter = 6f;


    [SerializeField]
    private float lifeSpan;
    [SerializeField]
    private float radius;
    [SerializeField]
    private int damage;

    private BombWeapon _bombWeapon;
    private float countDown;
    private Vector3 localScale;
    private void Awake()
    {
        localScale = transform.lossyScale;
    }

    public void Spawn(int _damage, BombWeapon bombWeapon)
    {
        damage = _damage;
        _bombWeapon = bombWeapon;
        lifeSpan = secondsToExplodeAfter;
        gameObject.SetActive(true);
        transform.localScale = localScale;
        transform.parent = null;
        fired = false;
    }
    private bool fired;
    private void Update()
    {
        lifeSpan -= Time.deltaTime;
        if (!fired)
        {
            if (lifeSpan <= 0)
            {
                OnHit();
                fired = true;
                transform.localScale = new Vector3(radius, radius, radius);
            }
        }
    }
    public Collider[] e;
    public void OnHit()
    {
        e = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Enemy"));
        WeaponBase.ApplyDamage(e,damage);
        Invoke(nameof(Disable), 0.7f);
    }
    void Disable()
    {
        gameObject.SetActive(false);
        transform.localScale = localScale;
    }
}
