using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class VikramRoverAgent : Agent
{
    public Rigidbody rb;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 2.0f;
    public float turnSmoothVelocity;
    public float shiftMultiplier = 10.0f; // Multiplier for the speed when Shssift key is pressed
    public float raycastDistance = 1.0f; // Distance to cast the ray to detect terrain
    public LayerMask terrainLayer; // Layer mask for the terrain
    public LayerMask WhatIsGround;
    public AnimationCurve animCurve;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        terrainLayer = LayerMask.GetMask("Terrain"); // Set the terrainLayer to all terrain objects in the game
        WhatIsGround = LayerMask.GetMask("Terrain"); // Set the terrainLayer to all terrain objects in the game
        rb.maxAngularVelocity = 0.1f; // Set the maximum angular velocity
        rb.angularDrag = 5f; // Set the angular drag
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // Set the center of mass

    }
    public Transform Target;
    public override void OnEpisodeBegin() // episode conditions on reset 
    {
       // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            this.rb.angularVelocity = Vector3.zero;
            this.rb.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3( 10.0f, 20.0f, 10.0f);
        }

        // Move the target to a new spot
        // Target.localPosition = new Vector3(Random.value * 10 - 4,
        //                                    120.0f,
        //                                    Random.value * 10 - 4);
        Target.localPosition = new Vector3(39.0f,
                                           52.0f,
                                           94.0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        // rb.AddForce(controlSignal * forceMultiplier);
        rb.transform.position += controlSignal * Time.deltaTime * forceMultiplier;

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(10.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (this.transform.localPosition.y < 5)
        {
            SetReward(-100.0f);
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }


}