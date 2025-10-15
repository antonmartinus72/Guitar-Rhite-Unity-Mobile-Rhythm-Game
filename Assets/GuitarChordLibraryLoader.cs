using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GuitarChordLibraryLoader : MonoBehaviour
{
    /// <summary>
    /// Chord Field = Bilah papan chord secara utuh
    /// chordKeyContent = 
    /// </summary>

    [Header("References")]
    [SerializeField] UISwipeController_ChordLibrary swipeController;
    [SerializeField] Transform chordKeyContent; // tempat menampung key button prefab yang di instansiasi
    [SerializeField] Transform chordSuffixContent; // tempat menampung key suffix button prefab yang di instansiasi
    [SerializeField] RectTransform chordSuffixSelectorContainer; // panel key suffix
    [SerializeField] Transform chordKeyFieldContent; // tempat menampung key suffix button prefab yang di instansiasi

    [SerializeField] TextAsset fileDatabaseJSON;
    public GuitarData guitarDatas;
    [SerializeField] GameObject chordKeyPrefab;
    [SerializeField] GameObject chordKeySuffixPrefab;
    [SerializeField] GameObject chordKeyFieldPrefab;


    [SerializeField] float animateContainerTweenDuration;

    Vector3 chordSuffixSelectorContaineriOriginalPos;


    // Start is called before the first frame update
    void Start()
    {
        SerializeJson();
        InstantiateKeyPrefab();
        InstantiateChordKeyFieldPrefab("C", "major");
        swipeController.UpdateContent(4);

        //sembuyikan chordSuffixSelectorContainer di luar panel
        chordSuffixSelectorContaineriOriginalPos = chordSuffixSelectorContainer.anchoredPosition;
        Vector3 pos = new Vector3(-730f, 0, 0);
        chordSuffixSelectorContainer.anchoredPosition = pos;
    }

    private void InstantiateKeyPrefab()
    {
        DestroyChild(chordKeyContent); // clear content
        foreach (var key in guitarDatas.keys)
        {
            string prefabName = "Key " + key;

            // Instansiate, rename & setparent prefab
            GameObject instantiatedPrefab = Instantiate(chordKeyPrefab, transform.position, transform.rotation);
            instantiatedPrefab.name = prefabName;
            instantiatedPrefab.transform.SetParent(chordKeyContent); // Ubah instantiatedPrefab menjadi child dari object ini

            //Atur ukuran prefab yang di instansiate
            RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1f, 1f, 0f);

            //Ganti button text
            TextMeshProUGUI child = instantiatedPrefab.GetComponentInChildren<TextMeshProUGUI>();
            child.text = key;

            // Assing method ke Button Onclick() untuk instansiate KeySuffixes
            Button button = instantiatedPrefab.GetComponent<Button>();
            button.onClick.AddListener(() => InstantiateChordKeySuffixes(key));
            //button.onClick.AddListener(() => InstantiateChordKeySuffixes(guitarDatas.keys.IndexOf(key)));
        }

        // Instansiasi default chord field
        
    }

    private void InstantiateChordKeySuffixes(string keyString)
    {
        DestroyChild(chordSuffixContent);
        AnimateContainer();

        //Instansiasi default chord field saat chord key menu di click

        switch (keyString)
        {
            case "C#":
                keyString = "Csharp";
                break;
            case "F#":
                keyString = "Fsharp";
                break;
        }

        //InstantiateChordKeyFieldPrefab(keyString, "major");

        if (guitarDatas.chords.TryGetValue(keyString, out List<Chord> chordsList))
        {
            foreach (Chord chord in chordsList)
            {
                string prefabName = "KeySuffix " + chord.suffix;

                // Instansiate, rename & setparent prefab
                GameObject instantiatedPrefab = Instantiate(chordKeySuffixPrefab, transform.position, transform.rotation);
                instantiatedPrefab.name = prefabName;
                instantiatedPrefab.transform.SetParent(chordSuffixContent); // Ubah instantiatedPrefab menjadi child dari object ini

                //Atur ukuran prefab yang di instansiate
                RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1f, 1f, 0f);

                //Ganti button text
                TextMeshProUGUI child = instantiatedPrefab.GetComponentInChildren<TextMeshProUGUI>();
                child.text = chord.suffix;

                //Assing method ke Button Onclick() untuk instansiate KeyFieldPrefab
                Button button = instantiatedPrefab.GetComponent<Button>();
                button.onClick.AddListener(() => InstantiateChordKeyFieldPrefab(keyString, chord.suffix));

            }
        }
        else
        {
            Debug.LogError("Chord dengan key " + keyString + " tidak ditemukan.");
        }
    }

    private void AnimateContainer()
    {
        chordSuffixSelectorContainer.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f; // atur scroll agar ke atas

        Vector3 pos = new Vector3(-730f, 0, 0);
        chordSuffixSelectorContainer.anchoredPosition = pos;

        chordSuffixSelectorContainer.DOLocalMoveX(chordSuffixSelectorContaineriOriginalPos.x, animateContainerTweenDuration).SetEase(Ease.InCubic);

    }

    private void InstantiateChordKeyFieldPrefab(string keyString, string keyStringSuffix)
    {
        DestroyChild(chordKeyFieldContent); // clear content

        if (guitarDatas.chords.TryGetValue(keyString, out List<Chord> chordsList)) // masuk ke chord dictionary berdasarkan 'keyString' (C,Csharp,D,Eb etc.)
        {
            foreach (Chord chordKey in chordsList)
            {
                if (chordKey.suffix == keyStringSuffix) // masuk ke chord dictionary berdasarkan 'keyStringSuffix' (major, minor, dim etc. )
                {
                    swipeController.UpdateContent(chordKey.positions.Count);
                    foreach (var chordPosition in chordKey.positions)
                    {
                        string prefabName = keyString+ " " + keyStringSuffix; // rename prefab, contoh output : "C Major"

                        // Instansiate, rename & setparent prefab
                        GameObject instantiatedPrefab = Instantiate(chordKeyFieldPrefab, transform.position, transform.rotation);
                        instantiatedPrefab.name = prefabName;
                        instantiatedPrefab.transform.SetParent(chordKeyFieldContent); // Ubah instantiatedPrefab menjadi child dari object ini

                        //Atur ukuran prefab yang di instansiate
                        RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
                        rectTransform.localScale = new Vector3(1.32f, 1.32f, 1.32f);

                        GuitarChordLibrary_ChordKeyField prefabChordFieldObjects = instantiatedPrefab.GetComponent<GuitarChordLibrary_ChordKeyField>();
                        prefabChordFieldObjects.versionCaption.text = "Versi " + (chordKey.positions.IndexOf(chordPosition) + 1).ToString(); // rename berdasarkan index chordPosition
                        prefabChordFieldObjects.baseFretCaption.text = "Fret " + chordPosition.baseFret.ToString();


                        // aktif/nonaktif baseFret separator
                        if (chordPosition.baseFret == 1)
                        {
                            prefabChordFieldObjects.neckSeparator.SetActive(true);
                        }
                        else
                        {
                            prefabChordFieldObjects.neckSeparator.SetActive(false);
                        }

                        // aktif/nonaktif capo
                        if (chordPosition.capo.HasValue) 
                        {
                            prefabChordFieldObjects.capo.SetActive(true);
                        }
                        else
                        {
                            prefabChordFieldObjects.capo.SetActive(false);
                        }

                        // Mengatur finger bullet pada chord field
                        for (int i = 0; i < chordPosition.frets.Count; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    {
                                        var currentFrets = prefabChordFieldObjects.frets_0;
                                        var currentFretString = prefabChordFieldObjects.frets_min1[0];
                                        InitializeChordKeyField(chordPosition, i, currentFrets, currentFretString);
                                        break;
                                    }

                                case 1:
                                    {
                                        var currentFrets = prefabChordFieldObjects.frets_1;
                                        var currentFretString = prefabChordFieldObjects.frets_min1[1];
                                        InitializeChordKeyField(chordPosition, i, currentFrets, currentFretString);
                                        break;
                                    }

                                case 2:
                                    {
                                        var currentFrets = prefabChordFieldObjects.frets_2;
                                        var currentFretString = prefabChordFieldObjects.frets_min1[2];
                                        InitializeChordKeyField(chordPosition, i, currentFrets, currentFretString);
                                        break;
                                    }

                                case 3:
                                    {
                                        var currentFrets = prefabChordFieldObjects.frets_3;
                                        var currentFretString = prefabChordFieldObjects.frets_min1[3];
                                        InitializeChordKeyField(chordPosition, i, currentFrets, currentFretString);
                                        break;
                                    }

                                case 4:
                                    {
                                        var currentFrets = prefabChordFieldObjects.frets_4;
                                        var currentFretString = prefabChordFieldObjects.frets_min1[4];
                                        InitializeChordKeyField(chordPosition, i, currentFrets, currentFretString);
                                        break;
                                    }

                                case 5:
                                    {
                                        var currentFrets = prefabChordFieldObjects.frets_5;
                                        var currentFretString = prefabChordFieldObjects.frets_min1[5];
                                        InitializeChordKeyField(chordPosition, i, currentFrets, currentFretString);
                                        break;
                                    }
                            }

                        }

                    }

                }

            }
        }
        else
        {
            Debug.LogError("Chord dengan key " + keyString + " tidak ditemukan.");
        }
    }

    private void InitializeChordKeyField(Position chordPosition, int i, List<GameObject> currentFrets, GameObject currentFretString)
    {
        for (int j = 0; j < currentFrets.Count; j++)
        {
            //Debug.Log("__childObj : " + j);
            if (currentFrets[j].name == chordPosition.frets[i].ToString() && chordPosition.frets[i] != -1)
            {
                //currentFrets[j].GetComponent<Image>().color = Color.green; // finger bullet color

                // finger bullet text
                string text = string.Empty;
                for (int k = 0; k < chordPosition.fingers.Count; k++)
                {
                    if (k == i)
                    {
                        text = chordPosition.fingers[k].ToString();
                    }
                }
                currentFrets[j].GetComponentInChildren<TextMeshProUGUI>().text = text;
                
                // Disable finger bullet image component if capo enabled (Finger bullet artinya tidak ditekan pada senar/string namun tetap dibunyikan)
                if (chordPosition.capo.HasValue && j == 0) // j adalah index/child pertama dari frets_min1
                {
                    currentFrets[j].GetComponent<Image>().enabled = false;
                }
            }
            else
            {
                currentFrets[j].GetComponent<Image>().enabled = false;
                currentFrets[j].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            }

            // Ubah warna string gitar jika nilai fret[i] == -1. (menandakan senar/string tidak di bunyikan)
            if (chordPosition.frets[i] == -1)
            {
                currentFretString.GetComponent<Image>().color = Color.red;
                Debug.Log("chord pos" + chordPosition.frets[i]);
            }
        }
    }

    private void DestroyChild(Transform objTransform)
    {
        foreach (Transform child in objTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SerializeJson()
    {
        if (fileDatabaseJSON != null)
        {
            guitarDatas = JsonConvert.DeserializeObject<GuitarData>(fileDatabaseJSON.text);
            Debug.Log("Guitar Name: " + guitarDatas.main.name);
            Debug.Log("Number of Chords: " + guitarDatas.main.numberOfChords);
            Debug.Log("Standard Tuning: " + string.Join(", ", guitarDatas.tunings.standard));
        }
        else
        {
            Debug.LogError("JSON file not assigned.");
        }
    }

}


// Class untuk file json
[Serializable]
public class GuitarData
{
    public Main main;
    public Tunings tunings;
    public List<string> keys;
    public List<string> suffixes;
    public Dictionary<string, List<Chord>> chords;
}

[Serializable]
public class Main
{
    public int strings;
    public int fretsOnChord;
    public string name;
    public int numberOfChords;
}

[Serializable]
public class Tunings
{
    public List<string> standard;
}

[Serializable]
public class Chord
{
    public string key;
    public string suffix;
    public List<Position> positions;
}

[Serializable]
public class Position
{
    public List<int> frets;
    public List<int> fingers;
    public int baseFret;
    public List<int> barres;
    public bool? capo;
    public List<int> midi;
}