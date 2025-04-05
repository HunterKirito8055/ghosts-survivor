using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levels Container", menuName = "Scriptable Objects/Levels/Levels container")]
public class ScriptableLevel : ScriptableObject
{

    [SerializeField] protected List<int> levelsList = new List<int>();

    public List<int> LeveList
    {
        get
        {
            return levelsList;
        }
    }
}
