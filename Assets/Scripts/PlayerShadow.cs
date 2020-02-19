using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : BasicScriptBehaviour
{
    private float opacity;  // The current opacity of the Player Shadow

    protected override void OnStart()
    {
        opacity = 1f;
    }

    protected override void StatusLiveBehaviour()
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

    protected override void StatusStopBehaviour()
    {
    }

    protected override void StatusOverBehaviour()
    {
        StatusLiveBehaviour();
    }
}
