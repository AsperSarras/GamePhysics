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
            if (obj.lockPos) continue;
            if (obj.grounded) continue;
            //if (obj.grounded)
            //{
            //    //obj.velocity = new Vector3(obj.velocity.x, 0.0f, obj.velocity.z);
            //    obj.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            //}
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

        //int numCheck = 0;
        for (int i = 0; i < fizziksCollider.Count; i++)
        {
            for (int j = i + 1; j < fizziksCollider.Count; j++)
            {
                if (i == j) continue;

                //Debug.Log("Checking: " + i + " with " + j);

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
        //Debug.Log(numCheck);
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

    void Grounded(FizziksObj a)
    {
        a.grounded = true;
    }

    void SphereSphereCollision(FizzikCollisionSphere a, FizzikCollisionSphere b)
    {
        Vector3 displacementBetweenSpheresAtoB = b.transform.position - a.transform.position;
        float distanceBetween = displacementBetweenSpheresAtoB.magnitude;
        float sumRadii = a.radius + b.radius;
        //
        float penetration = sumRadii - distanceBetween;
        //
        //bool isOverlapping = distanceBetween < sumRadii;
        bool isOverlapping = penetration > 0.0f;

        if (!isOverlapping)
        {
            return;
        }

        Color colorA = a.GetComponent<Renderer>().material.color = Color.red;
        Color colorB = b.GetComponent<Renderer>().material.color = Color.red;
        //Debug.Log(a.name + " collided with " + b.name);
        a.GetComponent<Renderer>().material.color = Color.red;
        b.GetComponent<Renderer>().material.color = Color.red;

        Vector3 CollisionNormalAtoB = displacementBetweenSpheresAtoB / distanceBetween;

        ComputeMovementScalars(a.fizzikObject, b.fizzikObject, out float mtvScalarA,out float mtvScalarB);

        Vector3 minTranslationVectorAtoB = CollisionNormalAtoB * penetration;
        Vector3 TransA = -minTranslationVectorAtoB * mtvScalarA;
        Vector3 TransB = minTranslationVectorAtoB * mtvScalarB;

        a.transform.position += TransA;
        b.transform.position += TransB;

        //CollisionInfo collisionInfo;
        //collisionInfo.colliderA = a;
        //collisionInfo.colliderB = b;
        //collisionInfo.colisionNormalAtoB
        //ApplyVelocityResponse(collisionInfo);

    }

    void SpherePlaneCollision(FizzikCollisionSphere a, FizzikCollisionPlane b)
    {
        Vector3 fromPlaneToSphereCenter = b.transform.position - a.transform.position;

        float dot = Vector3.Dot(fromPlaneToSphereCenter, b.GetNormal());
        Debug.Log("Name:" +a.name + "dot product: " + dot);

        float distanceFromPlaneToSphereCenter = Mathf.Abs(dot);

        //bool isOverlapping = a.radius > distanceFromPlaneToSphereCenter;

        bool isOverlapping = a.radius > dot*-1;


        //if (isOverlapping)
        //{
        //    Color colorA = a.GetComponent<Renderer>().material.color = Color.red;
        //    Color colorB = b.GetComponent<Renderer>().material.color = Color.red;
        //    //Debug.Log(a.name + " collided with " + b.name);
        //    //a.GetComponent<Renderer>().material.color = Color.red;
        //    //b.GetComponent<Renderer>().material.color = Color.red;
        //}


        //float penetration = a.radius - distanceFromPlaneToSphereCenter;
        //bool isOverlapping = penetration > 0.0f;

        if (!isOverlapping)
        {
            return;
        }

        Color colorA = a.GetComponent<Renderer>().material.color = Color.red;
        Color colorB = b.GetComponent<Renderer>().material.color = Color.red;
        //Debug.Log(a.name + " collided with " + b.name);
        ////a.GetComponent<Renderer>().material.color = Color.red;
        ////b.GetComponent<Renderer>().material.color = Color.red;

        //ComputeMovementScalars(a.fizzikObject, b.fizzikObject, out float mtvScalarA, out float mtvScalarB);

        //Vector3 CollisionNormalAtoB = fromPlaneToSphereCenter / distanceFromPlaneToSphereCenter;

        //Vector3 minTranslationVectorAtoB = CollisionNormalAtoB * penetration;
        
        Vector3 GroundedDist = new Vector3(a.transform.position.x, b.transform.position.y + a.radius, a.transform.position.z);
        
        ////Vector3 TransB = minTranslationVectorAtoB * mtvScalarB;

        a.transform.position = GroundedDist;
        Grounded(a.fizzikObject);
        
        ////b.transform.position += TransB;

    }

    //void ApplyVelocityResponse(CollisionInfo collisionInfo)
    //{

    //}
}
