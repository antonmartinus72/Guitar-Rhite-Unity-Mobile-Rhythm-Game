using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UI_SwipeController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public ScrollRect scrollView;
    public int pageCount = 3; // Jumlah halaman di "Scroll View"
    public float swipeThreshold = 0.1f; // Ambang batas swipe (dalam persentase lebar halaman)
    public float snapDuration = 0.5f; // Durasi snapping

    private int currentPage = 0; // Halaman saat ini
    private float targetScrollPosition = 0f; // Posisi scroll target

    void Start()
    {
        // Menghitung lebar setiap halaman
        float pageWidth = 1f / pageCount;

        // Menghitung lebar threshold swipe
        swipeThreshold *= pageWidth;

        // Mengatur ukuran konten "Scroll View" sesuai dengan jumlah halaman
        scrollView.content.sizeDelta = new Vector2(pageCount, 1f);

        // Mengatur snap pada halaman awal
        SnapToCurrent();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Menghitung perbedaan posisi saat drag
        float delta = eventData.delta.x / Screen.width;

        // Menambahkan perbedaan posisi ke target scroll position
        targetScrollPosition += delta;

        // Membatasi target scroll position
        targetScrollPosition = Mathf.Clamp01(targetScrollPosition);

        // Mengatur scroll position "Scroll View"
        scrollView.horizontalNormalizedPosition = targetScrollPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Menghitung halaman yang dituju berdasarkan target scroll position
        int targetPage = Mathf.RoundToInt(targetScrollPosition * (pageCount - 1));

        // Menghitung jarak antara target halaman dan halaman saat ini
        float deltaPage = Mathf.Abs(targetPage - currentPage);

        // Jika jarak melebihi threshold, maka pindah halaman
        if (deltaPage > swipeThreshold * (pageCount - 1))
        {
            currentPage = targetPage;
        }

        // Menghitung target scroll position berdasarkan halaman saat ini
        targetScrollPosition = (float)currentPage / (pageCount - 1);

        // Melakukan snapping ke halaman yang dipilih
        SnapToCurrent();
    }

    void SnapToCurrent()
    {
        // Menggunakan DOTween untuk melakukan snapping
        scrollView.DOHorizontalNormalizedPos(targetScrollPosition, snapDuration);
    }
}
