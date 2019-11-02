using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public FlockManager myManager;
    float speed;
    public float avoidRadius = 1.0f;

    bool turning = false;    

    [SerializeField] bool fishDebug = false;

    [SerializeField] MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(myManager.MinSpeed, myManager.MaxSpeed);  
    }

    // Update is called once per frame
    void Update()
    {
        avoidRadius = myManager.AvoidRadius;
        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero;                     
        
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, myManager.raycastLenght, LayerMask.GetMask("Obstacles")))
        {
            //Avoid obstacles
            turning = true;
            Debug.DrawRay(this.transform.position, this.transform.forward * myManager.raycastLenght, Color.red);
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
            meshRenderer.material.color = Color.red;
        }         
        else if (IsOutsideSwimLimits())
        {
            //Keep within boundaries
            turning = true;
            direction = myManager.transform.position - transform.position;
            meshRenderer.material.color = Color.yellow;
        }
        else
        {
            //Normal Flock behaviour
            meshRenderer.material.color = Color.green;
            turning = false;
        }

        if (turning)
        {  
            transform.rotation =                                                    //Interpolation between:
                        Quaternion.Slerp(transform.rotation,                        //Quaternion value for the current rotation
                                         Quaternion.LookRotation(direction),        //Quaternion value for the target rotation
                                         myManager.RotationSpeed * Time.deltaTime); //by the rotation speed scaled to the time passed between frames
        }
        else
        {
            //Randomly change speed now and then so the fish don end swimming all at the same speed
            if (Random.Range(0, 100) < 3)
            {
                speed = Random.Range(myManager.MinSpeed, myManager.MaxSpeed);
            }

            //Flock behaviour rules do not need to be added on every frame (too expensive)
            if (Random.Range(0, 100) < 15) AddFlockBehaviourRules();
        }
                
        transform.position += transform.forward * speed * Time.deltaTime;    
    }

    private bool IsOutsideSwimLimits()
    {
        Vector3 fishPos = transform.position;
        SwimBoundary limit = myManager.GetSwimmingBoundary();        

        if (fishPos.x < limit.minX ||
            fishPos.x > limit.maxX ||
            fishPos.y < limit.minY ||
            fishPos.y > limit.maxY ||
            fishPos.z < limit.minZ ||
            fishPos.z > limit.maxZ)
        {
            return true;
        }
        else
        {
            return false;
        }        
    }

    private void AddFlockBehaviourRules()
    {
        GameObject[] allFish = myManager.allFish;

        Vector3 centerOfMass = Vector3.zero;
        Vector3 avoidVector = Vector3.zero;        
        float neighbourDistance;
        int neighbourCount = 0;
        float groupSpeed = 0.01f;

        neighbourCount = 0;

        foreach (GameObject fish in allFish)
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
        }

        if (neighbourCount > 0)
        {
            centerOfMass = (centerOfMass / neighbourCount);
            if (fishDebug) DebugCenterOfMass(centerOfMass);
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

    private void DebugCenterOfMass(Vector3 position)
    {
        myManager.debugBall.transform.position = position;
    }
}
