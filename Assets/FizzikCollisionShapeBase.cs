using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FizziksColliderShape
{
    Sphere,
    Plane,
    AABB
}

public abstract class FizzikCollisionShapeBase : MonoBehaviour
{
    public void Start()
    {
        FindObjectOfType<FizziksSys>().fizziksCollider.Add(this);
    }

    abstract public FizziksColliderShape GetColliderShape();
}
