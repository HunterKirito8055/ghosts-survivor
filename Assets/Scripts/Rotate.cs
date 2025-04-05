using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 50f;
    public enum RotateAxis
    {
        Y_Axis,
        X_Axis,
        Z_Axis
    }
    public RotateAxis onAxis;
    void Update()
    {
        if (onAxis == RotateAxis.Y_Axis)
        {
            transform.Rotate(new Vector3(0, speed, 0) * Time.deltaTime);
        }
        else if (onAxis == RotateAxis.X_Axis)
        {
            transform.Rotate(new Vector3(speed, 0, 0) * Time.deltaTime);
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, speed) * Time.deltaTime);
        }
    }
}
