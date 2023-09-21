using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int verticalMin;
    [SerializeField] int verticalMax;
    [SerializeField] bool invertY;

    float xRotation;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime *  sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        if (invertY) xRotation += mouseY;
        else xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, verticalMin, verticalMax);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.parent.Rotate(Vector3.up * mouseX);
    }

}
