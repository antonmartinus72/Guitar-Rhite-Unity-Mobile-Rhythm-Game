using System;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class CMNoteDetectorController : MonoBehaviour
{
    private int countBeat = 0;
    private float startTime = 0;
    //private AudioClip metronomeClick;
    [SerializeField] GameObject musicManager;
    [SerializeField] CMScoreManager scoreManager;
    private AudioSource metronomeAudioSource;
    private AudioSource musicAudioSource;
    [SerializeField] PauseController pauseController;

    //[Header("Debug")]
    //Timer Ref Debug
    //[SerializeField] private Text timerText; // Debug
    //[SerializeField] private Text counterText; // Debug
    //[SerializeField] private Text captureText; // Debug

    private bool timerIsRunning = false;
    //private bool isMusicPlaying = false;
    private int numberOfTickSound; // number of metronome sound before music start

    // Note Dectecton Ref
    [SerializeField] private Text currentNoteKeyTxtObj; // Debug
    private string currentNoteKey;
    //bool isPrimaryNoteController = false; // true = script akan ngehandle timer & play audio, false = tidak menghandle timer & play audio (hanya butuh 1 audio player pada instance class ini)
    [SerializeField] NoteDetectorType detectorTypeSide;
    enum NoteDetectorType
    {
        left,
        right
    }

    public GameObject collidedLeftNoteObject;
    public GameObject collidedRightNoteObject;    
    public GameObject collidedLeftNoteScoreObject;
    public GameObject collidedRightNoteScoreObject;
    public GameObject collidedAntispamLeftNoteObject;
    public GameObject collidedAntispamRightNoteObject;
    //public BoxCollider2D collidedLeftNoteObject_Collider;
    //public BoxCollider2D collidedRightNoteObject; // NOT IMPLEMENTED YET



    //[SerializeField] CMUIController UIManager;

    //public static event Action<GameObject> OnTriggerStay2DNoteColliderObjEvent;

    public string currentKey
    {
        get { return currentKey; }
    }
    private void Awake()
    {
        metronomeAudioSource = GetComponent<AudioSource>();
        musicAudioSource = musicManager.GetComponent<AudioSource>(); // get music audio from obj 'Music Manager'
    }


    void Start()
    {
        var timeSignature = musicManager.GetComponent<MusicBeatLoader>().timeSignature;
        var noteStartMeasurePos = musicManager.GetComponent<MusicBeatLoader>().noteStartMeasurePos - 1;
        numberOfTickSound = (timeSignature * noteStartMeasurePos);

        //if (detectorTypeSide == NoteDetectorType.left)
        //{
        //    isPrimaryNoteController = true;
        //}
        //else
        //{
        //    isPrimaryNoteController = false;
        //}

        //if (isPrimaryNoteController)
        //{
        //    Debug.Log("isPrimaryNoteController = TRUE");
        //}
        //else
        //{
        //    Debug.Log("isPrimaryNoteController = FALSE");
        //}



    }



    // Update is called once per frame
    void Update()
    {
        //if (isPrimaryNoteController) // check apakah nama gameObject adalah "note_detector_1"
        //{
        //    //Timer();
        //}
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (detectorTypeSide == NoteDetectorType.left) // check apakah nama gameObject adalah "note_detector_1"
        {
            if (collision.name == "MUSIC_PLAY")
            {
                musicAudioSource.Play();
                pauseController.isBGMReadyToPlay = true;
            }

            //if (collision.CompareTag("note"))
            //{
            //    scoreManager.allowScoreLeftBlockToUpdate = true;
            //    Debug.Log("Allowed_Score Left Block to Update ");
            //}
            //else
            //{
            //    scoreManager.allowScoreLeftBlockToUpdate = false;
            //    Debug.Log("Not_Allowed_Score Left Block to Update ");
            //}
        }

        if (collision.gameObject.CompareTag("antispam"))
        {
            if (detectorTypeSide == NoteDetectorType.left)
            {
                collidedAntispamLeftNoteObject = collision.gameObject;
                //Debug.Log("LEFT ANTISPAM ENTER");
            }
            else
            {
                collidedAntispamRightNoteObject = collision.gameObject;
                //Debug.Log("RIGHT ANTISPAM ENTER");
            }
        }

        if (collision.gameObject.CompareTag("noteEnd"))
        {
            if (detectorTypeSide == NoteDetectorType.left)
            {
                collidedLeftNoteScoreObject = null;
                //Debug.Log("LEFT ANTISPAM ENTER");
            }
            if (detectorTypeSide == NoteDetectorType.right)
            {
                collidedRightNoteScoreObject = null;
                //Debug.Log("RIGHT ANTISPAM ENTER");
            }
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleNoteDetectorOnStayCollision(collision);

    }

    

    private void OnTriggerExit2D(Collider2D collision)
    {
        //HandleNoteDetectorOnExitCollision(collision);
        //BeatCounterAndAudioPlayer(collision);

        if (collision.gameObject.CompareTag("note"))
        {
            if (detectorTypeSide == NoteDetectorType.left)
            {
                collidedLeftNoteObject = null;
                collidedLeftNoteScoreObject = null;
                string txt = "NoteKey (Left): ";
                currentNoteKeyTxtObj.text = txt;
            }
            else if (detectorTypeSide == NoteDetectorType.right)
            {
                collidedRightNoteObject = null;
                collidedRightNoteScoreObject = null;
                string txt = "NoteKey (Right): ";
                currentNoteKeyTxtObj.text = txt;
            }
        }

        if (collision.gameObject.CompareTag("antispam"))
        {
            if (detectorTypeSide == NoteDetectorType.left)
            {
                collidedAntispamLeftNoteObject = null;
                ////Debug.Log("LEFT ANTISPAM EXIT");
            }
            else if (detectorTypeSide == NoteDetectorType.right)
            {
                collidedAntispamRightNoteObject = null;
                //Debug.Log("RIGHT ANTISPAM EXIT");
            }
        }

    }

    //private void BeatCounterAndAudioPlayer(Collider2D collision) 
    //{
    //    if (collision.CompareTag("beat") || collision.CompareTag("beatMeasure"))
    //    {

    //        countBeat++;
    //        timerIsRunning = true;
    //        counterText.text = countBeat.ToString();
    //        captureText.text = counterText.text + "||" + timerText.text;

    //        if (metronomeAudioSource != null && numberOfTickSound > 0)
    //        {
    //            metronomeAudioSource.Play();
    //            numberOfTickSound--;
    //        }
    //    }
    //}

   

    private void HandleNoteDetectorOnStayCollision(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("note"))
        {
            if (detectorTypeSide == NoteDetectorType.left)
            {
                collidedLeftNoteObject = collision.gameObject;
                currentNoteKey = collision.GetComponent<CMNoteScript>().key;
                string txt = "NoteKey (Left): ";
                currentNoteKeyTxtObj.text = txt + currentNoteKey;
            }
            else
            {
                collidedRightNoteObject = collision.gameObject;
                currentNoteKey = collision.GetComponent<CMNoteScript>().key;
                string txt = "NoteKey (Right): ";
                currentNoteKeyTxtObj.text = txt + currentNoteKey;
            }
        }

        if (collision.gameObject.CompareTag("scorenote"))
        {
            if (detectorTypeSide == NoteDetectorType.left)
            {
                collidedLeftNoteScoreObject = collision.gameObject.transform.parent.gameObject;//get the parent
            }
            if (detectorTypeSide == NoteDetectorType.right)
            {
                collidedRightNoteScoreObject = collision.gameObject.transform.parent.gameObject;//get the parent
            }
        }

        if (collision.gameObject.CompareTag("antispam"))
        {
            if (detectorTypeSide == NoteDetectorType.left)
            {
                collidedAntispamLeftNoteObject = collision.gameObject;
                //Debug.Log("LEFT ANTISPAM COLLIDED");
            }
            else
            {
                collidedAntispamRightNoteObject = collision.gameObject;
                //Debug.Log("RIGHT ANTISPAM COLLIDED");
            }
        }

    }

    private void HandleNoteDetectorOnExitCollision(Collider2D collision)
    {
        
    }

    public string GetCurrentNoteKey()
    {
        return currentNoteKey;
    }

    //private void Timer() // UNTUK DEBUG
    //{
    //    if (timerIsRunning && timerText.isActiveAndEnabled)
    //    { 
    //        float timeElapsed = Time.time - startTime;
    //        int minutes = (int)(timeElapsed / 60);
    //        int seconds = (int)(timeElapsed % 60);
    //        int milliseconds = (int)((timeElapsed * 1000) % 1000);

    //        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    //    }
        
    //}

}
