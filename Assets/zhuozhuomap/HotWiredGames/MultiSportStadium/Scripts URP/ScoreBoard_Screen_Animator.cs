using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard_Screen_Animator : MonoBehaviour
{
    /*  Scoreboard Animator

        * * * * U R P   V E R S I O N * * * *

        This script waits a set number of seconds before updating the UV offsets on the screen, moving it to each
        corner of the bitmap.
 
        Included with Multi Sport Stadium Asset
        Twitter @Intoxio

     */

    float lastSwitchTime;
    float nextSwitchTime;
    public float waitTime;                  // time to wait between switching
    int screenCounter;               // counts the the four quadrents
    Renderer textureRenderer;        // used to update UVs

    void Start()
    {
        waitTime = 4f;                   
        screenCounter = 1;
        lastSwitchTime = Time.time;
        textureRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
    
    if (Time.time > nextSwitchTime)
        {

            nextSwitchTime = Time.time + waitTime;

            switch (screenCounter)
            {
                case 1:
                    {
                        textureRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0f,0f));    // update the texture
                        break;
                    }
                case 2:
                    {
                        textureRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0.5f, 0f));    // update the texture
                        break;
                    }
                case 3:
                    {
                        textureRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0f,0.5f));    // update the texture
                        break;
                    }
                case 4:
                    {
                        textureRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0.5f, 0.5f));    // update the texture
                        break;
                    }
            }

            screenCounter++;
            if (screenCounter > 4)
            {
                screenCounter = 1;
            }
        }
    }
}
