using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class NoteSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            Debug.Log("Droppin item to Answer Placeholder");

            GameObject dropped = eventData.pointerDrag;
            DragableItem draggableItem = dropped.GetComponent<DragableItem>();
            Debug.Log("parentBeforeDrag name : " + draggableItem.parentBeforeDrag.gameObject.name);
            draggableItem.parentAfterDrag = transform;

            // cek jika item berada pada "Notes Placeholder" sebelum dipindahkan
            if (draggableItem.parentBeforeDrag.parent.name == "Notes Placeholder")
            {
                // Nonaktifkan slot jika parent item saat ini TIDAK SAMA dengan parent tujuan saat di drag
                // Ini untuk mencegah slot dinonaktifkan saat item di drag ke dirinya sendiri
                if (draggableItem.parentAfterDrag != draggableItem.parentBeforeDrag && draggableItem.parentAfterDrag.parent.name != "Notes Placeholder")
                {
                    draggableItem.parentBeforeDrag.gameObject.SetActive(false);
                    draggableItem.parentBeforeDrag.SetAsLastSibling();

                }
            }
        }
        else
        {
            if (eventData.pointerDrag.tag == "note")
            {
                Debug.Log("Droppin item to swap"); // swap dapat antara sesama slot di satu placeholder atau antara placeholder lain

                GameObject dropped = eventData.pointerDrag;
                DragableItem draggableItem = dropped.GetComponent<DragableItem>();

                GameObject current = transform.GetChild(0).gameObject;
                DragableItem currentDraggable = current.GetComponent<DragableItem>();

                currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
                draggableItem.parentAfterDrag = transform;
            }
            else
            {
                Debug.Log("Droppin item cancelled");
            }
            //Debug.Log("Droppin item to swap"); // swap dapat antara sesama slot di satu placeholder atau antara placeholder lain

            //GameObject dropped = eventData.pointerDrag;
            //DragableItem draggableItem = dropped.GetComponent<DragableItem>();

            //GameObject current = transform.GetChild(0).gameObject;
            //DragableItem currentDraggable = current.GetComponent<DragableItem>();

            //currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
            //draggableItem.parentAfterDrag = transform;
        }
    }

}


    //public void OnDrop(PointerEventData eventData)
    //{
    //    if (transform.childCount == 0)
    //    {
    //        Debug.Log("Drag Dropped");
    //        GameObject dropped = eventData.pointerDrag;
    //        DragableItem dragableItem = dropped.GetComponent<DragableItem>();
    //        dragableItem.parentAfterDrag = transform;
    //    }
    //}

