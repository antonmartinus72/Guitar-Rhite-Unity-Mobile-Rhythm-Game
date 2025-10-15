using UnityEngine;

public class TestRedDot : MonoBehaviour
{
    public Color collisionColor; // Warna yang ingin Anda terapkan saat tabrakan terjadi

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("note"))
    //    {
    //        Debug.Log("Collision Enter detected!");
    //        GetComponent<SpriteRenderer>().color = Color.red;
    //    }
    //}

    //private void OnCollisionStay2D(Collision2D collision)
    //{
        
    //    if (collision.gameObject.CompareTag("note")) // Ganti "Player" dengan tag yang sesuai
    //    {
    //        Debug.Log("Collision Stay detected!");
    //        GetComponent<SpriteRenderer>().color = Color.blue;
    //    }
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("note"))
    //    {
    //        Debug.Log("Collision Exit detected!");
    //        GetComponent<SpriteRenderer>().color = Color.black;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("note"))
        {
            Debug.Log("Collider Enter detected!");
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("note"))
        {
            Debug.Log("Collider Stay detected!");
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("note"))
        {
            Debug.Log("Collider Exit detected!");
            GetComponent<SpriteRenderer>().color = Color.black;
        }
    }



    //public AnotherTest anotherTestScript;

    //private void OnEnable()
    //{
    //    anotherTestScript.redCircleAction += DisableObject;
    //    Debug.Log("RED SUBSCRIBED");
    //}

    //private void Start()
    //{

    //}

    //private void DisableObject()
    //{
    //    Debug.Log("RED DEACTIVATED");
    //    gameObject.SetActive(false);
    //}

    //private void OnDisable()
    //{
    //    anotherTestScript.redCircleAction -= DisableObject;
    //    Debug.Log("RED UNSUBSCRIBED");
    //}
}
