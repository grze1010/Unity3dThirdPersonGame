using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    public GameObject shootFrom { get; set; }
    private float cooldown = 0.5f;
    private float cooldownRemaining = 0f;
    private float maxDistance = 10f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        cooldown -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
	}

    void Fire()
    {
        if(cooldownRemaining > 0)
        {
            return;
        }

        Ray ray = new Ray(shootFrom.transform.position, shootFrom.transform.forward);

        RaycastHit hitInfo = FindClosestHitInfo(ray);
        if (hitInfo.transform != null) //hit
        {
            Debug.Log(hitInfo.collider.name);
            Crate crate = hitInfo.transform.GetComponent<Crate>();
            if(crate == null && hitInfo.transform.parent)
            {
                crate = hitInfo.transform.parent.GetComponent<Crate>();
            }
            if(crate != null)
            {
                Debug.Log("remaining hp: " + crate.TakeDamage(30));
            }
        }

        cooldownRemaining = cooldown;
    }

    RaycastHit FindClosestHitInfo(Ray ray)
    {
        RaycastHit closestHit = new RaycastHit();
        float distance = 0f;
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in hits){
            if(hit.transform != this.transform && (closestHit.transform == null || hit.distance < distance))
            {
                closestHit = hit;
                distance = hit.distance;
            }
        }

        return closestHit;
    }
}
