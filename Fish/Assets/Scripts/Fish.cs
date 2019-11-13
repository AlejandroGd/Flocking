using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**Common functionality to all fishes independent of what state is active.
 * 
 * The movement of the fish is always forward. All behaviours will compute any change of direction and rotate the gameobject as necesary.
 */
public class Fish : MonoBehaviour
{
    public FlockManager myManager;
    float raycastDistance = 1f;   

    //Debug variables    
    [SerializeField] MeshRenderer meshRenderer;

    //State machine variables
    public bool chasingLight = false;
    public bool fleeing = false;
      
    //Add tint to the fish mesh to identify specific behaviours
    public void AddDebugColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    /**
     * Casts a front, left and right rays to check if the fish needs to turn due to obstacles. If it dies, direction returns 
     * a vector with the direction the fish needs to turn to. Not having up and down raycast sometimes results in fish slightly 
     * getting into objects but, so far, I do not think the difference was enough to worth the extra computing time. (Right now 
     * is just a small fish tank and may not seem different but I tried to keep it scalable just in case)     
     */
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

    //Checks if position is inside swim limits (within the fish tank and within swimming area)
    //If it is not, direction contains the vector with the direction the fish needs to turn to to go back to the swimming area.
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

    //The fleeing flag == true is condition to change to a flee state.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Submarine>())
        {
            fleeing = true;
        }
    }
}