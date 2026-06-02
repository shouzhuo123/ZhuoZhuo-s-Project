using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hoarding_Ad_Timer_Script : MonoBehaviour
{
    // Advertising Hoarding Timer
    // This timer is referenced by all the hoarding objects. It holds the UV offset value and also triggers the adverts to change.
    // via an event.

    // Included with Multi Sport Stadium Asset
    // Twitter @Intoxio

    // set up events
    public static event Action UpdateBoardMaterialEvent;

    [Header("Hoarding Advert Settings")]
    public float scrollSpeed = 0.1f;                        // hoarding advert scroll speed
    public Texture[] hoardingAdverts;                       // materials used for hoarding adverts
    public int[] hoardingAdList;                            // the order in which the adverts appear on the hoarding (1-16 segments)

    // hoarding vars
    [HideInInspector]
    public int adCounter;                                   // counter cycles through 1-16 for all hoarding panels
    [HideInInspector]
    public float U_Offset;                                  // UV offset value for the scrolling advert (used globally)

    void Start()
    {

        // initialization
        U_Offset = 0f;
        adCounter = 0;
    }

    void FixedUpdate()
    {
        // update hoarding advert UV global offset
        // I use FixedUpdate() as opposed to Update() to prevent the material changing mid-frame (causing a visual glitch)

        U_Offset += (scrollSpeed * Time.deltaTime);

        // when texture has fully scrolled from view, reset it (temporary test)
        if (U_Offset > 1f)
        {
            U_Offset = 0f;

            adCounter++;

            if (adCounter > 15)
            {
                adCounter = 0;                          // reset counter once ad has travelled across all 16 hoarding panels

            }

            // trigger material update event (alternatively, use UpdateBoardMaterialEvent?.Invoke();)
            if (UpdateBoardMaterialEvent != null)
            {
                UpdateBoardMaterialEvent();
            }

        }

    }
}