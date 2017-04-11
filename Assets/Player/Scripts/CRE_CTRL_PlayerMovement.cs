using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRE_CTRL_PlayerMovement : MonoBehaviour {

    private CharacterController cc;
    private Animator animator;

    private bool isCursorLocked = true;
    private float vertVelocity = 0f;
    private bool inJump = false;
    private float cameraOffset = 4f;

    //from player
    private Vector3 direction;
    private float speedFactor;
    private bool startJump = false;
    private Vector2 mouseDelta;
    private float minY = -20;
    private float maxY = 80;

    //public vars
    public float jumpSpeed = 4f;
    public float movementSpeed = 1.5f;
    public float sprintSpeed = 2.5f;
    public Vector2 mouseSpeedDelta = new Vector2(10f, 5f);
    public GameObject playerCam;
    public GameObject cameraTarget;

    void Start () {
        animator = this.GetComponent<Animator>();
        cc = this.GetComponent<CharacterController>();
    }
	
	void Update () {
        ReadInputs();
        InternalLockUpdate();
    }

    void FixedUpdate()
    {
        MouseLook();
        StandardMovementAndAnimator();
    }

    void LateUpdate()
    {
        
    }

    void ReadInputs()
    {
        speedFactor = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        mouseDelta.x = Input.GetAxisRaw("Mouse X") * mouseSpeedDelta.x;
        mouseDelta.y = Input.GetAxisRaw("Mouse Y") * mouseSpeedDelta.y;

        if (cc.isGrounded && Input.GetButtonDown("Jump"))
        {
            startJump = true;
        }
    }

    void StandardMovementAndAnimator()
    {
        //standard movement
        animator.SetFloat("VerticalF", speedFactor * direction.z);
        animator.SetFloat("HorizontalF", speedFactor * direction.x);

        float speed = speedFactor == 2f ? sprintSpeed : movementSpeed;
        Vector3 movement = transform.rotation * direction * speed * Time.deltaTime; //standard movement

        //jump
        if (startJump) //start jump
        {
            StartJump();
        }
        else if (inJump && !cc.isGrounded) //in air
        {
            //
        }
        else if (inJump && cc.isGrounded) //startland
        {
            LandJump();
        }


        //falling/gravity
        if (!cc.isGrounded)
        {
            vertVelocity += Physics.gravity.y * Time.deltaTime;
        }
        movement.y = vertVelocity * Time.deltaTime;
        
        //apply movement
        cc.Move(movement);
    }

    void StartJump()
    {
        Debug.Log("StartJump");
        //start jump animation
        animator.SetBool("Jump", true);
        //start upwords movement
        vertVelocity = speedFactor == 2f ? (jumpSpeed * (sprintSpeed/movementSpeed))*2/3 : jumpSpeed; //jump higher if running
        //setup new collider shape
        cc.height -= 0.2f;
        cc.radius += 0.4f;
        //set vars
        inJump = true;
        startJump = false;
    }

    void LandJump()
    {
        Debug.Log("LandJump");
        //start land animation
        animator.SetBool("Jump", false);
        //reset collider
        cc.height += 0.2f;
        cc.radius -= 0.4f;
        //set vars
        inJump = false;
    }

    void MouseLook() //mouse look y - rotate cam
    {
        //mouse look x - rotate player (cam goes with him)
        if (mouseDelta.x != 0)
        {
            Quaternion xQuaternion = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
            transform.localRotation = transform.localRotation * xQuaternion;
        }

        //mouse look y - move camera, then look at player
        if (mouseDelta.y != 0)
        {
            playerCam.transform.localPosition = CalculateNewCameraPosition();
            playerCam.transform.LookAt(cameraTarget.transform);
        }
        //scroll - funkcja dla zblizania kamery? przejscie do 1st person
        //rays
    }

    private Vector3 CalculateNewCameraPosition()
    {
        //nope
        return playerCam.transform.localPosition;
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isCursorLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isCursorLocked = true;
        }

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
