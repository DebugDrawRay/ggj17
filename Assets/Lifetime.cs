using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float life;

    void Update()
    {
        if(life > 0)
        {
            life -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
