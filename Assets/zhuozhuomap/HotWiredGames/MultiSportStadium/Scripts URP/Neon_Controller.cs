using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neon_Controller : MonoBehaviour
{

    // Neon Controller
    // * * *   U R P   V e r s i o n   * * *
    // Included with Multi Sport Stadium Asset
    // Twitter @Intoxio

    public float scrollSpeed;                               // speed and direction in which neon travels
    public float U_Offset;                                  // current UV offset
    public Renderer neonRenderer;                           // link to Renderer so we can update UV offset

    // Start is called before the first frame update
    void Start()
    {
        // set up vars

        U_Offset = 0;
        neonRenderer = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // update uv offset

        neonRenderer.material.SetTextureOffset("_BaseMap", new Vector2(U_Offset, 0));    // update the texture

        U_Offset += scrollSpeed * Time.deltaTime;

        // reset before wrap

        if (U_Offset > 1)
        {
            U_Offset = 0;
        }

    }
}
