using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base condition for testing if the state needs to change
public class Condition
{
    public virtual bool Test()
    {
        return false;
    }
}

//Pair condition-target to state which condition change to a different state
public class Transition
{
    public Transition(Condition condition, BaseState target)
    {
        this.condition = condition;
        this.target = target;
    }

    public Condition condition;
    public BaseState target;
}

/**BASE CLASS FOR A STATE
 * 
 * Awake:       Initialises a list of "transition condition -> target state"
 * OnEnable:    Override for state variable inisialisation.
 * OnDisable:   Override for out-of-state changes and variable cleanup.
 * Update:      Contains state behaviour.
 * LateUpdate:  Take advantage of late update to check change of state conditions after all gameobject updates have been calculated,
 *              disabling the script and enabling the new state one if transiction conditions are met. 
 *
 */

public class BaseState : MonoBehaviour
{
    public List<Transition> transitions;

    //Set the list of condition->transition for this state
    public virtual void Awake()
    {
        transitions = new List<Transition>();        
    }

    //Code for state initialisation here
    public virtual void OnEnable() { }

    //Code for state finalisation here
    public virtual void OnDisable() { }

    //Code for behaviour here
    public virtual void Update() { }


    //LateUpdate test all transitions to check if we need to change the state
    public virtual void LateUpdate()
    {
        foreach(Transition t in transitions)
        {
            if(t.condition.Test())
            {
                t.target.enabled = true;
                this.enabled = false;
                return;
            }
        }
    }
}