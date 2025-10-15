using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using System;
using TMPro;

public class MIMGameController : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] MIMScoreManager scoreManager;
    [SerializeField] GameObject noteItemPrefab;
    [SerializeField] GameObject noteSlotPrefab;
    public List<Transform> noteAnswerSlotList;
    public List<Transform> noteContainerSlotList;
    [SerializeField] private List<GameObject> answerPlaceholderTypeList;
    //public GameObject selectedAnswerPlaceholderType;
    public Transform answerPlaceholder;
    public Transform notesPlaceholder;
    public List<MIMKeySO> notesToInstantiate;
    [SerializeField] GameObject itemDropperPanel; // untuk di assign ke item prefab
    [SerializeField] TextMeshProUGUI captionTextMeshProUGUI;
    [SerializeField] Button validateAnswerButton;

    [Header("Sequences")]
    public List<int> unansweredSequenceIndexList; // berisi urutan index sequence yang akan di load.
    public int currentSequenceIndex = 0;
    [SerializeField] private List<MIMSequenceData> sequencesList;
    [SerializeField] Vector2 keyPrefabSize = new Vector2(54f, 54f);

    [Header("Audio")]
    public static AudioSource guitarToneAudioSource;

    private void Awake()
    {
        guitarToneAudioSource = gameObject.AddComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        validateAnswerButton.interactable = false; // disable interact button validasi

        InitializeCurrentSelectedSequence();
        InitializeSequenceIndexes(); // Masukan index sequence ke List<unansweredSequenceIndexList> untuk urutan load sequence
        ClearItemInPlaceholderSlots(answerPlaceholder); // Clear item didalam answerPlaceholder's slot Harus dibawah "InitializeCurrentSelectedSequence"
        ClearItemInPlaceholderSlots(notesPlaceholder); // Clear item didalam answerPlaceholder's slot. Harus dibawah "InitializeCurrentSelectedSequence"
        SetActiveSlots(notesPlaceholder);
        ReassignNoteAnswerParentList(answerPlaceholder); // Re-assign noteAnswerParentList
        InstantiateNoteItemPrefab();
    }

    private void InitializeSequenceIndexes()
    {
        unansweredSequenceIndexList = new List<int>();
        // Menambahkan int index sequence pada beatmap ke "sequenceIndexLoadList"
        for (int i = 0; i < sequencesList.Count; i++)
        {
            unansweredSequenceIndexList.Add(i); // masukan semua index yang ada di "sequenceData" saat Start()
        }
    }

    private void InstantiateNoteItemPrefab()
    {
        var keys = sequencesList[currentSequenceIndex].keysData; // Assign answerPlaceholder di dalam item menjadi answerPlaceholder saat ini (yg sudah ditetapkan berdasarkan beatmap)
        var additionalKeys = sequencesList[currentSequenceIndex].additionalKeysData;
        var combinedKeys = keys.Concat(additionalKeys); // Combine "keys" and "additionalKeys"
        var shuffledKeys = combinedKeys.OrderBy(x => System.Guid.NewGuid()).ToList(); // Randomize note sebelum prefab di instansiasi

        // Instansiasi Item Prefab
        for (int i = 0; i < shuffledKeys.Count; i++)
        {
            GameObject noteItem = Instantiate(noteItemPrefab, Vector3.zero, Quaternion.identity);
            noteItem.name = shuffledKeys[i].keySO.keyName; // rename
            //noteItem.transform.SetParent(noteContainerParentList[i]);
            if (i >= notesPlaceholder.childCount)
            {
                Debug.Log("Slot not Found, instantiate new slot");
                GameObject slotPrefab = Instantiate(noteSlotPrefab, Vector3.zero, Quaternion.identity, notesPlaceholder);
                slotPrefab.transform.SetParent(notesPlaceholder);
                slotPrefab.transform.localPosition = new Vector3(slotPrefab.transform.localPosition.x, slotPrefab.transform.localPosition.y, 0f);
                //slotPrefab.GetComponent<RectTransform>() = notesPlaceholder.GetChild(0).GetComponent<RectTransform>();
                noteItem.transform.SetParent(slotPrefab.transform);
                slotPrefab.SetActive(false);
            }
            else
            {
                Debug.Log("Slot Found");
                noteItem.transform.SetParent(notesPlaceholder.GetChild(i));
            }
            noteItem.GetComponent<Image>().sprite = shuffledKeys[i].keySO.keySprite; // assgn note image

            RectTransform rect = noteItem.GetComponent<RectTransform>();
            rect.localScale = new Vector3(1f, 1f, 1f);
            rect.localPosition = Vector3.zero;
            rect.sizeDelta = keyPrefabSize;

            DragableItem dragableItem = noteItem.GetComponent<DragableItem>();
            dragableItem.itemDropperPanel = itemDropperPanel; //assign item drag dropper
            dragableItem.answerPlaceholder = answerPlaceholder.gameObject; // re-assign answerPlaceholder yang dipakai.
            dragableItem.notesPlaceholder = notesPlaceholder.gameObject; // re-assign answerPlaceholder yang dipakai.
            dragableItem.keySO = shuffledKeys[i].keySO; // assign note ScriptableObject ke item
            dragableItem.validateAnswerButton = validateAnswerButton;

            //dragableItem.noteAnswerParentList = noteAnswerParentList;
            //dragableItem.noteContainerParentList = noteContainerParentList;


        }
        
        SetActiveSlots(notesPlaceholder);
    }

    public void LoadNewSequences()
    {
        if (unansweredSequenceIndexList.Count > 0)
        {
            currentSequenceIndex = unansweredSequenceIndexList[0];
            InitializeCurrentSelectedSequence();
            //InitializeCurrentSelectedSequence();
            // Clear item didalam answerPlaceholder's slot 
            //var slotParentInAnswerPlaceholder = answerPlaceholder.Find("Answer Placeholder"); // must after "InitializeCurrentSelectedSequence"
            ClearItemInPlaceholderSlots(answerPlaceholder); // must after "InitializeCurrentSelectedSequence"
            ClearItemInPlaceholderSlots(notesPlaceholder); // must after "InitializeCurrentSelectedSequence"

            ReassignNoteAnswerParentList(answerPlaceholder); // Re-assign noteAnswerParentList
            InstantiateNoteItemPrefab();
            SetActiveSlots(notesPlaceholder);

            //ReassignItemDropper();
        }
        else
        {
            Debug.Log("Last Sequence");
            EndGameAction();
        }
    }

    private void EndGameAction()
    {
        StartCoroutine(WaitOneSecondToOpenGameResultPanel());
    }

    IEnumerator WaitOneSecondToOpenGameResultPanel()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("1 detik telah berlalu");
        scoreManager.ShowGameResultPanel();
    }


    private void DisableAllSlots(Transform parent)
    {
        foreach (Transform slot in parent)
        {
            slot.gameObject.SetActive(false);
        }
    }

    private void EnableAllSlots(Transform parent)
    {
        foreach (Transform slot in parent)
        {
            slot.gameObject.SetActive(false);
        }
    }

    private void SetActiveSlots(Transform parent)
    {
        foreach (Transform slot in parent)
        {
            if (slot.childCount != 0)
            {
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    private void ReassignNoteAnswerParentList(Transform slotParentInAnswerPlaceholder)
    {
        noteAnswerSlotList.Clear();
        foreach (Transform slot in slotParentInAnswerPlaceholder)
        {
            // clear List
            noteAnswerSlotList.Add(slot); // add slot to List
        }
    }

    private void ClearItemInPlaceholderSlots(Transform placeholder)
    {
        foreach (Transform slot in placeholder)
        {
            if (slot.childCount != 0)
            {
                var item = slot.GetChild(0);
                Destroy(item.gameObject);
            }
        }
    }

    private void InitializeCurrentSelectedSequence()
    {
        sequencesList = gameObject.GetComponent<MIMBeatmapLoader>().sequenceData;
        int noteIntervalTypeSelectedIndex = 0; // index dari List "noteIntervalType" di inspector
        string newCaptionText = string.Empty;

        // Nonaktifkan semua panel pada answerPlaceholderType
        foreach (var item in answerPlaceholderTypeList)
        {
            item.SetActive(false);
        }

        // Pilih tipe placeholder berdasarkan sequence di beatmap
        switch (sequencesList[currentSequenceIndex].intervalType)
        {
            case MIMSequenceSO.noteIntervalType.diatonis_mayor:
                noteIntervalTypeSelectedIndex = 0; // index diatonis mayor, pastikan urutan list (noteIntervalType) di inspector benar
                newCaptionText = string.Empty;
                newCaptionText = "1 = " + sequencesList[currentSequenceIndex].rootKey + " (Interval/Tangga Nada Diatonis Mayor)";
                Debug.LogWarning("[WARNING] Answer Placeholder diganti ke '" + answerPlaceholder.name + "'");
                break;
            case MIMSequenceSO.noteIntervalType.diatonis_minor:
                noteIntervalTypeSelectedIndex = 1; // index diatonis minor, pastikan urutan list (noteIntervalType) di inspector benar
                newCaptionText = string.Empty;
                newCaptionText = "1 = " + sequencesList[currentSequenceIndex].rootKey + " (Interval/Tangga Nada Diatonis Minor)";
                Debug.LogWarning("[WARNING] Answer Placeholder diganti ke '" + answerPlaceholder.name + "'");
                break;
        }

        // Ganti Caption text pada answer placeholder yang terpilih
        captionTextMeshProUGUI = answerPlaceholderTypeList[noteIntervalTypeSelectedIndex].transform.Find("Caption").GetComponent<TextMeshProUGUI>(); // Cari component TextMeshProUGUI
        captionTextMeshProUGUI.text = newCaptionText; // ganti text caption

        // Aktifkan answerPlaceholderTypeList yang terpilih
        answerPlaceholder = answerPlaceholderTypeList[noteIntervalTypeSelectedIndex].transform.Find("Answer Placeholder");
        answerPlaceholderTypeList[noteIntervalTypeSelectedIndex].SetActive(true); // aktifkan panel yang dibutuhkan di answerPlaceholderType.

    }

    public void RestartGameMode()
    {
        GameManager.Instance.sceneController.ReloadCurrentScene();
    }

    public void PauseGame()
    {
        GameManager.Instance.PauseGame();

    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }

    public void GoToMenu()
    {
        GameManager.Instance.ResumeGame();
        GameManager.Instance.sceneController.SwitchSceneMainMenu();
    }
}
