using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {

    CharacterController cc;
    private Animator animator;
    private Vector3 direction;
    private float speedFactor;

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
        MouseLook();
    }

    void LateUpdate()
    {
        //camera updates
    }

    void ReadInputs()
    {
        speedFactor = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    void StandardMovementAndAnimator()
    {
        animator.SetFloat("VerticalF", speedFactor * direction.z);
        animator.SetFloat("HorizontalF", speedFactor * direction.x);
        cc.SimpleMove(transform.rotation*direction*speedFactor);
    }

    void MouseLook()
    {

    }
}
