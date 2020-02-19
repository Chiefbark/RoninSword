using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicScriptBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameRuler.GAMESTATUS == GameRuler.GAME_STATUS_LIVE)
            StatusLiveBehaviour();
        if (GameRuler.GAMESTATUS == GameRuler.GAME_STATUS_STOP)
            StatusStopBehaviour();
        if (GameRuler.GAMESTATUS == GameRuler.GAME_STATUS_OVER)
            StatusOverBehaviour();
    }

    /// <summary>
    /// OnStart is called before the first frame update
    /// </summary>
    protected abstract void OnStart();

    /// <summary>
    /// Handles the behaviour of the Object when the game is running
    /// </summary>
    protected abstract void StatusLiveBehaviour();

    /// <summary>
    /// Handles the behaviour of the Object when the game is stopped
    /// </summary>
    protected abstract void StatusStopBehaviour();

    /// <summary>
    /// Handles the behaviour of the Object when the game is over
    /// </summary>
    protected abstract void StatusOverBehaviour();
}
