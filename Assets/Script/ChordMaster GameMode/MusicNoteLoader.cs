using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicNoteLoader : MonoBehaviour
{
    [Header("Beatmap Properties")]
    private int subBeat; // Number sub beat in a single beat (LBP)
    private int timeSignature; // default time signature (4/4)
    private int noteStartMeasurePos;

    [Header("Gameobject References")]
    [SerializeField] private GameObject instansiatedNoteContainer;
    [SerializeField] private GameObject noteSinglePrefab;
    [SerializeField] private GameObject noteLongPrefab;
    [SerializeField] private Transform noteBlock0;
    [SerializeField] private Transform noteBlock1;
    [SerializeField] private Transform noteRenderDistance;
    private AudioClip musicAudioFile;

    [Header("Collection")]
    private List<GameObject> subBeatList;
    private List<CMBeatmapData> beatmapListCM;
    private List<CMFiles> filesListCM;
    public List<noteChord> noteLongSO;
    public List<GameObject> notesList;
    [SerializeField] private List<CMKeySet> keySetListCM;

    //public UnityAction OptimizeNotePrefabAction;

    //public delegate void OptimizeNotePrefabHandler();
    //public event OptimizeNotePrefabHandler OnOptimizeNotePrefabEvent;

    private void Awake()
    {
        LoadMusicFile();
        LoadKeySetData();
    }

    private void LoadKeySetData()
    {
        int selectedGameLevelIndex = GameManager.Instance.selectedGameLevelIndex;
        keySetListCM = GameManager.Instance.beatmapLoader.beatmapCMList[selectedGameLevelIndex].data.keySet;
    }

    void Start()
    {
        subBeat = GetComponent<MusicBeatLoader>().subBeat;
        timeSignature = GetComponent<MusicBeatLoader>().timeSignature;
        noteStartMeasurePos = GetComponent<MusicBeatLoader>().noteStartMeasurePos;
        subBeatList = GetComponent<MusicBeatLoader>().subBeatList;
        beatmapListCM = GameManager.Instance.beatmapLoader.beatmapCMList;
        //filesListCM = GameManager.Instance.beatmapLoader.beatmapCMFiles;
        InstantiateNotePrefab();
        DestroySubNotePrefab(subBeatList);

        //OptimizeNotePrefabAction?.Invoke();
    }

    private void DestroySubNotePrefab(List<GameObject> subBeatList)
    {
        foreach (var note in subBeatList)
        {
            if (note.tag == "beatSub")
            {
                Destroy(note);
            }
        }
        subBeatList.Clear();
    }

    private void DeactivateNotviewedObject(List<GameObject> subBeatList, List<GameObject> notesList)
    {
        float noteRenderDistancePosY = noteRenderDistance.transform.position.y;
        foreach (var item in subBeatList)
        {
            if (item.transform.position.y > noteRenderDistancePosY)
            {
                item.SetActive(false);
            }
        }
        foreach (var item in notesList)
        {
            if (item.transform.position.y > noteRenderDistancePosY)
            {
                item.SetActive(false);
            }
        }
    }

    
    private void LoadMusicFile()
    {
        int selectedGameLevelIndex = GameManager.Instance.selectedGameLevelIndex;
        musicAudioFile = GameManager.Instance.beatmapLoader.beatmapCMSOFiles[selectedGameLevelIndex].musicAudioFile;
        //musicAudioFile = GameManager.Instance.beatmapLoader.beatmapCMFiles[selectedGameLevelIndex].audio;
        gameObject.GetComponent<AudioSource>().clip = musicAudioFile;

    }

    private void InstantiateNotePrefab()
    {
        var noteStartIndex = (noteStartMeasurePos - 1) * (subBeat * timeSignature); // tentukan note akan di mulai pada measure keberapa
        float lastNotePosY = subBeatList[0].transform.position.y; // nilai awal

        List<CMNote> notes = beatmapListCM[GameManager.Instance.selectedGameLevelIndex].data.notes;

        Vector3 currentNoteBlock0WorldPos = new Vector3(noteBlock0.position.x, lastNotePosY, transform.position.z); // Create new position
        Vector3 currentNoteBlock1WorldPos = new Vector3(noteBlock1.position.x, lastNotePosY, transform.position.z); // Create new position


        for (int i = 0; i < notes.Count; i++)
        {
            if (notes[i].type == 1 & notes[i].block == 0)
            {
                InstantiateSingleNotePrefab(notes, noteStartIndex, currentNoteBlock0WorldPos, lastNotePosY, i);
            }
            else if (notes[i].type == 1 & notes[i].block == 1)
            {
                InstantiateSingleNotePrefab(notes, noteStartIndex, currentNoteBlock1WorldPos, lastNotePosY, i);
            }
            else if (notes[i].type == 2 & notes[i].block == 0)
            {
                InstantiateLongNotePrefab(notes,noteStartIndex,currentNoteBlock0WorldPos, lastNotePosY, i);
            }
            else if (notes[i].type == 2 & notes[i].block == 1)
            {
                InstantiateLongNotePrefab(notes, noteStartIndex, currentNoteBlock1WorldPos, lastNotePosY, i);
            }
        }
    }

    private void InstantiateSingleNotePrefab(List<CMNote> notes, int noteStartIndex, Vector3 currentNoteBlockWorldPos, float lastNotePosY, int i_INDEX)
    {
        int currentNoteNum = notes[i_INDEX].num + noteStartIndex;

        currentNoteBlockWorldPos.y = subBeatList[currentNoteNum].transform.position.y;
        GameObject singleNote = Instantiate(noteSinglePrefab, currentNoteBlockWorldPos, Quaternion.identity);
        notesList.Add(singleNote);
        Transform currentBeatParent = subBeatList[currentNoteNum].transform.parent;
        singleNote.transform.SetParent(currentBeatParent);
        if (notes[i_INDEX].notes.Count > 0)
        {
            for (int j = 0; j < notes[i_INDEX].notes.Count; j++)
            {
                int currentSubNoteNum = notes[j].num + noteStartIndex;

                currentNoteBlockWorldPos.y = subBeatList[currentSubNoteNum].transform.position.y;
                GameObject singleNoteSub = Instantiate(noteSinglePrefab, currentNoteBlockWorldPos, Quaternion.identity);
                notesList.Add(singleNoteSub);
                singleNoteSub.transform.SetParent(currentBeatParent);
            }
        }
    }

    private void InstantiateLongNotePrefab(List<CMNote> notes, int noteStartIndex, Vector3 currentNoteBlockWorldPos, float lastNotePosY, int i_INDEX)
    {
        int currentNoteNum = notes[i_INDEX].num + noteStartIndex;

        currentNoteBlockWorldPos.y = subBeatList[currentNoteNum].transform.position.y;

        // define note's chord caption
        noteChord selectedNoteSO = GetNoteScriptableObjectByKey(noteLongSO, notes[i_INDEX].key);
        if (selectedNoteSO == null)
        {
            // set chord to "null" sprite (Scriptable Object) if note not found in 'List<noteChord> noteLongSO'
            selectedNoteSO = GetNoteScriptableObjectByKey(noteLongSO, "null");
            Debug.LogWarning("Chord Scriptable Object not found in 'noteLongSO'. Object replaced by 'null' Scriptable Object instead");
        }

        //Instantiate note
        GameObject longNote = Instantiate(noteLongPrefab, currentNoteBlockWorldPos, Quaternion.identity);
        longNote.transform.position = CalculatePositionBasedOnChildPivot(longNote);
        longNote.transform.SetParent(instansiatedNoteContainer.transform);
        notesList.Add(longNote);
        lastNotePosY = currentNoteBlockWorldPos.y;
        // change longNote's sprite to scriptable object's chord sprite
        Sprite longNoteNewSprite = selectedNoteSO.chordKeySprite;
        longNote.transform.Find("start_content").GetComponent<SpriteRenderer>().sprite = longNoteNewSprite;
        longNote.transform.GetComponent<CMNoteScript>().noteData = selectedNoteSO; // add noteChord SP to instantiated prefab
        longNote.transform.GetComponent<CMNoteScript>().key = selectedNoteSO.chordKeyValue; // add noteChord SP to instantiated prefab
                                                                                            // Set new parent
        Transform currentBeatParent = subBeatList[currentNoteNum].transform.parent;//cari parent baru untuk longNote
        longNote.transform.SetParent(currentBeatParent);// terapkan parent baru untuk longNote
                                                        // other
        var boxColl = longNote.GetComponent<BoxCollider2D>(); //untuk diatur ulang
        if (notes[i_INDEX].notes.Count > 0)
        {
            for (int j = 0; j < notes[i_INDEX].notes.Count; j++)
            {
                int currentSubNoteNum = notes[i_INDEX].notes[j].num + noteStartIndex; // save current index's num
                currentNoteBlockWorldPos.y = subBeatList[currentSubNoteNum].transform.position.y; // Update pos
                float calcSubNoteFillScaleY = currentNoteBlockWorldPos.y - lastNotePosY;// calculate new note size according to y position

                // apply the new position to current note's child
                Transform longNoteChildFill = longNote.transform.Find("fill");
                Transform longNoteChildEnd = longNote.transform.Find("end");
                Vector3 newLongNoteChildFillScale = new Vector3(longNote.transform.localScale.x, calcSubNoteFillScaleY, longNote.transform.localScale.z);
                Vector3 newLongNoteChildEndPos = new Vector3(longNote.transform.localPosition.x, currentNoteBlockWorldPos.y, longNote.transform.localPosition.z);

                if (longNoteChildFill != null)
                {
                    longNoteChildFill.localScale = newLongNoteChildFillScale;
                    longNoteChildEnd.position = newLongNoteChildEndPos;
                    boxColl.size = new Vector2(1f, newLongNoteChildFillScale.y); // atur ulang ukuran boxcollider
                    boxColl.offset = new Vector2(0f, (newLongNoteChildFillScale.y / 2)); // atur ulang offset boxcollider
                }
                else
                {
                    Debug.LogError("Game object 'fill' not found in 'longNote'");
                }
            }
        }
    }


    private noteChord GetNoteScriptableObjectByKey(List<noteChord> longNoteSO, string chordKey)
    {
        noteChord selectedSO = null;
        foreach (var item in noteLongSO)
        {
            if (chordKey == item.name)
            {
                selectedSO = item;
                break;
            }
        }
        return selectedSO;
    }

    private Vector3 CalculatePositionBasedOnChildPivot(GameObject longNote)
    {
        Transform childPivot = longNote.transform.Find("start");
        Vector3 newPosition = childPivot.position;
        return newPosition;
    }

    private int GetNoteStartIndex(int noteStartMeasurePos)
    {
        // calc start position of Notes 
        int calculateNoteStartMeasurePosValue = noteStartMeasurePos * (subBeat * timeSignature);
        return calculateNoteStartMeasurePosValue;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
