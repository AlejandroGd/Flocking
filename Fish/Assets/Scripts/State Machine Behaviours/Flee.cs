using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** FLEE BEHAVIOUR
 * 
 * Any fish close to the moving submarine will try to rapidly swim away from it.
 * Speed and rotation speed serialized as they will be specific from a flee behaviour.
 */
public class Flee : BaseState
{
    [SerializeField] float speed = 10f;
    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] float fleeTimer = 1.5f;

    Submarine submarine;
    Fish thisFish;

    float timer = 0;

    public override void Awake()
    {
        thisFish = gameObject.GetComponent<Fish>();
        submarine = FindObjectOfType<Submarine>();

        //Define the transitions from this state
        transitions = new List<Transition>();
       
        transitions.Add(new Transition(new FleeingTimerReachedZero(this), gameObject.GetComponent<FlockBehaviour>()));
    }

    //State initialisation
    public override void OnEnable()
    {
        //Initialise Timer
        timer = fleeTimer;
    }

    // Update is called once per frame.
    //The fish will swim away from the submarine for a specific amount of time. (Which seems better than checking distances 
    //for all fishes involved, which is more expensive in terms of computation if scaling, and still gets a nice result)
    void Update()
    {
        Vector3 direction = Vector3.zero;

        //Avoiding obstacles and stay within the fishtank take preference over behaviour.
        bool turning = thisFish.IsTurningDueObstacles(ref direction);
        if (!turning) turning = thisFish.IsTurningDueSwimmingLimits(ref direction);

        if (turning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  rotationSpeed * Time.deltaTime);
        }
        else
        {
            //Flee behaviour
            if (thisFish.myManager.FlockDebug) thisFish.AddDebugColor(Color.blue);

            //Turn the fish away from the submarine
            direction = transform.position - submarine.gameObject.transform.position;
            if (Vector3.Angle(transform.forward, direction) > 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  rotationSpeed * Time.deltaTime);
            }            
        }

        //Flee
        transform.position += transform.forward * speed * Time.deltaTime;

        //UpdateTimer 
        timer -= Time.deltaTime;       
    }

    public override void OnDisable()
    {
        //Before leaving state, leave the lightball ready for another fish to check it out
        thisFish.fleeing = false;
    } 


    //Specific for getting out this state only, so private.
    private class FleeingTimerReachedZero : Condition
    {
        Flee fleeBehaviour;

        public FleeingTimerReachedZero(Flee fleeBehaviour)
        {
            this.fleeBehaviour = fleeBehaviour;
        }

        public override bool Test()
        {
            return fleeBehaviour.timer <= 0;
        }
    }
}
