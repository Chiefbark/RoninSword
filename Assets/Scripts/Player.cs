using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BasicScriptBehaviour
{
    // The initial position of the Player
    private Vector2 InitPosition { get; set; }
    // The queue of target Enemies pending to kill
    private Queue<Enemy> TargetList = new Queue<Enemy>();
    // The current target position of the Player where to move
    private Vector2 CurrTargetPosition { get; set; }
    // The current Enemy to kill
    private Enemy CurrTargetEnemy { get; set; }
    private int nEnemies;   // Count of Enemies dequeues from the target list

    [SerializeField] // DEBUG
    // The base speed of the Player
    private float Speed;
    // Flag variable to check if the Player has to attack or not
    private bool Attack { get; set; }
    private int shadowCounter;

    protected override void OnStart()
    {
        InitPosition = transform.position;
        CurrTargetPosition = InitPosition;
    }

    protected override void StatusLiveBehaviour()
    {
        // Updates the speed of the animator
        GetComponent<Animator>().SetFloat("speed", GameRuler.SPEED);
        // If the current position is not the target position
        if ((Vector2)transform.position != CurrTargetPosition)
            HandleMovement();
        // If the Player has to attack
        else if (Attack)
            HandleAttack();
        // if there are more target positions inside the queue
        else if (!GetComponent<Animator>().GetBool("attack") && TargetList.Count > 0)
        {
            // Dequeues the Enemy from the list and prepares the Player to move towards it
            CurrTargetEnemy = TargetList.Dequeue();
            MoveTo(CurrTargetEnemy.transform.position, true);
        }
        // If there are no more target positions inside the queue
        else if (!GetComponent<Animator>().GetBool("attack"))
        {
            // Moves the Player to the initial position
            MoveTo(InitPosition, false, GameRuler.DIRECTION_NONE);
            nEnemies = 0;
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
    /// Handles the behaviour of the movement
    /// </summary>
    private void HandleMovement()
    {
        // Moves the Player
        transform.position = Vector2.MoveTowards(transform.position, CurrTargetPosition, Speed * GameRuler.SPEED);
        // Creates the Player Shadow
        if (shadowCounter == 4)
        {
            shadowCounter = 0;
            // Loads the Player Shadow
            GameObject shadow = (GameObject)Instantiate(Resources.Load("Player_Shadow"));
            // Renders the current sprite into the Player Shadow
            shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            // Sets the position of the Player Shadow to the current
            shadow.transform.position = transform.position;
        }
        shadowCounter++;
    }

    /// <summary>
    /// Handles the behaviour of the attack
    /// </summary>
    private void HandleAttack()
    {
        // Resets the attack flag
        Attack = false;
        // If the Player kills the Enemy
        if (CurrTargetEnemy.Kill(nEnemies++))
            // Sets the attack to the animator
            GetComponent<Animator>().SetBool("attack", true);
        // If the Player failed
        else
        {
            // TODO: improve the dead animation
            GetComponent<Animator>().SetBool("dead", true);
            GameRuler.GAMESTATUS = GameRuler.GAME_STATUS_OVER;
        }
    }

    /// <summary>
    /// Adds a new Enemy to the targetList
    /// </summary>
    /// <param name="enemy">The enemy to add</param>
    public void AddEnemy(Enemy enemy)
    {
        // Adds the target position to the queue
        TargetList.Enqueue(enemy);
    }

    /// <summary>
    /// Prepares the Player to move to a specific position
    /// </summary>
    /// <param name="targetPosition">The position where to move</param>
    /// <param name="attack">If the Player has to attack when on the target position</param>
    /// <param name="forceDirection">If the Player has to move in a specific direction</param>
    private void MoveTo(Vector2 targetPosition, bool attack, int forceDirection = -5)
    {
        // Sets the attack flag
        this.Attack = attack;

        // Sets the default direction of the Player
        int dir = GameRuler.DIRECTION_NONE;
        // Calculates the difference between the target position and current position
        Vector2 diff = targetPosition - (Vector2)transform.position;

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

        // If the target position is different than the init position & matches any of these conditions,
        // adds an offset to the target position
        if (targetPosition != InitPosition && dir == GameRuler.DIRECTION_TOP || dir == GameRuler.DIRECTION_BOTTOM)
            targetPosition -= new Vector2(0, 0.5f);
        if (targetPosition != InitPosition && dir == GameRuler.DIRECTION_LEFT)
            targetPosition += new Vector2(0.5f, 0);
        if (targetPosition != InitPosition && dir == GameRuler.DIRECTION_RIGHT)
            targetPosition -= new Vector2(0.5f, 0);

        // Sets the target position where to move
        CurrTargetPosition = targetPosition;

        // Sets the direction to the animator
        GetComponent<Animator>().SetInteger("direction", dir);
    }
}