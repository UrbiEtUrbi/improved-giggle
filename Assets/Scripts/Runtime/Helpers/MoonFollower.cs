using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonFollower : MonoBehaviour
{

    float y;
    private void Start()
    {
        y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
