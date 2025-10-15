using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("Awake " + GameManager.Instance.selectedGameLevelIndex);
    }
    void Start()
    {
        Debug.Log("Start " + GameManager.Instance.selectedGameLevelIndex);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (GameManager.Instance.CurrentGameState == GameManager.GameState.Playing)
    //    {
    //        // Lakukan sesuatu
    //        Debug.Log("Game Sedang Berjalan");
    //    }
    //}

    public void StartTheGame(){

        
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Playing)
        {
            GameManager.Instance.PauseGame();
            Debug.Log("Game Dijeda");
        }
        else
        {
            GameManager.Instance.StartGame();
        }
    }
}
