using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour {

    public float maxHealth = 100f;
    float currentHealth;

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public float TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if(currentHealth <= 0)
        {
            Die();
        }
        return currentHealth;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
