using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicScriptBehaviour : MonoBehaviour
{
    private int prevGameStatus;
    // Start is called before the first frame update
    void Start()
    {
        prevGameStatus = GameRuler.GAMESTATUS;
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        // Detects whenever the game status changes
        if (GameRuler.GAMESTATUS != prevGameStatus)
            OnGameStatusChanged(GameRuler.GAMESTATUS);

        if (GetComponent<AudioSource>() != null)
        {
            // Updated the sound effects volume
            GetComponent<AudioSource>().volume = AppManager.GAME_VOLUME_EFFECTS;
            // Updates the speed of the sound effect
            GetComponent<AudioSource>().pitch = GameRuler.SPEED;
        }

        prevGameStatus = GameRuler.GAMESTATUS;

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
    protected virtual void StatusStopBehaviour() { }

    /// <summary>
    /// Handles the behaviour of the Object when the game is over
    /// </summary>
    protected virtual void StatusOverBehaviour() { }

    /// <summary>
    /// Notifies whenever the game status has been changed
    /// </summary>
    /// <param name="newStatus">The new game status of the game</param>
    protected virtual void OnGameStatusChanged(int newStatus) { }
}
