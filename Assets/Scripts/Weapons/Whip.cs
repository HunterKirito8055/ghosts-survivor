using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whip : WeaponBase
{

    [SerializeField] private GameObject whip;
    [SerializeField] private ScriptableReference playerRef;

    private Animator animator;
    private void Awake()
    {
        base.Awake();
        AttackAction += WhipOn;
    }
    private void OnDisable()
    {
        AttackAction -= WhipOn;
    }
    public float whipScale = 10f;
    private bool canFire;
    public override void Update()
    {
        if (!canFire)
        {
            base.Update();
        }
        else if (whip.transform.localScale.x > whipScale)
        {
            WhipOff();
        }

    }
    private void Start()
    {
        animator = whip.GetComponent<Animator>();
    }

    public void WhipOff()
    {
        animator.SetBool("Whip", false);
        whip.gameObject.SetActive(false);
        canFire = false;
        StopCoroutine(AnimationControl());
    }
    public void WhipOn()
    {
        StartCoroutine(AnimationControl());
    }
    private IEnumerator AnimationControl()
    {
        onTriggerRegister.TakeDamage = stats.Damage;
        canFire = true;
        whip.gameObject.SetActive(true);
        animator.SetBool("Whip", true);
        yield return new WaitForSecondsRealtime(0.5f);
        WhipOff();
    }
    private void OnDestroy()
    {
        AttackAction -= WhipOn;
    }
}
