using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelListHeader : MonoBehaviour
{
    private Button button;
    public Transform levelScoreRow_ContentContainer; // Initialized by MM_GameLevelManager
    public GameObject levelScoreRow_Prefab; //Initialized by MM_GameLevelManager
    //[SerializeField] private BeatmapLoader beatmapLoaderObj;
    //[SerializeField] private Button playLevelButton;
    public int currentIndex;
    public TextAsset saveFile;
    public AudioClip audioFile;


    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(InstantiateScoreTableParentPrefab);
    }

    private void InstantiateScoreTableParentPrefab()
    {
        ClearList();
        //List<CMSavesData> saveCMData = new List<CMSavesData>(GameManager.Instance.beatmapLoader.savesCMData);
        
        //List<CMSavesList> currentScoreData = GetSortedDataByScoreDescending(saveCMData[currentIndex]);
        InstansiatePrefab(levelScoreRow_Prefab);
    }

    private void ClearList() //  Clear child (row) in levelScoreRow_ContentContainer
    {
        foreach (Transform child in levelScoreRow_ContentContainer)
        {
            // Destroy each child object
            Destroy(child.gameObject);
        }
    }

    private void InstansiatePrefab(GameObject levelScoreRow_Prefab)
    {
        GameManager.Instance.selectedGameLevelIndex = currentIndex; // ganti value di Gamemanager
        List<CMSaveData> saveCMDataCollection = GameManager.Instance.saveManager.LoadScoreCM(currentIndex);

        //saveCMList = SortByScore(saveCMList);

        int numbering = 1;
        foreach (var item in saveCMDataCollection)
        {
            
            string prefabName = numbering.ToString();

            // Instansiate, rename & setparent prefab
            GameObject instantiatedPrefab = Instantiate(levelScoreRow_Prefab, transform.position, transform.rotation);
            instantiatedPrefab.name = prefabName;
            instantiatedPrefab.transform.SetParent(levelScoreRow_ContentContainer); // Ubah instantiatedPrefab menjadi child dari object ini

            //Atur ukuran prefab yang di instansiate
            RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1f, 1f, 0f);


            foreach (Transform child in instantiatedPrefab.transform)
            {
                if (child.name == "Rank Text")
                {
                    // Do something with the child object
                    TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                    textComponent.text = item.performanceRating.ToString();
                }
                else if (child.name == "Score Text")
                {
                    TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                    textComponent.text = item.performanceScore.ToString();
                }
                else if (child.name == "Date Text")
                {
                    TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                    string dateFormatted = convertDateFromJson(item.date.ToString());
                    textComponent.text = dateFormatted;
                }
            }
            numbering++;
        }

    }

    private string convertDateFromJson(string jsonDateString)
    {
        try
        {
            DateTime date = DateTime.ParseExact(jsonDateString.Trim(), "ddMMyy", null);
            string dateFormatted = date.ToString("dd/MM/yyyy");
            return dateFormatted;
        }
        catch (FormatException)
        {
            // Handling the exception if the format is not correct
            return "Invalid date format";
        }
        //Debug.Log(jsonDateString);
        //DateTime date = DateTime.ParseExact(jsonDateString, "ddMMyy", null);
        //string dateFormatted = date.ToString("dd/MM/yyyy");
        //return dateFormatted;
    }

    public List<CMSavesList> GetSortedDataByScoreDescending(CMSavesData saveData)
    {
        Debug.Log("Sorting CM Level Data");
        return saveData.data.OrderByDescending(x => x.score).ToList();
    }

    public List<CMSavesData> SortByScore(List<CMSavesData> saveList)
    {
        // Sorting setiap data dalam beatmapList berdasarkan score
        foreach (CMSavesData data in saveList)
        {
            data.data = data.data.OrderByDescending(item => item.score).ToList();
        }

        // Mengembalikan beatmapList yang telah diurutkan
        var sortedList = saveList.OrderBy(data => {
            if (data.data.Count > 0)
            {
                return data.data[0].score;
            }
            else
            {
                return 0;
            }
        });

        return sortedList.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
