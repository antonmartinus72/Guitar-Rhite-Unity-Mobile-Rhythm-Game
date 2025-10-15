using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "noteChord", menuName = "ScriptableObjects/Music Notes")]
public class noteChord : ScriptableObject
{
    public string chordKeyValue;
    public Sprite chordKeySprite;
    public GameObject noteLongPrefab;
    public Color colorDefault = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
