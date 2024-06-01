using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjectDestroyer : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    [SerializeField] GameObject enableOnDestroy;

    private void Start()
    {
        enableOnDestroy.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == followObject)
        {
            Destroy(followObject);
            enableOnDestroy.SetActive(true);
        }
    }
}