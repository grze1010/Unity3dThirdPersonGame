using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRE_Skill : MonoBehaviour {

    public CRE.CRE_Skill_Settings skillSettings;

    public Object skillTarget;

    public void init(Object target)
    {
        skillTarget = target;
    }

    void Start () {
        transform.LookAt(skillTarget is GameObject ? ((GameObject)skillTarget).transform : (Transform)skillTarget);
        transform.GetComponent<Rigidbody>().AddForce(transform.forward * skillSettings.FORCE);
        Destroy(this.gameObject, skillSettings.TTL);
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
