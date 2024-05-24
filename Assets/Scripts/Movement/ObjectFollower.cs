using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    [SerializeField] float defaultVelocity=0.1f;

    Vector3 followObjectPosition;

    // Update is called once per frame
    void Update()
    {
        if(followObject != null)
        {
            followObjectPosition = followObject.transform.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, followObjectPosition, defaultVelocity * Time.deltaTime);
    }
}
