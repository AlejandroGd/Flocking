using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Vector3 initialPosition = new Vector3(-100, 0, 100);
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float rotationSpeed = 50f;

    float rotationX = 0f;
    float rotationY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = initialPosition;
    }

    // Update is called once per frame
    void Update()
    {
        GetDirection();
        GetMovement();
    }

    private void GetDirection()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationY += Input.GetAxis("Mouse Y") * rotationSpeed;

        Camera.main.transform.localRotation = Quaternion.Euler(-rotationY, rotationX, 0f);
    }

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
