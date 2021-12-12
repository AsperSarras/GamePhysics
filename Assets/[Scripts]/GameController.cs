using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Sphere Launch Properties")]
    public List<GameObject> sPrefabs;
    public float frameDelay;
    public float Offset;
    int ball = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxisRaw("B1") > 0)
        {
            ball = 0;
        }
        if (Input.GetAxisRaw("B2") > 0)
        {
            ball = 1;
        }
        if (Input.GetAxisRaw("B3") > 0)
        {
            ball = 2;
        }
        if (Input.GetAxisRaw("B4") > 0)
        {
            ball = 3;
        }

        if ((Input.GetAxisRaw("Fire1") > 0) && (Time.frameCount % frameDelay == 0))
        {
            var bullet = Instantiate(sPrefabs[ball], Camera.main.transform.position + Camera.main.transform.forward * Offset, Quaternion.identity);
            bullet.GetComponent<FizziksObj>().velocity = Camera.main.transform.forward*5;
        }
    }
}
