using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRE_MNG_Player : MonoBehaviour{

    public Animator animator;

    private bool isCursorLocked = true;

    public CRE_MNG_Player()
    {

    }

    public CRE_MNG_Player(Animator animator)
    {
        this.animator = animator;
    }


    // Use this for initialization
    void Start () {
		
	}

    void Update()
    {
        InternalLockUpdate();
    }

    public void Animator_SetVar(string varName, System.Object val)
    {
        if (val is float)
        {
            animator.SetFloat(varName, (float)val);
        }
        else if (val is bool)
        {
            animator.SetBool(varName, (bool)val);
        }
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

    public void Skill_StartFromEmiterTowardTarget(string skillName, GameObject emiter, GameObject target)
    {
        Debug.Log("StartSkillShot: " + skillName);
        //get target location
        //Transform skillTarget;
        //Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        //RaycastHit hitInfo = FindClosestHitInfo(ray);
        //if (hitInfo.transform != null) //hit
        {
            //Debug.Log("towards: " + hitInfo.collider.name);
            //skillTarget = hitInfo.transform;
        }
        //else
        {
            //Debug.Log("towards cameraTarget");
            //skillTarget = cameraTarget.transform;
        }

        //create skillObject on playerlocation with rotation set towards target
        //skillPosition.y = 1.4f;
        GameObject createdSkill = (GameObject)PhotonNetwork.Instantiate(skillName, emiter.transform.position, emiter.transform.rotation, 0);
        CRE_Skill script = (CRE_Skill)createdSkill.GetComponent("CRE_Skill");
        CRE.CRE_Skill_Settings skillSettings;
        CRE.SKILL_SETTINGS.TryGetValue(skillName, out skillSettings);
        script.skillSettings = skillSettings;
        script.init(target);
    }

    public void Rotate(Object obj, float horizontal, float vertical, bool local)
    {
        Quaternion xQuaternion = Quaternion.AngleAxis(horizontal, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(vertical, Vector3.left);
        if (local)
        {
            (obj is GameObject ? ((GameObject)obj).transform : (Transform)obj)
                .localRotation *= xQuaternion * yQuaternion;
        }
        else
        {
            (obj is GameObject ? ((GameObject)obj).transform : (Transform)obj)
                .rotation *= xQuaternion * yQuaternion;
        }
    }

    public void StartAnimation(string animationName, out bool inAnimation, out bool startAnimation)
    {
        Animator_SetVar(animationName, true);
        inAnimation = true;
        startAnimation = false;
    }

    public void EndAnimation(string animationName, out bool inAnimation)
    {
        Animator_SetVar(animationName, false);
        inAnimation = false;
    }
}
