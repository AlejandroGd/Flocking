using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Common functionality to all fishes independent of what state is active.
public class Fish : MonoBehaviour
{
    public FlockManager myManager;
    float raycastDistance = 1f;   

    //Debug variables
    [SerializeField] bool fishDebug = false; public bool FishDebug { get => fishDebug; }
    [SerializeField] MeshRenderer meshRenderer;

    //State machine variables
    public bool chasingLight = false;
      
    //Add tint to the fish mesh to identify specific behaviours
    public void AddDebugColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    //Cast a front, left and right rays to check if the fish needs to turn due to obstacles.
    //If it dies, direction returns a vector with the direction the fish needs to turn to.
    public bool IsTurningDueObstacles(ref Vector3 direction)
    {
        bool turning = false;
        RaycastHit hit = new RaycastHit();
        raycastDistance = myManager.rayCastDistance;

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, myManager.raycastLenght, LayerMask.GetMask("Obstacles")) ||
            Physics.Raycast(this.transform.position, this.transform.right, out hit, raycastDistance, LayerMask.GetMask("Obstacles")) ||
            Physics.Raycast(this.transform.position, -this.transform.right, out hit, raycastDistance, LayerMask.GetMask("Obstacles")))
        {
            //Avoid obstacles
            turning = true;
            if (myManager.FlockDebug)
            {
                Debug.DrawRay(this.transform.position, this.transform.forward * myManager.raycastLenght, Color.red);
                Debug.DrawRay(this.transform.position, this.transform.right * raycastDistance, Color.red);
                Debug.DrawRay(this.transform.position, -this.transform.right * raycastDistance, Color.red);
                meshRenderer.material.color = Color.red;
            }
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }

        return turning;
    }

    //Cast a front, left and right rays to check if the fish needs to turn due to being swimming out of limits.
    //If it dies, direction contains the vector with the direction the fish needs to turn to.
    public bool IsTurningDueSwimmingLimits(ref Vector3 direction)
    {
        bool turning = false;
        if (IsOutsideSwimLimits())
        {
            //Keep within boundaries
            turning = true;
            direction = myManager.transform.position - transform.position;
            if (myManager.FlockDebug) meshRenderer.material.color = Color.yellow;
        }
        return turning;
    }
    //Combines the fish tank limits with the flock one and determines if the fish is within the union.
    //(both inside the fisk tank and in the flock swim limits)
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
}
