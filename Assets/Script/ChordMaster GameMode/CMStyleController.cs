using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMStyleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera laneCamera;
    [SerializeField] GameObject musicManager;
    [SerializeField] CMUIManager UIController;
    [SerializeField] CMNoteDetectorController noteDetectorLeft;
    [SerializeField] CMNoteDetectorController noteDetectorRight;
    [SerializeField] GameObject noteBlockLeftParticleObj;
    [SerializeField] GameObject noteBlockRightParticleObj;
    [SerializeField] List<CMGameStylePreset> gameStylePreset;
    CMGameStylePreset selectedgameStylePresetSO;


    [Header("Stylized Gameobject Ref")]
    //[SerializeField] Transform gameBackgroundParentObj;
    [SerializeField] GameObject gamepadLeftContainerObj;
    [SerializeField] GameObject gamepadRightContainerObj;
    [SerializeField] GameObject noteLaneLeftObj;
    [SerializeField] GameObject noteLaneRightObj;
    [SerializeField] GameObject noteLaneAccentLeftObj;
    [SerializeField] GameObject noteLaneAccentRightObj;
    [SerializeField] GameObject laneSparkLeftObj;
    [SerializeField] GameObject laneSparkRightObj;
    List<GameObject> notesList;
    List<GameObject> beatList;

    int SELECTEDGAMEINDEX_JSON; // Default = 0

    private void Awake()
    {
        SELECTEDGAMEINDEX_JSON = GameManager.Instance.beatmapLoader.beatmapCMList[GameManager.Instance.selectedGameLevelIndex].header.stylePresetIndex;
        selectedgameStylePresetSO = gameStylePreset[SELECTEDGAMEINDEX_JSON];
        // DEBUG, diganti dengan value dari JSON
         // Akan diganti dengan value dari JSON 
    }

    // Start is called before the first frame update
    void Start()
    {
        ApplyGameBackground();
        ApplyNoteFillMaterial();
        ApplyGlobalParticle();
        ApplyNoteLaneColorMaterial();
        ApplyBeatColor();
        ApplyLaneSpark();

    }

    private void ApplyLaneSpark()
    {
        var leftComponent = laneSparkLeftObj.GetComponent<ParticleSystem>();
        var leftComponentRenderer = leftComponent.GetComponent<Renderer>();
        var rightComponent = laneSparkRightObj.GetComponent<ParticleSystem>();
        var rightComponentRenderer = rightComponent.GetComponent<Renderer>();

        leftComponent.startColor = selectedgameStylePresetSO.laneSpark;
        leftComponentRenderer.material.SetColor("_GlowColor", selectedgameStylePresetSO.laneSparkHDRColor);
        rightComponent.startColor = selectedgameStylePresetSO.laneSpark;
        rightComponentRenderer.material.SetColor("_GlowColor", selectedgameStylePresetSO.laneSparkHDRColor);

        var colorLeft = leftComponent.colorOverLifetime;
        var colorRight = rightComponent.colorOverLifetime;
        colorLeft.color = selectedgameStylePresetSO.laneSparkColorOverTime;
        colorRight.color = selectedgameStylePresetSO.laneSparkColorOverTime;
    }

    private void ApplyBeatColor()
    {
        beatList = musicManager.GetComponent<MusicBeatLoader>().beatList;
        foreach (var beat in beatList)
        {
            if (beat.CompareTag("beat"))
            {
                beat.transform.Find("beat").GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.beatSpriteColor;
            }
            else
            {
                beat.transform.Find("beat").GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.beatMeasureSpriteColor;
            }
        }
    }

    private void ApplyNoteLaneColorMaterial()
    {
        noteLaneLeftObj.GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.laneColor;
        noteLaneRightObj.GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.laneColor;

        noteLaneAccentLeftObj.GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.lanePressIndicatorColor;
        noteLaneAccentRightObj.GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.lanePressIndicatorColor;
        noteLaneAccentLeftObj.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", selectedgameStylePresetSO.lanePressIndicatorHDRColor);
        noteLaneAccentRightObj.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", selectedgameStylePresetSO.lanePressIndicatorHDRColor);
    }

    private void ApplyGlobalParticle()
    {
        GameObject newGlobalParticle = Instantiate(selectedgameStylePresetSO.globalParticleObj, musicManager.transform.position, musicManager.transform.rotation);
        newGlobalParticle.transform.SetParent(musicManager.transform);
        newGlobalParticle.name = "Global Particle";
    }

    private void ApplyNoteFillMaterial()
    {
        notesList = musicManager.GetComponent<MusicNoteLoader>().notesList;
        selectedgameStylePresetSO.noteMaterial.SetColor("_GlowColor", selectedgameStylePresetSO.noteMaterialHDRColor);
        selectedgameStylePresetSO.noteTextMaterial.SetColor("_GlowColor", selectedgameStylePresetSO.noteStartContentTextSpriteHDRColor);

        foreach (var note in notesList)
        {
            if (note.transform.Find("fill") != null)
            {
                var fillChild = note.transform.Find("fill");
                var noteStartContentTextChild = note.transform.Find("start_content");
                var noteStartContentBgChild = noteStartContentTextChild.transform.Find("background");

                // note 'fill'
                fillChild.GetComponent<SpriteRenderer>().material = selectedgameStylePresetSO.noteMaterial;
                fillChild.GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.noteSpriteColor;
                noteStartContentTextChild.GetComponent<SpriteRenderer>().material = selectedgameStylePresetSO.noteTextMaterial; // the text is .png not 'text'

                // note 'start_content'
                noteStartContentTextChild.GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.noteStartContentTextSpriteColor;
                noteStartContentBgChild.GetComponent<SpriteRenderer>().color = selectedgameStylePresetSO.noteStartContentBackgroundSpriteColor;
                //Debug.Log("Applying Material");
            }
        }
    }

    private void ApplyGameBackground()
    {
        GameObject newBackground = Instantiate(selectedgameStylePresetSO.gameBackgroundImageObj, transform.position, transform.rotation);
        newBackground.name = "Background Image Canvas";
        newBackground.transform.SetParent(musicManager.transform);
        newBackground.GetComponent<Canvas>().worldCamera = laneCamera;
    }

    // Update is called once per frame
    void Update()
    {

        if (UIController.isLeftNoteKeyMatch == true && noteDetectorLeft.collidedLeftNoteScoreObject != null)
        {
            noteBlockLeftParticleObj.SetActive(true);
        }
        else
        {
            noteBlockLeftParticleObj.SetActive(false);
        }

        if (UIController.isRightNoteKeyMatch == true && noteDetectorRight.collidedRightNoteScoreObject != null)
        {
            noteBlockRightParticleObj.SetActive(true);
        }
        else
        {
            noteBlockRightParticleObj.SetActive(false);
        }
    }
}
