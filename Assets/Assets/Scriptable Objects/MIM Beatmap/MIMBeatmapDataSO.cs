using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MIMBeatmapPreset", menuName = "ScriptableObjects/Musical Interval Master/MIM Beatmap Preset")]
public class MIMBeatmapDataSO : ScriptableObject
{
    [Header("Values")]
    public List<MIMSequenceSO> sequenceList;
}

