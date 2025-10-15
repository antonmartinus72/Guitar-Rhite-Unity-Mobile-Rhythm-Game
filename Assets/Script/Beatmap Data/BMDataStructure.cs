using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kelas untuk mendefinisikan struktur dari data JSON
[System.Serializable]
public class BMHeader
{
    public string songTitle;
    public string songAuthor;
    public string gameMode;
    public string fileAuthor;
    public string saveFilename;
    public string audioFilename;
}

[System.Serializable]
public class BMNote
{
    public int LPB;
    public int num;
    public int block;
    public int type;
    public List<BMNote> notes;
}

[System.Serializable]
public class BMData
{
    public int maxBlock;
    public int BPM;
    public int offset;
    public List<BMNote> notes;
}

// Kelas utama untuk data beatmap
//[System.Serializable]
//public class BMBeatmapData
//{
//    public BMHeader header;
//    public BMData data;
//}
