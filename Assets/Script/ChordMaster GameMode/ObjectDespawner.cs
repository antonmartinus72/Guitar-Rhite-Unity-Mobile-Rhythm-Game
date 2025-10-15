using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDespawner : MonoBehaviour
{
    GameObject parent = null;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "beat" || collision.tag == "beatMeasure" || collision.tag == "beatSub" || collision.tag == "note") 
        {
            parent = collision.transform.parent.gameObject;
            
            Destroy(parent);
        }
    }
}
