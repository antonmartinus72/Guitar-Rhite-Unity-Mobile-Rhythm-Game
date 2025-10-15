using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AnotherLoadTest : MonoBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();

    void Start()
    {
        LoadAudioFiles();
    }

    void LoadAudioFiles()
    {
        string[] audioFiles = Directory.GetFiles(Application.persistentDataPath, "*.wav"); // Ubah ekstensi file sesuai kebutuhan
        foreach (string filePath in audioFiles)
        {
            StartCoroutine(LoadAudioCoroutine(filePath));
        }
    }

    IEnumerator LoadAudioCoroutine(string filePath)
    {
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);
                audioClips.Add(audioClip);
            }
            else
            {
                Debug.LogError("Failed to load audio file: " + filePath + ", Error: " + uwr.error);
            }
        }
    }
}
