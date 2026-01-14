using System;
using UnityEngine;

// 오브젝트가 카메라만 계속 바라보게 하는 기능
public class Billboard : MonoBehaviour
{
    private Transform camTransform;
    
    private void LateUpdate()
    {
        camTransform = Camera.main.transform;

        transform.LookAt(transform.position + (camTransform.rotation * Vector3.forward));
    }
}
