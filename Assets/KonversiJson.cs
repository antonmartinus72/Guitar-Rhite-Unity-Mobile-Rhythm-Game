using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class KonversiJson : MonoBehaviour
{
    [SerializeField] TextAsset fileOriginal;  // Menampung file JSON asli
    [SerializeField] TextAsset daftarKey;     // Menampung file .txt untuk daftar key baru

    // Kelas untuk JSON bentuk Original
    public class OriginalNote
    {
        public int LPB { get; set; }
        public int num { get; set; }
        public int block { get; set; }
        public int type { get; set; }
        public List<OriginalNote> notes { get; set; }
    }

    public class OriginalJson
    {
        public string name { get; set; }
        public int maxBlock { get; set; }
        public int BPM { get; set; }
        public int offset { get; set; }
        public List<OriginalNote> notes { get; set; }
    }

    // Kelas untuk JSON bentuk Baru
    public class NewNote
    {
        public int LPB { get; set; }
        public int num { get; set; }
        public int block { get; set; }
        public string key { get; set; }
        public string eventTrigger { get; set; }
        public int type { get; set; }
        public List<NewNote> notes { get; set; }
    }

    public class KeySet
    {
        public bool isDefaultKeySet { get; set; }
        public string eventId { get; set; }
        public List<string> keys { get; set; }
    }

    public class NewData
    {
        public int maxBlock { get; set; }
        public int BPM { get; set; }
        public int offset { get; set; }
        public List<KeySet> keySet { get; set; }
        public List<NewNote> notes { get; set; }
    }

    public class NewHeader
    {
        public string songTitle { get; set; }
        public string songAuthor { get; set; }
        public string gameMode { get; set; }
        public string fileAuthor { get; set; }
        public string saveFilename { get; set; }
        public string audioFilename { get; set; }
        public int stylePresetIndex { get; set; }
    }

    public class NewJson
    {
        public NewHeader header { get; set; }
        public NewData data { get; set; }
    }

    // Fungsi untuk melakukan konversi
    public string ConvertJson(string originalJson)
    {
        // Deserialize JSON asli ke objek
        OriginalJson original = JsonConvert.DeserializeObject<OriginalJson>(originalJson);

        // Membuat JSON baru
        NewJson newJson = new NewJson
        {
            header = new NewHeader
            {
                songTitle = original.name, // Mengambil value 'name' dari Original JSON
                songAuthor = "AUTHOR",
                gameMode = "chordmaster",
                fileAuthor = "Anton M",
                saveFilename = "-",
                audioFilename = "-",
                stylePresetIndex = 3
            },
            data = new NewData
            {
                maxBlock = original.maxBlock,
                BPM = original.BPM, // Mengambil BPM dari Original JSON
                offset = original.offset, // Dapat diatur sesuai kebutuhan
                keySet = new List<KeySet>
                {
                    new KeySet
                    {
                        isDefaultKeySet = true,
                        eventId = "default",
                        keys = new List<string> { "C", "Dm", "Em", "F", "G", "Am", "Bm" }
                    },
                    new KeySet
                    {
                        isDefaultKeySet = false,
                        eventId = "minorScale",
                        keys = new List<string> { "C", "Dm", "Em", "F", "G", "Am", "Bm" }
                    }
                },
                notes = ConvertNotes(original.notes) // Mengonversi notes dari Original JSON
            }
        };

        // Serialize objek ke JSON baru
        return JsonConvert.SerializeObject(newJson, Formatting.Indented);
    }

    // Fungsi untuk mengonversi list catatan dari format Original ke format Baru
    private List<NewNote> ConvertNotes(List<OriginalNote> originalNotes)
    {
        List<NewNote> newNotes = new List<NewNote>();

        foreach (var originalNote in originalNotes)
        {
            NewNote newNote = new NewNote
            {
                LPB = originalNote.LPB, // Menggunakan LPB dari Original
                num = originalNote.num, // Menggunakan num dari Original
                block = originalNote.block, // Menggunakan block dari Original
                key = "C", // Placeholder key, akan diganti setelah konversi
                eventTrigger = "null",
                type = originalNote.type, // Menggunakan type dari Original
                notes = ConvertNotes(originalNote.notes) // Rekursif untuk nested notes
            };

            newNotes.Add(newNote);
        }

        return newNotes;
    }

    // Fungsi untuk menyimpan file JSON hasil konversi ke folder persistent data
    private void SaveConvertedJson(string convertedJson)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "convertedJson");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, "convertedFile.json");
        File.WriteAllText(filePath, convertedJson);
        Debug.Log("File berhasil disimpan di: " + filePath);
    }

    // Fungsi untuk membaca file .txt dan memparsing daftar key
    private List<string> ParseKeyFile(TextAsset keyFile)
    {
        List<string> keys = new List<string>();
        string[] lines = keyFile.text.Split('\n');  // Membaca per baris
        foreach (string line in lines)
        {
            string[] keyLine = line.Split(' ');  // Memisahkan key dengan spasi
            foreach (string key in keyLine)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    keys.Add(key.Trim());
                }
            }
        }
        return keys;
    }

    // Fungsi untuk mengisi field 'key' dari file .txt
    private void FillNoteKeys(List<NewNote> notes, List<string> keys)
    {
        int keyIndex = 0;
        foreach (var note in notes)
        {
            if (keyIndex < keys.Count)
            {
                note.key = keys[keyIndex];
                keyIndex++;
            }
            else
            {
                Debug.LogWarning("Tidak cukup key untuk mengisi semua notes.");
                break;
            }
        }
    }

    // Tes konversi di fungsi Start
    void Start()
    {
        if (fileOriginal != null && daftarKey != null)
        {
            string originalJson = fileOriginal.text;
            string convertedJson = ConvertJson(originalJson);

            // Ambil daftar key dari file .txt
            List<string> keys = ParseKeyFile(daftarKey);

            // Ambil data notes dari JSON yang telah dikonversi
            NewJson newJson = JsonConvert.DeserializeObject<NewJson>(convertedJson);

            // Isi field 'key' pada notes hirarki pertama
            FillNoteKeys(newJson.data.notes, keys);

            // Simpan file yang telah dikonversi dengan key baru
            string updatedJson = JsonConvert.SerializeObject(newJson, Formatting.Indented);
            SaveConvertedJson(updatedJson);
        }
        else
        {
            Debug.LogError("File Original atau Daftar Key belum diset di Inspector!");
        }
    }
}





//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using Newtonsoft.Json;

//public class KonversiJson : MonoBehaviour
//{
//    [SerializeField] TextAsset fileOriginal;  // Menampung file JSON asli
//    [SerializeField] TextAsset daftarKey;     // Menampung file .txt untuk daftar key baru

//    // Kelas untuk JSON bentuk Original
//    public class OriginalNote
//    {
//        public int LPB { get; set; }
//        public int num { get; set; }
//        public int block { get; set; }
//        public int type { get; set; }
//        public List<OriginalNote> notes { get; set; }
//    }

//    public class OriginalJson
//    {
//        public string name { get; set; }
//        public int maxBlock { get; set; }
//        public int BPM { get; set; }
//        public int offset { get; set; }
//        public List<OriginalNote> notes { get; set; }
//    }

//    // Kelas untuk JSON bentuk Baru
//    public class NewNote
//    {
//        public int LPB { get; set; }
//        public int num { get; set; }
//        public int block { get; set; }
//        public string key { get; set; }
//        public string eventTrigger { get; set; }
//        public int type { get; set; }
//        public List<NewNote> notes { get; set; }
//    }

//    public class KeySet
//    {
//        public bool isDefaultKeySet { get; set; }
//        public string eventId { get; set; }
//        public List<string> keys { get; set; }
//    }

//    public class NewData
//    {
//        public int maxBlock { get; set; }
//        public int BPM { get; set; }
//        public int offset { get; set; }
//        public List<KeySet> keySet { get; set; }
//        public List<NewNote> notes { get; set; }
//    }

//    public class NewHeader
//    {
//        public string songTitle { get; set; }
//        public string songAuthor { get; set; }
//        public string gameMode { get; set; }
//        public string fileAuthor { get; set; }
//        public string saveFilename { get; set; }
//        public string audioFilename { get; set; }
//        public int stylePresetIndex { get; set; }
//    }

//    public class NewJson
//    {
//        public NewHeader header { get; set; }
//        public NewData data { get; set; }
//    }

//    // Fungsi untuk melakukan konversi
//    public string ConvertJson(string originalJson)
//    {
//        // Deserialize JSON asli ke objek
//        OriginalJson original = JsonConvert.DeserializeObject<OriginalJson>(originalJson);

//        // Membuat JSON baru
//        NewJson newJson = new NewJson
//        {
//            header = new NewHeader
//            {
//                songTitle = "Twinkle Twinkle Little Star",
//                songAuthor = "Mozart",
//                gameMode = "chordmaster",
//                fileAuthor = "Studio Ammie",
//                saveFilename = "-",
//                audioFilename = "-",
//                stylePresetIndex = 3
//            },
//            data = new NewData
//            {
//                maxBlock = original.maxBlock,
//                BPM = 100, // Diset manual karena berbeda
//                offset = 3300, // Diset manual karena berbeda
//                keySet = new List<KeySet>
//                {
//                    new KeySet
//                    {
//                        isDefaultKeySet = true,
//                        eventId = "default",
//                        keys = new List<string> { "C", "Dm", "Em", "F", "G", "Am", "Bm" }
//                    },
//                    new KeySet
//                    {
//                        isDefaultKeySet = false,
//                        eventId = "minorScale",
//                        keys = new List<string> { "C", "Dm", "Em", "F", "G", "Am", "Bm" }
//                    }
//                },
//                notes = ConvertNotes(original.notes)
//            }
//        };

//        // Serialize objek ke JSON baru
//        return JsonConvert.SerializeObject(newJson, Formatting.Indented);
//    }

//    // Fungsi untuk mengonversi list catatan dari format Original ke format Baru
//    private List<NewNote> ConvertNotes(List<OriginalNote> originalNotes)
//    {
//        List<NewNote> newNotes = new List<NewNote>();

//        foreach (var originalNote in originalNotes)
//        {
//            NewNote newNote = new NewNote
//            {
//                LPB = originalNote.LPB / 2, // Ubah nilai LPB sesuai kebutuhan
//                num = originalNote.num * 2, // Ubah num sesuai kebutuhan
//                block = originalNote.block,
//                key = "C", // Placeholder key, akan diganti setelah konversi
//                eventTrigger = "null",
//                type = originalNote.type,
//                notes = ConvertNotes(originalNote.notes)
//            };

//            newNotes.Add(newNote);
//        }

//        return newNotes;
//    }

//    // Fungsi untuk menyimpan file JSON hasil konversi ke folder persistent data
//    private void SaveConvertedJson(string convertedJson)
//    {
//        string directoryPath = Path.Combine(Application.persistentDataPath, "convertedJson");
//        if (!Directory.Exists(directoryPath))
//        {
//            Directory.CreateDirectory(directoryPath);
//        }

//        string filePath = Path.Combine(directoryPath, "convertedFile.json");
//        File.WriteAllText(filePath, convertedJson);
//        Debug.Log("File berhasil disimpan di: " + filePath);
//    }

//    // Fungsi untuk membaca file .txt dan memparsing daftar key
//    private List<string> ParseKeyFile(TextAsset keyFile)
//    {
//        List<string> keys = new List<string>();
//        string[] lines = keyFile.text.Split('\n');  // Membaca per baris
//        foreach (string line in lines)
//        {
//            string[] keyLine = line.Split(' ');  // Memisahkan key dengan spasi
//            foreach (string key in keyLine)
//            {
//                if (!string.IsNullOrWhiteSpace(key))
//                {
//                    keys.Add(key.Trim());
//                }
//            }
//        }
//        return keys;
//    }

//    // Method baru untuk mengisi field 'key' dari file .txt
//    private void FillNoteKeys(List<NewNote> notes, List<string> keys)
//    {
//        int keyIndex = 0;
//        foreach (var note in notes)
//        {
//            if (keyIndex < keys.Count)
//            {
//                note.key = keys[keyIndex];
//                keyIndex++;
//            }
//            else
//            {
//                Debug.LogWarning("Tidak cukup key untuk mengisi semua notes.");
//                break;
//            }
//        }
//    }

//    // Tes konversi di fungsi Start
//    void Start()
//    {
//        if (fileOriginal != null && daftarKey != null)
//        {
//            string originalJson = fileOriginal.text;
//            string convertedJson = ConvertJson(originalJson);

//            // Ambil daftar key dari file .txt
//            List<string> keys = ParseKeyFile(daftarKey);

//            // Ambil data notes dari JSON yang telah dikonversi
//            NewJson newJson = JsonConvert.DeserializeObject<NewJson>(convertedJson);

//            // Isi field 'key' pada notes hirarki pertama
//            FillNoteKeys(newJson.data.notes, keys);

//            // Simpan file yang telah dikonversi dengan key baru
//            string updatedJson = JsonConvert.SerializeObject(newJson, Formatting.Indented);
//            SaveConvertedJson(updatedJson);
//        }
//        else
//        {
//            Debug.LogError("File Original atau Daftar Key belum diset di Inspector!");
//        }
//    }
//}




//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using Newtonsoft.Json;

//public class KonversiJson : MonoBehaviour
//{
//    [SerializeField] TextAsset fileOriginal; // Menampung file JSON asli

//    // Kelas untuk JSON bentuk Original
//    public class OriginalNote
//    {
//        public int LPB { get; set; }
//        public int num { get; set; }
//        public int block { get; set; }
//        public int type { get; set; }
//        public List<OriginalNote> notes { get; set; }
//    }

//    public class OriginalJson
//    {
//        public string name { get; set; }
//        public int maxBlock { get; set; }
//        public int BPM { get; set; }
//        public int offset { get; set; }
//        public List<OriginalNote> notes { get; set; }
//    }

//    // Kelas untuk JSON bentuk Baru
//    public class NewNote
//    {
//        public int LPB { get; set; }
//        public int num { get; set; }
//        public int block { get; set; }
//        public string key { get; set; }
//        public string eventTrigger { get; set; }
//        public int type { get; set; }
//        public List<NewNote> notes { get; set; }
//    }

//    public class KeySet
//    {
//        public bool isDefaultKeySet { get; set; }
//        public string eventId { get; set; }
//        public List<string> keys { get; set; }
//    }

//    public class NewData
//    {
//        public int maxBlock { get; set; }
//        public int BPM { get; set; }
//        public int offset { get; set; }
//        public List<KeySet> keySet { get; set; }
//        public List<NewNote> notes { get; set; }
//    }

//    public class NewHeader
//    {
//        public string songTitle { get; set; }
//        public string songAuthor { get; set; }
//        public string gameMode { get; set; }
//        public string fileAuthor { get; set; }
//        public string saveFilename { get; set; }
//        public string audioFilename { get; set; }
//        public int stylePresetIndex { get; set; }
//    }

//    public class NewJson
//    {
//        public NewHeader header { get; set; }
//        public NewData data { get; set; }
//    }

//    // Fungsi untuk melakukan konversi
//    public string ConvertJson(string originalJson)
//    {
//        // Deserialize JSON asli ke objek
//        OriginalJson original = JsonConvert.DeserializeObject<OriginalJson>(originalJson);

//        // Membuat JSON baru
//        NewJson newJson = new NewJson
//        {
//            header = new NewHeader
//            {
//                songTitle = "JUDUL",
//                songAuthor = "PEMILIK LAGU / AUTHOR",
//                gameMode = "chordmaster",
//                fileAuthor = "Anton M",
//                saveFilename = "-",
//                audioFilename = "-",
//                stylePresetIndex = 3
//            },
//            data = new NewData
//            {
//                maxBlock = original.maxBlock,
//                BPM = 100, // Diset manual karena berbeda
//                offset = 3300, // Diset manual karena berbeda
//                keySet = new List<KeySet>
//                {
//                    new KeySet
//                    {
//                        isDefaultKeySet = true,
//                        eventId = "default",
//                        keys = new List<string> { "C", "Dm", "Em", "F", "G", "Am", "Bm" }
//                    },
//                    new KeySet
//                    {
//                        isDefaultKeySet = false,
//                        eventId = "minorScale",
//                        keys = new List<string> { "C", "Dm", "Em", "F", "G", "Am", "Bm" }
//                    }
//                },
//                notes = ConvertNotes(original.notes)
//            }
//        };

//        // Serialize objek ke JSON baru
//        return JsonConvert.SerializeObject(newJson, Formatting.Indented);
//    }

//    // Fungsi untuk mengonversi list catatan dari format Original ke format Baru
//    private List<NewNote> ConvertNotes(List<OriginalNote> originalNotes)
//    {
//        List<NewNote> newNotes = new List<NewNote>();

//        foreach (var originalNote in originalNotes)
//        {
//            NewNote newNote = new NewNote
//            {
//                LPB = originalNote.LPB / 2, // Ubah nilai LPB sesuai kebutuhan
//                num = originalNote.num * 2, // Ubah num sesuai kebutuhan
//                block = originalNote.block,
//                key = "C", // Set key secara manual karena data original tidak memiliki
//                eventTrigger = "null",
//                type = originalNote.type,
//                notes = ConvertNotes(originalNote.notes)
//            };

//            newNotes.Add(newNote);
//        }

//        return newNotes;
//    }

//    // Fungsi untuk menyimpan file JSON hasil konversi ke folder persistent data
//    private void SaveConvertedJson(string convertedJson)
//    {
//        string directoryPath = Path.Combine(Application.persistentDataPath, "convertedJson");
//        if (!Directory.Exists(directoryPath))
//        {
//            Directory.CreateDirectory(directoryPath);
//        }

//        string filePath = Path.Combine(directoryPath, "convertedFile.json");
//        File.WriteAllText(filePath, convertedJson);
//        Debug.Log("File berhasil disimpan di: " + filePath);
//    }

//    // Tes konversi di fungsi Start
//    void Start()
//    {
//        if (fileOriginal != null)
//        {
//            string originalJson = fileOriginal.text;
//            string convertedJson = ConvertJson(originalJson);
//            SaveConvertedJson(convertedJson);
//        }
//        else
//        {
//            Debug.LogError("File Original belum diset di Inspector!");
//        }
//    }
//}
