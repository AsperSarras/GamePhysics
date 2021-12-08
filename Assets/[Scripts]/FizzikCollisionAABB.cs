using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizzikCollisionAABB : FizzikCollisionShapeBase
{
    public Vector3 GetSize()
    {
        return transform.lossyScale;
    }

    public Vector3 GetHalfSize()
    {
        return transform.lossyScale * 0.5f;
    }

    public Vector3 GetMin()
    {
        return transform.position - GetSize()/2;
    }

    public Vector3 GetMax()
    {
        return transform.position + GetHalfSize();
    }

    public override FizziksColliderShape GetColliderShape()
    {
        return FizziksColliderShape.AABB;
    }
}
