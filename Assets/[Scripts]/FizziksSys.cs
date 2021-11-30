using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizziksSys : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<FizziksObj> fizziksObjects = new List<FizziksObj>();
    public List<FizzikCollisionShapeBase> fizziksCollider = new List<FizzikCollisionShapeBase>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        foreach (FizziksObj obj in fizziksObjects)
        {
            if (obj.lockPos) continue;
            obj.transform.position = obj.transform.position + obj.velocity * Time.fixedDeltaTime;
        }

        foreach (FizziksObj obj in fizziksObjects)
        {
            if (obj.NoGravity)
            {
                obj.gravityScale = 0.0f;
            }
            obj.velocity += gravity * obj.gravityScale * Time.fixedDeltaTime;

        }

        CollisionUpdate();
    }

    void CollisionUpdate()
    {

        for (int i = 0; i < fizziksObjects.Count; i++)
        {
            fizziksObjects[i].velocity += gravity * fizziksObjects[i].gravityScale * Time.deltaTime;
        }

        for (int i = 0; i < fizziksCollider.Count; i++)
        {
            for (int j = i + 1; j < fizziksCollider.Count; j++)
            {
                if (i == j) continue;;

                FizzikCollisionShapeBase objectA = fizziksCollider[i];
                FizzikCollisionShapeBase objectB = fizziksCollider[j];
                FizziksColliderShape shapeA = objectA.GetColliderShape();
                FizziksColliderShape shapeB = objectB.GetColliderShape();

                if (shapeA == FizziksColliderShape.Sphere && shapeB == FizziksColliderShape.Sphere)
                {
                    SphereSphereCollision((FizzikCollisionSphere)objectA,
                       (FizzikCollisionSphere)objectB);
                }

                if (shapeA == FizziksColliderShape.Sphere && shapeB == FizziksColliderShape.Plane)
                {
                    SpherePlaneCollision((FizzikCollisionSphere)objectA,
                       (FizzikCollisionPlane)objectB);
                }
                if (shapeB == FizziksColliderShape.Sphere && shapeA == FizziksColliderShape.Plane)
                {
                    SpherePlaneCollision((FizzikCollisionSphere)objectB,
                       (FizzikCollisionPlane)objectA);
                }

            }
        }
    }

    void ComputeMovementScalars(FizziksObj a, FizziksObj b, out float mtvScalarA, out float mtvScalarB)
    {
        if (a.lockPos && !b.lockPos)
        {
            mtvScalarA = 0.0f;
            mtvScalarB = 1.0f;
            return;
        }
        else if (!a.lockPos && b.lockPos)
        {
            mtvScalarA = 1.0f;
            mtvScalarB = 0.0f;
            return;
        }
        else if (!a.lockPos && !b.lockPos)
        {
            mtvScalarA = 0.5f;
            mtvScalarB = 0.5f;
            return;
        }
        mtvScalarA = 0.0f;
        mtvScalarB = 0.0f;
    }

    void SphereSphereCollision(FizzikCollisionSphere a, FizzikCollisionSphere b)
    {
        Vector3 displacementBetweenSpheresAtoB = b.transform.position - a.transform.position;
        float distanceBetween = displacementBetweenSpheresAtoB.magnitude;
        float sumRadii = a.radius + b.radius;
        float penetration = sumRadii - distanceBetween;
        bool isOverlapping = penetration > 0.0f;

        if (!isOverlapping)
        {
            return;
        }

        {
            Color colorA = a.GetComponent<Renderer>().material.color = Color.red;
            Color colorB = b.GetComponent<Renderer>().material.color = Color.red;
            a.GetComponent<Renderer>().material.color = Color.red;
            b.GetComponent<Renderer>().material.color = Color.red;
        }

        Vector3 CollisionNormalAtoB = displacementBetweenSpheresAtoB / distanceBetween;

        ComputeMovementScalars(a.fizzikObject, b.fizzikObject, out float mtvScalarA, out float mtvScalarB);

        Vector3 minTranslationVectorAtoB = CollisionNormalAtoB * penetration;
        Vector3 TransA = -minTranslationVectorAtoB * mtvScalarA;
        Vector3 TransB = minTranslationVectorAtoB * mtvScalarB;

        a.transform.position += TransA;
        b.transform.position += TransB;

        //
        CollisionInfo collisionInfo;

        collisionInfo.colliderA = a;
        collisionInfo.colliderB = b;
        collisionInfo.collisionNormalAtoB = CollisionNormalAtoB;
        collisionInfo.contactPoint = a.transform.position + CollisionNormalAtoB * a.radius;
        //
        ApplyVelocityResponse(collisionInfo);

    }

    void SpherePlaneCollision(FizzikCollisionSphere a, FizzikCollisionPlane b)
    {
        Vector3 fromPlaneToSphereCenter = a.transform.position - b.transform.position;

        float dot = Vector3.Dot(fromPlaneToSphereCenter, b.GetNormal());
        float distanceFromPlaneToSphereCenter = Mathf.Abs(dot);
        bool isOverlapping = a.radius > dot;

        if (!isOverlapping)
        {
            return;
        }

        Color colorA = a.GetComponent<Renderer>().material.color = Color.red;

        float penetration = a.radius - distanceFromPlaneToSphereCenter;

        Vector3 CollisionNormalAtoB = b.GetNormal();

        Vector3 minTranslationVectorAtoB = CollisionNormalAtoB * penetration;
        Vector3 TransA = minTranslationVectorAtoB * 1.0f;

        a.transform.position += minTranslationVectorAtoB;

        ////
        //CollisionInfo collisionInfo;

        //collisionInfo.colliderA = a;
        //collisionInfo.colliderB = b;
        //collisionInfo.collisionNormalAtoB = CollisionNormalAtoB;
        //collisionInfo.contactPoint = a.transform.position + CollisionNormalAtoB * a.radius;

        //ApplyVelocityResponse(collisionInfo);
    }

    void ApplyVelocityResponse(CollisionInfo collisionInfo)
    {
        FizziksObj objA = collisionInfo.colliderA.fizzikObject;
        FizziksObj objB = collisionInfo.colliderB.fizzikObject;
        Vector3 normal = collisionInfo.collisionNormalAtoB;

        Vector3 relativeVelocityAB = objB.velocity - objA.velocity;

        float relativeNormalVelocityAB = Vector3.Dot(relativeVelocityAB, normal);

        if (relativeNormalVelocityAB >= 0.0f)
        {
            return;
        }

        float restitution = (objA.bounciness + objB.bounciness) * 0.5f;

        float deltaV = -(relativeNormalVelocityAB * (1.0f + restitution));
        float impulse;

        if (!objA.lockPos && objB.lockPos)
        {
            impulse = deltaV * objA.mass;
            objA.velocity -= normal * impulse / objA.mass;
        }

        else if (objA.lockPos && !objB.lockPos)
        {
            impulse = deltaV * objB.mass;
            objB.velocity += normal * impulse / objB.mass;
        }

        else if (!objA.lockPos && !objB.lockPos)
        {
            impulse = deltaV / ((1.0f / objA.mass) + (1.0f / objB.mass)); //something with deltaV and both masses. Avarage of both masses
            objA.velocity -= normal * impulse / objA.mass;
            objB.velocity += normal * impulse / objB.mass;
        }
        else
        {
            return;
        }


    }

}
