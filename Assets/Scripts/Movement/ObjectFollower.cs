using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField] GameObject objectToFollow;
    [SerializeField] float defaultVelocity=0.1f;
    [SerializeField] TMP_InputField velocityInput;
    [SerializeField] bool increaseVelocityWhenBeingCloseToObjectToFollow;
    [SerializeField] float increasedVelocity = 0.5f;
    [SerializeField] float maxDistanceToObjectToFollowAtIncreasedVelocity = 5f;

    Vector3 followObjectPosition;
    float velocity;

    void Update()
    {
        UpdateVelocity();
        FollowObject();
    }

    private void UpdateVelocity()
    {
        if (increaseVelocityWhenBeingCloseToObjectToFollow && IsCloseToObjectToFollow())
        {
            velocity = increasedVelocity;
        }
        else
        {
            velocity = defaultVelocity;
        }
    }

    private bool IsCloseToObjectToFollow()
    {
        return Vector3.Distance(transform.position, objectToFollow.transform.position) < maxDistanceToObjectToFollowAtIncreasedVelocity;
    }

    private void FollowObject()
    {
        if (objectToFollow != null)
        {
            followObjectPosition = objectToFollow.transform.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, followObjectPosition, velocity * Time.deltaTime);
    }
}
