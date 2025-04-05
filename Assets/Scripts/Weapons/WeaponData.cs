using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;
[Serializable]
public class WeaponStats
{
    public WeaponStats(WeaponStats stats)
    {
        damage = stats.damage;
        speed = stats.speed;
        simulataneousAmount = stats.simulataneousAmount;
        amount = stats.amount;
        duration = stats.duration;
        coolDown = stats.coolDown;
        projectileInterval = stats.projectileInterval;
        knockback = stats.knockback;
        blockedByWalls = stats.blockedByWalls;
        pierce = stats.pierce;

    }

    [SerializeField] protected int damage;

    [SerializeField] protected float speed;

    [Tooltip("No. of bullets in single Amount"), SerializeField] protected int simulataneousAmount;

    [SerializeField] protected int amount;

    [SerializeField] protected float duration;

    [SerializeField] protected float coolDown;

    [SerializeField] protected float projectileInterval;

    [SerializeField] protected float knockback;

    [SerializeField] protected bool blockedByWalls;


    [Tooltip("Max amount of enemies that a projectile can hit "), SerializeField]
    protected int pierce;



    // public float AttackRange
    // {
    //     get
    //     {
    //         return attackRange;
    //     }
    // }
    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }
    public float Speed
    {
        get
        {
            return speed;
        }
    }
    public int Amount
    {
        get
        {
            return amount;
        }
    }
    public int SimulataneousAmount
    {
        get
        {
            return simulataneousAmount;
        }
    }
    public float Duration
    {
        get
        {
            return duration;
        }
    }
    public float CoolDown
    {
        get
        {
            return coolDown;
        }
        set
        {
            coolDown = value;
        }
    }
    public float ProjectileInterval
    {
        get
        {
            return projectileInterval;
        }
    }
    public float Knockback
    {
        get
        {
            return knockback;
        }
    }
    public bool BlockedByWalls
    {
        get
        {
            return blockedByWalls;
        }
    }
    public int Pierce
    {
        get
        {
            return pierce;
        }
    }
}
[CreateAssetMenu(menuName = "Scriptable Objects/Weapon Data")]
public class WeaponData : ScriptableObject
{

    public string weaponName;

    public WeaponStats stats;

}
