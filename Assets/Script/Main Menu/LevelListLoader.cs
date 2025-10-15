#if false
// THIS SCRIPT IS DISABLED


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelListLoader : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject buttonPrefab;
    //[SerializeField] private BeatmapLoader beatmapLoaderObj;

    void Start()
    {
        //beatmapLoaderObj = GameManager.Instance.beatmapLoader;

        //GameManager.Instance.SelectLevelCM(); // DEBUG

        if (GameManager.Instance.CurrentGameModeState == GameManager.GameModeState.ChordMasterMode)
        {
            InstansiateRowPrefab_CM(buttonPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void GetIndex()
    //{
    //    int siblingIndex = transform.GetSiblingIndex();
    //    Debug.Log("Sibling Index of this object: " + siblingIndex);
    //}
    private void InstansiateRowPrefab_CM(GameObject levelPrefab)
    {
        List<CMFiles> beatmapCMFiles = GameManager.Instance.beatmapLoader.beatmapCMFiles;
        //List<CMFiles> beatmapCMFiles = GameManager.Instance.beatmapLoader.beatmapCMFiles;
        List<CMBeatmapData> beatmapCMList = GameManager.Instance.beatmapLoader.beatmapCMList;

        int numberingName = 1;
        foreach (var item in beatmapCMList)
        {
            string prefabName = numberingName + "." + item.header.songTitle + " by " + item.header.songAuthor;
            GameObject instantiatedPrefab = Instantiate(levelPrefab, transform.position, transform.rotation);
            instantiatedPrefab.name = prefabName;
            instantiatedPrefab.transform.SetParent(transform); // Ubah instantiatedPrefab menjadi child dari object ini

            RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1f, 1f, 0f);

            int currentIndex = item.beatmapIndex;
            instantiatedPrefab.GetComponent<LevelListHeader>().currentIndex = currentIndex;
            instantiatedPrefab.GetComponent<LevelListHeader>().saveFile = beatmapCMFiles[currentIndex].save;
            instantiatedPrefab.GetComponent<LevelListHeader>().audioFile = beatmapCMFiles[currentIndex].audio;

            Text textComponent = instantiatedPrefab.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = prefabName;
            }
            else
            {
                Debug.LogWarning("Text component not found in instantiated prefab.");
            }

            numberingName++;
        }
    }
}
#endif