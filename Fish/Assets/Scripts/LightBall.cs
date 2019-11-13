using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBall : MonoBehaviour
{
    float temp = 0f;
    Vector3 initialPosition = Vector3.zero;

    [SerializeField] float speed = 1;
    [SerializeField] float maxDisplacement = 2;

    public bool fishAssigned = false;  //Only one fish at a time. Rudimentary "slot" (already a fish on it or not) system.

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;    
    }

    // Update is called once per frame
    void Update()
    {
        //Simulate floating movement using oscilating values of Sin function.
        temp += Time.deltaTime * speed;
        if (temp >= 360) temp -=360;

        transform.position = initialPosition + transform.up * Mathf.Sin(temp) * maxDisplacement; 
    }

    //Lightball only collide with fish layer so it is a fish for sure
    private void OnTriggerEnter(Collider other)
    {       
        if (!fishAssigned)
        {
            fishAssigned = true;
            other.gameObject.GetComponent<Fish>().chasingLight = true;
        }        
    }
}
