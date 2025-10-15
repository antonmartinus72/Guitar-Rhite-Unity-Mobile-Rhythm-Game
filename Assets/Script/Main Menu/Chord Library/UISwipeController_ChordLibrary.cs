using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISwipeController_ChordLibrary : MonoBehaviour
{
    [SerializeField] GameObject contentMaxPage;
    [SerializeField] private int maxPage;
    public int currentPage;
    [SerializeField] Vector3 targetPos;
    Vector3 defaulTargetPos; // di isi setelah instansiasi 'targetPos'
    //[SerializeField] Vector3 targetPos_FirstObj;

    [SerializeField] Vector3 pageStep; // nilai posX diambil dari perhitungan : childObjct2.posX - childObject1.posX (konversi ke negatif)
    [SerializeField] RectTransform levelPageRect;

    [SerializeField] float tweenTime;
    [SerializeField] Ease easeType;

    float dragTreshhold;

    [SerializeField] GameObject nextPage;
    [SerializeField] GameObject prevPage;



    // Start is called before the first frame update

    private void OnEnable()
    {
        UpdateContent(maxPage);
    }
    private void Start()
    {
        maxPage = contentMaxPage.transform.childCount;

        currentPage = 1;
        targetPos = levelPageRect.localPosition;
        defaulTargetPos = targetPos;

        dragTreshhold = Screen.width / 10;

        DisableButton(); // Disable tombol Next/Prev

        //targetPos_FirstObj = targetPos;
    }

    public void UpdateContent(int totalPage)
    {
        maxPage = totalPage;

        currentPage = 1;
        targetPos = defaulTargetPos;

        RectTransform rect = contentMaxPage.GetComponent<RectTransform>();
        Vector3 newContentMaxPagePos = new Vector2(0f, rect.anchoredPosition.y);
        rect.anchoredPosition = newContentMaxPagePos;

        nextPage.GetComponent<Button>().interactable = true;
        prevPage.GetComponent<Button>().interactable = true;

        DisableButton(); // Disable tombol Next/Prev

    }
    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }

    public void MovePage()
    {
        levelPageRect.DOLocalMove(targetPos, tweenTime).SetEase(easeType);
        Debug.Log("Current Page : " + currentPage);
        DisableButton();
    }

    public void DisableButton()
    {

        nextPage.GetComponent<Button>().interactable = true;
        prevPage.GetComponent<Button>().interactable = true;

        // Nonaktifkan tombol Next jika di halaman terakhir
        if (currentPage >= maxPage)
        {
            nextPage.GetComponent<Button>().interactable = false;
        }

        // Nonaktifkan tombol Previous jika di halaman pertama
        if (currentPage <= 1)
        {
            prevPage.GetComponent<Button>().interactable = false;
        }

        //if (currentPage == maxPage)
        //{
        //    nextPage.GetComponent<Button>().interactable = false;
        //}
        //else if (currentPage == 1)
        //{
        //    prevPage.GetComponent<Button>().interactable = false;
        //}
        //else
        //{
        //    nextPage.GetComponent<Button>().interactable = true;
        //    prevPage.GetComponent<Button>().interactable = true;
        //}

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragTreshhold)
        {
            if (eventData.position.x > eventData.pressPosition.x)
            {
                Previous();
            }
            else
            {
                Next();
            }
        }
        else
        {
            MovePage();
        }
    }
}
