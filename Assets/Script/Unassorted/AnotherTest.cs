using UnityEngine;
using UnityEngine.Events;

public class AnotherTest : MonoBehaviour
{
    public bool activeRedCircle = false;
    public bool activeBlueCircle = false;
    public UnityAction redCircleAction;
    public UnityEvent blueCircleEvent;

    private void Start()
    {
        
        

    }
    private void Update()
    {
        if (activeRedCircle == true)
        {
            redCircleAction?.Invoke();
            Debug.Log("RED INVOKED");
            //activeRedCircle = false;
        }

        if (activeBlueCircle == true)
        {
            blueCircleEvent?.Invoke();
            Debug.Log("BLUE INVOKED");
            //activeBlueCircle = false;
        }

        
    }
}
