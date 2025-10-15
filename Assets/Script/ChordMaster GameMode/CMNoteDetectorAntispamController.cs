using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMNoteDetectorAntispamController : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("antispam"))
        {
            DeactiveNoteChildObjects(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("antispam"))
        {
            DeactiveNoteChildObjects(collision);
        }
    }

    private void DeactiveNoteChildObjects(Collider2D collision)
    {
        //collision.transform.parent.gameObject.SetActive(false);
        var parent = collision.transform.parent.gameObject;

        var antispamObj = parent.GetComponent<CMNoteScript>().antispam;
        var startObj = parent.GetComponent<CMNoteScript>().start;
        var startContentObj = parent.GetComponent<CMNoteScript>().startcontent;
        var endObj = parent.GetComponent<CMNoteScript>().end;
        var fillObj = parent.GetComponent<CMNoteScript>().fill;

        antispamObj.SetActive(false);
        startObj.SetActive(false);
        startContentObj.SetActive(false);
        endObj.SetActive(false);
        fillObj.SetActive(false);
    }
}
