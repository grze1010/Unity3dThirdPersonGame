using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CRE {

    //STATIC VARIABLES
    //jump
    public static float JUMP_POWER = 6f;

    //flip jump
    public static float FLIP_JUMP_POWER = 5f;
    public static float FLIP_JUMP_HORIZONTAL_POWER = 4f;

    //standard movement
    public static float MOVEMENT_SPRINT = 1.5f;
    public static float SPRINT_SPEED = 2.5f;
    public static Vector2 MOUSE_SPEED_DELTA = new Vector2(10f, 10f);

    //player camera
    public static float MAX_TARGET_Y_ANGLE_WITHOUT_Z_FIX = 20;
    public static float MAX_TARGET_Y_ANGLE = 70;

    //other
    public static float MAX_TIME_BETWEEN_DOUBLE_CLICKS = 0.3f;

    //skills
    public static string SKILL_FIREBALL = "Fireball";
    public static Dictionary<string, CRE_Skill_Settings> SKILL_SETTINGS = new Dictionary<string, CRE_Skill_Settings>
    {
        { SKILL_FIREBALL, new CRE_Skill_Settings(SKILL_FIREBALL, 200f, 10f) }
    };
    //public static float FIREBALL_FORWARD_FORCE = 200f;


    //STATIC METHODS
    public static float RadianToDegree(float angle)
    {
        return angle * (180f / Mathf.PI);
    }

    public static float DegreeToRadian(float angle)
    {
        return Mathf.PI * angle / 180f;
    }

    public static float DistanceFromGround(GameObject obj) //cre_todo change to check with  rays
    {
        return obj.transform.position.y;
    }

    public static float CalculateCathetus(float catheusA, float hypotenuseC) //float a, c from c2 = a2 + b2, return b
    {
        return Mathf.Sqrt(Mathf.Pow(hypotenuseC, 2) - Mathf.Pow(catheusA, 2)); ;
    }

    public static float CalculateHypotenuse(float catheusA, float catheusB) //float a, b from c2 = a2 + b2, return c
    {
        return Mathf.Sqrt(Mathf.Pow(catheusA, 2) + Mathf.Pow(catheusB, 2)); ;
    }

    public static float CalculateAngle(float catheusA, float catheusB) // a, b from tanX = a/b, return X
    {
        return 90f - CRE.RadianToDegree(Mathf.Atan2(catheusA, Mathf.Abs(catheusB)));
    }

    public static RaycastHit FindClosestHitInfo(Ray ray, Transform raySource)
    {
        RaycastHit closestHit = new RaycastHit();
        float distance = 0f;
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform != raySource && (closestHit.transform == null || hit.distance < distance))
            {
                closestHit = hit;
                distance = hit.distance;
            }
        }

        return closestHit;
    }

    public class CRE_Skill_Settings {

        public string OBJ_NAME;
        public float FORCE;
        public float TTL;
        
        public CRE_Skill_Settings(string name, float force, float ttl)
        {
            OBJ_NAME = name;
            FORCE = force;
            TTL = ttl;
        }
    }
}
