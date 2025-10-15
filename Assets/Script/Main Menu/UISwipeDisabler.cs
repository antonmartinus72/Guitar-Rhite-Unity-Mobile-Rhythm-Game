using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISwipeDisabler : MonoBehaviour
{
    public ScrollRect scrollRect;

    void Start()
    {
        // Menonaktifkan fungsi swipe horizontal
        scrollRect.horizontal = false;

        // Menonaktifkan scrollbar horizontal
        //foreach (Scrollbar scrollbar in GetComponentsInChildren<Scrollbar>())
        //{
        //    if (scrollbar.direction == Scrollbar.Direction.LeftToRight || scrollbar.direction == Scrollbar.Direction.RightToLeft)
        //    {
        //        scrollbar.gameObject.SetActive(false);
        //    }
        //}
    }
}
