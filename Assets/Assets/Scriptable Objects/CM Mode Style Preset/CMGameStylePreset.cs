using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CMGameStylePreset", menuName = "ScriptableObjects/CM Mode Style Preset")]
public class CMGameStylePreset : ScriptableObject
{
    [Header("References")]
    public GameObject gameBackgroundImageObj;
    public Material noteMaterial;
    public Material noteTextMaterial;
    public GameObject globalParticleObj;

    [Header("Game Object Colors")]
    // Note
    public Color noteStartContentTextSpriteColor = Color.white;
    [ColorUsage(true, true)] public Color noteStartContentTextSpriteHDRColor = Color.white;
    public Color noteStartContentBackgroundSpriteColor = Color.black;
    public Color noteSpriteColor = Color.white;
    [ColorUsage(true, true)] public Color noteMaterialHDRColor = Color.white;
    [ColorUsage(true, true)] public Color globalParticleHDRColor = Color.white;

    // Beat
    public Color beatSpriteColor = Color.white;
    public Color beatMeasureSpriteColor = Color.red;

    // Lane/Block
    public Color laneColor = Color.blue;
    public Color lanePressIndicatorColor = Color.blue;
    [ColorUsage(true, true)] public Color lanePressIndicatorHDRColor = Color.white;
    public Color laneSpark = Color.white;
    [ColorUsage(true, true)] public Color laneSparkHDRColor = Color.white;
    public Gradient laneSparkColorOverTime;

    // Button / Gamepad
    [Header("UI Colors")]
    public Color gamepadNormalColor = Color.white;
    public Color gamepadPressColor = Color.white;
    public Color gamepadTextColor = Color.white;
    public Color gameUITextXColor = Color.white;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
