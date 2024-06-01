using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    [SerializeField] float defaultVelocity=0.1f;
    [SerializeField] TMP_InputField velocityInput;

    Vector3 followObjectPosition;

    private void Start()
    {
        OnUserChangedVelocityInInputField();
    }

    // Update is called once per frame
    void Update()
    {
        if (followObject != null)
        {
            followObjectPosition = followObject.transform.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, followObjectPosition, defaultVelocity * Time.deltaTime);
    }

    public void OnUserChangedVelocityInInputField()
    {
        defaultVelocity = float.Parse(velocityInput.text);
    }
}
