using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CMKeypadPointerHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler

{
    public static bool isAnyButtonPressedLeftGamepad = false;  // Flag untuk mengecek apakah ada tombol yang sedang ditekan
    public static bool isAnyButtonPressedRightGamepad = false;  // Flag untuk mengecek apakah ada tombol yang sedang ditekan

    [SerializeField] CMUIManager UIManager;
    [SerializeField] int gamepadBtnIndex;
    [SerializeField] CMNoteDetectorController noteDetector;
    [SerializeField] CMNoteDetectorAntispamController noteDetectorAntispam;
    private BoxCollider2D antiSpamCollider;


    public noteChord noteSO; // SerializeField & Manually added for debug only. Planning to add from clas MusicNoteLoader
    [SerializeField] GamepadSideType gamepadSideType;
    enum GamepadSideType
    {
        left,
        right
    }

    private void Awake()
    {
        antiSpamCollider = noteDetectorAntispam.GetComponent<BoxCollider2D>();
        //UIManager = gameObject.transform.root.Find("UI Manager").gameObject.GetComponent<CMUIManager>();
    }
    private void Start()
    {
        if (gamepadSideType == GamepadSideType.left)
        {
            gameObject.name = "L" + gamepadBtnIndex.ToString() + ("(" + noteSO.chordKeyValue + " Key)");
            gameObject.transform.GetComponentInChildren<TextMeshProUGUI>().text = noteSO.chordKeyValue;
        }
        else
        {
            gameObject.name = "R" + gamepadBtnIndex.ToString() + ("(" + noteSO.chordKeyValue + " Key)");
            gameObject.transform.GetComponentInChildren<TextMeshProUGUI>().text = noteSO.chordKeyValue;
        }

        UpdateCurrentNoteKey();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateCurrentNoteKey();

        if (gamepadSideType == GamepadSideType.left)
        {
            if (!isAnyButtonPressedLeftGamepad) // Set flag saat tombol ditekan
            {
                isAnyButtonPressedLeftGamepad = true;
                Debug.Log("Pointer Down on: " + gameObject.name);
                // Tambahkan logika apa yang terjadi saat tombol ditekan
            }
            else
            {
                Debug.Log("Another button [LEFT] is already pressed.");
            }

            UIManager.ActivateNoteBlockStyleLeftAccentPressIndicator();
            UIManager.MatchLeftGamepadKey(gamepadBtnIndex);
            DeactivateLeftNoteOnPointerDown();
        }
        else
        {
            if (!isAnyButtonPressedRightGamepad) // Set flag saat tombol ditekan
            {
                isAnyButtonPressedRightGamepad = true;
                Debug.Log("Pointer Down on: " + gameObject.name);
                // Tambahkan logika apa yang terjadi saat tombol ditekan
            }
            else
            {
                Debug.Log("Another button [RIGHT] is already pressed.");
            }

            UIManager.ActivateNoteBlockRightAccentPressIndicator();
            UIManager.MatchRightGamepadKey(gamepadBtnIndex);
            DeactivateRightNoteOnPointerDown();
        }

        ActiveNoteAntispamCollider();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UpdateCurrentNoteKeyToNull();

        if (gamepadSideType == GamepadSideType.left)
        {
            if (isAnyButtonPressedLeftGamepad) // Reset flag saat tombol dilepas
            {
                isAnyButtonPressedLeftGamepad = false;
                Debug.Log("Pointer Up on: " + gameObject.name);
                // Tambahkan logika apa yang terjadi saat tombol dilepas
            }

            UIManager.DectivateNoteBlockStyleLeftAccentPressIndicator();
            UIManager.UnmatchLeftGamepadKeyOnPointerUp();
            DeactivateLeftNoteOnPointerUp();
        }
        else
        {
            if (isAnyButtonPressedRightGamepad) // Reset flag saat tombol dilepas
            {
                isAnyButtonPressedRightGamepad = false;
                Debug.Log("Pointer Up on: " + gameObject.name);
                // Tambahkan logika apa yang terjadi saat tombol dilepas
            }

            UIManager.DectivateNoteBlockRightAccentPressIndicator();
            UIManager.UnmatchRightGamepadKeyOnPointerUp();
            DeactivateRightNoteOnPointerUp();
        }

        DeactiveNoteAntispamCollider();
    }


    // ================================================================== METHOD POINTER DOWN ================================================================================
    public int GetGamepadBtnIndex()
    {
        return gamepadBtnIndex;
    }

    private void UpdateCurrentNoteKey()
    {
        if (gamepadSideType == GamepadSideType.left)
        {
            UIManager.SetLeftPadKey(noteSO.chordKeyValue, gamepadBtnIndex);
        }
        else
        {
            UIManager.SetRightPadKey(noteSO.chordKeyValue, gamepadBtnIndex);
        }
    }

    private void DeactivateLeftNoteOnPointerDown() // Aksi jika tombol yang detekan tidak cocok dengan chord note
    {
        var leftNoteScoreObj = noteDetector.collidedLeftNoteScoreObject;

        if (UIManager.isLeftNoteKeyMatch == false && leftNoteScoreObj != null)
        {
            //Debug.Log("Pointer_LEFT DOWN DEACTIVATED");
            DeactivateNoteChild(leftNoteScoreObj);
        }
    }
    public void DeactivateRightNoteOnPointerDown() // Aksi jika tombol yang detekan tidak cocok dengan chord note
    {
        var rightNoteScoreObj = noteDetector.collidedRightNoteScoreObject;

        if (UIManager.isRightNoteKeyMatch == false && rightNoteScoreObj != null)
        {
            //Debug.Log("Pointer_RIGHT DOWN DEACTIVATED");
            DeactivateNoteChild(rightNoteScoreObj);
        }

    }

    private void DeactivateNoteChild(GameObject noteCollidedObj)
    {
        var antispamObj = noteCollidedObj.GetComponent<CMNoteScript>().antispam;
        var startObj = noteCollidedObj.GetComponent<CMNoteScript>().start;
        var startContentObj = noteCollidedObj.GetComponent<CMNoteScript>().startcontent;
        var endObj = noteCollidedObj.GetComponent<CMNoteScript>().end;
        var fillObj = noteCollidedObj.GetComponent<CMNoteScript>().fill;

        antispamObj.SetActive(false);
        startObj.SetActive(false);
        startContentObj.SetActive(false);
        endObj.SetActive(false);
        fillObj.SetActive(false);
    }

    private void ActiveNoteAntispamCollider()
    {
        antiSpamCollider.enabled = true;
    }

    // ================================================================== METHOD POINTER UP ================================================================================
    private void UpdateCurrentNoteKeyToNull()
    {
        if (gamepadSideType == GamepadSideType.left)
        {
            UIManager.SetLeftPadKey("null", gamepadBtnIndex); // null is value in beatmap
        }
        else
        {
            UIManager.SetRightPadKey("null", gamepadBtnIndex); // null is value in beatmap
        }
    }

    public void DeactivateLeftNoteOnPointerUp()
    {
        var leftNoteObj = noteDetector.collidedLeftNoteObject; // digunakan untuk update "total score"
        var leftNoteScoreObj = noteDetector.collidedLeftNoteScoreObject; // Aksi jika tombol yang detekan cocok dengan chord note (digunakan untuk update "score")

        if (leftNoteObj != null && leftNoteScoreObj != null)
        {
            //Debug.Log("Pointer_LEFT EXIT DEACTIVATED");
            DeactivateNoteChild(leftNoteScoreObj);
        }
    }

    public void DeactivateRightNoteOnPointerUp()
    {
        var rightNoteObj = noteDetector.collidedRightNoteObject; // digunakan untuk update "total score"
        var rightNoteScoreObj = noteDetector.collidedRightNoteScoreObject; // Aksi jika tombol yang detekan cocok dengan chord note (digunakan untuk update "score")

        if (rightNoteObj != null && rightNoteScoreObj != null)
        {
            //Debug.Log("Pointer_RIGHT EXIT DEACTIVATED");
            DeactivateNoteChild(rightNoteScoreObj);
        }

    }

    public void DeactiveNoteAntispamCollider()
    {
        antiSpamCollider.enabled = false;
    }
}
