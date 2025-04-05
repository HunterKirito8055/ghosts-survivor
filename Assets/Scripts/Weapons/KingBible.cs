using MathHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class KingBible : WeaponBase
{
    [SerializeField] private List<GameObject> bibleList;
    [SerializeField] private GameObject biblePrefab;
    [SerializeField] private int biblesCount = 1;
    [SerializeField] private float radius = 15;

    private void Awake()
    {
        base.Awake();
        AttackAction += OnAttack;
        CooldownAction += OnCool;
    }
    private void OnEnable()
    {
        biblesCount = stats.Amount;
    }
    private void OnDisable()
    {
        AttackAction -= OnAttack;
        CooldownAction -= OnCool;
    }
    private void OnCool()
    {
        foreach (GameObject o in bibleList)
        {
            o.SetActive(false);
        }
    }
    private void OnAttack()
    {
        float angle = 360f / biblesCount;
        float spawnAngle = 0;
        Vector3 position = transform.position;

        Vector3 spawAt = position;

        for (int i = 0; i < biblesCount; i++)
        {
            Vector3 newVec = position + (VectorHelper.GetDirectionAtAngle(spawnAngle) * radius);
            GetBible(newVec).GetComponent<OnTriggerRegister>().TakeDamage = stats.Damage;
            spawnAngle += (angle);
        }
    }
    public override void Update()
    {
        base.Update();
        transform.Rotate(new Vector3(0, stats.Speed, 0) * Time.deltaTime);
    }
    private GameObject GetBible(Vector3 atPosition)
    {
        GameObject bible = null;
        if (bibleList != null)
        {
            if (bibleList.Count > 0)
            {
                bible = bibleList.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
            }
        }
        if (bible == null)
        {
            if (biblePrefab != null)
            {
                bible = Instantiate(biblePrefab, atPosition, Quaternion.identity, transform);
                if (bibleList == null)
                {
                    bibleList = new List<GameObject>();
                }
                bibleList.Add(bible);
            }
        }
        bible.transform.position = atPosition;
        bible.gameObject.SetActive(true);
        return bible;
    }

    private List<Vector3> newVector;


    public void OnDrawGizmosSelected()
    {
        float angle = 360f / biblesCount;
        float spawnAngle = 0;
        Vector3 position = transform.position;

        Vector3 spawAt = position;

        for (int i = 0; i < biblesCount; i++)
        {
            Vector3 newVec = position +
                             (VectorHelper.GetDirectionAtAngle(spawnAngle) * radius);
            // GetBible(newVector);
            Gizmos.DrawSphere(newVec, 0.5f);
            spawnAngle += (angle);
        }
    }
}
