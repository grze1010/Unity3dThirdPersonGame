using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour {

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    Vector2 realAnimPos = Vector2.zero;
    Animator animator;

    float smoothingVar = 0.3f;

	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if(photonView.isMine) //is mine character
        {
            //do nothing
        }
        else
        {
            //position
            transform.position = Vector3.Lerp(transform.position, realPosition, smoothingVar);
            //rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, smoothingVar);
            //animations
            Vector2 realAnimPosAftLerp = Vector2.Lerp(new Vector2(animator.GetFloat("VerticalF"), animator.GetFloat("HorizontalF")), realAnimPos, smoothingVar);
            animator.SetFloat("VerticalF", realAnimPosAftLerp.x);
            animator.SetFloat("HorizontalF", realAnimPosAftLerp.y);
        }
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            //send out pos to network
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("VerticalF"));
            stream.SendNext(animator.GetFloat("HorizontalF"));
            stream.SendNext(animator.GetBool("Jump"));
        }
        else
        {
            //recieve position of other player
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            realAnimPos.x = (float)stream.ReceiveNext();
            realAnimPos.y = (float)stream.ReceiveNext();
            animator.SetBool("Jump", (bool)stream.ReceiveNext());
        }
    }
}
