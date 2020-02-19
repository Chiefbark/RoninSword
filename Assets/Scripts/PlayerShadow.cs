using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private float opacity;  // The current opacity of the Player Shadow

    // Start is called before the first frame update
    void Start()
    {
        opacity = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // Reduces the opacity of the Player Shadow
        opacity -= Mathf.Pow(0.01f * (1 / GameRuler.SPEED), opacity) * GameRuler.SPEED;
        // Sets the opacity to the Player Shadow
        Color color = GetComponent<Renderer>().material.color;
        color.a = opacity;
        GetComponent<Renderer>().material.color = color;
        // Destroys the Player Shadow if the opacity is 0 or less
        if (opacity <= 0)
            Destroy(gameObject);
    }
}
