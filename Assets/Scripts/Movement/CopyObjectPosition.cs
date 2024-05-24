using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyObjectPosition : MonoBehaviour
{
    [SerializeField] Transform copyObject;

    private void Update()
    {
        transform.position = copyObject.position;
    }
}
