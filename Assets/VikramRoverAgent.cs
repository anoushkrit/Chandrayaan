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
    public Light directionalLight;


    void Start()
    {
        rb = GameObject.Find("VikramRover").GetComponent<Rigidbody>();
        terrainLayer = LayerMask.GetMask("MoonTerrain"); // Set the terrainLayer to all terrain objects in the game
        rb.maxAngularVelocity = 0.1f; // Set the maximum angular velocity
        rb.angularDrag = 5f; // Set the angular drag
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // Set the center of mass
    }
    public Transform Target;
    public override void OnEpisodeBegin() // episode conditions on reset 
    {
       // If the Agent fell, zero its momentum
       SetReward(0.0f);

       if (this.transform.localPosition.y < 0)
        {
            this.rb.angularVelocity = Vector3.zero;
            this.rb.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3( Random.value*10.0f + 600.0f, 150.0f, Random.value*10.0f + 500.0f);

        }

        Target.localPosition = new Vector3( Random.value*1000.0f - 100.0f, 200.0f, Random.value*1000.0f - 100.0f);
                                        //     Hopefully the Target is not in a crater and should land on the surface


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
    public float forceMultiplier = 2.0f;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        Ray ray;

        raycastDistance = 10.0f;
        Vector3 rayForward = transform.TransformDirection(0,0,transform.position.z);
        Vector3 rayLeft = transform.TransformDirection(-transform.position.x,0,0);
        Vector3 rayRight = transform.TransformDirection(transform.position.x,0,0);
        Vector3 rayBack = transform.TransformDirection(0,0,-transform.position.z);

        Vector3 rayDownFront = transform.TransformDirection(0, -transform.position.y, transform.position.z);
        Vector3 rayDownBack = transform.TransformDirection(0, -transform.position.y, -transform.position.z);
        Vector3 rayDownRight = transform.TransformDirection(transform.position.x, -transform.position.y, 0);
        Vector3 rayDownLeft = transform.TransformDirection(-transform.position.x, -transform.position.y,0);



        RaycastHit hit_right, hit_left, hit_back, hit_front, hit_down_front, hit_down_back, hit_down_right, hit_down_left, hit;

        // if (Physics.Raycast(ray, out hit, )) 
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayForward) * raycastDistance, out hit_front, 10.0f))
        {
            Debug.DrawRay(transform.position, rayForward, Color.green);
            Debug.Log(hit_front.collider.gameObject.name + "was hit!");
        }
        else
        {
            Debug.DrawRay(transform.position,  rayForward, Color.red);
            // Debug.Log("Infinity Fall");
        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayLeft) * raycastDistance, out hit_left, 10.0f))
        {
            Debug.DrawRay(transform.position, rayLeft, Color.green);
            Debug.Log(hit_left.collider.gameObject.name + "was hit!");
        }
        else
        {
            Debug.DrawRay(transform.position,  rayLeft, Color.red);
            // Debug.Log("Infinity Fall");
        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayRight) * raycastDistance, out hit_right, 10.0f))
        {
            Debug.DrawRay(transform.position, rayRight, Color.green);
            Debug.Log(hit_right.collider.gameObject.name + "was hit!");
        }
        else
        {
            Debug.DrawRay(transform.position,  rayRight, Color.red);
            // Debug.Log("Infinity Fall");
        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayBack) * raycastDistance, out hit_back, 10.0f))
        {
            Debug.DrawRay(transform.position, rayBack, Color.green);
            Debug.Log(hit_back.collider.gameObject.name + "was hit!");
        }
        else
        {
            Debug.DrawRay(transform.position,  rayBack, Color.red);
            // Debug.Log("Infinity Fall");
        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayDownFront) * raycastDistance, out hit_down_front, 10.0f))
        {
            Debug.DrawRay(transform.position, rayDownFront, Color.green);
            Debug.Log(hit_down_front.collider.gameObject.name + "was hit!");
            if (hit_down_front.distance > 5.0f)
            {
                AddReward(-0.4f);
            }
        }
        else
        {
            Debug.DrawRay(transform.position,  rayDownFront, Color.red);
            // Debug.Log("Infinity Fall");
        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayDownBack) * raycastDistance, out hit_down_back, 10.0f))
        {
            Debug.DrawRay(transform.position, rayDownBack, Color.green);
            Debug.Log(hit_down_back.collider.gameObject.name + "was hit!");
            if (hit_down_back.distance > 5.0f)
            {
                AddReward(-0.4f);
            }
        }
        else
        {
            Debug.DrawRay(transform.position,  rayDownBack, Color.red);
            // Debug.Log("Infinity Fall");
        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayDownRight) * raycastDistance, out hit_down_right, 10.0f))
        {
            Debug.DrawRay(transform.position, rayDownRight, Color.green);
            Debug.Log(hit_down_right.collider.gameObject.name + "was hit!");
            if (hit_down_right.distance > 5.0f)
            {
                AddReward(-0.4f);
            }
        }
        else
        {
            Debug.DrawRay(transform.position,  rayDownRight, Color.red);
            // Debug.Log("Infinity Fall");
        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayDownLeft) * raycastDistance, out hit_down_left, 10.0f))
        {
            Debug.DrawRay(transform.position, rayDownLeft, Color.green);
            Debug.Log(hit_down_left.collider.gameObject.name + "was hit!");

            if (hit_down_left.distance > 5.0f)
            {
                AddReward(-0.4f);
            }
        }
        else
        {
            Debug.DrawRay(transform.position,  rayDownLeft, Color.red);
            // Debug.Log("Infinity Fall");
        }
        
        if (directionalLight.intensity < 0.1f) // Change the threshold as per your requirement
        {
            // If the light has dimmed, set the reward to -1
            AddReward(-1.0f);
            if (rb.velocity != Vector3.zero)
            {
                AddReward(-5.0f);
            }
        }

        


        rb.transform.position += controlSignal * Time.deltaTime * forceMultiplier;

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (this.transform.localPosition.y < 5)
        {
            AddReward(-10.0f);
            EndEpisode();
        }
        if (distanceToTarget < 10.0f)
        {
            AddReward(50.0f);
            EndEpisode();
        }
        if (distanceToTarget < 25.0f)
        {
            AddReward(5.0f);
        }

        if (distanceToTarget < 50.0f)
        {
            AddReward(1.0f);
        }


        if (distanceToTarget >400.0f)
        {
            AddReward(-10.0f);
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }


}