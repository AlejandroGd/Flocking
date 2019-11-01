using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public FlockManager myManager;
    float speed;
    public float avoidRadius = 1.0f;

    bool turning = false;
    Bounds b;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(myManager.MinSpeed, myManager.MaxSpeed);
        b = new Bounds(myManager.transform.position, myManager.swimLimits * 3);
    }

    // Update is called once per frame
    void Update()
    {
        avoidRadius = myManager.AvoidRadius;
        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero;

        //Avoid obstacles

        

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, myManager.raycastLenght))
        {
            turning = true;
            Debug.DrawRay(this.transform.position, this.transform.forward * myManager.raycastLenght, Color.red);
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        } 
        //Keep within boundaries
        else if (!b.Contains(transform.position))
        {
            turning = true;
            direction = b.center - transform.position;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {  
            transform.rotation =                                            //Interpolation between:
                        Quaternion.Slerp(transform.rotation,                        //Quaternion value for the current rotation
                                         Quaternion.LookRotation(direction),        //Quaternion value for the target rotation
                                         myManager.RotationSpeed * Time.deltaTime); //by the rotation speed scaled to the time passed between frames
        }
        else
        {
            if (Random.Range(0, 100) < 3)
            {
                speed = Random.Range(myManager.MinSpeed, myManager.MaxSpeed);
            }

            if (Random.Range(0, 100) < 15) AddFlockBehaviourRules();
        }
                
        transform.position += transform.forward * speed * Time.deltaTime;    
    }

    private void AddFlockBehaviourRules()
    {
        GameObject[] allFish = myManager.allFish;

        Vector3 centerOfMass = Vector3.zero;
        Vector3 avoidVector = Vector3.zero;
        float groupSpeed = 0.01f;
        float neighbourDistance;
        int neighbourCount = 0;

        foreach(GameObject fish in allFish)
        {
            if (fish != this.gameObject)
            {
                neighbourDistance = Vector3.Distance(fish.transform.position, this.transform.position);
                if (neighbourDistance <= myManager.NeighbourDistance)
                {
                    centerOfMass += fish.transform.position;
                    neighbourCount++;

                    if (neighbourDistance < avoidRadius)
                    {
                        avoidVector += (this.transform.position - fish.transform.position).normalized * (avoidRadius - neighbourDistance);
                    }

                    Fish f = fish.GetComponent<Fish>();
                    groupSpeed += f.speed;
                }
            }

            if (neighbourCount > 0)
            {
                centerOfMass = centerOfMass / neighbourCount + (myManager.target.transform.position - this.transform.position);
                speed = groupSpeed / neighbourCount;

                Vector3 direction = centerOfMass + avoidVector - transform.position;
                if (direction != Vector3.zero)
                {
                    transform.rotation =                                            //Interpolation between:
                        Quaternion.Slerp(transform.rotation,                        //Quaternion value for the current rotation
                                         Quaternion.LookRotation(direction),        //Quaternion value for the target rotation
                                         myManager.RotationSpeed * Time.deltaTime); //by the rotation speed scaled to the time passed between frames
                }
            }
        }
    }
}
