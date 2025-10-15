using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BMBeatController : MonoBehaviour
{
    //[SerializeField] BMBeatManager bmBeatManager;
    //public int totalHitNote = 0;

    [SerializeField] TextMeshProUGUI scoreText;
    public int score = 0;
    [SerializeField] bool noteCollide = false;
    [SerializeField] bool hasPressed = false;
    Collider2D currentCollision = null; // Menyimpan referensi collider


    [SerializeField] AudioClip playerMissInputAudio;
    [SerializeField] AudioClip playerSuccessInputAudio;
    [SerializeField] AudioClip listenModeIndicatorAudio;


    AudioSource playerMissInputAudioSource;
    AudioSource playerSuccessInputAudioSource;
    AudioSource listenModeAudioSource;
    [SerializeField] Animator avatarAnimator;
    [SerializeField] Animator signAnimator;

    public enum GameState
    {
        Go,
        CaptureMode,
        ListenMode,
        TransitionMode
    }

    public GameState currentState;

    private void Start()
    {

        SetState(GameState.Go);

        if (scoreText == null)
        {
            Debug.LogError("ScoreTextDebug is not assigned in the Inspector.");
            return;
        }
        scoreText.text = score.ToString();

        playerMissInputAudioSource = gameObject.AddComponent<AudioSource>();
        playerSuccessInputAudioSource = gameObject.AddComponent<AudioSource>();
        listenModeAudioSource = gameObject.AddComponent<AudioSource>();
        playerMissInputAudioSource.clip = playerMissInputAudio;
        playerSuccessInputAudioSource.clip = playerSuccessInputAudio;
        listenModeAudioSource.clip = listenModeIndicatorAudio;
        //listenModeAudioSource.pitch = 1f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("note"))
        {
            currentCollision = collision; // Menyimpan referensi collider saat tabrakan
        }
    }

    private void IncrementScore()
    {
        score++;
        scoreText.text = score.ToString();
        Debug.Log("[SCORE UP]");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("note") && currentState == GameState.ListenMode)
        {
            currentCollision = collision; // Menyimpan referensi collider saat tabrakan
            listenModeAudioSource.Play();
            Debug.Log("[ENTER][LISTEN] Collided with note : " + collision.name);
        }
        else if (collision.CompareTag("note") && currentState == GameState.CaptureMode)
        {
            currentCollision = collision; // Menyimpan referensi collider saat tabrakan
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("note") && collision == currentCollision)
        {
            currentCollision = null; // Menghapus referensi collider saat tabrakan berakhir
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && currentState == GameState.CaptureMode && !hasPressed)
            {
                if (currentCollision != null)
                {
                    HandleInput(currentCollision);
                }
                else
                {
                    HandleMiss();
                }
                hasPressed = true;
            }
        }
        else if (Input.GetMouseButtonDown(0) && currentState == GameState.CaptureMode && !hasPressed)
        {
            if (currentCollision != null)
            {
                HandleInput(currentCollision);
            }
            else
            {
                HandleMiss();
            }
            hasPressed = true;
        }
        else
        {
            hasPressed = false;
        }
    }

    private void HandleInput(Collider2D collision)
    {
        // Menghindari play audio saat pause
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Paused)
        {
            return;
        }

        playerSuccessInputAudioSource.Play();
        IncrementScore();
        PlayAvatarAnimation_HitSuccess();
        collision.gameObject.SetActive(false); // Menonaktifkan objek collider
        currentCollision = null; // Menghapus referensi collider setelah diproses
    }
    private void HandleMiss()
    {
        // Menghindari play audio saat pause
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Paused)
        {
            return;
        }

        //playerInputAudioSource.pitch = 1f;
        playerMissInputAudioSource.Play();
        PlayAvatarAnimation_HitMiss();
    }

    // Animasi Avatar
    private void PlayAvatarAnimation_HitMiss()
    {
        avatarAnimator.SetBool("IdleNeutral", false);
        avatarAnimator.SetBool("IdleMiss", true);
        avatarAnimator.SetTrigger("HitMiss");
    }

    private void PlayAvatarAnimation_HitSuccess()
    {
        avatarAnimator.SetBool("IdleNeutral", true);
        avatarAnimator.SetBool("IdleMiss", false);
        avatarAnimator.SetTrigger("Hit");
    }

    public void PlayAvatarAnimation_Idle()
    {
        avatarAnimator.SetBool("IdleNeutral", true);
        avatarAnimator.SetBool("IdleMiss", false);
    }

    private void PlayAvatarAnimation_IdleMiss()
    {
        avatarAnimator.SetBool("IdleNeutral", false);
        avatarAnimator.SetBool("IdleMiss", true);
    }

    // Animasi Sign
    private void PlaySignAnimation_Ready()
    {
        signAnimator.SetBool("Ready", true);
        signAnimator.SetBool("Listen", false);
        signAnimator.SetBool("Capture", false);
    }

    private void PlaySignAnimation_Go()
    {
        signAnimator.SetTrigger("Go");
    }

    private void PlaySignAnimation_Listen()
    {
        signAnimator.SetBool("Ready", false);
        signAnimator.SetBool("Listen", true);
        signAnimator.SetBool("Capture", false);
    }

    private void PlaySignAnimation_Capture()
    {
        signAnimator.SetBool("Ready", false);
        signAnimator.SetBool("Listen", false);
        signAnimator.SetBool("Capture", true);
    }

    // State
    public void SetState(GameState newState)
    {
        if (currentState != newState)
        {
            ExitState(currentState);
            currentState = newState;
            EnterState(currentState);
            Debug.Log("Current State = " + currentState);
        }
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Go:
                Debug.Log("Entering Go Mode State");
                PlaySignAnimation_Go();
                break;
            case GameState.CaptureMode:
                Debug.Log("Entering Capture Mode State");
                PlaySignAnimation_Capture();
                break;
            case GameState.ListenMode:
                Debug.Log("Entering Listen Mode State");
                PlaySignAnimation_Listen();
                break;
            case GameState.TransitionMode:
                Debug.Log("Entering Transition Mode State");
                PlaySignAnimation_Ready();
                break;
        }
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Go:
                Debug.Log("Exiting Go Mode State");
                break;
            case GameState.CaptureMode:
                Debug.Log("Exiting Capture Mode State");
                break;
            case GameState.ListenMode:
                Debug.Log("Exiting Listen Mode State");
                break;
            case GameState.TransitionMode:
                Debug.Log("Exiting Transition Mode State");
                break;
        }
    }
}