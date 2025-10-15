using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPlayModeSelector : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MainMenuUIManager UIManager;

    [SerializeField] List<RectTransform> modeSelectorPanels;
    [SerializeField] GameObject scrollViewPage;
    int currentPageSelected;

    public void OpenLevelSelectorPanel()
    {
        int currentPageSelected = scrollViewPage.GetComponent<UISwipeController>().currentPage;
        //int panelIndexToShow = currentPageSelected - 1; // disesuaikan antara nomor currenPageSelecter dengan index List modeSelectorPanels

        switch (currentPageSelected) // index menurut gamemode slider yang ditampilkan pada main menu
        {
            case 1:
                ShowPanelIfValid(0);
                //GameManager.Instance.SetGameModeToChordMasterMode();
                break;
            case 2:
                ShowPanelIfValid(1);
                GameManager.Instance.SetGameModeToChordMasterMode();
                break;
            case 3:
                ShowPanelIfValid(2);
                GameManager.Instance.SetGameModeToBeatMasterMode();
                break;
            case 4:
                ShowPanelIfValid(3);
                GameManager.Instance.SetGameModeToMusicalIntervalMasterMode();
                break;
            // tambah nanti
            default:
                Debug.LogWarning("Invalid panel index!");
                break;
        }

        //Debug.Log("Game State : " + GameManager.Instance.CurrentGameState);
        //Debug.Log("Game Mode State : " + GameManager.Instance.CurrentGameModeState);
;    }

    private void ShowPanelIfValid(int index)
    {
        GameManager.Instance.selectedGameLevelIndex = 0; // Reset Game Index setiap masuk ke Level Selector
        GameManager.Instance.isMusicalJourneyMode = false; // Reset mode Musical Journey setiap memasuki Level Selector "Musical Journey Level Selector"

        // nonaktifkan semua panel di dalam List<modeSelectorPanels> 
        foreach (var panel in modeSelectorPanels)
        {
            if (panel != null)
            {
                panel.gameObject.SetActive(false);
            }
        }

        UIManager.ShowPanel(modeSelectorPanels[index]);

        //Debug.Log("ENABLE PANEL : " + modeSelectorPanels[index].name);

        //if (index >= 0 && index < modeSelectorPanels.Count)
        //{
            
        //}
        //else
        //{
        //    Debug.LogWarning("Invalid panel index!");
        //}
    }
}
