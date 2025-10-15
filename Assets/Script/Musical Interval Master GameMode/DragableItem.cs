using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public MIMKeySO keySO;
    //public List<Transform> noteAnswerParentList; // dapat bertipe 7 note atau 12 note
    //public List<Transform> noteContainerParentList; // dapat bertipe 7 note atau 12 note
    public GameObject answerPlaceholder;
    public GameObject notesPlaceholder;
    public GameObject itemDropperPanel; // Di isi saat prefab di instansiasi
    public Button validateAnswerButton; 

    public Image image; // untuk deteksi rayCast pada slotItem
    //public Image dropFromAnswerSlotImage; // untuk deteksi rayCast saat drop item dari slot Answer ke slot Container 
    [HideInInspector] public Transform parentAfterDrag; // parent milik item setelah di drag
    [HideInInspector] public Transform parentBeforeDrag; // parent milik item sebelum di drag

    bool isShaking = false; // untuk shake animation dotween

    //private void Start()
    //{
    //    Transform root = transform.root.Find("MIM Gameplay Panel");
    //    validateAnswerButton = root.Find("Button_Validate Answer").gameObject.GetComponent<Button>();
    //    //Transform Button = root.transform.Find("Button_Validate Answer").GetComponent<Button>();
    //}

    // Touch Interaction
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnClick");

        parentBeforeDrag = transform.parent; // parent saat ini

        if (parentBeforeDrag.parent.name == "Answer Placeholder") // menbandingkan parent dari parent 
        {
            //Transform placeholder = notesPlaceholder.transform.Find("Notes Placeholder");
            MoveNoteItemOnPressButton(notesPlaceholder.transform);

        }
        else if (parentBeforeDrag.parent.name == "Notes Placeholder")
        {
            // Cek slot kosong di Answer Placeholder
            //var slotParentInAnswerPlaceholder = answerPlaceholder.transform.Find("Answer Placeholder");
            foreach (Transform item in answerPlaceholder.transform)
            {
                if (item.childCount == 0) // Cek slot kosong
                {
                    //Transform placeholder = answerPlaceholder.transform.Find("Answer Placeholder");
                    MoveNoteItemOnPressButton(answerPlaceholder.transform);
                    parentBeforeDrag.gameObject.SetActive(false);
                    parentBeforeDrag.SetAsLastSibling();
                    PlayNoteSfx();

                    break; // kalo ketemu slot yang kosong langsung break
                }
                else
                {
                    Debug.Log("Slot AnswerPlaceholder Penuh");
                    //var originalPos = transform.parent.position;
                    PlayAnimation_Shake(transform.parent);

                }
            }
        }

        // Enable validateAnswerButton button saat slot answer penuh
        ToggleValidateButtonInteractable();

    }

    private void ToggleValidateButtonInteractable()
    {
        foreach (Transform item in answerPlaceholder.transform)
        {
            if (item.childCount == 0) // Cek slot kosong
            {
                validateAnswerButton.interactable = false;
                break; // kalo ketemu slot yang kosong langsung break
            }
            else
            {
                validateAnswerButton.interactable = true;
            }
        }
    }

    private void PlayNoteSfx()
    {
        MIMGameController.guitarToneAudioSource.clip = keySO.keyGuitarSfxClip;
        MIMGameController.guitarToneAudioSource.Play();
    }

    private void PlayAnimation_Shake(Transform transform)
    {
        if (!isShaking)
        {
            isShaking = true;
            transform.DOShakePosition(0.5f, 3.0f).OnComplete(() =>
            {
                //transform.parent.position = originalPos;
                isShaking = false;
            }); // duration, strength
        }
    }

    public void MoveNoteItemOnPressButton(Transform targetPlaceholder)
    {
        Debug.Log("parent name = " + targetPlaceholder.name);
        for (int i = 0; i < targetPlaceholder.childCount; i++)
        {
            Debug.Log("slot name : " + transform.name);
            if (targetPlaceholder.GetChild(i).childCount == 0)
            {
                Debug.Log("Selected slot name : " + targetPlaceholder.GetChild(i).name);
                gameObject.transform.SetParent(targetPlaceholder.GetChild(i));
                targetPlaceholder.GetChild(i).gameObject.SetActive(true);

                break;
            }
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Drag Interaction ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");

        parentBeforeDrag = transform.parent;
        Debug.Log("[DragableItem] parentBeforeDrag name : " + parentBeforeDrag.name);
        //var parentObj = transform.parent.gameObject;
        if (parentBeforeDrag.parent.name == "Answer Placeholder") // menbandingkan parent dari parent 
        {
            itemDropperPanel.SetActive(true);
        }
        else
        {
            itemDropperPanel.SetActive(false);
            PlayNoteSfx();
        }

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Draging");

        // Swap item when dragging
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.root as RectTransform,
            Input.mousePosition,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        transform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");

        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;

        itemDropperPanel.SetActive(false);

        ToggleValidateButtonInteractable();
    }


}
