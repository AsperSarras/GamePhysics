using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestScript : MonoBehaviour
{
    public float mass = 1.0f;
    public Vector3 velocity = Vector3.zero;
    //public FizziksSys fizzikSys;
    public float gravityScale= 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Hello World! " + gameObject.name + " !");
        //fizzikSys = FindObjectOfType<FizziksSys>();
        //fizzikSys.fizziksObjects.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = transform.position + new Vector3(0,Mathf.Sin(Time.time),0);
        transform.position = transform.position + velocity * Time.deltaTime;
        //velocity += fizzikSys.gravity * gravityScale * Time.deltaTime;
    }
}
