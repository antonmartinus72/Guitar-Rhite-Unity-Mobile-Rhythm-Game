using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuitarChordLibrary_ChordKeyField : MonoBehaviour
{
    public TextMeshProUGUI versionCaption;
    public TextMeshProUGUI baseFretCaption;
    public GameObject neckSeparator;
    public GameObject capo;

    [Header("Frets")] // refer to 'frets' in json file
    public List<GameObject> frets_min1;
    public List<GameObject> frets_0;
    public List<GameObject> frets_1;
    public List<GameObject> frets_2;
    public List<GameObject> frets_3;
    public List<GameObject> frets_4;
    public List<GameObject> frets_5;
}
