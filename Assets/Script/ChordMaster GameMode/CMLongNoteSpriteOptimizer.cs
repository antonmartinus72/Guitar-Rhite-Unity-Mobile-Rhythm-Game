using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CMLongNoteSpriteOptimizer : MonoBehaviour
{
    [SerializeField] GameObject antispam;
    [SerializeField] GameObject start;
    [SerializeField] GameObject start_content;
    [SerializeField] GameObject end;
    [SerializeField] GameObject fill;

    void Start()
    {
        antispam.SetActive(false);
        start.SetActive(false);
        start_content.SetActive(false);
        end.SetActive(false);
        fill.SetActive(false);

        if (fill.activeSelf == false)
        {
            transform.localScale = fill.transform.localScale;
        }
    }

    private void OnBecameVisible()
    {
        antispam.SetActive(true);
        start.SetActive(true);
        start_content.SetActive(true);
        end.SetActive(true);
        fill.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        antispam.SetActive(false);
        start.SetActive(false);
        start_content.SetActive(false);
        end.SetActive(false);
        fill.SetActive(false);
    }
}
