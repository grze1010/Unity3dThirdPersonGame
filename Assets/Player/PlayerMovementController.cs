using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {

    private CharacterController cc;
    private Animator animator;

    private float vertVelocity = 0f;
    private bool inJump = false;

    //from player
    private Vector3 direction;
    private float speedFactor;
    private bool startJump = false;
    private float mouseX;
    private float mouseY;
    private float minY;
    private float maxY;

    //public vars
    public float jumpSpeed = 4f;
    public float movementSpeed = 1.5f;
    public float sprintSpeed = 2.5f;
    public float mouseSpeedX = 10f;
    public float mouseSpeedY = 5f;
    public GameObject playerCam;

    void Start () {
        animator = this.GetComponent<Animator>();
        cc = this.GetComponent<CharacterController>();
    }
	
	void Update () {
        ReadInputs();
    }

    void FixedUpdate()
    {
        StandardMovementAndAnimator();
    }

    void LateUpdate()
    {
        MouseLook();
    }

    void ReadInputs()
    {
        speedFactor = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        mouseX = Input.GetAxis("Mouse X") * mouseSpeedX;
        mouseY = Input.GetAxis("Mouse Y") * mouseSpeedY;

        if (cc.isGrounded && Input.GetButtonDown("Jump"))
        {
            startJump = true;
        }
    }

    void StandardMovementAndAnimator()
    {
        //mouse look x - rotate player (cam goes with him)
        Quaternion xQuaternion = Quaternion.AngleAxis(mouseX, Vector3.up);
        transform.localRotation = transform.localRotation * xQuaternion;

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

    void MouseLook()
    {
        //mouse look y - rotate cam
        Quaternion yQuaternion = Quaternion.AngleAxis(mouseY, Vector3.left);
        playerCam.transform.localRotation = playerCam.transform.localRotation * yQuaternion;
    }
}
