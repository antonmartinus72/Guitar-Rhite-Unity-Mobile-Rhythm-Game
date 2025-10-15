using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMBeatNoteDeactivator : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("note"))
        {
            collision.gameObject.SetActive(false);
        }
    }
}
