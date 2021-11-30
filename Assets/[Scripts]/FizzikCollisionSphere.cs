using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizzikCollisionSphere : FizzikCollisionShapeBase
{
    public float radius = 1.0f;

    public override FizziksColliderShape GetColliderShape()
    {
        return FizziksColliderShape.Sphere;
    }
}
