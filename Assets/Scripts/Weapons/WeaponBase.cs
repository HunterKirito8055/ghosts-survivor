using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected EnemySpawnerManager enemySpawnerManager;
    [SerializeField] protected ScriptableReference enemymanagerRef;
    protected bool pauseTimer;
    public enum TimeInterval
    {
        COOLDOWN,
        ATTACKSTARTED,
        WAITDURATION
    }
    public WeaponData weaponData;
    public WeaponStats stats;
    [SerializeField] protected OnTriggerRegister onTriggerRegister;
    public int currentWeaponLevel;

    private float countDown = 0;
    protected Action AttackAction { get; set; }
    protected Action CooldownAction { get; set; }
    // protected bool isFired;
    protected TimeInterval timeInterval;
    internal void Awake()
    {
        SetData(weaponData);
        currentWeaponLevel = 1;
        timeInterval = TimeInterval.COOLDOWN;
    }
    public void OnLevelUp(bool val)
    {
        if (val)
        {
            stats.CoolDown -= (stats.CoolDown / 10);
        }
        else
        {
            stats.Damage += (stats.Damage / 10);
        }
        currentWeaponLevel++;
        GameManager.Instance.OnLevelup.Invoke(false);
    }
    private void Start()
    {
        enemySpawnerManager = enemymanagerRef.Reference.GetComponent<EnemySpawnerManager>();
    }
    private bool fireProjectiles;
    protected int amount = 0;
    public virtual void Update()
    {
        if (!pauseTimer)
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                switch (timeInterval)
                {
                    case TimeInterval.COOLDOWN:
                        timeInterval = TimeInterval.ATTACKSTARTED;
                        amount = weaponData.stats.Amount;
                        break;
                    case TimeInterval.ATTACKSTARTED:
                        countDown = weaponData.stats.ProjectileInterval;
                        if (AttackAction != null)
                        {
                            AttackAction.Invoke();
                        }
                        amount--;
                        if (amount <= 0)
                        {
                            timeInterval = TimeInterval.WAITDURATION;
                            countDown = weaponData.stats.Duration;
                        }
                        break;
                    case TimeInterval.WAITDURATION:
                        if (CooldownAction != null)
                        {
                            CooldownAction.Invoke();
                        }
                        timeInterval = TimeInterval.COOLDOWN;
                        countDown = weaponData.stats.CoolDown;
                        break;
                }
            }
        }

        // if (!isFired)
        // {
        //     //if cool down period over, we need to fire
        //     if (countDown >= weaponData.stats.CoolDown)
        //     {
        //         countDown = 0;
        //         fireProjectiles = true;
        //         isFired = true;
        //         if (AttackAction != null)
        //         {
        //             AttackAction.Invoke();
        //         }
        //     }
        //     if (fireProjectiles)
        //     {
        //         if (countDown >= weaponData.stats.ProjectileInterval)
        //         {
        //
        //         }
        //     }
        // }
        // else
        // {
        //     //if active duration over, we need to cool down the weapon 
        //     if (countDown >= weaponData.stats.Duration)
        //     {
        //         countDown = 0;
        //         isFired = false;
        //         if (CooldownAction != null)
        //         {
        //             CooldownAction.Invoke();
        //         }
        //     }
        // }
    }
    public static void ApplyDamage(Collider[] colliders, int damage, Vector3 direction = default, float knockBack = -1)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamageable e = colliders[i].GetComponent<IDamageable>();
            if (e != null)
            {
                if (direction == default && knockBack == -1)
                {
                    e.OnHit(damage);
                }
                else if (direction != default)
                {
                    e.OnHit(damage, direction);
                }
                else if (knockBack != -1)
                {
                    e.OnHit(damage, knockBack);
                }
            }
        }
    }
    private int GetDamage => weaponData.stats.Damage;
    public virtual void SetData(WeaponData data)
    {
        weaponData = data;
        stats = new WeaponStats(data.stats);
    }


    protected GameObject SelectEnemyInRange(Vector3 position)
    {
        float distance = Mathf.Infinity;
        GameObject nearObject = null;
        List<GameObject> activeEnemies = enemySpawnerManager.ActiveGameObjects;
        if (activeEnemies.Count > 0)
        {
            activeEnemies.ForEach(x =>
            {
                float tempDistance = Vector2.Distance(x.transform.position, position);
                // if (tempDistance <= WeaponStats.AttackRange)
                {
                    if (tempDistance < distance)
                    {
                        nearObject = x;
                        distance = tempDistance;
                    }
                }
            });
        }
        return nearObject;
    }

}
