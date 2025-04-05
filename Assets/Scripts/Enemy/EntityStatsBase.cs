using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Scriptable Objects/Characters/Create New Entity Stats")]
public class EntityStatsBase : ScriptableObject
{
    public GameObject entityModel;

    [SerializeField] protected Difficulty difficulty;

    [SerializeField] protected float movingSpeed;

    [SerializeField] protected float health;

    [SerializeField] protected float damage;

    [SerializeField] protected float predictAheadOffset;

    [SerializeField] protected PoolContainer pooler;

    public float Speed
    {
        get
        {
            return movingSpeed;
        }
    }
    public float Health
    {
        get
        {
            return health;
        }
    }
    public float Damage
    {
        get
        {
            return damage;
        }
    }
    public float PredictAheadOffset
    {
        get
        {
            return predictAheadOffset;
        }
    }
    public Difficulty Difficulty
    {
        get
        {
            return difficulty;
        }
    }
}

public enum Difficulty
{
    NORMAL,
    HARD,
    BOSS
}
