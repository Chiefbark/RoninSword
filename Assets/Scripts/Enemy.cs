using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BasicScriptBehaviour
{
    public Vector2 InitPosition { get; set; }

    private Vector2 currTargetPosition; // The current target position of the Enemy where to move
    private Queue<Vector2> targetList = new Queue<Vector2>();  // The queue of target positions pending to go

    private bool attack;    // Flag variable to check if the Enemy has to attack or not

    public float Speed { get; set; }
    public int Index { get; set; }

    private int delay;
    public int Delay { get; set; }

    protected override void OnStart()
    {
        delay = 0;
        transform.position = InitPosition;
        currTargetPosition = InitPosition;

        //targetList = new Queue<Vector2>();
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
            if ((Vector2)transform.position != currTargetPosition)
                transform.position = Vector2.MoveTowards(transform.position, currTargetPosition, Speed * GameRuler.SPEED);
            // if there are more target positions inside the queue
            else if (!GetComponent<Animator>().GetBool("attack") && targetList.Count > 0)
            {
                // Dequeues the nest position from the list and prepares the Enemy to move towards it
                currTargetPosition = targetList.Dequeue();
                MoveTo(currTargetPosition);
            }
            // If there are no more target positions inside the queue
            else if (!GetComponent<Animator>().GetBool("attack"))
                MoveTo(transform.position, GameRuler.DIRECTION_BOTTOM);
        }
    }

    protected override void StatusOverBehaviour()
    {
        // If the Enemy has to attack
        if (attack)
        {
            attack = false;
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
    public bool Kill(int expectedIndex)
    {
        // If both indexes are equal
        if (expectedIndex == Index)
        {
            // Delays the blood effect
            StartCoroutine(Blood());
            // TODO: Death Enemy animation with behaviour for autodestroy
            // The next line of code is just for testing
            Destroy(gameObject, 0.75f * GameRuler.SPEED);
            return true;
        }
        // Enables the attack flag
        attack = true;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Blood()
    {
        yield return new WaitForSeconds(0.2f);
        // Loads and places the blood effect
        GameObject blood = (GameObject)Instantiate(Resources.Load("blood"));
        blood.transform.SetParent(transform);
        blood.transform.localPosition = new Vector2(0, 0.2f);
    }

    /// <summary>
    /// Adds a new position to the targetList
    /// </summary>
    /// <param name="target">The position to add</param>
    public void AddTargetPosition(Vector2 target)
    {
        // Adds the target position to the queue
        targetList.Enqueue(target);
    }

    /// <summary>
    /// Prepares the Player to move to a specific position
    /// </summary>
    /// <param name="targetPosition">The position where to move</param>
    /// <param name="forceDirection">If the Player has to move in a specific direction</param>
    private void MoveTo(Vector2 targetPosition, int forceDirection = -5)
    {
        // Sets the target position where to move
        this.currTargetPosition = targetPosition;

        // Sets the default direction of the Player
        int dir = GameRuler.DIRECTION_NONE;
        // Calculates the difference between the target position and current position
        Vector2 diff = this.currTargetPosition - (Vector2)transform.position;

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
    private void OnMouseDown()
    {
        // Notifies the GameRuler that this Enemy has been clicked
        GameObject.Find("GameRuler").GetComponent<GameRuler>().NotifyClick(this);
        // Loads and places the select effect
        GameObject select = (GameObject)Instantiate(Resources.Load("select"));
        select.transform.SetParent(transform);
        select.transform.localPosition = new Vector2(0, -0.3f);
        // Destroys the collider so the Enemy cannot be clicked again
        Destroy(GetComponent<BoxCollider2D>());
    }
}
