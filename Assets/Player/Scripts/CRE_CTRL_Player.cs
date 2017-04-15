using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRE_CTRL_Player : MonoBehaviour {

    private CharacterController cc;
    private CRE_MNG_Player playerManager;

    //variables for scripts
    private float zVelocity = 0f;
    //private float verticalVelocity = 0f;
    private float horizontalVelocity = 0f;
    private bool inJump = false;
    private bool inFlipDodge = false;
    private static float currentTargetYAngle;
    //private static float currentCameraYAngle;
    private Vector2 maxCurrentTime_DoubleClickFlipDodge = Vector2.zero;

    //from player
    private Vector3 direction;
    private float speedFactor;
    private bool startJump = false;
    private Vector2 mouseDelta;
    private float scrollInput;
    private bool startFlipDodge = false;
    private bool isFlipDodgeLeft = false;

    //objects
    public GameObject playerCam;
    public GameObject cameraTarget;
    public GameObject cameraAnchor;

    public GameObject fireballEmiter;

    //settings

    private float __cameraTarget_distance;
    private Vector3 __camera_offset_to_anchor;
    private Vector3 __cameraTarget_offset_to_player;


    void Start () {
        playerManager = this.GetComponent<CRE_MNG_Player>();
        cc = this.GetComponent<CharacterController>();

        playerCam.transform.LookAt(cameraTarget.transform);
        __cameraTarget_offset_to_player = cameraTarget.transform.localPosition;
        __camera_offset_to_anchor = playerCam.transform.localPosition;
        __cameraTarget_distance = CRE.CalculateHypotenuse(cameraTarget.transform.localPosition.y, cameraTarget.transform.localPosition.z);
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

        mouseDelta.x = Input.GetAxisRaw("Mouse X") * CRE.MOUSE_SPEED_DELTA.x;
        mouseDelta.y = Input.GetAxisRaw("Mouse Y") * CRE.MOUSE_SPEED_DELTA.y;

        scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");

        if (cc.isGrounded && Input.GetButtonDown("Jump"))
        {
            startJump = true;
        }

        if (cc.isGrounded && Input.GetKeyDown(KeyCode.A))
        {
            if (!inFlipDodge && maxCurrentTime_DoubleClickFlipDodge.x >= Time.time)
            {
                isFlipDodgeLeft = true;
                startFlipDodge = true;
            }
            maxCurrentTime_DoubleClickFlipDodge.x = Time.time + CRE.MAX_TIME_BETWEEN_DOUBLE_CLICKS;
        }
        if (cc.isGrounded && Input.GetKeyDown(KeyCode.D))
        {
            if (!inFlipDodge && maxCurrentTime_DoubleClickFlipDodge.y >= Time.time)
            {
                isFlipDodgeLeft = false;
                startFlipDodge = true;
            }
            maxCurrentTime_DoubleClickFlipDodge.y = Time.time + CRE.MAX_TIME_BETWEEN_DOUBLE_CLICKS;
        }

        //
        if (Input.GetKeyDown(KeyCode.Mouse0) )//todo && isCursorLocked)
        {
            //
        }
    }

    void StandardMovementAndAnimator()
    {
        Vector3 movement = Vector3.zero;
        //standard movement
        playerManager.Animator_SetVar("VerticalF", speedFactor * direction.z);
        playerManager.Animator_SetVar("HorizontalF", speedFactor * direction.x);

        float speed = speedFactor == 2f ? CRE.SPRINT_SPEED : CRE.MOVEMENT_SPRINT;
        if (!inFlipDodge) {
            movement = transform.rotation * direction * speed * Time.deltaTime; //standard movement
        }

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

        //FlipDodge
        if (startFlipDodge)
        {
            StartFlipDodge();
        }
        else if (inFlipDodge && cc.isGrounded)
        {
            EndFlipDodge();
        }


        //falling/gravity
        if (!cc.isGrounded)
        {
            zVelocity += Physics.gravity.y * Time.deltaTime;
        }
        movement.y = zVelocity * Time.deltaTime;

        if (inFlipDodge)
        {
            movement.x = horizontalVelocity * Time.deltaTime;
            movement = transform.rotation * movement;
        }
        

        //apply movement
        cc.Move(movement);
    }

    void StartJump()
    {
        zVelocity = speedFactor == 2f ? (CRE.JUMP_POWER * (CRE.SPRINT_SPEED / CRE.MOVEMENT_SPRINT)) * 2/3 : CRE.JUMP_POWER; //jump higher if running

        playerManager.StartAnimation("Jump", out inJump, out startJump);
    }

    void LandJump()
    {
        playerManager.EndAnimation("Jump", out inJump);
    }

    void StartFlipDodge()
    {
        if (isFlipDodgeLeft) {
            playerManager.Animator_SetVar("IsFlipDodgeLeft", true);
        }
        else
        {
            playerManager.Animator_SetVar("IsFlipDodgeLeft", false);
        }

        horizontalVelocity = CRE.FLIP_JUMP_HORIZONTAL_POWER * (isFlipDodgeLeft ? -1 : 1);
        zVelocity = CRE.FLIP_JUMP_POWER;

        playerManager.StartAnimation("FlipDodge", out inFlipDodge, out startFlipDodge);
    }

    void EndFlipDodge()
    {
        playerManager.EndAnimation("FlipDodge", out inFlipDodge);
        horizontalVelocity = 0f;
    }

    void MouseLook() //mouse look y - rotate cam
    {
        //mouse look x - rotate player (cam goes with him)
        if (mouseDelta.x != 0)
        {
            playerManager.Rotate(transform, mouseDelta.x, 0f, true);
        }

        //mouse look y - move camera, then look at cam target
        if (mouseDelta.y != 0)
        {
            //move camera target with mouse(keep always in same distance from anchor)
            MoveCameraTargetBasedOnMouseInput();
            //move camera to be "in line" with cameraTarget and cameraAnchor
            MoveCameraToLineWithCameraTargetAndCameraAnchor();
            playerCam.transform.LookAt(cameraTarget.transform);
        }

        //camera offset - change distance between camera and player
        //

        //rays
        //
    }

    private void MoveCameraTargetBasedOnMouseInput()
    {
        Vector3 currentPosition = cameraTarget.transform.localPosition;
        Vector3 newPosition = currentPosition;

        //move up/down based on mouse input
        newPosition.y += mouseDelta.y * __cameraTarget_distance/100; // cre_todo: "/ 6" change to be static or sth
        //update z to keep same distance from player
        newPosition.z = CRE.CalculateCathetus(newPosition.y - __cameraTarget_offset_to_player.y, __cameraTarget_distance);

        //apply movement to cameraTarget if camera angle < maxCameraYAngle
        float newTargetYAngle = CRE.CalculateAngle(newPosition.z, newPosition.y - __cameraTarget_offset_to_player.y);
        if (newTargetYAngle < CRE.MAX_TARGET_Y_ANGLE) {
            cameraTarget.transform.localPosition = newPosition;
            currentTargetYAngle = newTargetYAngle;
        }
    }
    

    private void MoveCameraToLineWithCameraTargetAndCameraAnchor()
    {
        cameraAnchor.transform.LookAt(cameraTarget.transform); //changes position of camera
        UpdateCameraLocalYPosition(); //updates the angle between camera and cameraAnchor
        UpdateCameraLocalZPosition(); //update the distance between camera and anchor
    }

    private void UpdateCameraLocalYPosition()
    {
        Vector3 newPosition = playerCam.transform.localPosition;
        newPosition.y = (-1 / (CRE.MAX_TARGET_Y_ANGLE / __camera_offset_to_anchor.y)) * currentTargetYAngle + __camera_offset_to_anchor.y;
        playerCam.transform.localPosition = newPosition;
    }

    private void UpdateCameraLocalZPosition()
    {
        Vector3 newPosition = playerCam.transform.localPosition;
        if (currentTargetYAngle > CRE.MAX_TARGET_Y_ANGLE_WITHOUT_Z_FIX && cameraTarget.transform.localPosition.y > __cameraTarget_offset_to_player.y)
        {
            float a = (1 - __camera_offset_to_anchor.z) / (CRE.MAX_TARGET_Y_ANGLE - CRE.MAX_TARGET_Y_ANGLE_WITHOUT_Z_FIX);
            float b = __camera_offset_to_anchor.z - CRE.MAX_TARGET_Y_ANGLE_WITHOUT_Z_FIX *a;
            newPosition.z = a * currentTargetYAngle + b;
        }
        else
        {
            newPosition.z = __camera_offset_to_anchor.z;
        }
        playerCam.transform.localPosition = newPosition;
    }

    

    
}
