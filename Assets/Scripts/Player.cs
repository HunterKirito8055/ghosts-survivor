using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AarquieSolutions.DependencyInjection.ComponentField;

public class Player : MonoBehaviour, IDamageable
{

    [SerializeField] private float moveSpeed;
    [SerializeField] private ScriptableLevel playerScriptableLevel;
    [GetComponent] private Rigidbody rigidbody;
    [SerializeField] private SpriteRenderer spriteBody;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform whip;

    [SerializeField] private float startHealth = 300;
    private float health;

    [Header("UI")]
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image healthBar;

    [Header("Decoupled events")]
    [SerializeField] private Vector3Event moveAhead;


    [SerializeField] private float moveAheadOffset;


    private float horizontal;
    private float vertical;


    private Vector3 moveVector;
    private bool targetable;
    private bool invincible;
    private void Awake()
    {
        this.InitializeDependencies();
    }
    private void Start()
    {
        healthBar.fillAmount = 1;
        health = startHealth;
    }
    private Vector3 predictAheadPos;

    private void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        moveVector = new Vector3(horizontal, 0, vertical).normalized;
        if (horizontal != 0 || vertical != 0)
        {
            rigidbody.MovePosition(rigidbody.position + moveVector * (moveSpeed * Time.fixedDeltaTime));
            spriteBody.flipX = horizontal < 0;
            whip.localScale = new Vector3(spriteBody.flipX ? -1 : 1, 1, 1);
        }
        animator.SetFloat("MoveX", moveVector.magnitude);
        predictAheadPos = rigidbody.position + (moveVector * moveAheadOffset);
        moveAhead.Invoke(predictAheadPos);
    }

    public void Damage(float value)
    {
        Health -= value;
        value = Mathf.InverseLerp(0, startHealth, Health);
        healthBar.fillAmount = value;
        healthBar.color = gradient.Evaluate(value);

        if (Health <= 0)
        {
            gameObject.SetActive(false);
            GameManager.Instance.GameOver();
        }
    }

    public void OnHit(float damage, Vector3 knockback)
    {
        throw new NotImplementedException();
    }
    public void OnHit(float damage, float knockback)
    {
        throw new NotImplementedException();
    }
    public void OnHit(float damage)
    {
        Health -= damage;
        damage = Mathf.InverseLerp(0, startHealth, Health);
        healthBar.fillAmount = damage;
        healthBar.color = gradient.Evaluate(damage);

        if (Health <= 0)
        {
            gameObject.SetActive(false);
            GameManager.Instance.GameOver();
        }
    }
    public void OnObjectDestroyed()
    {
        throw new NotImplementedException();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(predictAheadPos, 0.5f);
    }
    public string Tag
    {
        get
        {
            return gameObject.tag;
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

}
