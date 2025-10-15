using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropperSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] Transform notesPlaceholder;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped on ItemDropper");

        foreach (Transform slot in notesPlaceholder)
        {
            if (slot.childCount == 0)
            {
                eventData.pointerDrag.transform.SetParent(slot);
                slot.gameObject.SetActive(true);
                //Debug.Log("Index kosong = " + slot.indexO);

                GameObject dropped = eventData.pointerDrag;
                DragableItem draggableItem = dropped.GetComponent<DragableItem>();
                draggableItem.parentAfterDrag = slot;
                break;
            }

        }

    }
}
