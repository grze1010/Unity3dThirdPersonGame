using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {

    private Animator animator;


    void Start () {
        animator = this.GetComponent<Animator>();
	}
	
	void Update () {

        float speedFactor = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        float verticalMovFactor = Input.GetAxis("Vertical");
        float horizontalMovFactor = Input.GetAxis("Horizontal");

        animator.SetFloat("VerticalF", speedFactor * verticalMovFactor);
        animator.SetFloat("HorizontalF", speedFactor * horizontalMovFactor);
    }
}
