using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VikramRoverMovement : MonoBehaviour
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


    void Update()
    {
        SurfaceAlignment(); // Call the SurfaceAlignment method to align the rover with the surface
        float horizontal = Input.GetAxis("Horizontal"); // A and D keys
        float vertical = Input.GetAxis("Vertical"); // W and S keys
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); // Shift key
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; // Angle between the x and z axis
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); // Smoothly rotate the rover
            transform.rotation = Quaternion.Euler(0f, angle, 0f); // Rotate the rover to the target angle

            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward; // Move the rover in the direction of the target angle
            float currentSpeed = speed;
            if (shiftPressed) // If Shift key is pressed, increase the speed
            {
                currentSpeed *= shiftMultiplier;
            }

            if (Input.GetKey(KeyCode.Q)) // If Q key is pressed, increase the speed
            {
                currentSpeed =0;
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, moveDirection, out hit, raycastDistance, terrainLayer)) // Cast a ray to detect terrain
            {
                Vector3 surfaceNormal = hit.normal; // Get the normal of the surface
                moveDirection = Vector3.ProjectOnPlane(moveDirection, surfaceNormal).normalized; // Project the move direction onto the surface
                transform.position = hit.point + surfaceNormal * 0.1f; // Adjust the position of the rover to be slightly above the surface
            }

            rb.AddForce(moveDirection.normalized * currentSpeed * Time.deltaTime, ForceMode.VelocityChange); // Move the rover in the moveDirection
        }
    }

    private void SurfaceAlignment()
    {
        Ray ray = new Ray(transform.position, -transform.up); // Create a ray pointing downwards from the rover (transform.position
        RaycastHit info = new RaycastHit();
        Quaternion RotationRef = Quaternion.Euler(0,0,0);
        if (Physics.Raycast(ray, out info , WhatIsGround)) // Cast a ray to detect terrain
        {
            RotationRef = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, info.normal), animCurve.Evaluate(Time.time)); // Get the normal of the surface
            transform.rotation = Quaternion.Euler(RotationRef.eulerAngles.x, transform.eulerAngles.y, RotationRef.eulerAngles.z); // Rotate the rover to the target angle  
        }
    }   
}