using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverMovement : MonoBehaviour
{
    // Speed of the rover
    public float speed = 10.0f;

    // Rigidbody component of the rover
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // Find the object by its name
        GameObject roverObject = GameObject.Find("Rover");

        // Get the RoverMovement component of the rover object
        RoverMovement roverMovement = roverObject.GetComponent<RoverMovement>();

        // Get the Rigidbody component of the rover
        rb = GetComponent<Rigidbody>();

        // Raycast to detect the ground below the rover
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            // Set the initial position of the rover to be in line with the ground below
            transform.position = new Vector3(133.85f, 7.69f, 150.34f);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if the Rigidbody component is not null
        // Move the rover forward when the up arrow key is pressed
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(transform.forward * speed);
        }

        // Move the rover backward when the down arrow key is pressed
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(-transform.forward * speed);
        }

        // Rotate the rover left when the left arrow key is pressed
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down * speed * Time.deltaTime);
        }

        // Rotate the rover right when the right arrow key is pressed
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
        }
    }

    // Set the drag and angularDrag properties of the Rigidbody component to reduce the force taking the gravity into consideration
    // public void OnEnable()
    // {
    //     if (rb != null)
    //     {
    //         rb.drag = 1.5f;
    //         rb.angularDrag = 1.5f;
    //     }
    // }
}
