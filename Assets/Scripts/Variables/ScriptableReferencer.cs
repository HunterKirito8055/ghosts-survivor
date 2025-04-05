using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableReferencer<T> : ScriptableObject
{
    [SerializeField]
    private T reference;
    
    
    public T Reference
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
