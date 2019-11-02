using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    [SerializeField] GameObject fishPrefab;
    [SerializeField] int numFish = 20;    
    [SerializeField] public Vector3 swimLimits = new Vector3(5, 5, 5);

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


    [SerializeField] public GameObject debugBall;


    // Start is called before the first frame update
    void Start()
    {        
        allFish = new GameObject[numFish];
        
        for (int i=0; i<numFish;i++)
        {
            Vector3 position = this.transform.position + new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                      Random.Range(-swimLimits.y, swimLimits.y),
                                      Random.Range(-swimLimits.z, swimLimits.z));            

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
        target.transform.position = this.transform.position + 
                                    new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                                Random.Range(-swimLimits.y, swimLimits.y),
                                                Random.Range(-swimLimits.z, swimLimits.z));       
    }
}
