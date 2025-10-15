using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class BeatmapLoader : MonoBehaviour
{
    [Header("Beatmap/Files/Json")]
    //public List<CMFiles> beatmapCMFiles;
    public List<CMBeatmapDataSO> beatmapCMSOFiles;
    public List<BMBeatmapDataSO> beatmapBMSOFiles;
    public List<MIMBeatmapDataSO> beatmapMIMSOFiles;

    [Header("Other")]
    public List<CMBeatmapData> beatmapCMList = new List<CMBeatmapData>(); // Serialize dari json yang ada di index beatmapCMSOFiles
    //public List<CMSavesData> savesCMData = new List<CMSavesData>();


    //public List<BMBeatmapData> beatmapBMList = new List<BMBeatmapData>();

    private void Awake()
    {
        //GameManager.Instance.beatmapLoader = this;
        ReadFilesCM();
        //beatmapCMList = SortCMBySongTitle(beatmapCMList);
    }

    public void ReadFilesCM()
    {
        // Bikin List baru. Nanti list ini akan menggantikan List yang ada di public variabel.
        List<CMBeatmapData> beatmapCMList = new List<CMBeatmapData>();
        List<CMSavesData> savesCMData = new List<CMSavesData>();

        int index = 0;
        foreach (var jsonBeatmap in beatmapCMSOFiles)
        {
            // Baca file beatmap dari beatmapCMSOFiles
            string jsonBeatmapText = jsonBeatmap.beatmapJSONFile.text;
            CMBeatmapData beatmapData = JsonConvert.DeserializeObject<CMBeatmapData>(jsonBeatmapText); // Deserialisasi JSON ke objek BeatmapData
            beatmapData.beatmapIndex = beatmapCMSOFiles.IndexOf(jsonBeatmap);
            beatmapCMList.Add(beatmapData); // tambahkan data ini ke beatmapCMList local

            index++;
        }
        this.beatmapCMList = beatmapCMList;
    }




    //public void ReadFilesCM()
    //{
    //    // Bikin List baru. Nanti list ini akan menggantikan List yang ada di public variabel.
    //    List<CMBeatmapData> beatmapCMList = new List<CMBeatmapData>();
    //    List<CMSavesData> savesCMData = new List<CMSavesData>();

    //    int index = 0;
    //    foreach (var textAsset in beatmapCMFiles)
    //    {

    //        if (textAsset != null)
    //        {
    //            // Baca file beatmap dari beatmapCMFiles
    //            string jsonBeatmapText = textAsset.beatmap.text;
    //            CMBeatmapData beatmapData = JsonConvert.DeserializeObject<CMBeatmapData>(jsonBeatmapText); // Deserialisasi JSON ke objek BeatmapData
    //            beatmapData.beatmapIndex = beatmapCMFiles.IndexOf(textAsset);
    //            beatmapCMList.Add(beatmapData); // tambahkan data ini ke beatmapCMList local


    //            // Baca file save di beatmapCMFiles
    //            string jsonSaveText = textAsset.save.text;
    //            CMSavesData saveData = JsonConvert.DeserializeObject<CMSavesData>(jsonSaveText); // Deserialisasi JSON ke objek BeatmapData
    //            saveData.beatmapIndex = beatmapCMFiles.IndexOf(textAsset);
    //            savesCMData.Add(saveData); // tambahkan data ini ke savesCMData local

    //            //Debug.Log("Author" + beatmapCMList);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("One or more Json Beatmap/Save Tests are not assigned!");
    //        }

    //        index++;
    //    }
    //    this.beatmapCMList = beatmapCMList;
    //    this.savesCMData = savesCMData;
    //}

    //public void ReadFilesBM()
    //{
    //    foreach (TextAsset textAsset in beatmapBMFiles)
    //    {
    //        if (textAsset != null)
    //        {
    //            // Baca teks dari file
    //            string jsonText = textAsset.text;

    //            // Deserialisasi JSON ke objek BeatmapData
    //            BMBeatmapData beatmapData = JsonConvert.DeserializeObject<BMBeatmapData>(jsonText);

    //            beatmapBMList.Add(beatmapData);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("One or more Json Beatmap Tests are not assigned!");
    //        }
    //    }
    //}

    //public void ReadSave() { 

    //}

    public List<CMBeatmapData> SortCMBySongTitle(List<CMBeatmapData> beatmapList)
    {
        return beatmapList.OrderBy(beatmapData => beatmapData.header.songTitle).ToList();
    }

    public List<CMBeatmapData> SortCMBySongAuthor(List<CMBeatmapData> beatmapList)
    {
        return beatmapList.OrderBy(beatmapData => beatmapData.header.songAuthor).ToList();
    }
    
    //public List<BMBeatmapData> SortBMBySongTitle(List<BMBeatmapData> beatmapList)
    //{
    //    return beatmapList.OrderBy(beatmapData => beatmapData.header.songTitle).ToList();
    //}

    //public List<BMBeatmapData> SortBMBySongAuthor(List<BMBeatmapData> beatmapList)
    //{
    //    return beatmapList.OrderBy(beatmapData => beatmapData.header.songAuthor).ToList();
    //}
}
