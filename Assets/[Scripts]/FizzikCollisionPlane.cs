using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    X = 0,
    Y,
    Z
}

public class FizzikCollisionPlane : FizzikCollisionShapeBase
{
    public Axis alignment = Axis.Y;
    public bool Ground = false;

    public override FizziksColliderShape GetColliderShape()
    {
        return FizziksColliderShape.Plane;
    }

    public Vector3 GetNormal()
    {
        switch(alignment)
        {
            case (Axis.X):
                {
                    return transform.right;
                }
            case (Axis.Y):
                {
                    return transform.up;
                }
            case (Axis.Z):
                {
                    return transform.forward;
                }
            default:
                {
                    throw new System.Exception("Invalid plane alignment");
                }
        }
    }
}
