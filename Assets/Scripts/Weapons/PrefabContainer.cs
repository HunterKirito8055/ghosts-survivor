using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/New Prefab Container")]
public class PrefabContainer : ScriptableObject
{
   public GameObject prefabObject;
   
   
   public OnTriggerRegister OnTriggerRegister
   {
      get
      {
         return prefabObject?.GetComponent<OnTriggerRegister>();
      }
   }
}
