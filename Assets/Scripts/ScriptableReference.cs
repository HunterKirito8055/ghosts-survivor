using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Reference", menuName = "Scriptable Objects/Referencing/New Scriptable Reference")]
public class ScriptableReference : ScriptableObject
{
    private GameObject reference;
    
    public GameObject Reference
    {
        get
        {
            return reference;
        }
        set
        {
            reference = value;
        }
    }
}
