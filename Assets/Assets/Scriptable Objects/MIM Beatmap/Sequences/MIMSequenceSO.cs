using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MIMBeatmapPreset", menuName = "ScriptableObjects/Musical Interval Master/MIM Beatmap Sequence Preset")]
public class MIMSequenceSO : ScriptableObject
{
    public MIMKeySO rootKey;
    public enum noteIntervalType // ada dua tipe, dengan panjang 7 dan 12 (tidak termasuk nada root di akhir, misal > C......B-C)
    {
        diatonis_mayor,
        diatonis_minor
    }

    public noteIntervalType intervalType;
    public List<MIMKeySO> keyList;
    public List<MIMKeySO> additionalKeyList;
}

