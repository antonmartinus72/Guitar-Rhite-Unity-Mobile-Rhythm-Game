using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMSaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class BMPlayerPerformanceScore
{
    public int performanceScore;
    public int noteHitSuccess;
    public int noteHitMiss;
    public float noteHitSuccessRate;
    public string performanceRating;

}
