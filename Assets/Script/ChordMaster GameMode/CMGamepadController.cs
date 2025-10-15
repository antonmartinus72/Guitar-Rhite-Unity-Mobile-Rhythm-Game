using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CMGamepadController : MonoBehaviour/*, IPointerDownHandler, IPointerUpHandler*/
{
    [SerializeField] CMUIManager UIManager;
    [SerializeField] CMNoteDetectorController noteDetector;
    [SerializeField] CMNoteDetectorAntispamController noteDetectorAntispam;
    //[SerializeField] CMStyleController styleManager;
    //[SerializeField] CMScoreManager ScoreManager;
    public noteChord noteSO; // SerializeField & Manually added for debug only. Planning to add from clas MusicNoteLoader
    [SerializeField] int gamepadBtnIndex;
    private BoxCollider2D antiSpamCollider;

    [SerializeField] GamepadSideType gamepadSideType;
    //private bool isPointerEnter = false;

    enum GamepadSideType
    {
        left,
        right
    }

    private void Awake()
    {
        FindIndexOfGameObject();

        //if (gamepadSideType == GamepadSideType.left)
        //{
        //    Debug.Log("Index L using FindIndex : " + gamepadBtnIndex);
        //}
        //else
        //{
        //    Debug.Log("Index R using FindIndex : " + gamepadBtnIndex);
        //}

        antiSpamCollider = noteDetectorAntispam.GetComponent<BoxCollider2D>();
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
    }

    private void FindIndexOfGameObject()
    {
        var leftDatas = UIManager.gamepadLeftData;
        var rightDatas = UIManager.gamepadRightData;
        if (gamepadSideType == GamepadSideType.left)
        {
            gamepadBtnIndex = leftDatas.FindIndex(data => data.keyPadObj == gameObject);
        }
        else
        {
            gamepadBtnIndex = rightDatas.FindIndex(data => data.keyPadObj == gameObject);
        }

        //return -1; // Kembalikan -1 jika GameObject tidak ditemukan & baris tidak akan dieksekusi jika return sebelumnya dilakukan
    }

    public int GetGamepadBtnIndex()
    {
        return gamepadBtnIndex;
    }
    public string GetCurrentNoteKey()
    {
        string key = noteSO.chordKeyValue;
        return key;
    }
    public void UpdateCurrentNoteKey()
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

    //public void TestBtnDownPointerCallback()
    //{
    //    Debug.Log("Pointer Down");
    //}

    public void UpdateCurrentNoteKeyToNull()
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

    public void ActiveNoteAntispamCollider()
    {
        antiSpamCollider.enabled = true;
    }
    public void DeactiveNoteAntispamCollider()
    {
        antiSpamCollider.enabled = false;
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

    // PENTING!! LETAKAN DIBAWAH CMUIController.MatchNoteBlockLeft
    public void DeactivateLeftNoteOnPointerDown() // Aksi jika tombol yang detekan tidak cocok dengan chord note
    {
        var leftNoteScoreObj = noteDetector.collidedLeftNoteScoreObject;

        if (UIManager.isLeftNoteKeyMatch == false && leftNoteScoreObj != null)
        {
            //Debug.Log("Pointer_LEFT DOWN DEACTIVATED");
            DeactivateNoteChild(leftNoteScoreObj);
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

    // PENTING!! LETAKAN DIBAWAH CMUIController.MatchNoteBlockLeft
    public void DeactivateRightNoteOnPointerDown() // Aksi jika tombol yang detekan tidak cocok dengan chord note
    {
        var rightNoteScoreObj = noteDetector.collidedRightNoteScoreObject;

        if (UIManager.isRightNoteKeyMatch == false && rightNoteScoreObj != null)
        {
            //Debug.Log("Pointer_RIGHT DOWN DEACTIVATED");
            DeactivateNoteChild(rightNoteScoreObj);
        }

    }

    public void CheckButtonUPClicked()
    {
        Debug.Log("Button " + gameObject.name + " Triggered (UP)");
    }

    public void CheckButtonDOWNClicked()
    {
        Debug.Log("Button " + gameObject.name + " Triggered (DOWN)");
    }
}
