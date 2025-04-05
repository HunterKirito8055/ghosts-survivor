using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AarquieSolutions.DependencyInjection.ComponentField;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum EnemyType
{
    NORMAL,
    BOSS,
}
public class Enemy : MonoBehaviour, IDamageable
{

    public EntityStatsBase entityStats;

    [GetComponent] private Rigidbody rigidbody;
    [SerializeField] private SpriteRenderer spriteBody;
    [SerializeField] private SimpleFlash simpleFlash;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask playerMask;

    [SerializeField] private ScriptableReference playerRef;
    [SerializeField] private Vector3Event playerAheadPosition;
    [SerializeField] private Vector3 moveAheadPosition;

    [GetComponent] private NavMeshAgent agent;
    [SerializeField] private bool knockBack;
    [SerializeField] private Vector3 knockBackDirection;


    [SerializeField] private EnemyType type;
    [SerializeField] private ScriptableReference enemyManager;
    [SerializeField] private EnemySpawnerManager EnemySpawnerManager;
    public EnemyType Type => type;
    private float moveSpeed;
    private float health;
    private bool freeze;
    [SerializeField] private float freezeTimer;


    private Player player;
    private Vector3 moveVector;
    private Coroutine dieCoroutine;
    private void Awake()
    {
        this.InitializeDependencies();
        simpleFlash = GetComponentInChildren<SimpleFlash>();
    }
    private void Start()
    {
        playerMask = LayerMask.GetMask("Player");
        moveSpeed = entityStats.Speed;
        EnemySpawnerManager = enemyManager.Reference.GetComponent<EnemySpawnerManager>();
        DefaultData();
    }
    private void OnEnable()
    {
        player = playerRef.Reference.GetComponent<Player>();
        Health = entityStats.Health;
        agent.ResetPath();
        isDead = false;
        agent.isStopped = false;
        if (dieCoroutine != null)
        {
            StopCoroutine(dieCoroutine);
        }
        playerAheadPosition.AddListener(MoveAheadPosition);
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            agent.isStopped = true;
            return;
        }
        if (knockBack)
        {
            agent.velocity = knockBackDirection;
        }
        if (player != null)
        {
            agent.autoRepath = true;
            agent.speed = moveSpeed;
            moveVector = ((player.transform.position - rigidbody.position)).normalized;
            //moveVector.y = 0;
            // agent.SetDestination(moveVector);

            spriteBody.flipX = moveVector.x < 0;
            animator.SetInteger("MoveOrDead", 1);
        }
    }
    private void DefaultData()
    {
        knockBack = false;
        knockBackDirection = Vector3.zero;
    }
    public void MoveAheadPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }
    private IEnumerator KnockBackCoroutine()
    {
        knockBack = true;
        agent.speed = moveSpeed * 10;
        float acceleration = agent.acceleration;
        agent.acceleration = acceleration * 100;

        yield return new WaitForSeconds(0.2f);

        knockBack = false;
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
    }
    private void OnCollisionEnter(Collision col)
    {
        if (!freeze)
        {
            IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (damageable.Tag.Equals("Player") && col.gameObject.layer == playerMask)
                {
                    damageable.OnHit(entityStats.Damage);
                }
            }
        }
    }
    private IEnumerator OnTriggerEnter(Collider col)
    {
        if (!freeze)
        {
            IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
            yield return new WaitForSecondsRealtime(0.01f);
            if (col.CompareTag("Player"))
            {
                damageable.OnHit(entityStats.Damage);
            }
        }
    }
    private IEnumerator OnTriggerStay(Collider other)
    {
        if (!freeze)
        {
            yield return new WaitForSecondsRealtime(1f);
            if (other.CompareTag("Player"))
            {
                player.Damage(entityStats.Damage / 12f);
            }
        }
    }

    private bool isDead;
    private bool targetable;
    private bool invincible;
    private IEnumerator Die()
    {
        isDead = true;
        animator.SetInteger("MoveOrDead", 0);
        yield return new WaitForSecondsRealtime(0.7f);
        GameManager.Instance.gem.CreateGem(transform.position);
        simpleFlash.StopCoroutines();
        gameObject.SetActive(false);
        animator.SetInteger("MoveOrDead", 1);

    }
    public string Tag
    {
        get
        {
            return this.tag;
        }
    }
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }
    public bool Targetable
    {
        get
        {
            return targetable;
        }
        set
        {
            targetable = value;
        }
    }
    public bool Invincible
    {
        get
        {
            return invincible;
        }
        set
        {
            invincible = value;
        }
    }
    public void OnHit(float damage, Vector3 knockback)
    {
        simpleFlash.Flash();
        Health -= damage;
        if (Health <= 0)
        {
            dieCoroutine = StartCoroutine(Die());
        }
        knockBackDirection = knockback;
        StartCoroutine(KnockBackCoroutine());
    }
    public void OnHit(float damage, float knockback)
    {
        simpleFlash.Flash();
        Health -= damage;
        if (Health <= 0)
        {
            dieCoroutine = StartCoroutine(Die());
        }
        knockBackDirection = (-agent.velocity.normalized) * knockback;
        StartCoroutine(KnockBackCoroutine());
    }
    public void OnHit(float damage)
    {
        if (damage == -1)
        {
            StartCoroutine(Freeze());
            return;
        }
        simpleFlash.Flash();
        Health -= damage;
        if (Health <= 0)
        {
            dieCoroutine = StartCoroutine(Die());
        }
    }
    private void OnDisable()
    {
        EnemySpawnerManager?.enemies.Remove(this);
        playerAheadPosition.RemoveListener(MoveAheadPosition);
    }
    public void OnObjectDestroyed()
    {
        throw new NotImplementedException();
    }
    IEnumerator Freeze()
    {
        freeze = true;
        agent.isStopped = true;


        yield return new WaitForSeconds(freezeTimer);
        agent.isStopped = false;

        freeze = false;
    }
}
