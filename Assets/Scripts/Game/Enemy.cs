using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : BasicScriptBehaviour
{
    // The initial position of the Enemy
    public Vector2 InitPosition { get; set; }
    // The queue of target positions pending to go
    private Queue<Vector2> TargetList = new Queue<Vector2>();
    // The current target position of the Enemy where to move
    private Vector2 CurrTargetPosition { get; set; }

    // The base speed of the Enemy
    public float Speed { get; set; }
    // Flag variable to check if the Enemy has to attack or not
    protected bool Attack { get; set; }
    // Delay with which the Enemy appears
    public int Delay { get; set; }
    private int delay;  // Delay counter

    private bool placed;

    protected override void OnStart()
    {
        delay = 0;
        transform.position = InitPosition;
        CurrTargetPosition = InitPosition;
        placed = false;
    }

    protected override void StatusLiveBehaviour()
    {
        if (delay < Delay)
            delay += 1;
        if (delay >= Delay)
        {
            // Updates the speed of the animator
            GetComponent<Animator>().SetFloat("speed", GameRuler.SPEED);
            // If the current position is not the target position
            if ((Vector2)transform.position != CurrTargetPosition)
                transform.position = Vector2.MoveTowards(transform.position, CurrTargetPosition, Speed * GameRuler.SPEED);
            // if there are more target positions inside the queue
            else if (!GetComponent<Animator>().GetBool("attack") && TargetList.Count > 0)
            {
                placed = false;
                // Dequeues the nest position from the list and prepares the Enemy to move towards it
                CurrTargetPosition = TargetList.Dequeue();
                MoveTo(CurrTargetPosition);
            }
            // If there are no more target positions inside the queue
            else if (!GetComponent<Animator>().GetBool("attack"))
            {
                MoveTo(transform.position, GameRuler.DIRECTION_BOTTOM);
                if (!placed)
                {
                    placed = true;
                    GameObject.Find("GameRuler").GetComponent<GameRuler>().NotifyEnemyPlaced();
                }
            }
        }
    }

    protected override void StatusOverBehaviour()
    {
        // If the Enemy has to attack
        if (Attack)
        {
            Attack = false;
            GetComponent<Animator>().SetBool("attack", true);
        }
    }

    protected override void OnGameStatusChanged(int newStatus)
    {
        if (newStatus == GameRuler.GAME_STATUS_STOP)
            GetComponent<Animator>().enabled = false;
        else
            GetComponent<Animator>().enabled = true;
    }

    /// <summary>
    /// Returns TRUE if the Enemy is killed, FALSE otherwise
    /// </summary>
    /// <param name="expectedIndex">The expected index of the Enemy</param>
    /// <returns>TRUE if the Enemy dies, FALSE otherwise</returns>
    public abstract bool Kill(int expectedIndex);

    /// <summary>
    /// Adds a new position to the targetList
    /// </summary>
    /// <param name="target">The position to add</param>
    public void AddTargetPosition(Vector2 target)
    {
        // Adds the target position to the queue
        TargetList.Enqueue(target);
    }

    /// <summary>
    /// Prepares the Player to move to a specific position
    /// </summary>
    /// <param name="targetPosition">The position where to move</param>
    /// <param name="forceDirection">If the Player has to move in a specific direction</param>
    private void MoveTo(Vector2 targetPosition, int forceDirection = -5)
    {
        // Sets the target position where to move
        CurrTargetPosition = targetPosition;

        // Sets the default direction of the Player
        int dir = GameRuler.DIRECTION_NONE;
        // Calculates the difference between the target position and current position
        Vector2 diff = this.CurrTargetPosition - (Vector2)transform.position;

        // Bottom direction
        if (diff.y < 0 && Mathf.Abs(diff.x) < Mathf.Abs(diff.y) || Mathf.Abs(diff.x) == Mathf.Abs(diff.y))
            dir = GameRuler.DIRECTION_BOTTOM;
        // Top direction
        if (diff.y > 0 && Mathf.Abs(diff.x) < Mathf.Abs(diff.y) || Mathf.Abs(diff.x) == Mathf.Abs(diff.y))
            dir = GameRuler.DIRECTION_TOP;
        // Left direction
        if (diff.x < 0 && Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            dir = GameRuler.DIRECTION_LEFT;
        // Right direction
        if (diff.x > 0 && Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            dir = GameRuler.DIRECTION_RIGHT;

        // If the force direction is inside the bounds
        if (forceDirection >= GameRuler.DIRECTION_RIGHT && forceDirection <= GameRuler.DIRECTION_TOP)
            dir = forceDirection;

        // Sets the direction to the animator
        GetComponent<Animator>().SetInteger("direction", dir);
    }

    /// <summary>
    /// Detects when the Enemy has been clicked
    /// </summary>
    protected virtual void OnMouseDown()
    {
        if (GameRuler.GAMESTATUS == GameRuler.GAME_STATUS_LIVE)
        {
            // Notifies the GameRuler that this Enemy has been clicked
            GameObject.Find("GameRuler").GetComponent<GameRuler>().NotifyClick(this);
            // Loads and places the select effect
            GameObject select = (GameObject)Instantiate(Resources.Load("Prefabs/select"));
            select.transform.SetParent(transform);
            select.transform.localPosition = new Vector2(0, -0.3f);
            // Destroys the collider so the Enemy cannot be clicked again
            Destroy(GetComponent<BoxCollider2D>());
        }
    }
}
