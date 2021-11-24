using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FizziksColliderShape
{
    Sphere,
    Plane,
    AABB
}
public struct CollisionInfo
{
    public FizzikCollisionShapeBase colliderA;
    public FizzikCollisionShapeBase colliderB;
    public Vector3 collisionNormalAtoB;
    public Vector3 contactPoint;
}

[RequireComponent(typeof(FizziksObj))]
public abstract class FizzikCollisionShapeBase : MonoBehaviour
{
    public FizziksObj fizzikObject;
    public void Start()
    {
        fizzikObject = GetComponent<FizziksObj>();
        FindObjectOfType<FizziksSys>().fizziksCollider.Add(this);
    }

    abstract public FizziksColliderShape GetColliderShape();
}
