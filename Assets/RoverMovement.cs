using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverMovement : MonoBehaviour
{
    // Speed of the rover
    public float speed = 10.0f;

    // Rigidbody component of the rover
    private Rigidbody rb;

    // Camera component of the scene
    private FollowCamera followCamera;

    // Rotation speed of the camera
    public float cameraRotationSpeed = 5.0f;

    // Minimum and maximum pitch angle of the camera
    public float minPitchAngle = -60.0f;
    public float maxPitchAngle = 60.0f;

    // Current pitch angle of the camera
    private float currentPitchAngle = 0.0f;

    // Zoom speed of the camera
    public float zoomSpeed = 5.0f;

    // Minimum and maximum distance of the camera from the rover
    public float minDistance = 5.0f;
    public float maxDistance = 20.0f;

    // Current distance of the camera from the rover
    private float currentDistance = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component of the rover
        rb = GetComponent<Rigidbody>();

        // Find the FollowCamera object in the scene
        followCamera = GameObject.Find("FollowCamera").GetComponent<FollowCamera>();


        // Set the followCamera as the active camera
        followCamera.gameObject.SetActive(true);
        followCamera.transform.position = new Vector3(173.85f, 77.5f, 133.85f);

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
        // Move the rover forward when the W key is pressed
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * speed);
        }

        // Move the rover backward when the S key is pressed
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * speed);
        }

        // Rotate the rover left when the A key is pressed
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.down * speed * Time.deltaTime);
        }

        // Rotate the rover right when the D key is pressed
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
        }

        // Move the camera to follow the rover's movement from a 3rd person view
        if (followCamera != null)
        {
            // Move the camera up when the Up arrow key is pressed
            if (Input.GetKey(KeyCode.UpArrow))
            {
                followCamera.transform.position += Vector3.up * speed * Time.deltaTime;
            }

            // Move the camera down when the Down arrow key is pressed
            if (Input.GetKey(KeyCode.DownArrow))
            {
                followCamera.transform.position += Vector3.down * speed * Time.deltaTime;
            }

            // Move the camera right when the Right arrow key is pressed
            if (Input.GetKey(KeyCode.RightArrow))
            {
                followCamera.transform.position += Vector3.right * speed * Time.deltaTime;
            }

            // Move the camera left when the Left arrow key is pressed
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                followCamera.transform.position += Vector3.left * speed * Time.deltaTime;
            }

            // Zoom in and out with the mouse wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            // Rotate the camera with the mouse
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            currentPitchAngle -= mouseY * cameraRotationSpeed;
            currentPitchAngle = Mathf.Clamp(currentPitchAngle, minPitchAngle, maxPitchAngle);

            followCamera.transform.RotateAround(transform.position, Vector3.up, mouseX * cameraRotationSpeed);
            followCamera.transform.localEulerAngles = new Vector3(currentPitchAngle, followCamera.transform.localEulerAngles.y, followCamera.transform.localEulerAngles.z);

            followCamera.transform.position = transform.position - followCamera.transform.forward * currentDistance;
            followCamera.transform.LookAt(transform.position);
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
