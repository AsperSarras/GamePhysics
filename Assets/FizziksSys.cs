using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizziksSys : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<TestScript> fizziksObjects = new List<TestScript>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i =0; i < fizziksObjects.Count;i++)
        {
            fizziksObjects[i].velocity += gravity * fizziksObjects[i].gravityScale * Time.deltaTime;
        }
    }
}
