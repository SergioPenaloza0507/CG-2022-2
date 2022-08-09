using System;
using UnityEngine;

public class ParentConstraintSimple : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }
    
    [Serializable]
    public struct AxisRemapData
    {
        public Axis x;
        public Axis y;
        public Axis z;
    }
    
    [SerializeField] private Transform parent;
    [SerializeField] private AxisRemapData rotationRemapping;

    Quaternion GetRemappedRotation(Vector3 eulerAngles)
    {
        float x = 0, y = 0, z = 0;
        switch (rotationRemapping.x)
        {
            case Axis.X:
                x = eulerAngles.x;
                break;
            case Axis.Y:
                x = eulerAngles.y;
                break;
            case Axis.Z:
                x = eulerAngles.z;
                break;
        }
        switch (rotationRemapping.y)
        {
            case Axis.X:
                y = eulerAngles.x;
                break;
            case Axis.Y:
                y = eulerAngles.y;
                break;
            case Axis.Z:
                y = eulerAngles.z;
                break;
        }
        switch (rotationRemapping.z)
        {
            case Axis.X:
                z = eulerAngles.x;
                break;
            case Axis.Y:
                z = eulerAngles.y;
                break;
            case Axis.Z:
                z = eulerAngles.z;
                break;
        }
        return Quaternion.Euler(new Vector3(x,y,x));
    }
    private void Update()
    {
        if (transform == null) return;
        transform.SetPositionAndRotation(parent.position, GetRemappedRotation(parent.eulerAngles));
    }
}
