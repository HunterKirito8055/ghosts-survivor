using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private ScriptableReference player;

    [SerializeField] private PoolContainer gemPooler;

    public void CreateGem(Vector3 position)
    {
        GameObject gem = gemPooler.Retrieve(position + Vector3.up * 2);
        gem.SetActive(true);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            GameManager.Instance.CollectedGems();
        }
    }
}
