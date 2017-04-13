using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRE_CTRL_PlayerMovement : MonoBehaviour {

    private CharacterController cc;
    private Animator animator;

    //variables for scripts
    private bool isCursorLocked = true;
    private float vertVelocity = 0f;
    private bool inJump = false;
    private static float currentTargetYAngle;
    //private static float currentCameraYAngle;

    //from player
    private Vector3 direction;
    private float speedFactor;
    private bool startJump = false;
    private Vector2 mouseDelta;
    private float scrollInput;

    //objects
    public GameObject playerCam;
    public GameObject cameraTarget;
    public GameObject cameraAnchor;

    //settings
    private static float JUMP_SPEED = 6f;
    private static float MOVEMENT_SPRINT = 1.5f;
    private static float SPRINT_SPEED = 2.5f;
    private static Vector2 MOUSE_SPEED_DELTA = new Vector2(10f, 10f);
    private float CAMERA_TARGET_DISTANCE;
    private static float MAX_TARGET_Y_ANGLE_WITHOUT_Z_FIX = 20;
    private static float MAX_TARGET_Y_ANGLE = 70;
    private Vector3 CAMERA_OFFSET_TO_ANCHOR;
    private Vector3 CAMERA_TARGET_OFFSET_TO_PLAYER;

    void Start () {
        animator = this.GetComponent<Animator>();
        cc = this.GetComponent<CharacterController>();

        playerCam.transform.LookAt(cameraTarget.transform);
        CAMERA_TARGET_OFFSET_TO_PLAYER = cameraTarget.transform.localPosition;
        CAMERA_OFFSET_TO_ANCHOR = playerCam.transform.localPosition;
        CAMERA_TARGET_DISTANCE = CalculateHypotenuse(cameraTarget.transform.localPosition.y, cameraTarget.transform.localPosition.z);
    }
	
	void Update () {
        ReadInputs();
        InternalLockUpdate();
    }

    void FixedUpdate()
    {
        StandardMovementAndAnimator();
    }

    void LateUpdate()
    {
        MouseLook();
        //
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hitInfo = FindClosestHitInfo(ray);
        if (hitInfo.transform != null) //hit
        {
            Debug.Log("towards: " + hitInfo.collider.name);
        }
    }

    void ReadInputs()
    {
        speedFactor = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        mouseDelta.x = Input.GetAxisRaw("Mouse X") * MOUSE_SPEED_DELTA.x;
        mouseDelta.y = Input.GetAxisRaw("Mouse Y") * MOUSE_SPEED_DELTA.y;

        scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");

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

        float speed = speedFactor == 2f ? SPRINT_SPEED : MOVEMENT_SPRINT;
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
        vertVelocity = speedFactor == 2f ? (JUMP_SPEED * (SPRINT_SPEED/MOVEMENT_SPRINT)) * 2/3 : JUMP_SPEED; //jump higher if running
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
        newPosition.y += mouseDelta.y * CAMERA_TARGET_DISTANCE/100; // cre_todo: "/ 6" change to be static or sth
        //update z to keep same distance from player
        newPosition.z = CalculateCathetus(newPosition.y - CAMERA_TARGET_OFFSET_TO_PLAYER.y, CAMERA_TARGET_DISTANCE);

        //apply movement to cameraTarget if camera angle < maxCameraYAngle
        float newTargetYAngle = CalculateAngle(newPosition.z, newPosition.y - CAMERA_TARGET_OFFSET_TO_PLAYER.y);
        if (newTargetYAngle < MAX_TARGET_Y_ANGLE) {
            cameraTarget.transform.localPosition = newPosition;
            currentTargetYAngle = newTargetYAngle;
        }
    }
    private float CalculateCathetus(float catheusA, float hypotenuseC) //float a, c from c2 = a2 + b2, return b
    {
        return Mathf.Sqrt(Mathf.Pow(hypotenuseC, 2) - Mathf.Pow(catheusA, 2)); ;
    }
    private float CalculateHypotenuse(float catheusA, float catheusB) //float a, b from c2 = a2 + b2, return c
    {
        return Mathf.Sqrt(Mathf.Pow(catheusA, 2) + Mathf.Pow(catheusB, 2)); ;
    }
    private float CalculateAngle(float catheusA, float catheusB) // a, b from tanX = a/b, return X
    {
        return 90f - RadianToDegree(Mathf.Atan2(catheusA, Mathf.Abs(catheusB)));
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
        newPosition.y = (-1 / (MAX_TARGET_Y_ANGLE / CAMERA_OFFSET_TO_ANCHOR.y)) * currentTargetYAngle + CAMERA_OFFSET_TO_ANCHOR.y;
        playerCam.transform.localPosition = newPosition;
    }

    private void UpdateCameraLocalZPosition()
    {
        Vector3 newPosition = playerCam.transform.localPosition;
        if (currentTargetYAngle > MAX_TARGET_Y_ANGLE_WITHOUT_Z_FIX && cameraTarget.transform.localPosition.y > CAMERA_TARGET_OFFSET_TO_PLAYER.y)
        {
            float a = (1 - CAMERA_OFFSET_TO_ANCHOR.z) / (MAX_TARGET_Y_ANGLE - MAX_TARGET_Y_ANGLE_WITHOUT_Z_FIX);
            float b = CAMERA_OFFSET_TO_ANCHOR.z - MAX_TARGET_Y_ANGLE_WITHOUT_Z_FIX*a;
            newPosition.z = a * currentTargetYAngle + b;
        }
        else
        {
            newPosition.z = CAMERA_OFFSET_TO_ANCHOR.z;
        }
        playerCam.transform.localPosition = newPosition;
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

    private float RadianToDegree(float angle)
    {
        return angle * (180f / Mathf.PI);
    }

    private float DegreeToRadian(float angle)
    {
        return Mathf.PI * angle / 180f;
    }

    private float DistanceFromGround(GameObject obj) //cre_todo change to check with  rays
    {
        return obj.transform.position.y;
    }

    RaycastHit FindClosestHitInfo(Ray ray)
    {
        RaycastHit closestHit = new RaycastHit();
        float distance = 0f;
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform != this.transform && (closestHit.transform == null || hit.distance < distance))
            {
                closestHit = hit;
                distance = hit.distance;
            }
        }

        return closestHit;
    }
}
