using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class OnTriggerListener : MonoBehaviour
{
    [SerializeField]
    private string[] targetTags;

    [SerializeField]
    private UnityEvent OnTriggerEnterEvent;

    [SerializeField]
    private UnityEvent OnTriggerExitEvent;

    public void OnTriggerEnter(Collider collision)
    {
        if (HasTargetTag(collision.gameObject))
        {
            OnTriggerEnterEvent.Invoke();
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if (HasTargetTag(collision.gameObject))
        {
            OnTriggerExitEvent.Invoke();
        }
    }

    private bool HasTargetTag(GameObject gameObject)
    {
        for (int i = 0; i < targetTags.Length; i++)
        {
            if (gameObject.CompareTag(targetTags[i]))
            {
                return true;
            }
        }

        return false;
    }
}
