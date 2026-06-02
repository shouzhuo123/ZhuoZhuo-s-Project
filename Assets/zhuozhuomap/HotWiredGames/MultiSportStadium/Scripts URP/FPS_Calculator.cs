using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Calculator : MonoBehaviour
{

    // Frames Per Second Counter
    // Included with Multi Sport Stadium Asset
    // Twitter @Intoxio

    public float fps;

    int frameCounter = 0;
    float timeCounter = 0.0f;
    float lastFramerate = 0.0f;
    public float refreshTime = 0.5f;
    Text UIText;

    private void Start()
    {
        UIText = this.GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {

        if (timeCounter < refreshTime)
        {
            timeCounter += Time.deltaTime;
            frameCounter++;
        }
        else
        {
            lastFramerate = (float)frameCounter / timeCounter;
            frameCounter = 0;
            timeCounter = 0.0f;

            //update the UI

            UIText.text = (Mathf.RoundToInt(lastFramerate).ToString()) + " fps";

        }

    }

}
