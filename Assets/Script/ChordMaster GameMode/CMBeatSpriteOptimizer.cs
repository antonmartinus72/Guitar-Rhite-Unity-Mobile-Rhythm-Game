using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CMBeatSpriteOptimizer : MonoBehaviour
{
    [SerializeField] GameObject beat;
    //Transform beatNoteActivatorObj;
    Transform beatNoteDeactivatorObj;
    [SerializeField] GameObject parentObj;
    //bool isObjActivated = false;
    //float parentPosY;
    private void Awake()
    {
        beatNoteDeactivatorObj = GameObject.Find("NOTE_DEACTIVATOR").transform;
    }
    void Start()
    {
        beat.SetActive(false);
    }
    private void OnBecameVisible()
    {
        beat.SetActive(true);
    }
    private void OnBecameInvisible()
    {
        beat.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (parentObj.transform.position.y <= beatNoteDeactivatorObj.position.y)
        {
            // Nonaktifkan beat jika posisi y lebih kurang dari beatNoteDeactivatorObj.position.y
            parentObj.SetActive(false);
        }
    }
}
