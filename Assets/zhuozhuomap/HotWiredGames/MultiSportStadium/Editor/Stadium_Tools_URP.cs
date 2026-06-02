using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;


public class Stadium_Tools : EditorWindow
{

    // * * * *   U R P   V E R S I O N   * * * *

    // define variables here

    GameObject _Sun;                                        // used to turn sun on or off
    GameObject _spotLightPitch;                           // used to turn lights on or off
    GameObject _spotLightStands;                             // used to turn lights on or off
    GameObject _neons;                                // used to turn lights on or off
    GameObject _SkyAndFogVolume;                            // used to change SkyAndFog Profile

    GameObject _seatParent;
    GameObject _seatHigh;
    GameObject _seatLow;


    // Define Unity Dropdown Menu

    [MenuItem("Tools/Stadium Configurator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Stadium_Tools));
    }


    private void OnGUI()
    {

        // Select Game Type

        GUILayout.Label("Select Game", EditorStyles.boldLabel);          // Window Label

        if (GUILayout.Button("Football / Soccer"))
        {
            SwitchMarkings("Football");
            SwitchPitchObjects("Football");
            EditorSceneManager.MarkAllScenesDirty();                          //  // Update the UI
        }

        if (GUILayout.Button("Hockey"))
        {
            SwitchMarkings("Hockey");
            SwitchPitchObjects("Hockey");
            FinishChanges();                                                                  // Update the UI
        }

        if (GUILayout.Button("Rugby"))
        {
            SwitchMarkings("Rugby");
            SwitchPitchObjects("Rugby");
            FinishChanges();                                                                  // Update the UI
        }

        if (GUILayout.Button("Cricket"))
        {
            SwitchMarkings("Cricket");
            SwitchPitchObjects("Cricket");
            FinishChanges();                                                                  // Update the UI
        }



        // Select Lighting Here

        GUILayout.Label("Select Lighting", EditorStyles.boldLabel);           // Window Label

        if (GUILayout.Button("Daytime"))
        {
            _Sun = GameObject.Find("Lighting").transform.Find("Directional Light (Sun)").gameObject;

            _Sun.SetActive(true);

            SetLightColourTempAndIntensity(_Sun, 5104f, 7f);                            // object, temp, intensity

            _Sun.transform.localEulerAngles = new Vector3(-643, -146, -211);            // set sun orientation

            // change Skybox & fog colour


            ChangeSkybox("SkyBox_Day", new Color (59f / 255, 76f / 255, 99f / 255));

            // update lights

            SetPitchLights(true);
            SetStandLights(false);
            SetNeon(false);

            FinishChanges();                                                                  // Update the UI

        }

        if (GUILayout.Button("Dusk"))
        {

            // set dusk config
            _Sun = GameObject.Find("Lighting").transform.Find("Directional Light (Sun)").gameObject;

            _Sun.SetActive(true);

            SetLightColourTempAndIntensity(_Sun, 2672f, 2.5f);                            // object, temp, intensity

            _Sun.transform.localEulerAngles = new Vector3(-676, -95, -316);             // set sun orientation

            // change Skybox & fog colour

            ChangeSkybox("Skybox_Dusk", new Color(83f / 255, 59f / 255, 99f / 255));

            // update lights

            SetPitchLights(true);
            SetStandLights(true);
            SetNeon(false);

            FinishChanges();                                                                  // Update the UI

        }

        if (GUILayout.Button("Nighttime"))
        {

            // set nighttime config
            _Sun = GameObject.Find("Lighting").transform.Find("Directional Light (Sun)").gameObject;
            _Sun.SetActive(false);

            // change Skybox & fog colour

            ChangeSkybox("Skybox_Night", new Color(52f / 255, 61f / 255, 90f / 255));

            // update lights

            SetPitchLights(true);
            SetStandLights(true);
            SetNeon(true);

            FinishChanges();                                                                  // Update the UI

        }

        // Select Seat Complexity Here

        // NOTE: IT IS NOT CURRENTLY POSSIBLE TO SORT THE SEAT OBJECTS (POLYS WITH TRANSPARENCY) CORRECTLY IN THE
        // URP RENDERER. THEREFORE THE OPTION IS DISABLED.

        /*

        GUILayout.Label("Select Seat Complexity", EditorStyles.boldLabel);          // Window Label

        if (GUILayout.Button("High Poly Seats"))
        {
            SwitchSeatModel("high");
        }

        if (GUILayout.Button("Low Poly Seats"))
        {
            SwitchSeatModel("low");
        }

        */

        // Select Seat Colours Here

        GUILayout.Label("Select Seat Colours", EditorStyles.boldLabel);          // Window Label

        if (GUILayout.Button("Yellow, Blue & Turqoise"))
        {
            UpdateSeatColours(255,231,0, 31,255,255, 0, 133, 241);         // Send RGB/RGB/RGB
        }

        if (GUILayout.Button("Red, Yellow & White"))
        {
            UpdateSeatColours(255, 0, 12, 255, 249, 1, 255, 255, 255);         // Send RGB/RGB/RGB
        }


        // Set Grass Pattern

        GUILayout.Label("Select Grass Pattern", EditorStyles.boldLabel);          // Window Label
        
        if (GUILayout.Button("Stripes Short"))
        {
            SwitchGrassPattern("Stripes Short");
        }

        if (GUILayout.Button("Stripes Long"))
        {
            SwitchGrassPattern("Stripes Long");
        }

        if (GUILayout.Button("Stripes Diagonal"))
        {
            SwitchGrassPattern("Stripes Diagonal");
        }

        if (GUILayout.Button("Stripes Overlap"))
        {
            SwitchGrassPattern("Stripes Overlap");
        }

        if (GUILayout.Button("Stripes Overlap Diagonal"))
        {
            SwitchGrassPattern("Stripes Overlap Diagonal");
        }

        if (GUILayout.Button("Stripes Circles"))
        {
            SwitchGrassPattern("Stripes Circles");
        }

        if (GUILayout.Button("No Stripes"))
        {
            SwitchGrassPattern("No Stripes");
        }

    }


    void ChangeSkybox(string skyBoxMaterialFile, Color fogColour)
    {
        Material newSkyboxMaterial;
        newSkyboxMaterial = Resources.Load(skyBoxMaterialFile, typeof(Material)) as Material;
        RenderSettings.skybox = newSkyboxMaterial;
        RenderSettings.fogColor = fogColour;
    }




    void SwitchGrassPattern(string requestedPattern)
    {
        // Get Pitch Grass Links
        GameObject _grassStripesShort = GameObject.Find("Pitch").transform.Find("Pitch_Stripes_Short").gameObject;
        GameObject _grassStripesLong = GameObject.Find("Pitch").transform.Find("Pitch_Stripes_Long").gameObject;
        GameObject _grassStripesDiagonal = GameObject.Find("Pitch").transform.Find("Pitch_Stripes_Diagonal").gameObject;
        GameObject _grassStripesOverlap = GameObject.Find("Pitch").transform.Find("Pitch_Overlap").gameObject;
        GameObject _grassStripesOverlapDiagonal= GameObject.Find("Pitch").transform.Find("Pitch_Overlap_Diagonal").gameObject;
        GameObject _grassStripesCircles = GameObject.Find("Pitch").transform.Find("Pitch_Circles").gameObject;
        GameObject _noStripes = GameObject.Find("Pitch").transform.Find("Pitch_Plain").gameObject;

        // Hide Everything
        _grassStripesShort.SetActive(false);
        _grassStripesLong.SetActive(false);
        _grassStripesDiagonal.SetActive(false);
        _grassStripesOverlap.SetActive(false);
        _grassStripesOverlapDiagonal.SetActive(false);
        _grassStripesCircles.SetActive(false);
        _noStripes.SetActive(false);

        switch (requestedPattern)
        {
            case "Stripes Short":
                _grassStripesShort.SetActive(true);
                break;

            case "Stripes Long":
                _grassStripesLong.SetActive(true);
                break;

            case "Stripes Diagonal":
                _grassStripesDiagonal.SetActive(true);
                break;

            case "Stripes Overlap":
                _grassStripesOverlap.SetActive(true);
                break;

            case "Stripes Overlap Diagonal":
                _grassStripesOverlapDiagonal.SetActive(true);
                break;

            case "Stripes Circles":
                _grassStripesCircles.SetActive(true);
                break;

            case "No Stripes":
                _noStripes.SetActive(true);
                break;

        }

    }

    void SwitchMarkings(string requestedMarkings)
    {

        // Get Pitch Marking Links
        GameObject _markingsFootball = GameObject.Find("Pitch").transform.Find("Markings_Football").gameObject;
        GameObject _markingsHockey = GameObject.Find("Pitch").transform.Find("Markings_Hockey").gameObject;
        GameObject _markingsRugby = GameObject.Find("Pitch").transform.Find("Markings_Rugby").gameObject;
        GameObject _markingsCricket = GameObject.Find("Pitch").transform.Find("Markings_Cricket").gameObject;

        // Hide Everything
        _markingsFootball.SetActive(false);
        _markingsHockey.SetActive(false);
        _markingsRugby.SetActive(false);
        _markingsCricket.SetActive(false);

        switch (requestedMarkings)
        {
            case "Football":
                _markingsFootball.SetActive(true);
                break;

            case "Hockey":
                _markingsHockey.SetActive(true);
                break;

            case "Rugby":

                _markingsRugby.SetActive(true);
                break;

            case "Cricket":
                _markingsCricket.SetActive(true);
                break;
        }

    }


    void SwitchPitchObjects(string requestedObjects)
    {
        // Get Pitch Marking Links
        GameObject _objectsFootball = GameObject.Find("Pitch_Objects").transform.Find("Football_Objects").gameObject;
        GameObject _objectsHockey = GameObject.Find("Pitch_Objects").transform.Find("Hockey_Objects").gameObject;
        GameObject _objectsRugby = GameObject.Find("Pitch_Objects").transform.Find("Rugby_Objects").gameObject;
        GameObject _objectsCricket = GameObject.Find("Pitch_Objects").transform.Find("Cricket_Objects").gameObject;

        // Hide Everything
        _objectsFootball.SetActive(false);
        _objectsHockey.SetActive(false);
        _objectsRugby.SetActive(false);
        _objectsCricket.SetActive(false);

        switch (requestedObjects)
        {
            case "Football":
                _objectsFootball.SetActive(true);
                break;

            case "Hockey":
                _objectsHockey.SetActive(true);
                break;

            case "Rugby":

                _objectsRugby.SetActive(true);
                break;

            case "Cricket":
                _objectsCricket.SetActive(true);
                break;
        }

    }



    void GetSeatOjects()
    {
        // get links to seats
        _seatParent = GameObject.Find("Seats");                                      // Get seat parent
        _seatHigh = _seatParent.transform.Find("Seating_HIGH_Poly").gameObject;      // Get HIGH poly seats
        _seatLow = _seatParent.transform.Find("Seating_LOW_Poly").gameObject;        // Get LOW poly seats

    }

    void UpdateSeatColours(float r1, float g1, float b1, float r2, float g2, float b2, float r3, float g3, float b3)
    {
        GetSeatOjects();                                                            // get links to seats

        // HIGH POLY

        // find objects that contain the 3 seat colour materials:

        GameObject seatMaterial1 = _seatHigh.transform.Find("Seating_Corner_HIGH_01").gameObject;
        GameObject seatMaterial2 = _seatHigh.transform.Find("Seating_01_HIGH_02").gameObject;
        GameObject seatMaterial3 = _seatHigh.transform.Find("Seating_01_HIGH_01").gameObject;

        // acquire the renderer for each object:

        Renderer seatRenderer1 = seatMaterial1.GetComponent<Renderer>();
        Renderer seatRenderer2 = seatMaterial2.GetComponent<Renderer>();
        Renderer seatRenderer3 = seatMaterial3.GetComponent<Renderer>();

        // update the HDRP colour for each material:

        seatRenderer1.sharedMaterial.SetColor("_BaseColor", new Color(r1 / 255, g1 / 255, b1 / 255));
        seatRenderer2.sharedMaterial.SetColor("_BaseColor", new Color(r2 / 255, g2 / 255, b2 / 255));
        seatRenderer3.sharedMaterial.SetColor("_BaseColor", new Color(r3 / 255, g3 / 255, b3 / 255));

        // LOW POLY

        // find objects that contain the 3 seat colour materials:

        GameObject seatLowMaterial1 = _seatLow.transform.Find("Seating_Corner_LOW_01").gameObject;
        GameObject seatLowMaterial2 = _seatLow.transform.Find("Seating_01_LOW_02").gameObject;
        GameObject seatLowMaterial3 = _seatLow.transform.Find("Seating_01_LOW_01").gameObject;

        // acquire the renderer for each object:

        Renderer seatLowRenderer1 = seatLowMaterial1.GetComponent<Renderer>();
        Renderer seatLowRenderer2 = seatLowMaterial2.GetComponent<Renderer>();
        Renderer seatLowRenderer3 = seatLowMaterial3.GetComponent<Renderer>();

        // update the HDRP colour for each material:

        seatLowRenderer1.sharedMaterial.SetColor("_BaseColor", new Color(r1 / 255, g1 / 255, b1 / 255));
        seatLowRenderer2.sharedMaterial.SetColor("_BaseColor", new Color(r2 / 255, g2 / 255, b2 / 255));
        seatLowRenderer3.sharedMaterial.SetColor("_BaseColor", new Color(r3 / 255, g3 / 255, b3 / 255));

    }

    void SwitchSeatModel (string seatType)
    {
        GetSeatOjects();                                                            // get links to seats

        switch (seatType)
        {
            case "high":
                _seatHigh.SetActive(true);
                _seatLow.SetActive(false);
                break;

            case "low":
                _seatHigh.SetActive(false);
                _seatLow.SetActive(true);
                break;
        }
    }

    void SetLightColourTempAndIntensity (GameObject lightToChange, float temp, float intensity)
    {

        // set colour temp
        lightToChange.GetComponent<Light>().colorTemperature = temp;                        // set sun colour

        // set intensity
        lightToChange.GetComponent<Light>().intensity = intensity;

    }

    void ChangeSkyAndFogVolumeProfile(string profilePath)
    {
        _SkyAndFogVolume = GameObject.Find("Sky and Fog Volume");
        Volume _volume = _SkyAndFogVolume.GetComponent<Volume>();
        _volume.sharedProfile = AssetDatabase.LoadAssetAtPath(profilePath, typeof(Object)) as VolumeProfile;
    }

    void SetStandLights(bool state)
    {
        _spotLightStands = GameObject.Find("Lighting").transform.Find("SpotlightStands").gameObject;

        if (state)
        {
            _spotLightStands.SetActive(true);
        }
        else
        {
            _spotLightStands.SetActive(false);
        }
    }

    void SetPitchLights(bool state)
    {
        _spotLightPitch = GameObject.Find("Lighting").transform.Find("Spotlight_Pitch").gameObject;
        _spotLightStands = GameObject.Find("Lighting").transform.Find("SpotlightStands").gameObject;

        if (state)
        {
            _spotLightPitch.SetActive(true);
        }
        else
        {
            _spotLightPitch.SetActive(false);
        }
    }

    void SetNeon(bool state)
    {
        _neons = GameObject.Find("Neon").transform.Find("NeonParent").gameObject;

        if (state)
        {
            _neons.SetActive(true);
        }
        else
        {
            _neons.SetActive(false);
        }


    }



    void FinishChanges()
    {
        Repaint();                                                      // repaint UI
        EditorSceneManager.MarkAllScenesDirty();                        // mark scene as dirty (modified) to retain changes
    }
    
}
