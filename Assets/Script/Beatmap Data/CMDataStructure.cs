using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CMSavesData
{
    public int beatmapIndex;
    public int progress;
    public List<CMSavesList> data;
}

[System.Serializable]
public class CMSavesList
{
    public string date;
    public int score;
}

[System.Serializable]
public class CMFiles
{
    public TextAsset beatmap;
    public TextAsset save;
    public AudioClip audio;
    public bool displayOnLevelSelector = true;
}
// Kelas untuk mendefinisikan struktur dari data JSON
[System.Serializable]
public class CMHeader
{
    public string songTitle;
    public string songAuthor;
    public string gameMode;
    public string fileAuthor;
    public string saveFilename;
    public string audioFilename;
    public int stylePresetIndex;
}

[System.Serializable]
public class CMNote
{
    public int LPB;
    public int num;
    public int block;
    public int type;
    public string key;
    public string eventTrigger;
    public List<CMNote> notes;
}

[System.Serializable]
public class CMData
{
    public int maxBlock;
    public int BPM;
    public int offset;
    public List<CMKeySet> keySet;
    public List<CMNote> notes;
}

[System.Serializable]
public class CMKeySet
{
    public bool isDefaultKeySet;
    public string eventId;
    public List<string> keys;
}



// Kelas utama untuk data beatmap
[System.Serializable]
public class CMBeatmapData
{
    public int beatmapIndex;
    public CMHeader header;
    public CMData data;
    public bool displayOnLevelSelector;
}

[System.Serializable]
public class CMGamepadData
{
    public GameObject keyPadObj;
    public string keyValue;
}

