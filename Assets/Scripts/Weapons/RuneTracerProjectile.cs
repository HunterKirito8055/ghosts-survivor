using MathHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RuneTracerProjectile : OnTriggerRegister
{
    public Direct direct;
    private Rigidbody selfBody;
    private float speed;
    private int damage;
    private float duration;

    private float changeDirTime;
    private Vector3 moveVector;

    private float lifeSpan;
    private void Awake()
    {
        selfBody = GetComponent<Rigidbody>();
    }
    public void Spawn(Direct _direct, float _speed, int _damage, float _duration, Vector3 _movedirection)
    {
        speed = _speed;
        direct = _direct;
        TakeDamage = damage = _damage;
        lifeSpan = duration = _duration;
        moveVector = _movedirection;
        gameObject.SetActive(true);
        angle = Vector3.Angle(transform.forward, moveVector);
    }
    public float angle;
    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            gameObject.SetActive(false);
        }
        if (changeDirTime <= 0)
        {
            changeDirTime = Random.Range(lifeSpan / 5f, lifeSpan / 2);
            if (direct == Direct.LEFT)
            {
                angle -= 90;
            }
            else
            {
                angle += 90;
            }
            moveVector += VectorHelper.GetDirectionAtAngle(angle);
        }
        else
        {
            changeDirTime -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        moveVector.y = 0;
        selfBody.MovePosition(selfBody.position + (moveVector.normalized * speed * Time.fixedDeltaTime));
    }
    private IEnumerator DiamondHoming()
    {
        changeDirTime = Random.Range(duration / 2, duration);
        yield return new WaitForSecondsRealtime(changeDirTime);
        float angle = transform.eulerAngles.y;
        if (direct == Direct.LEFT)
        {
            angle -= 90;
        }
        else
        {
            angle += 90;
        }

    }
}
public enum Direct
{
    LEFT,
    RIGHT
}
