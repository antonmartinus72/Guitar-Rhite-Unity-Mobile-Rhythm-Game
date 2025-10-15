using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MusicBeatLoader : MonoBehaviour
{
    [Header("Gameplay Properties")]
    [SerializeField] private int maxBlock = 2; // number of music lane
    [SerializeField] private float bpm = 60; // json
    public int subBeat = 8; // Number sub beat in a single beat (LBP)
    [SerializeField] private int maxNum = 100; // Max number of Sub Beat
    public int noteStartMeasurePos = 1; // Start position of the Note in beats
    [SerializeField] private float offset = 0; // json
    [SerializeField] private float speed = 1f; // default beat movement speed, recalculated after in runtime
    [System.NonSerialized] public int timeSignature = 4; // default time signature (4/4)
    private float measureLength = 16f;//Default
    public float beatLengthValue;// Calculated value
    public float subBeatLengthValue;

    [Header("Objects Reference")]
    [SerializeField] private GameObject beatSubPrefab;
    [SerializeField] private GameObject beatPrefab;
    [SerializeField] private GameObject beatMeasurePrefab;
    [SerializeField] private Transform noteDetector;
    [SerializeField] private Transform beatNoteActivatorObj;

    // experimental 2- InstansiateBeatPrefab ----------------------------------
    [SerializeField] private Transform noteLaneCenterPos;
    [SerializeField] private Transform noteSpawnPos;
    private int noteStartPositionIndex;

    [SerializeField] private Transform beatContainerPrefab;

    [Header("Collection")]
    public List<GameObject> subBeatList;
    public List<Transform> beatListPerMeasure;
    public List<GameObject> beatList;

    [Header("Other")]
    [SerializeField] Ease beatTweenEase = Ease.Linear;

    // Start is called before the first frame update
    void Start()
    {
        bpm = GetBpmInBeatmapList(GameManager.Instance.beatmapLoader.beatmapCMList, GameManager.Instance.selectedGameLevelIndex);
        //bpm = 60f; // debug
        subBeat = GetSubBeatInBeatmapList(GameManager.Instance.beatmapLoader.beatmapCMList, GameManager.Instance.selectedGameLevelIndex);
        //subBeat = 8; // temporary for debug


        maxNum = GetLastNumInBeatmapList(GameManager.Instance.beatmapLoader.beatmapCMList, GameManager.Instance.selectedGameLevelIndex) + maxNum;
        offset = GetOffsetInBeatmapList(GameManager.Instance.beatmapLoader.beatmapCMList, GameManager.Instance.selectedGameLevelIndex);
        //noteStartPositionIndex = subBeat * timeSignature;


        //Debug.Log("bpm : " + bpm);
        //Debug.Log("subBeat : " + subBeat);
        //Debug.Log("MaxNum : " + maxNum);
        //Debug.Log("Lastnum : " + GetLastNumInBeatmapList(GameManager.Instance.beatmapLoader.beatmapCMList, GameManager.Instance.selectedGameLevelIndex));
        //
        //
        //("offset : " + offset);


        InstansiateBeatPrefab();
        MoveInstansiatedPrefabToContainer();
        SetNoteStartPosition();
        //SetSpeedToMeasureContainerObj(); // give speed to measure container
        SetSpeedToMeasureContainerObj(); // give speed to measure container

        //BeatNoteDeactivator(); // test
    }

    private void BeatNoteDeactivator()
    {
        foreach (var beat in subBeatList)
        {
            if (beat.transform.position.y > beatNoteActivatorObj.position.y)
            {
                beat.SetActive(false);
            }
        }
    }

    //public float CalculateBeatmapSpeed()
    //{
    //    float baseVal = 1f;
    //    float speed = (baseVal * (measureLength / timeSignature)) / (60f / bpm);
    //    return speed;
    //}

    void SetSpeedToMeasureContainerObj() // membuat objek 'Measure Container' bergerak ke bawah 
    {
        speed = (speed * (measureLength / timeSignature)) / (60f / bpm);
        //Debug.Log("Speed = " + speed);
        foreach (Transform obj in beatListPerMeasure)
        {
            obj.transform.DOMoveY(noteLaneCenterPos.position.y, speed).SetEase(beatTweenEase).SetSpeedBased().OnComplete(() =>
            {
                obj.transform.DOMoveY(-150f, speed).SetEase(Ease.Linear).SetSpeedBased().OnComplete(() =>
                {
                    Destroy(obj.gameObject);
                    //Debug.Log("FINISHED OBJ NAME = " + obj.gameObject.name);
                });
            });
        }
    }


    private void MoveInstansiatedPrefabToContainer()
    {
        // Move instansiated subbeat in subBeatList
        int numberOfSubbeatPerMeasure = subBeat * timeSignature;
        Transform currentContainer = Instantiate(beatContainerPrefab, transform.position, Quaternion.identity);

        currentContainer.SetParent(GameObject.Find("Instantiated Beats Container").transform); // move measure container to a parent object

        for (int i = 0; i < subBeatList.Count; i++)
        {
            if (i == 0)
            {
                beatListPerMeasure.Add(currentContainer);
                subBeatList[i].transform.SetParent(currentContainer, transform);
                Vector3 newContaineWorldPosition = new Vector3(transform.localPosition.x, subBeatList[i].transform.localPosition.y, transform.localPosition.z);
                currentContainer.transform.position = newContaineWorldPosition;

                //if (subBeatList[i].name == "note_beatmeasure(Clone)")
                //{
                //    Vector3 newContaineWorldPosition = new Vector3(transform.localPosition.x, subBeatList[i].transform.localPosition.y, transform.localPosition.z);
                //    currentContainer.transform.position = newContaineWorldPosition;
                //}
            }
            else if (i != 0 & i % numberOfSubbeatPerMeasure == 0)
            {
                Transform container = Instantiate(beatContainerPrefab, transform.position, Quaternion.identity); // Inisialisasi currentContainer
                currentContainer = container;
                Vector3 newContaineWorldPosition = new Vector3(transform.localPosition.x, subBeatList[i].transform.localPosition.y, transform.localPosition.z);
                currentContainer.transform.position = newContaineWorldPosition;
                //if (subBeatList[i].name == "note_beatmeasure(Clone)")
                //{
                //    Vector3 newContaineWorldPosition = new Vector3(transform.localPosition.x, subBeatList[i].transform.localPosition.y, transform.localPosition.z);
                //    currentContainer.transform.position = newContaineWorldPosition;
                //}
                beatListPerMeasure.Add(currentContainer);
                subBeatList[i].transform.SetParent(currentContainer, transform);
                currentContainer.SetParent(GameObject.Find("Instantiated Beats Container").transform); // move measure container to a parent object


                //Debug.Log("IF Modulo 32 % " + i + ", indeks i = " + i + " : " + ((float)i) % (float)numberOfSubbeatPerMeasure);
                //Debug.Log("INSTANSIATED NEW CONTAINER====================================================================================");
            }
            else
            {
                subBeatList[i].transform.SetParent(currentContainer, transform);
                //Debug.Log("ELSE Modulo 32 % " + i + " = " + ((float)i) % (float)numberOfSubbeatPerMeasure);
            }
        }
    }


    private void SetNoteStartPosition()
    {
        // define start position of Notes 
        Transform noteStartObj = GameObject.Find("NOTE_START").transform;
        Vector3 newPosition = noteStartObj.position; // Dapatkan posisi saat ini
        int newNoteStartMeasurePosValue = (noteStartMeasurePos - 1) * (subBeat * timeSignature);
        newPosition.y = subBeatList[newNoteStartMeasurePosValue].transform.position.y; // Ubah komponen y posisi
        noteStartObj.position = newPosition; // Tetapkan posisi baru ke Transform

        SetMusicPlayOffsetPosition(newNoteStartMeasurePosValue);
    }

    private void SetMusicPlayOffsetPosition(int beatIndex)
    {
        GameObject selectedSubBeat = subBeatList[beatIndex];

        Vector3 newPosition = selectedSubBeat.transform.position;
        GameObject musicStartObj = GameObject.Find("MUSIC_PLAY");
        

        float worldUnitOffsetY = ConvertOffsetValueToWorldUnit(newPosition);
        //Debug.Log("worldUnitOffsetY" + worldUnitOffsetY);
        newPosition.y = worldUnitOffsetY;

        musicStartObj.transform.position = newPosition;


        Transform selectedSubBeatParent = selectedSubBeat.transform.parent;
        if (selectedSubBeatParent != null && selectedSubBeatParent.gameObject.activeSelf)
        {
            musicStartObj.transform.SetParent(selectedSubBeatParent);
            //Debug.Log("SetParent successful to " + selectedSubBeatParent.name);
        }
        else
        {
            Debug.LogError("Failed to SetParent. SelectedSubBeat has no parent or parent is inactive.");
        }
    }

    private float ConvertOffsetValueToWorldUnit(Vector3 newPosition)
    {
        float standardOffset = 48000f;
        float standardBpmPerSec = 60f; // bpm per 1 detik
        float jsonBpm = bpm;
        float calculatedOffsetJsonbpm = (standardBpmPerSec * standardOffset) / jsonBpm;


        //float generatedBeatLengthValue = beatLengthValue;
        //float offsetToWorldUnit = (calculatedOffsetJsonbpm * standardBpmPerSec) / jsonOffset);

        float jsonOffset = offset;
        float currentBeatLength = beatLengthValue;
        float offsetToWorldUnit = (jsonOffset * currentBeatLength) / calculatedOffsetJsonbpm;

        float newPositionY = newPosition.y - offsetToWorldUnit;

        return newPositionY;
    }

    private int GetNoteStartIndex(int noteStartMeasurePos)
    {
        // calc start position of Notes 
        int calculateNoteStartMeasurePosValue = noteStartMeasurePos * (subBeat * timeSignature);
        return calculateNoteStartMeasurePosValue;
    }




    // EKSPERIMENTAL
    // Testing using 'beatmapDataList' index 0

    private void InstansiateBeatPrefab()
    {
        // Calculate measure length based on speed
        float calculateMeasureLength = measureLength * speed;

        //Debug.Log("Measure Length = " + calculateMeasureLength);

        // Calculate beat length
        float beatLength = calculateMeasureLength / timeSignature;
        beatLengthValue = beatLength; // Untuk digunakan di MusicNoteLoader / keperluan lain

        // Calculate sub beat length
        float subBeatLength = beatLength / subBeat;
        subBeatLengthValue = subBeatLength; // simpan di variabel untuk keperluan lain
        //Debug.Log("subBeatLengthValue IN musicBeatLoader = " + subBeatLength);

        // Calculate time interval between beats based on tempo
        float timeInterval = 60f / (float)bpm;

        // Instantiate beats prefab
        float lastBeatPosY = noteDetector.position.y; // simpan posisi setiap instansiasi selesai dilakukan
        int beatSubCount = 0;
        int beatCount = 0;

        for (int i = 0; i < maxNum; i++)
        {
            //Spawn SUB BEAT

            Vector3 startPos = new Vector3(noteLaneCenterPos.position.x, lastBeatPosY); // Create new position
            float countInstansiatedSubbeat = (float)beatSubCount % (float)subBeat; // Calculate to instansiate beatPrefab
            float countInstansiatedBeat = (float)beatCount % (float)timeSignature; // Calculate to instansiate beatMeasurePrefab

            if (countInstansiatedSubbeat == 0)
            {
                if (countInstansiatedBeat != 0)
                {
                    // Instansiate beatPrefab
                    GameObject beat = Instantiate(beatPrefab, startPos, Quaternion.identity);
                    lastBeatPosY = beat.transform.position.y + subBeatLength;
                    subBeatList.Add(beat); // add to List<>
                    beatList.Add(beat);
                    beatCount++;
                }
                else
                {
                    // Instansiate beatMeasurePrefab
                    GameObject beatMeasure = Instantiate(beatMeasurePrefab, startPos, Quaternion.identity);
                    lastBeatPosY = beatMeasure.transform.position.y + subBeatLength;
                    subBeatList.Add(beatMeasure);
                    beatList.Add(beatMeasure);
                    beatCount++;
                }
            }
            else
            {
                // Instansiate beatSubPrefab
                GameObject subBeat = Instantiate(beatSubPrefab, startPos, Quaternion.identity);
                lastBeatPosY = subBeat.transform.position.y + subBeatLength;
                subBeatList.Add(subBeat);

            }
            beatSubCount++;
        }
    }

    // GET DATA METHOD

    private float GetOffsetInBeatmapList(List<CMBeatmapData> beatmapDataList, int selectedGameLevelIndex)
    {
        int offset = beatmapDataList[selectedGameLevelIndex].data.offset;
        return offset;
    }

    private int GetSubBeatInBeatmapList(List<CMBeatmapData> beatmapDataList, int selectedGameLevelIndex)
    {
        var currentLoadedBeatmapDataList = beatmapDataList[selectedGameLevelIndex]; // Select beatmap index using 'selectedGameLevelIndex' in GameManager
        var subBeat = currentLoadedBeatmapDataList.data.notes[0].LPB;
        return subBeat;

    }

    private float GetBpmInBeatmapList(List<CMBeatmapData> beatmapDataList, int selectedGameLevelIndex)
    {
        int bpm = beatmapDataList[selectedGameLevelIndex].data.BPM;
        float floatValue = bpm;
        return floatValue;
    }

    void FixedUpdate()
    {
        DOTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private int GetLastNumInBeatmapList(List<CMBeatmapData> beatmapDataList, int selectedGameLevelIndex)
    {
        int lastNum = 0;

        if (beatmapDataList != null && beatmapDataList.Count > 0)
        {
            var currentLoadedBeatmapDataList = beatmapDataList[selectedGameLevelIndex]; // Select beatmap index using 'selectedGameLevelIndex' in GameManager

            int LASTNOTEINDEX = currentLoadedBeatmapDataList.data.notes.Count - 1;

            lastNum = currentLoadedBeatmapDataList.data.notes[LASTNOTEINDEX].num;

            if (currentLoadedBeatmapDataList.data.notes[LASTNOTEINDEX].notes.Count > 0) // Cek apakah ada 'num' di dalam sub 'Notes'
            {
                int LASTNOTEOFSUBNOTESINDEX = currentLoadedBeatmapDataList.data.notes[LASTNOTEINDEX].notes.Count - 1;
                int lastNumInSubNotes = currentLoadedBeatmapDataList.data.notes[LASTNOTEINDEX].notes[LASTNOTEOFSUBNOTESINDEX].num;

                if (lastNumInSubNotes > lastNum)
                {
                    lastNum = lastNumInSubNotes;
                }

            }
        }
        else
        {
            Debug.LogError("List kosong atau null!");
            return 0; // Atau nilai default lainnya, tergantung kebutuhan Anda
        }

        return lastNum;
    }
}
