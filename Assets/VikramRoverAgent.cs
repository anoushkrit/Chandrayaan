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
    public float raycastDistance = 1.0f; // Distance to cast the ray to detect s
    public LayerMask terrainLayer; // Layer mask for the terrain
    public LayerMask WhatIsGround;
    public AnimationCurve animCurve;
    public float distanceToTarget;
    // public Light directionalLight;
    public RaycastHit hit_right, hit_left, hit_back, hit_front, hit_down_front, hit_down_back, hit_down_right, hit_down_left;
    public float lastDistanceToTarget = 0.0f;
    public int checkpointTemp = 0;
    public int checkpointTemp2 = 0;
    public int episode_data;
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
        SetReward(0.0f);
        checkpointTemp = 0;
        checkpointTemp2 = 0;

        if (this.transform.localPosition.y < 0)
        {
            this.rb.angularVelocity = Vector3.zero;
            this.rb.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(Random.Range(450.0f, 500.0f), 150.0f, Random.Range(450.0f, 500.0f));
        }
        
        // sequentially increasing the environment space

        if(episode_data<=25)
        {
            Target.localPosition = this.transform.localPosition + new Vector3(Random.Range(30.0f, 50.0f), 0.0f, Random.Range(30.0f, 50.0f));

        }

        if(episode_data>25 && episode_data<=100)
        {
            Target.localPosition = this.transform.localPosition + new Vector3(Random.Range(50.0f, 100.0f), 0.0f, Random.Range(50.0f, 100.0f));

        }

        if(episode_data>100 && episode_data<=500)
        {
            Target.localPosition = this.transform.localPosition + new Vector3(Random.Range(100.0f, 250.0f), 0.0f, Random.Range(100.0f, 250.0f));

        }
        if(episode_data>500)
        {
            Target.localPosition = new Vector3(Random.Range(100.0f, 900.0f), 0.0f, Random.Range(100.0f, 900.0f));

        }
        
        float elapsedTime = 0.0f;
        float MAX_TIME = 60.0f;


        elapsedTime += Time.deltaTime;

        if (elapsedTime > MAX_TIME && distanceToTarget > 10.0f)
        {
            SetReward(-10.0f);
            EndEpisode();
        }
        // Hopefully the Target is not in a crater and should land on the surface
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
        // sensor.AddObservation(directionalLight.intensity);
        // sensor.AddObservation(hit_front.distance);
        // sensor.AddObservation(hit_left.distance);
        // sensor.AddObservation(hit_right.distance);
        // sensor.AddObservation(hit_back.distance);
        // sensor.AddObservation(hit_down_front.distance);
        // sensor.AddObservation(hit_down_back.distance);
        // sensor.AddObservation(hit_down_right.distance);
        // sensor.AddObservation(hit_down_left.distance);

    }
    
    public float forceMultiplier = 10.0f;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {


        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        float raycastDistance = 10.0f;
        Vector3 rayForward = transform.TransformDirection(0,0,transform.position.z);
        Vector3 rayLeft = transform.TransformDirection(-transform.position.x,0,0);
        Vector3 rayRight = transform.TransformDirection(transform.position.x,0,0);
        Vector3 rayBack = transform.TransformDirection(0,0,-transform.position.z);

        Vector3 rayDownFront = transform.TransformDirection(0, -transform.position.y, transform.position.z);
        Vector3 rayDownBack = transform.TransformDirection(0, -transform.position.y, -transform.position.z);
        Vector3 rayDownRight = transform.TransformDirection(transform.position.x, -transform.position.y, 0);
        Vector3 rayDownLeft = transform.TransformDirection(-transform.position.x, -transform.position.y,0);

        RaycastHit hit_right, hit_left, hit_back, hit_front, hit_down_front, hit_down_back, hit_down_right, hit_down_left;
        
        rb.transform.position += controlSignal * Time.deltaTime * forceMultiplier;
        distanceToTarget = Vector3.Distance(rb.transform.localPosition, Target.localPosition);

        // float displacement = distanceToTarget - lastDistanceToTarget;
        // lastDistanceToTarget = distanceToTarget;

        if (rb.transform.localPosition.y < 0.0f)
        {
            SetReward(-10.0f);
            EndEpisode();
        }
        if (Target.transform.localPosition.y < -10.0f)
        {
            EndEpisode();
        }

        if (distanceToTarget < 25.0f)
        {
            AddReward(100.0f);
            EndEpisode();
            AddReward(5.0f);

        }


        
        if (distanceToTarget < 50.0f)
        {
            if (checkpointTemp == 0)
            {
                AddReward(50.0f);
                checkpointTemp = 1;
            }
            if (checkpointTemp > 0)
            {
                AddReward(0.0f);
            }

        }
        
        if (distanceToTarget < 100.0f)
        {
            if (checkpointTemp2 == 0)
            {
                AddReward(10.0f);
                checkpointTemp2 = 1;
            }
            if (checkpointTemp2 >0)
            {
                AddReward(0.0f);
            }
        }

        if (distanceToTarget > 300.0f)
        {

            AddReward(-0.5f);
        }


        if (distanceToTarget >500.0f)
        {
            AddReward(-50.0f);
            EndEpisode();
        }

        if(Physics.Raycast(transform.position, transform.TransformDirection(rayForward) * raycastDistance, out hit_front,100.0f))
        {
            Debug.DrawRay(transform.position, rayForward, Color.red);

            // if (hit_front.distance > 10.0f)
            // {
            //     AddReward(0.4f);
            // }

        }
        else
        {
            Debug.DrawRay(transform.position,  rayForward, Color.green);

        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayLeft) * raycastDistance, out hit_left,100.0f))
        {
            Debug.DrawRay(transform.position, rayLeft, Color.red);
            // if (hit_left.distance > 10.0f)
            // {
            //     AddReward(0.4f);
            // }


        }
        else
        {
            Debug.DrawRay(transform.position,  rayLeft, Color.green);


        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayRight) * raycastDistance, out hit_right,100.0f))
        {
            Debug.DrawRay(transform.position, rayRight, Color.red);
            // if (hit_right.distance > 10.0f)
            // {
            //     AddReward(0.4f);
            // }

        }
        else
        {
            Debug.DrawRay(transform.position,  rayRight, Color.green);

        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayBack) * raycastDistance, out hit_back,100.0f))
        {
            Debug.DrawRay(transform.position, rayBack, Color.red);
            // if (hit_back.distance > 10.0f)
            // {
            //     AddReward(0.4f);
            // }


        }
        else
        {
            Debug.DrawRay(transform.position,  rayBack, Color.green);

        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayDownFront) * raycastDistance, out hit_down_front,100.0f))
        {
            Debug.DrawRay(transform.position, rayDownFront, Color.red);

            // if (hit_down_front.distance > 30.0f)
            // {
            //     AddReward(-0.04f);
            // }
        
        }
        
        else
        {
            Debug.DrawRay(transform.position,  rayDownFront, Color.green);

        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayDownBack) * raycastDistance, out hit_down_back,100.0f))
        {
            Debug.DrawRay(transform.position, rayDownBack, Color.red);
            // if (hit_down_back.distance > 30.0f)
            // {
            //     AddReward(-0.04f);
            // }
            
        }
        else
        {
            Debug.DrawRay(transform.position,  rayDownBack, Color.green);

        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayDownRight) * raycastDistance, out hit_down_right,100.0f))
        {
            Debug.DrawRay(transform.position, rayDownRight, Color.red);
            // if (hit_down_right.distance > 30.0f)
            // {
            //     AddReward(-0.04f);
            // }
            
        }
        else
        {
            Debug.DrawRay(transform.position,  rayDownRight, Color.green);

        }
        if(Physics.Raycast(transform.position, transform.TransformDirection(rayDownLeft) * raycastDistance, out hit_down_left,100.0f))
        {
            Debug.DrawRay(transform.position, rayDownLeft, Color.red);
            // if (hit_down_left.distance > 30.0f)
            // {
            //     AddReward(-0.04f);
            // }
        }
        
        else
        {
            Debug.DrawRay(transform.position,  rayDownLeft, Color.green);
        }
        
        // if (rb.velocity != Vector3.zero && directionalLight.intensity > 0.1f)
        // {
        //     AddReward(0.03f);
        // }
        // if (rb.velocity == Vector3.zero && directionalLight.intensity > 0.1f)
        // {
        //     AddReward(-0.002f);
        // }
        // // Change the threshold as per your requirement
        // if (rb.velocity == Vector3.zero && directionalLight.intensity < 0.1f)
        // {
        //     AddReward(0.02f);
        // }
        // if (rb.velocity != Vector3.zero && directionalLight.intensity < 0.1f)
        // {
        //     AddReward(-1.0f);
        // }
        


        // if (rb.transform.localPosition.y > 50.0f)
        // {
        //     AddReward(1.0f);
        // }


        
        // if (distanceToTarget < 25.0f)
        // {
        //     AddReward(50.0f);
        // }

        // if (displacement < 0.0f)
        // {
        //     AddReward(-0.001f);
        // }
        // if (displacement < 0.0f)
        // {
        //     Debug.log("Getting CLOSER");
        //     AddReward(0.02f);
        // }
        // if (displacement > 0.0f)
        // {
        //     AddReward(-0.01f);
        // }

        // if (distanceToTarget < 50.0f)
        // {
        //     AddReward(1.0f);
        // }



    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}