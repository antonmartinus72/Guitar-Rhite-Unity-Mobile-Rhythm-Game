using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class FileReadWriteExample : MonoBehaviour
{
    // Struktur data untuk menampung file
    [Serializable]
    public class FileData
    {
        public string groupName;
        public List<string> files = new List<string>();
    }

    // Variabel untuk menampung FileData pada inspector
    public List<FileData> fileDatas = new List<FileData>();

    // Fungsi untuk memuat file dan mengelompokkannya
    public void LoadFiles()
    {
        string persistentDataPath = Application.persistentDataPath;
        string[] filePaths = Directory.GetFiles(persistentDataPath);

        Dictionary<string, FileData> fileGroups = new Dictionary<string, FileData>();

        // Proses setiap file
        foreach (string filePath in filePaths)
        {
            string fileName = Path.GetFileName(filePath);
            string groupName = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            // Membuat atau mendapatkan FileData untuk grup yang sesuai
            if (!fileGroups.ContainsKey(groupName))
            {
                fileGroups[groupName] = new FileData { groupName = groupName };
            }

            // Menambahkan file ke dalam grup yang sesuai
            fileGroups[groupName].files.Add(fileName);
        }

        // Mengonversi dictionary ke dalam List<FileData>
        fileDatas = new List<FileData>(fileGroups.Values);
    }

    // Metode untuk mendapatkan struktur FileData dalam bentuk string
    public string GetFileDataStructureAsString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (FileData fileData in fileDatas)
        {
            sb.AppendLine(fileData.groupName + " [");

            foreach (string file in fileData.files)
            {
                sb.AppendLine("    " + file);
            }

            sb.AppendLine("]");
        }

        return sb.ToString();
    }

    // Contoh penggunaan
    void Start()
    {
        LoadFiles();

        // Output struktur FileData
        Debug.Log(GetFileDataStructureAsString());
    }

}
