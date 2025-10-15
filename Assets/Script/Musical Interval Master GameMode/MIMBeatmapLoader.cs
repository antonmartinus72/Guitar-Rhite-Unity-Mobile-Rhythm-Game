using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIMBeatmapLoader : MonoBehaviour
{
    [SerializeField] MIMBeatmapDataSO loadedBeatmap;
    public List<MIMSequenceData> sequenceData;
    public MIMSequenceSO.noteIntervalType noteIntervalType;


    // Start is called before the first frame update
    void Start()
    {
        int selectedBeatmapIndex = GameManager.Instance.selectedGameLevelIndex;
        loadedBeatmap = GameManager.Instance.beatmapLoader.beatmapMIMSOFiles[selectedBeatmapIndex];
        Debug.Log("Load Beatmap Index :" + selectedBeatmapIndex + ", Name : '" + loadedBeatmap.name + "'");
        InitializeSequenceData();
    }

    private void InitializeSequenceData()
    {
        sequenceData = new List<MIMSequenceData>();

        foreach (var sequence in loadedBeatmap.sequenceList)
        {
            var newSeq = new MIMSequenceData();
            newSeq.rootKey = sequence.rootKey.keyName;
            newSeq.intervalType = sequence.intervalType;
            newSeq.keysData = new List<MIMKeyData>();
            newSeq.additionalKeysData = new List<MIMKeyData>();

            foreach (var key in sequence.keyList)
            {
                var newKey = new MIMKeyData();
                newKey.keySO = key;
                //newKey.keySprite = key.keySprite;
                //newKey.keyName = key.keyName;
                newSeq.keysData.Add(newKey);
            }
            
            foreach (var key in sequence.additionalKeyList)
            {
                var newKey = new MIMKeyData();
                newKey.keySO = key;
                //newKey.keySprite = key.keySprite;
                //newKey.keyName = key.keyName;
                newSeq.additionalKeysData.Add(newKey);
            }



            sequenceData.Add(newSeq);
        }
    }
}


[System.Serializable]
public class MIMSequenceData
{
    public string rootKey;
    public MIMSequenceSO.noteIntervalType intervalType;
    public List<MIMKeyData> keysData;
    public List<MIMKeyData> additionalKeysData; // for note placeholder
}

[System.Serializable]
public class MIMKeyData
{
    public MIMKeySO keySO;
    //public Sprite keySprite;
    //public string keyName;
}