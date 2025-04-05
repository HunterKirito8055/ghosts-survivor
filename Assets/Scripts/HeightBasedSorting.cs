using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Sorts the attached sorting group based on the Y axis.
/// </summary>
[RequireComponent(typeof(SortingGroup)), DisallowMultipleComponent]
public class HeightBasedSorting : MonoBehaviour
{
    [SerializeField]
    private SortingGroup sortingGroup;

    [SerializeField]
    private float positionScaling = -100;

    [SerializeField]
    private bool flip;

    private void OnValidate()
    {
        if (sortingGroup == null)
        {
            sortingGroup = GetComponent<SortingGroup>();
        }

        UpdateOrder();
    }

    private void Start()
    {
        UpdateOrder();
    }

    private void LateUpdate()
    {
        UpdateOrder();
    }

    public void UpdateOrder()
    {
        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = (int)(transform.position.y * positionScaling);
        }
    }
}
