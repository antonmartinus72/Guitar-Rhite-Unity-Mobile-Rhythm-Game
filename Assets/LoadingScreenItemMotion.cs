using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LoadingScreenItemMotion : MonoBehaviour
{
    RectTransform image; // Drag the image RectTransform here
    [SerializeField] float moveDistance = 100f; // Distance to move up and down
    [SerializeField] float moveDuration = 2f; // Duration for each move (up or down)

    void OnEnable()
    {
        // Start the up and down movement
        image = gameObject.GetComponent<RectTransform>();
        StartMovingUpDown();
    }

    void StartMovingUpDown()
    {
        // Move the image up
        image.DOAnchorPosY(image.anchoredPosition.y + moveDistance, moveDuration)
            .SetEase(Ease.InOutSine) // Smoother easing in and out
            .SetLoops(-1, LoopType.Yoyo); // Loop indefinitely with Yoyo (up and down)
    }
}
