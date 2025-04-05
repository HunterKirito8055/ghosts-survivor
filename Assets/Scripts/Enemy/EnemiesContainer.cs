using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "Enemies Container", menuName = "Scriptable Objects/Enemies Container")]
public class EnemiesContainer : ScriptableObject
{
    [SerializeField] private List<Enemy> enemies = new List<Enemy>();


    [SerializeField] private List<EntityStatsBase> enemysList = new List<EntityStatsBase>();


#if UNITY_EDITOR
    public static T GetInstance<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    public static T[] GetAllInstances<T>() where T : Object
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        T[] ts = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            ts[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }
        return ts;
    }

    private void OnValidate()
    {
        enemysList = new List<EntityStatsBase>();
        enemysList.AddRange(GetAllInstances<EntityStatsBase>());

        for (int i = 0; i < enemies.Count; i++)
        {
            if (i < enemysList.Count)
            {
                enemysList[i].entityModel = enemies[i].gameObject;
                enemies[i].entityStats = enemysList[i];
            }
        }
    }
#endif
}
