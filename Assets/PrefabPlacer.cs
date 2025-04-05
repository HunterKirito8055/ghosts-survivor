using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AarquieSolutions.InspectorAttributes;
using UnityEditor;
using UnityEngine;
using static AarquieSolutions.Utility.RandomExtension;

public class PrefabPlacer : MonoBehaviour
{
    public Rect size;
    public GameObject[] prefabs;
    public int spawnCount;

    [Button("Instantiate Many")]
    public void PlacePrefabs()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            PlacePrefab();
        }
    }

    [Button("Instantiate One")]
    public void PlacePrefab()
    {
        var go = Instantiate(Array(prefabs), PointInRect(size), Quaternion.identity);
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(go.transform.position.x, 0, go.transform.position.y);
        go.transform.Rotate(Vector3.up, Random.Range(0, 360));
        go.SetActive(true);
    }


    [Button("Delete All")]
    public void DeleteAll()
    {
        for (int i = transform.childCount-1; i >=0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
