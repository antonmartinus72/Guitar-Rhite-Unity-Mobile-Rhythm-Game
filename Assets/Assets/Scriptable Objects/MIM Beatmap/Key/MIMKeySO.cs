using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MIMKeyPreset", menuName = "ScriptableObjects/Musical Interval Master/MIM Beatmap Key Preset")]
public class MIMKeySO : ScriptableObject
{
    public Sprite keySprite;
    public string keyName;
    public AudioClip keyGuitarSfxClip;
}
