using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using static FileReadWriteExample;
using System.Security.Cryptography;
using System;

public class SaveManager : MonoBehaviour
{


    /// <CATATAN>
    /// - SaveScoreXX() Berisi data yang di urutkan secara historis (yang paling baru akan menggantikan yang lama)
    /// - SaveTopScoreXX() Berisi data yang di urutkan berdasarkan tinggi/jumlah score (score yang paling besar akan menggantikan score yang terkecil)
    /// 
    /// </CATATAN>




    private int maxSaveRecordListGlobal = 20; // jumlah entri max score, berurutan secara tanggal.
    private int maxSaveTopScoreListGlobal = 10; // jumlah entri hanya untuk topscore.

    public List<MJSaveData> saveRecordListMJ = new List<MJSaveData>(); // Riwayat score permainan
    string saveFolderMJString = "savemj"; // save folder for MJ Mode

    // Chord Master
    public List<CMSaveData> saveRecordListCM = new List<CMSaveData>(); // Riwayat score permainan
    public List<CMSaveData> saveTopScoreListCM = new List<CMSaveData>(); // Top Score Permainan
    string saveFolderCMString = "savecm"; // save folder for CM Mode

    // Beat Master
    public List<BMSaveData> saveRecordListBM = new List<BMSaveData>(); // Riwayat score permainan
    public List<BMSaveData> saveTopScoreListBM = new List<BMSaveData>(); // Top Score Permainan
    string saveFolderBMString = "savebm"; // save folder for BM Mode

    // Musical Interval Master
    public List<MIMSaveData> saveRecordListMIM = new List<MIMSaveData>(); // Riwayat score permainan
    //public List<MIMSaveData> saveTopScoreListMIM = new List<MIMSaveData>(); // Top Score Permainan
    string saveFolderMIMString = "savemim"; // save folder for BM Mode

    // Audio Config
    public List<AudioVolumeConfigData> globalAudioConfigDataList = new List<AudioVolumeConfigData>();
    string settingsFolderPathString = "settings";

    // ====== CHORD MASTER MODE =======
    // Save game after gameplay
    public void SaveScoreCM(int performanceScore, int noteHitSuccess, int noteHitMiss, float noteHitSuccessRate, string performanceRating, string date)
    {
        string saveFolderName = saveFolderCMString; // nama folder save untuk CM Gamemode
        string fileName = "save_" + GameManager.Instance.selectedGameLevelIndex.ToString(); // nama file (contoh = "save_00" untuk index beatmaploader list/selectedGameLevelIndex = 0)
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string filePath = Path.Combine(saveFolderPath, fileName + ".json");

        CMSaveData newScoreData = new CMSaveData
        {
            performanceScore = performanceScore,
            noteHitSuccess = noteHitSuccess,
            noteHitMiss = noteHitMiss,
            noteHitSuccessRate = noteHitSuccessRate,
            performanceRating = performanceRating,
            date = date
        };

        // Load existing data if file exists
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);// Baca file json
            string decryptedJson = CryptoUtility.Decrypt(existingJson);// decypt file json
            saveRecordListCM = JsonConvert.DeserializeObject<List<CMSaveData>>(decryptedJson) ?? new List<CMSaveData>();
        }
        else
        {
            saveRecordListCM = new List<CMSaveData>();
        }

        // Insert new data at the beginning of the list
        saveRecordListCM.Insert(0, newScoreData);

        // Remove the last item if the list exceeds 20 entries
        if (saveRecordListCM.Count > maxSaveRecordListGlobal)
        {
            //saveRecordListCM.RemoveAt(saveRecordListCM.Count - 1);

            int removeCount = saveRecordListCM.Count - maxSaveRecordListGlobal;

            //saveTopScoreListCM.RemoveAt(saveTopScoreListCM.Count - 1);
            saveRecordListCM.RemoveRange(maxSaveRecordListGlobal, removeCount);
        }

        // Create save folder if it doesn't exist
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }


        string json = JsonConvert.SerializeObject(saveRecordListCM, Formatting.Indented);
        string encryptedJson = CryptoUtility.Encrypt(json);


        File.WriteAllText(filePath, encryptedJson);
        //File.WriteAllText(filePath, json);

        // Serialize the updated list to JSON
        //string json = JsonConvert.SerializeObject(saveRecordListCM, Formatting.Indented);

        //// Write JSON to file
        //File.WriteAllText(filePath, json);

        Debug.Log("Data score saved to: " + filePath);

        SaveTopScoreCM(newScoreData); // Cek score apakah masuk ke dalam topscore. Jika masuk maka simpan ke file "savetop_(xx)"
    }

    // Load save file untuk diluar script seperti CM level selector di menu
    public List<CMSaveData> LoadScoreCM(int selectedGameLevelIndex_CM)
    {
        saveRecordListCM.Clear(); // Bersihkan List setiap load score data

        string saveFolderName = saveFolderCMString; // nama folder save untuk CM Gamemode
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string saveName = "save_" + GameManager.Instance.selectedGameLevelIndex.ToString() + ".json"; // nama file yang akan di-load
        string filePath = Path.Combine(saveFolderPath, saveName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file does not exist: " + filePath);
            return null;
        }

        string chiperTextjson = File.ReadAllText(filePath); // load from encrypted json file
        string encryptedJson = CryptoUtility.Decrypt(chiperTextjson); //decypt encrypted json file
        saveRecordListCM = JsonConvert.DeserializeObject<List<CMSaveData>>(encryptedJson);

        // Check if the list exceeds 20 entries and remove the last entry if necessary
        while (saveRecordListCM.Count > 20)
        {
            saveRecordListCM.RemoveAt(saveRecordListCM.Count - 1);
        }

        Debug.Log("Data loaded from: " + filePath);

        return saveRecordListCM;
    }

    // Top score mengacu pada score yang di sortir berdasarkan score tertinggi
    // Mennyimpan score pada file "savetop_(xx)" jika score setelah gameplay masuk dalam top score. Jumlah score yang masuk ke top score ada pada var "maxSaveTopScoreListCM" 

    // ====================================================================== CHORD MASTER MODE =======================================================================

    public void SaveTopScoreCM(CMSaveData newScoreData) 
    {
        string saveFolderName = saveFolderCMString; // nama folder save untuk CM Gamemode
        string fileName = "savetop_" + GameManager.Instance.selectedGameLevelIndex.ToString(); // nama file (contoh = "save_00" untuk index beatmaploader list/selectedGameLevelIndex = 0)
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string filePath = Path.Combine(saveFolderPath, fileName + ".json");

        // load JSON
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            string decryptedJson = CryptoUtility.Decrypt(existingJson);// decypt file json
            saveTopScoreListCM = JsonConvert.DeserializeObject<List<CMSaveData>>(decryptedJson) ?? new List<CMSaveData>();
        }
        else
        {
            saveTopScoreListCM = new List<CMSaveData>(); // Inisialisasi List baru jika file JSON tidak ditemukan
        }

        if (saveTopScoreListCM.Count > 0)
        {
            // Cek apakah currentScore melebihi topscore
            foreach (var item in saveTopScoreListCM)
            {
                if (newScoreData.performanceScore >= item.performanceScore)
                {
                    saveTopScoreListCM.Add(newScoreData);
                    saveTopScoreListCM.Sort((a, b) => b.performanceScore.CompareTo(a.performanceScore));

                    if (saveTopScoreListCM.Count > maxSaveTopScoreListGlobal) // hapus index > 10
                    {
                        int removeCount = saveTopScoreListCM.Count - maxSaveTopScoreListGlobal;

                        //saveTopScoreListCM.RemoveAt(saveTopScoreListCM.Count - 1);
                        saveTopScoreListCM.RemoveRange(maxSaveTopScoreListGlobal, removeCount);

                    }
                    break;
                }
                else
                {
                    Debug.Log("Score saat ini tidak mencapai topscore");
                }
            }
        }
        else
        {
            // Langsung Tambahkan jika saveTopScoreListCM == 0 
            saveTopScoreListCM.Add(newScoreData);
        }
        

        // Serialize the updated list to JSON
        string json = JsonConvert.SerializeObject(saveTopScoreListCM, Formatting.Indented);
        string encryptedJson = CryptoUtility.Encrypt(json);


        File.WriteAllText(filePath, encryptedJson);
        //File.WriteAllText(filePath, json);

        Debug.Log("Data top score saved to: " + filePath);

    }
    public void LoadTopScoreCM(int selectedGameLevelIndexCM) 
    {
        saveTopScoreListCM.Clear(); // Bersihkan List setiap load score data

        string saveFolderName = saveFolderCMString; // nama folder save untuk CM Gamemode
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string saveName = "savetop_" + GameManager.Instance.selectedGameLevelIndex.ToString() + ".json"; // nama file yang akan di-load
        string filePath = Path.Combine(saveFolderPath, saveName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file does not exist: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        saveTopScoreListCM = JsonConvert.DeserializeObject<List<CMSaveData>>(json);

        // Check if the list exceeds 20 entries and remove the last entry if necessary
        while (saveTopScoreListCM.Count > 20)
        {
            saveTopScoreListCM.RemoveAt(saveTopScoreListCM.Count - 1);
        }

        Debug.Log("Data loaded from: " + filePath);
    }


    // ====================================================================== BEAT MASTER MODE ========================================================================
    // Save game after gameplay
    public void SaveScoreBM(int performanceScore, int noteHitSuccess, int noteHitMiss, float noteHitSuccessRate, string performanceRating, string date)
    {
        var saveRecordList = saveRecordListBM;
        string saveFolderName = saveFolderBMString; // nama folder save untuk CM Gamemode
        string fileName = "save_" + GameManager.Instance.selectedGameLevelIndex.ToString(); // nama file (contoh = "save_00" untuk index beatmaploader list/selectedGameLevelIndex = 0)
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string filePath = Path.Combine(saveFolderPath, fileName + ".json");

        BMSaveData newScoreData = new BMSaveData
        {
            performanceScore = performanceScore,
            noteHitSuccess = noteHitSuccess,
            noteHitMiss = noteHitMiss,
            noteHitSuccessRate = noteHitSuccessRate,
            performanceRating = performanceRating,
            date = date
        };

        // Load existing data if file exists
        if (File.Exists(filePath))
        {
            Debug.Log("File save ditemukan");
            string existingJson = File.ReadAllText(filePath);
            string decryptedJson = CryptoUtility.Decrypt(existingJson);// decypt file json
            saveRecordList = JsonConvert.DeserializeObject<List<BMSaveData>>(decryptedJson) ?? new List<BMSaveData>();


            //saveRecordList = JsonConvert.DeserializeObject<List<BMSaveData>>(existingJson) ?? new List<BMSaveData>();
        }
        else
        {
            saveRecordList = new List<BMSaveData>();
        }

        // Insert new data at the beginning of the list
        saveRecordList.Insert(0, newScoreData);

        // Remove the last item if the list exceeds 20 entries
        if (saveRecordList.Count > maxSaveRecordListGlobal)
        {
            Debug.Log("SAVE MELEBIHI BATAS");
            //saveRecordList.RemoveAt(saveRecordList.Count - 1);

            int removeCount = saveRecordList.Count - maxSaveRecordListGlobal;

            //saveTopScoreListCM.RemoveAt(saveTopScoreListCM.Count - 1);
            saveRecordList.RemoveRange(maxSaveRecordListGlobal, removeCount);
        }

        // Create save folder if it doesn't exist
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        // Serialize the updated list to JSON
        string json = JsonConvert.SerializeObject(saveRecordList, Formatting.Indented);

        // Encrypt
        string encryptedJson = CryptoUtility.Encrypt(json);

        // Write JSON to file
        //File.WriteAllText(filePath, json);
        File.WriteAllText(filePath, encryptedJson);

        Debug.Log("Data score saved to: " + filePath);

        SaveTopScoreBM(newScoreData);
    }
    // Load game after gameplay

    public List<BMSaveData> LoadScoreBM(int selectedGameLevelIndex_BM)
    {
        saveRecordListBM.Clear(); // Bersihkan List setiap load score data
        var saveRecordList = saveRecordListBM;
        string saveFolderName = saveFolderBMString; // nama folder save untuk CM Gamemode
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string saveName = "save_" + GameManager.Instance.selectedGameLevelIndex.ToString() + ".json"; // nama file yang akan di-load
        string filePath = Path.Combine(saveFolderPath, saveName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file does not exist: " + filePath);
            return null;
        }

        string chiperTextjson = File.ReadAllText(filePath); // load from encrypted json file
        string encryptedJson = CryptoUtility.Decrypt(chiperTextjson); //decypt encrypted json file
        saveRecordList = JsonConvert.DeserializeObject<List<BMSaveData>>(encryptedJson);
        //saveRecordList = JsonConvert.DeserializeObject<List<BMSaveData>>(chiperTextjson); //// Ganti line diatas, HANYA UNUTK KEPERLUAN DEBUG!

        // Check if the list exceeds 20 entries and remove the last entry if necessary
        while (saveRecordList.Count > 20)
        {
            saveRecordList.RemoveAt(saveRecordList.Count - 1);
        }

        Debug.Log("Data loaded from: " + filePath);

        return saveRecordList;
    }

    public void SaveTopScoreBM(BMSaveData newScoreData)
    {
        var saveTopScoreList = saveTopScoreListBM;
        string saveFolderName = saveFolderBMString; // nama folder save untuk CM Gamemode
        string fileName = "savetop_" + GameManager.Instance.selectedGameLevelIndex.ToString(); // nama file (contoh = "save_00" untuk index beatmaploader list/selectedGameLevelIndex = 0)
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string filePath = Path.Combine(saveFolderPath, fileName + ".json");

        // load JSON
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            string decryptedJson = CryptoUtility.Decrypt(existingJson);// decypt file json
            saveTopScoreList = JsonConvert.DeserializeObject<List<BMSaveData>>(decryptedJson) ?? new List<BMSaveData>();
            //saveTopScoreList = JsonConvert.DeserializeObject<List<BMSaveData>>(existingJson) ?? new List<BMSaveData>(); // Ganti line diatas, HANYA UNUTK KEPERLUAN DEBUG!
        }
        else
        {
            saveTopScoreList = new List<BMSaveData>(); // Inisialisasi List baru jika file JSON tidak ditemukan
        }

        if (saveTopScoreList.Count > 0)
        {
            // Cek apakah currentScore melebihi topscore
            foreach (var item in saveTopScoreList)
            {
                if (newScoreData.performanceScore >= item.performanceScore)
                {
                    saveTopScoreList.Add(newScoreData);
                    saveTopScoreList.Sort((a, b) => b.performanceScore.CompareTo(a.performanceScore));

                    if (saveTopScoreList.Count > maxSaveTopScoreListGlobal) // hapus index > 10
                    {
                        int removeCount = saveTopScoreList.Count - maxSaveTopScoreListGlobal;

                        //saveTopScoreList.RemoveAt(saveTopScoreList.Count - 1);
                        saveTopScoreList.RemoveRange(maxSaveTopScoreListGlobal, removeCount);

                    }
                    break;
                }
            }
        }
        else
        {
            // Langsung Tambahkan jika saveTopScoreList == 0 
            saveTopScoreList.Add(newScoreData);
        }

        // Serialize the updated list to JSON
        string json = JsonConvert.SerializeObject(saveTopScoreList, Formatting.Indented);
        // Encrypt
        string encryptedJson = CryptoUtility.Encrypt(json);
        //File.WriteAllText(filePath, json);
        //File.WriteAllText(filePath, json);
        File.WriteAllText(filePath, encryptedJson);

        Debug.Log("Data top score saved to: " + filePath);

    }
    public void LoadTopScoreBM()
    {
        // NOT IMPLEMENTED YET
    }

    // ====================================================================== MUSICAL INTERVAL MASTER MODE ============================================================
    public void SaveScoreMIM(int starRating)
    {
        var saveRecordList = saveRecordListMIM;
        string saveFolderName = saveFolderMIMString; // nama folder save untuk MIM Gamemode
        string fileName = "save_" + GameManager.Instance.selectedGameLevelIndex.ToString(); // nama file (contoh = "save_00" untuk index beatmaploader list/selectedGameLevelIndex = 0)
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string filePath = Path.Combine(saveFolderPath, fileName + ".json");

        //Debug.Log("starRating : " + starRating);

        MIMSaveData newScoreData = new MIMSaveData
        {
            starRating = starRating,
            date = GetDateNow()
        };

        // Load existing data if file exists
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);// Baca file json
            string decryptedJson = CryptoUtility.Decrypt(existingJson);// decypt file json
            saveRecordList = JsonConvert.DeserializeObject<List<MIMSaveData>>(decryptedJson) ?? new List<MIMSaveData>();
            //saveRecordList = JsonConvert.DeserializeObject<List<MIMSaveData>>(existingJson) ?? new List<MIMSaveData>();

            // Cek apakah rating lebih besar dari rating yang ada di savefile
            if (starRating > saveRecordList[0].starRating)
            {
                // Insert new data at the beginning of the list
                saveRecordList[0] = newScoreData;
            }
            else
            {
                Debug.Log("Rating tidak lebih besar dari rekor sebelumnya");
                return;
            }
        }
        else
        {
            saveRecordList = new List<MIMSaveData>();
            saveRecordList.Add(newScoreData);
        }

        
        //if (saveRecordList.Count != 0)
        //{
            
        //    //saveRecordList.Insert(0, newScoreData);
        //}


        // Create save folder if it doesn't exist
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        string json = JsonConvert.SerializeObject(saveRecordList, Formatting.Indented);
        string encryptedJson = CryptoUtility.Encrypt(json);
        File.WriteAllText(filePath, encryptedJson);
        //File.WriteAllText(filePath, json);

        Debug.Log("Data score saved to: " + filePath);

        //SaveTopScoreCM(newScoreData); // Cek score apakah masuk ke dalam topscore. Jika masuk maka simpan ke file "savetop_(xx)"
    }
    public List<MIMSaveData> LoadScoreMIM(int selectedGameLevelIndex_MIM)
    {

        saveRecordListMIM.Clear(); // Bersihkan List setiap load score data
        var saveRecordList = saveRecordListMIM;
        string saveFolderName = saveFolderMIMString; // nama folder save untuk CM Gamemode
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string saveName = "save_" + GameManager.Instance.selectedGameLevelIndex.ToString() + ".json"; // nama file yang akan di-load
        string filePath = Path.Combine(saveFolderPath, saveName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file does not exist: " + filePath);
            return null;
        }

        string chiperTextjson = File.ReadAllText(filePath); // load from encrypted json file
        string encryptedJson = CryptoUtility.Decrypt(chiperTextjson); //decypt encrypted json file
        saveRecordList = JsonConvert.DeserializeObject<List<MIMSaveData>>(encryptedJson);
        //saveRecordList = JsonConvert.DeserializeObject<List<MIMSaveData>>(chiperTextjson);

        // Check if the list exceeds 20 entries and remove the last entry if necessary
        while (saveRecordList.Count > 20)
        {
            saveRecordList.RemoveAt(saveRecordList.Count - 1);
        }

        Debug.Log("Data loaded from: " + filePath);

        return saveRecordList;
    }


    // ====================================================================== MUSICAL JOURNEY MODE ============================================================
    public List<MJSaveData> LoadLevelDataMJ()
    {

        saveRecordListMJ = new List<MJSaveData>(); // Bersihkan List setiap load score data

        string saveFolderName = saveFolderMJString; // nama folder save untuk CM Gamemode
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string saveName = "savemj.json"; // nama file yang akan di-load
        string filePath = Path.Combine(saveFolderPath, saveName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file does not exist: " + filePath);
            return saveRecordListMJ;
        }

        string json = File.ReadAllText(filePath);
        saveRecordListMJ = JsonConvert.DeserializeObject<List<MJSaveData>>(json);


        Debug.Log("Loaded MJ Save Data from: " + filePath);

        return saveRecordListMJ;
    }


    public void SaveLevelDataMJ(int gameManagerSelectedMusicalJourneyLevelIndex, int starNumber, int levelMapPage)
    {
        var saveRecordList = saveRecordListMJ;
        string saveFolderName = saveFolderMJString; // nama folder save untuk CM Gamemode
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string saveName = "savemj.json"; // nama file yang akan di-load
        string filePath = Path.Combine(saveFolderPath, saveName);

        // Buat data baru
        MJSaveData newScoreData = new MJSaveData
        {
            isLevelCompleted = true,
            starNumber = starNumber,
            levelMapPage = levelMapPage
        };

        Debug.Log("isLevelCompleted = true");
        Debug.Log("starNumber = " + starNumber);
        Debug.Log("levelMapPage" + levelMapPage);

        // Load existing data if file exists
        if (File.Exists(filePath))
        {
            Debug.Log("File save ditemukan");
            string existingJson = File.ReadAllText(filePath);
            //saveRecordList = JsonConvert.DeserializeObject<List<MJSaveData>>(existingJson) ?? new List<MJSaveData>(); // Untuk test, akan diganti dengan "CryptoUtility.Decrypt"

            string decryptedJson = CryptoUtility.Decrypt(existingJson);// decypt file json
            saveRecordList = JsonConvert.DeserializeObject<List<MJSaveData>>(decryptedJson) ?? new List<MJSaveData>();

            // Replace index saveRecordList berdasarkan "GameManager.Instance.selectedMusicalJourneyLevelIndex".
            saveRecordList[gameManagerSelectedMusicalJourneyLevelIndex] = newScoreData;
        }
        else // Jika save tidak ditemukan
        {
            saveRecordList = new List<MJSaveData>(); // Instansiasi List Baru
            saveRecordList.Add(newScoreData); // Tambahkan ke index 1. Karena dalam kasus ini, level yang terbuka hanya Map Level 1-1
        }


        // Create save folder if it doesn't exist
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        // Serialize the updated list to JSON
        string json = JsonConvert.SerializeObject(saveRecordList, Formatting.Indented);

        //// Encrypt
        //string encryptedJson = CryptoUtility.Encrypt(json); // Dinonaktifkan sementara untuk test

        // Write JSON to file
        //File.WriteAllText(filePath, json);
        File.WriteAllText(filePath, json); // test
        //File.WriteAllText(filePath, encryptedJson);// // Dinonaktifkan sementara untuk test

        Debug.Log("Data score saved to: " + filePath);
    }


    // ====== Global Audio Config =======
    public void SaveAudioSettings(float audioMasterVolume, float audioMusicVolume, float audioSfxVolume)
    {
        globalAudioConfigDataList = new List<AudioVolumeConfigData>();

        string saveFolderName = settingsFolderPathString; // nama folder save untuk CM Gamemode
        string fileName = "audio_config"; // nama file (contoh = "save_00" untuk index beatmaploader list/selectedGameLevelIndex = 0)
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string filePath = Path.Combine(saveFolderPath, fileName + ".json");

        AudioVolumeConfigData newSaveData = new AudioVolumeConfigData
        {
            audioMasterVolume = audioMasterVolume,
            audioMusicVolume = audioMusicVolume,
            audioSfxVolume = audioSfxVolume
        };

        // Load existing data if file exists
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);// Baca file json
        }
        else
        {
            globalAudioConfigDataList = new List<AudioVolumeConfigData>();
        }

        // Create save folder if it doesn't exist
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        globalAudioConfigDataList.Add(newSaveData);

        string json = JsonConvert.SerializeObject(globalAudioConfigDataList, Formatting.Indented);


        File.WriteAllText(filePath, json);

        Debug.Log("Audio Settings saved to: " + filePath);
    }
    public void LoadAudioSettings()
    {
        globalAudioConfigDataList = new List<AudioVolumeConfigData>(); // Bersihkan List setiap load score data

        string saveFolderName = settingsFolderPathString; // nama folder save untuk CM Gamemode
        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolderName);
        string saveName = "audio_config.json"; // nama file yang akan di-load
        string filePath = Path.Combine(saveFolderPath, saveName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file does not exist: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        globalAudioConfigDataList = JsonConvert.DeserializeObject<List<AudioVolumeConfigData>>(json);

        Debug.Log("Audio Settings Loaded");
    }

    public string GetDateNow()
    {
        DateTime currentDate = DateTime.Now;
        string formattedDate = currentDate.ToString("dd/MM/yy");
        Debug.Log(formattedDate); // Output: 01/02/24

        return formattedDate;
    }
}


[System.Serializable]
public class CMSaveData
{
    public int performanceScore;
    public int noteHitSuccess;
    public int noteHitMiss;
    public float noteHitSuccessRate;
    public string performanceRating;
    public string date;
}

[System.Serializable]
public class BMSaveData
{
    public int performanceScore;
    public int noteHitSuccess;
    public int noteHitMiss;
    public float noteHitSuccessRate;
    public string performanceRating;
    public string date;
}

public class MIMSaveData
{
    public int starRating;
    public string date;
}

[System.Serializable]
public class AudioVolumeConfigData
{
    public float audioMasterVolume;
    public float audioMusicVolume;
    public float audioSfxVolume;
}


[System.Serializable]
public class MJSaveData
{
    public bool isLevelCompleted = false;
    public int starNumber = 0; // max 3
    public int levelMapPage = 1;
}