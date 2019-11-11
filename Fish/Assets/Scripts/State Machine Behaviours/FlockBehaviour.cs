using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockBehaviour : BaseState
{
    Fish thisFish;
    
    //Flock behaviour variables
    public float avoidRadius = 1.0f;

    //Movement
    float speed = 0f;
    bool turning = false;
    

    public override void Awake()
    {
        thisFish = gameObject.GetComponent<Fish>();

        //Define the transitions from this state
        transitions = new List<Transition>();

        //If fish is assigned to chase the light, change to ChasingLight state        
        transitions.Add(new Transition(new ShouldChaseLight(thisFish), GetComponent<ChaseLight>()));

        //If fish should be fleeing, change to the Flee state
        transitions.Add(new Transition(new ShouldFlee(thisFish), GetComponent<Flee>()));
    }

    // Start is called before the first frame update
    void Start()
    { 
        speed = Random.Range(thisFish.myManager.MinSpeed, thisFish.myManager.MaxSpeed);
    }

    //State initialisation
    public override void OnEnable()
    {
        thisFish.chasingLight = false;
    }

    // Update is called once per frame
    void Update()
    {
        avoidRadius = thisFish.myManager.AvoidRadius;
        Vector3 direction = Vector3.zero;
        turning = false;

        turning = thisFish.IsTurningDueObstacles(ref direction);
        if (!turning) turning = thisFish.IsTurningDueSwimmingLimits(ref direction);

        if (turning)
        {
            transform.rotation =                                                    //Interpolation between:
                        Quaternion.Slerp(transform.rotation,                        //Quaternion value for the current rotation
                                         Quaternion.LookRotation(direction),        //Quaternion value for the target rotation
                                         thisFish.myManager.RotationSpeed * Time.deltaTime); //by the rotation speed scaled to the time passed between frames
        }
        else
        {
            //Normal Flock behaviour
            if (thisFish.myManager.FlockDebug) thisFish.AddDebugColor(Color.green);

            //Randomly change speed now and then so the fish don end swimming all at the same speed
            if (Random.Range(0, 100) < 3)
            {
                speed = Random.Range(thisFish.myManager.MinSpeed, thisFish.myManager.MaxSpeed);
            }

            //Flock behaviour rules do not need to be added on every frame (too expensive)
            if (Random.Range(0, 100) < 15) AddFlockBehaviourRules();
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void AddFlockBehaviourRules()
    {
        GameObject[] allFish = thisFish.myManager.allFish;

        Vector3 centerOfMass = Vector3.zero;
        Vector3 avoidVector = Vector3.zero;
        float neighbourDistance;
        int neighbourCount = 0;
        float groupSpeed = 0.01f;

        neighbourCount = 0;

        foreach (GameObject fishGameObject in allFish)
        {
            if (fishGameObject != this.gameObject)
            {
                neighbourDistance = Vector3.Distance(fishGameObject.transform.position, this.transform.position);
                if (neighbourDistance <= thisFish.myManager.NeighbourDistance)
                {
                    centerOfMass += fishGameObject.transform.position;
                    neighbourCount++;

                    if (neighbourDistance < avoidRadius)
                    {
                        avoidVector += (this.transform.position - fishGameObject.transform.position).normalized * (avoidRadius - neighbourDistance);
                    }

                    FlockBehaviour f = fishGameObject.GetComponent<FlockBehaviour>();
                    groupSpeed += f.speed;
                }
            }
        }

        if (neighbourCount > 0)
        {
            centerOfMass = (centerOfMass / neighbourCount);            
            speed = groupSpeed / neighbourCount;

            Vector3 direction = centerOfMass + avoidVector - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation =                                            //Interpolation between:
                    Quaternion.Slerp(transform.rotation,                        //Quaternion value for the current rotation
                                        Quaternion.LookRotation(direction),        //Quaternion value for the target rotation
                                        thisFish.myManager.RotationSpeed * Time.deltaTime); //by the rotation speed scaled to the time passed between frames
            }
        }
    }

    
}

//Conditions for state transition. May be used in more than one state, so these are made public

//Condition to check if the fish has been assigned to chase the light ball
public class ShouldChaseLight : Condition
{
    Fish fish;

    public ShouldChaseLight(Fish fish) { this.fish = fish; }

    public override bool Test()
    {
        return fish.chasingLight;
    }
}

//Condition to check if the fish is fleeing from the submarine
public class ShouldFlee : Condition
{
    Fish fish;

    public ShouldFlee(Fish fish) { this.fish = fish; }

    public override bool Test()
    {
        return fish.fleeing;
    }
}

