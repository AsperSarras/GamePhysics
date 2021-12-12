using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FizziksObj : MonoBehaviour
{
    public float mass = 1.0f;
    public Vector3 velocity = Vector3.zero;
    public float speed = 1.0f;
    public float gravityScale = 1.0f;
    public float bounciness = 0.5f;
    public float friction = 0.5f;
    public bool lockPos = false;

    //
    public FizzikCollisionShapeBase shape = null;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<FizziksSys>().fizziksObjects.Add(this);
        shape = GetComponent<FizzikCollisionShapeBase>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
