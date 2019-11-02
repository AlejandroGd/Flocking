using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    [SerializeField] GameObject fishPrefab;
    [SerializeField] int numFish = 20;    
    [SerializeField] public Vector3 swimAreaLimit = new Vector3(5, 5, 5);
    [SerializeField] Bounds tankLimit;

    [SerializeField] bool movingTarget = false;
    [SerializeField] public float raycastLenght = 25f;

    [SerializeField] public GameObject target;
    public GameObject[] allFish;

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)]
    [SerializeField] float minSpeed; public float MinSpeed { get => minSpeed; }
    [Range(0.0f, 5.0f)]
    [SerializeField] float maxSpeed; public float MaxSpeed { get => maxSpeed; }
    [Range(1.0f, 20.0f)]
    [SerializeField] float neighbourDistance; public float NeighbourDistance { get => neighbourDistance; }
    [Range(1.0f, 10.0f)]
    [SerializeField] float avoidRadius; public float AvoidRadius { get => avoidRadius; }
    [Range(0.0f, 5.0f)]
    [SerializeField] float rotationSpeed; public float RotationSpeed { get => rotationSpeed; }

    [SerializeField] public bool FlockDebug = false;
    [SerializeField] public GameObject debugBall;


    // Start is called before the first frame update
    void Start()
    {        

        allFish = new GameObject[numFish];
        
        for (int i=0; i<numFish;i++)
        {
            SwimBoundary limits = GetSwimmingBoundary();
            Vector3 position = new Vector3(Random.Range(limits.minX, limits.maxX),
                                           Random.Range(limits.minY, limits.maxY),
                                           Random.Range(limits.minZ, limits.maxZ));

            allFish[i] = Instantiate(fishPrefab, position, Quaternion.identity);
            allFish[i].GetComponent<Fish>().myManager = this;
           // allFish[i].transform.parent = this.transform;           
        }       
    }

    // Update is called once per frame
    void Update()
    {
        if (movingTarget && Random.Range(0, 100) < 1)  ChangeTargetPosition();
    }

    private void ChangeTargetPosition()
    {
        SwimBoundary limits = GetSwimmingBoundary();
        target.transform.position = new Vector3(Random.Range(limits.minX, limits.maxX),
                                                Random.Range(limits.minY, limits.maxY),
                                                Random.Range(limits.minZ, limits.maxZ));       
    }

    //The swim area may not be enclosed within the tank if the Flock manager is moved near to the edge of the fish tank.
    //This function returns a vector that represent the area within the fish can swim. (The union between the swim area and the tank area)
    public SwimBoundary GetSwimmingBoundary()
    {
        SwimBoundary limit;

        limit.minX = tankLimit.center.x - (tankLimit.extents.x / 2);
        limit.maxX = tankLimit.center.x + (tankLimit.extents.x / 2);
        limit.minY = tankLimit.center.y - (tankLimit.extents.y / 2);
        limit.maxY = tankLimit.center.y + (tankLimit.extents.y / 2);
        limit.minZ = tankLimit.center.z - (tankLimit.extents.z / 2);
        limit.maxZ = tankLimit.center.z + (tankLimit.extents.z / 2);

        if (limit.minX < this.transform.position.x - swimAreaLimit.x) limit.minX = this.transform.position.x - swimAreaLimit.x;
        if (limit.maxX > this.transform.position.x + swimAreaLimit.x) limit.maxX = this.transform.position.x + swimAreaLimit.x;
        if (limit.minY < this.transform.position.y - swimAreaLimit.y) limit.minY = this.transform.position.y - swimAreaLimit.y;
        if (limit.maxY > this.transform.position.y + swimAreaLimit.y) limit.maxY = this.transform.position.y + swimAreaLimit.y;        
        if (limit.minZ < this.transform.position.z - swimAreaLimit.z) limit.minZ = this.transform.position.z - swimAreaLimit.z;
        if (limit.maxZ > this.transform.position.z + swimAreaLimit.z) limit.maxZ = this.transform.position.z + swimAreaLimit.z;
                     
        return limit;
    }   
}

public struct SwimBoundary
{
    public float minX, maxX, minY, maxY, minZ, maxZ;
}
