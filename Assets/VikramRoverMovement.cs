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
    public float shiftMultiplier = 10.0f; // Multiplier for the speed when Shift key is pressed
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
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
            rb.MovePosition(transform.position + moveDirection.normalized * currentSpeed * Time.deltaTime); // Move the rover in the moveDirection
        }
    }
}