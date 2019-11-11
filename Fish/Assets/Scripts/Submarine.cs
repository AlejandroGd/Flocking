using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    [SerializeField] GameObject[] pathPoints;    

    [SerializeField] float speed = 5f;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float targetReachedDistance = 2f;
    
    int currentPointIndex = 0;
    
    void Update()
    {
        Move();       
    }

    private void Move()
    {
        if ((gameObject.transform.position - pathPoints[currentPointIndex].transform.position).sqrMagnitude < targetReachedDistance * targetReachedDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % pathPoints.Length;            
        }
        else
        {
            Vector3 direction = pathPoints[currentPointIndex].transform.position - gameObject.transform.position;
            if (Vector3.Angle(transform.forward, direction) > 1)
            {
                transform.rotation =
                    Quaternion.Slerp(transform.rotation,
                                     Quaternion.LookRotation(direction),
                                     Time.deltaTime * rotationSpeed);
            }

            transform.position += transform.forward * speed * Time.deltaTime;           
        }
    }
}
