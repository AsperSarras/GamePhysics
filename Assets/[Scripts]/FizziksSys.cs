using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizziksSys : MonoBehaviour
{
    public float minRelativeVelocityBounce = 3.0f;
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
            obj.velocity += gravity * obj.gravityScale * Time.fixedDeltaTime;
        }

        foreach (FizziksObj obj in fizziksObjects)
        {
            if (obj.lockPos) continue;
            obj.transform.position = obj.transform.position + obj.velocity /* obj.speed*/ * Time.fixedDeltaTime;
        }

        CollisionUpdate();
    }

    void CollisionUpdate()
    {

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

                if (shapeA == FizziksColliderShape.AABB && shapeB == FizziksColliderShape.AABB)
                {
                    AABBAABBCollision((FizzikCollisionAABB)objectA,
                       (FizzikCollisionAABB)objectB);
                }

                if (shapeA == FizziksColliderShape.Sphere && shapeB == FizziksColliderShape.AABB)
                {
                    SphereAABBCollision((FizzikCollisionSphere)objectA,
                       (FizzikCollisionAABB)objectB);
                }
                if (shapeB == FizziksColliderShape.Sphere && shapeA == FizziksColliderShape.AABB)
                {
                    SphereAABBCollision((FizzikCollisionSphere)objectB,
                       (FizzikCollisionAABB)objectA);
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

        Vector3 minTranslationVectorAtoB = CollisionNormalAtoB * penetration;

        Vector3 contact = a.transform.position + CollisionNormalAtoB * a.radius;

        ApplyMinTranslationVector(a, b,minTranslationVectorAtoB, CollisionNormalAtoB, contact);

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

        //Vector3 contact = a.transform.position + CollisionNormalAtoB * a.radius;
        //ApplyMinTranslationVector(a, b, minTranslationVectorAtoB, CollisionNormalAtoB, contact);
    }

    void AABBAABBCollision(FizzikCollisionAABB a, FizzikCollisionAABB b)
    {
        Vector3 halfSizeA = a.GetHalfSize();
        Vector3 halfSizeB = b.GetHalfSize();

        Vector3 displacementAToB = b.transform.position - a.transform.position;
        float distX = Mathf.Abs(displacementAToB.x);
        float distY = Mathf.Abs(displacementAToB.y);
        float distZ = Mathf.Abs(displacementAToB.z);

        float penetrationX = halfSizeA.x + halfSizeB.x - distX;
        float penetrationY = halfSizeA.y + halfSizeB.y - distY;
        float penetrationZ = halfSizeA.z + halfSizeB.z - distZ;

        if (penetrationX < 0 || penetrationY < 0 || penetrationZ < 0)
        {
            return;
        }

        //Debug.Log("Debug ");

        Vector3 minimunTranslationVectorAToB;
        Vector3 collisionNormalAToB;
        Vector3 contact;

        if (penetrationX < penetrationY && penetrationX < penetrationZ)
        {
            collisionNormalAToB = new Vector3(Mathf.Sign(displacementAToB.x), 0, 0);
            minimunTranslationVectorAToB = collisionNormalAToB * penetrationX;
        }
        else if (penetrationY < penetrationX && penetrationY < penetrationZ)
        {
            collisionNormalAToB = new Vector3(0, Mathf.Sign(displacementAToB.y), 0);
            minimunTranslationVectorAToB = collisionNormalAToB * penetrationY;
        }
        else //if (penetrationZ < penetrationY && penetrationZ < penetrationX)
        {
            collisionNormalAToB = new Vector3(0, 0, Mathf.Sign(displacementAToB.z));
            minimunTranslationVectorAToB = collisionNormalAToB * penetrationZ;
        }
        contact = a.transform.position + minimunTranslationVectorAToB;
        ApplyMinTranslationVector(a, b, minimunTranslationVectorAToB, collisionNormalAToB, contact);
    }

    //
    void SphereAABBCollision(FizzikCollisionSphere a, FizzikCollisionAABB b)
    {
        Vector3 halfSizeB = b.GetHalfSize();

        Vector3 displacementAToB = b.transform.position - a.transform.position;
        float distX = Mathf.Abs(displacementAToB.x);
        float distY = Mathf.Abs(displacementAToB.y);
        float distZ = Mathf.Abs(displacementAToB.z);

        float penetrationX =  a.radius + halfSizeB.x - distX;
        float penetrationY =  a.radius + halfSizeB.y - distY;
        float penetrationZ =  a.radius + halfSizeB.z - distZ;

        if (penetrationX < 0 || penetrationY < 0 || penetrationZ < 0)
        {
            return;
        }

        Vector3 minimunTranslationVectorAToB;
        Vector3 collisionNormalAToB;
        Vector3 contact;

        if (penetrationX < penetrationY && penetrationX < penetrationZ)
        {
            collisionNormalAToB = new Vector3(Mathf.Sign(displacementAToB.x), 0, 0);
            minimunTranslationVectorAToB = collisionNormalAToB * penetrationX;
        }
        else if (penetrationY < penetrationX && penetrationY < penetrationZ)
        {
            collisionNormalAToB = new Vector3(0, Mathf.Sign(displacementAToB.y), 0);
            minimunTranslationVectorAToB = collisionNormalAToB * penetrationY;
        }
        else //if (penetrationZ < penetrationY && penetrationZ < penetrationX)
        {
            collisionNormalAToB = new Vector3(0, 0, Mathf.Sign(displacementAToB.z));
            minimunTranslationVectorAToB = collisionNormalAToB * penetrationZ;
        }
        contact = a.transform.position + minimunTranslationVectorAToB;
        ApplyMinTranslationVector(a, b, minimunTranslationVectorAToB, collisionNormalAToB, contact);
    }

    void ApplyMinTranslationVector(FizzikCollisionShapeBase a, FizzikCollisionShapeBase b, Vector3 minTranslationVector, Vector3 CollisionNormalAtoB, Vector3 contact)
    {
        ComputeMovementScalars(a.fizzikObject, b.fizzikObject, out float mtvScalarA, out float mtvScalarB);

        Vector3 TransA = -minTranslationVector * mtvScalarA;
        Vector3 TransB = minTranslationVector * mtvScalarB;

        a.transform.position += TransA;
        b.transform.position += TransB;

        //
        CollisionInfo collisionInfo;

        collisionInfo.colliderA = a;
        collisionInfo.colliderB = b;
        collisionInfo.collisionNormalAtoB = CollisionNormalAtoB;
        collisionInfo.contactPoint = contact;
        //
        ApplyVelocityResponse(collisionInfo);
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
        float deltaV;
        if (relativeNormalVelocityAB < -minRelativeVelocityBounce)
        {
            deltaV = -(relativeNormalVelocityAB * (1.0f + restitution));
        }
        else
        {
            deltaV = -(relativeNormalVelocityAB * (1.0f));
        }
        //float deltaV = -(relativeNormalVelocityAB * (1.0f + restitution));
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
            impulse = deltaV / ((1.0f / objA.mass) + (1.0f / objB.mass));
            objA.velocity -= normal * impulse / objA.mass;
            objB.velocity += normal * impulse / objB.mass;
        }
        else
        {
            return;
        }

        Vector3 relativeSurfaceVelocity = relativeVelocityAB - (relativeNormalVelocityAB * normal);

        ApplyFriction(objA, objB, relativeSurfaceVelocity, normal);
    }

    void ApplyFriction(FizziksObj a, FizziksObj b, Vector3 relativeSurfaceVelocity, Vector3 normalAToB)
    {
        float minFrictionVelocity = 0.1f;
        float kFrictionCoefficient = (a.friction + b.friction) * 0.2f;
        float relativeSpeed = relativeSurfaceVelocity.magnitude;



        if (relativeSpeed < minFrictionVelocity)
        {
            a.velocity = a.velocity * 0.0f;
            b.velocity = b.velocity * 0.0f;
            return;
        }

        Vector3 directionToApplyFriction = relativeSurfaceVelocity / relativeSpeed;
        float gravityAccelAlongNormal = Vector3.Dot(gravity, normalAToB);

        Vector3 frictionAccel = directionToApplyFriction * -gravityAccelAlongNormal * kFrictionCoefficient;

        if (!a.lockPos)
        {
            a.velocity -= frictionAccel * Time.fixedDeltaTime;
        }
        if (!b.lockPos)
        {
            b.velocity += frictionAccel * Time.fixedDeltaTime;
        }
    }

}
