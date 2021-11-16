using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizziksSys : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<TestScript> fizziksObjects = new List<TestScript>();
    public List<FizzikCollisionShapeBase> fizziksCollider = new List<FizzikCollisionShapeBase>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

                if(shapeA == FizziksColliderShape.Sphere && shapeB == FizziksColliderShape.Plane)
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

    void SphereSphereCollision(FizzikCollisionSphere a, FizzikCollisionSphere b)
    {
        Vector3 displacementBetweenSpheres = a.transform.position - b.transform.position;
        float distanceBetween = displacementBetweenSpheres.magnitude;
        float sumRadii = a.radius + b.radius;
        bool isOverlapping = distanceBetween < sumRadii;
        if(isOverlapping)
        {
            Color colorA= a.GetComponent<Renderer>().material.color = Color.red;
            Color colorB = b.GetComponent<Renderer>().material.color = Color.red;
            Debug.Log(a.name + " collided with " + b.name);
            a.GetComponent<Renderer>().material.color = Color.red;
            b.GetComponent<Renderer>().material.color = Color.red;
        }

    }

    void SpherePlaneCollision(FizzikCollisionSphere a, FizzikCollisionPlane b)
    {
        Vector3 fromPlaneToSphereCenter = a.transform.position - b.transform.position;

        float dot = Vector3.Dot(fromPlaneToSphereCenter, b.GetNormal());
        Debug.Log("dot product: " + dot);

        float distanceFromPlaneToSphereCenter = Mathf.Abs(dot);

        bool isOverlapping = a.radius > distanceFromPlaneToSphereCenter;

        if (isOverlapping)
        {
            Color colorA = a.GetComponent<Renderer>().material.color = Color.red;
            Color colorB = b.GetComponent<Renderer>().material.color = Color.red;
            Debug.Log(a.name + " collided with " + b.name);
            a.GetComponent<Renderer>().material.color = Color.red;
            b.GetComponent<Renderer>().material.color = Color.red;
        }

    }
}
