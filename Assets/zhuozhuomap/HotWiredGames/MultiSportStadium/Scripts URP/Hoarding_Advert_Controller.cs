using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Hoarding Advert Controller
  
   * * * U R P   V E R S I O N   * * * 

This script looks at the list of adverts stored in the Stadium_Configuator script and animates the
textures and UVs associated with this model.

Included with Multi Sport Stadium Asset
Twitter @Intoxio

*/

public class Hoarding_Advert_Controller : MonoBehaviour

{
    Hoarding_Ad_Timer_Script _hoarding_Ad_Timer_Script;                 // Settings are aquired from here.
    Renderer textureRenderer;                                           // Used to animate UVs
    public float _U_Offset;                                             // Global U Offset Value
    int[] _hoardingAdList;                                              // Sequence List of Hoarding Adverts
    Texture[] _hoardingAdverts;                                         // Array of Advert Materials
    public int _adCount;                                                // number of Ads
    public string boardPhase;                                           // type of board this script is attached to (in/out)

    // board configuration

    public int boardNumber;                                             // the number of this board in the scene (1-16)

    // ads being shown (based on hoarding number and Sequence List)

    public int adForThisHoarding;
    public int newAdIndex;


    void Start()
    {
        // subscribe to materal update request event
        Hoarding_Ad_Timer_Script.UpdateBoardMaterialEvent += SetMaterial;

        // links to the configurator data
        _hoarding_Ad_Timer_Script = GameObject.Find("Hoarding_Ad_Timer").GetComponent<Hoarding_Ad_Timer_Script>();
        _hoardingAdList = _hoarding_Ad_Timer_Script.hoardingAdList;                      // get local copy of advert sequence list
        _hoardingAdverts = _hoarding_Ad_Timer_Script.hoardingAdverts;                    // get local copy of advert textures

        // initialization
        textureRenderer = GetComponent<Renderer>();

        // set materials
        SetMaterial();
    }

    // Update is called once per frame
    void Update()
    {

        _U_Offset = _hoarding_Ad_Timer_Script.U_Offset;                                  // get global U value

        // adjust UV value for incoming texture
        if (boardPhase == "in")
        {
            _U_Offset -= 1f;
        }

        textureRenderer.material.SetTextureOffset("_BaseMap", new Vector2(_U_Offset, 0));    // update the texture

    }

    private void SetMaterial()
    {
                // advert leaving the hoarding
                adForThisHoarding = _hoarding_Ad_Timer_Script.adCounter + (boardNumber-1);

                // if this board is showing the 'incoming' advert, point to next advert 
                if (boardPhase == "in")
                {
                    adForThisHoarding ++;
                }

                // wrap value if necessary
                if (adForThisHoarding > 15)
                {
                    adForThisHoarding -= 16;
                }

                newAdIndex = _hoardingAdList[adForThisHoarding];

                Texture newTexture = _hoardingAdverts[newAdIndex];

                textureRenderer.material.SetTexture("_BaseMap", newTexture);
                textureRenderer.material.SetTexture("_EmissionMap", newTexture);
    }

    private void OnDisable()
    {
        // Unsubscribe from event when script ends
        Hoarding_Ad_Timer_Script.UpdateBoardMaterialEvent -= SetMaterial;
    }
}
