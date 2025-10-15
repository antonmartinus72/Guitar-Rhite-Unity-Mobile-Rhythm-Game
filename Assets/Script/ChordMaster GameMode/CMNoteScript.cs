using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CMNoteScript : MonoBehaviour
{
    [Header("Note Data")]
    public noteChord noteData;  // BIARKAN KOSONG DI INPECTOR
    public string key; // BIARKAN KOSONG DI INPECTOR

    [Header("Note Child Ref")]
    public GameObject antispam;
    public GameObject start;
    public GameObject startcontent;
    public GameObject fill;
    public GameObject end;
    public GameObject optimizer;
}
