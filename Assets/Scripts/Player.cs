using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 initPosition;   // The initial position of the Player
    private Vector2 currTargetPosition; // The current target position of the Player where to move
    private Enemy currEnemy;    // The current Enemy to kill
    private Queue<Enemy> targetList;  // The queue of target Enemies pending to kill

    [SerializeField]
    private float speed;    // The base speed of the Player

    private bool attack;    // Flag variable to check if the Player has to attack or not

    private int shadowCounter;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
        currTargetPosition = initPosition;

        targetList = new Queue<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        Behaviour();
    }

    /// <summary>
    /// Handles the movement of the Player
    /// </summary>
    private void Behaviour()
    {
        // Updates the speed of the animator
        GetComponent<Animator>().SetFloat("speed", GameRuler.SPEED);
        // If the current position is not the target position
        if ((Vector2)transform.position != currTargetPosition)
            HandleMovement();
        // If the Player has to attack
        else if (attack)
            HandleAttack();
        // if there are more target positions on the queue
        else if (!GetComponent<Animator>().GetBool("attack") && targetList.Count > 0)
        {
            // Dequeues the Enemy from the list and prepares the Player to move towards it
            currEnemy = targetList.Dequeue();
            MoveTo(currEnemy.transform.position, true);
        }
        // If there are no more target positions on the queue
        else if (!GetComponent<Animator>().GetBool("attack"))
            // Moves the Player to the initial position
            MoveTo(initPosition, false, 2);
    }

    /// <summary>
    /// handles the behaviour of the movement
    /// </summary>
    private void HandleMovement()
    {
        // Moves the Player
        transform.position = Vector2.MoveTowards(transform.position, currTargetPosition, speed * GameRuler.SPEED);
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
        // If the Player kills the Enemy
        if (currEnemy.Kill())
        {
            // Resets the attack flag
            attack = false;
            // Sets the attack to the animator
            GetComponent<Animator>().SetBool("attack", true);
        }
        // If the Player failed
        else
        {
            // Kill the player
        }
    }

    /// <summary>
    /// Adds a new Enemy to the targetList
    /// </summary>
    /// <param name="enemy">The enemy to add</param>
    public void AddEnemy(Enemy enemy)
    {
        // Adds the target position to the queue
        targetList.Enqueue(enemy);
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
        this.attack = attack;
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
}
