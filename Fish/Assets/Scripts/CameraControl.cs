using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{    
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float rotationSpeed = 15f;

    [SerializeField] float maxVerticalRotation = 70f;

    float rotationX = 0f;
    float rotationY = 0f;

    // Update is called once per frame
    void Update()
    {
        GetDirection();
        GetMovement();
    }

    //Translate mouse movement into camera rotation
    private void GetDirection()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationY += Input.GetAxis("Mouse Y") * rotationSpeed;

        //Limit vertical rotation to not end upside down
        rotationY = Mathf.Clamp(rotationY, -maxVerticalRotation, maxVerticalRotation);

        Camera.main.transform.localRotation = Quaternion.Euler(-rotationY, rotationX, 0f);
    }

    //Translate keyboard press into camera displacement
    private void GetMovement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Vertical") > 0)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            transform.position += transform.forward * -moveSpeed * Time.deltaTime;
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            transform.position += transform.right * -moveSpeed * Time.deltaTime;
        }
    }
}
