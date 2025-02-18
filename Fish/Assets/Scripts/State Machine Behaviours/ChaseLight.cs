﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** CHASE LIGHT BEHAVIOUR
 * 
 * Fishes will be checking the small light ball in the fish tank in turns. Only one fish will check the ball at a time.
 * A fish within radius will get assigned to check the ball, will get close to it and stay close for a while, 
 * when the timer gets to 0, the fish will go back to normal flock behaviour releasing the slot for another fish to check the ball after.
 * 
 */

public class ChaseLight : BaseState
{
    [SerializeField] float withinTargetAreaDistance = 3.5f;
    [SerializeField] float slowDownDistance = 5.5f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float rotationSpeed = 1f;

    [SerializeField] float timeCheckingLight = 4f;
    float timer = 4f; public float Timer { get => timeCheckingLight; }

    LightBall lightBall;
    Fish thisFish;
    Vector3 direction = Vector3.zero;

    
    public override void Awake()
    {
        lightBall = FindObjectOfType<LightBall>();
        thisFish = gameObject.GetComponent<Fish>();

        //Define the transitions from this state
        transitions = new List<Transition>();

        //When the timer gets to 0, back to flock behaviour        
        transitions.Add(new Transition(new TimerReachedZero(this), gameObject.GetComponent<FlockBehaviour>()));
        
        this.enabled = false;
    }
    
    //State initialisation
    public override void OnEnable()
    {
        //Initialise Timer
        timer = timeCheckingLight;
        if (thisFish.myManager.FlockDebug) thisFish.AddDebugColor(Color.cyan);
    }

    // Update is called once per frame.
    //The behaviour will make the fish to swim towards the ball and slow down its velocity once it reaches 
    //the slowDown distance, to stop near the ball.
    void Update()
    {
        //Turn the fish towards the light ball
        direction = lightBall.gameObject.transform.position - transform.position;
        if (Vector3.Angle(transform.forward, direction) > 1)
        {
            transform.rotation =
                Quaternion.Slerp(transform.rotation,
                                 Quaternion.LookRotation(direction),
                                 Time.deltaTime * rotationSpeed);
        }


        float squaredDistanceToBall = (lightBall.gameObject.transform.position - transform.position).sqrMagnitude;
        
        //If not reached the slow down distance, swim to the target
        if (squaredDistanceToBall > slowDownDistance * slowDownDistance)
        {            
            transform.position += transform.forward * chaseSpeed * Time.deltaTime;
        }
        //If within slow down distance but not reached target, slow down speed
        else if (squaredDistanceToBall > withinTargetAreaDistance * withinTargetAreaDistance)
        {
            
            float updatedSpeed = chaseSpeed * (lightBall.gameObject.transform.position - transform.position).magnitude / slowDownDistance;
            transform.position += transform.forward * updatedSpeed * Time.deltaTime;
        }
        //If target reached, update timer
        else
        {
            timeCheckingLight -= Time.deltaTime;
        }
    }

    public override void OnDisable()
    {
        //Before leaving state, leave the lightball ready for another fish to check it out
        lightBall.fishAssigned = false;
    }

    //Derive a condition to return true when the timer reach zero
    private class TimerReachedZero : Condition
    {
        ChaseLight chaseLight;

        public TimerReachedZero(ChaseLight chaseLight)
        {
            this.chaseLight = chaseLight;
        }

        public override bool Test()
        {
            return chaseLight.Timer <= 0;
        }
    }
}