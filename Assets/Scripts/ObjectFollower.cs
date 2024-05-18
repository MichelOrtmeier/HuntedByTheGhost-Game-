using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    [SerializeField] float defaultVelocity=0.1f;
    [SerializeField] float collisionVelocity=0.07f;

    Vector3 followObjectPosition;
    float velocity;

    private void Start()
    {
        velocity = defaultVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if(followObject != null)
        {
            followObjectPosition = followObject.transform.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, followObjectPosition, velocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == followObject)
        {
            Destroy(followObject);
        }
        else
        {
            velocity = collisionVelocity;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        velocity = defaultVelocity;
    }
}
