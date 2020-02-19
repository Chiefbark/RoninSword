using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 initPosition;   // The initial position of the Player
    private Vector2 nextTargetPosition; // The next target position of the Player
    private Enemy nextEnemy;
    private Queue<Enemy> targetList;  // The queue of target positions pending to go

    [SerializeField]
    private float speed;    // The base speed of the Player

    private bool attack;    // Flag variable to check if the Player has to attack or not

    private int iter;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
        nextTargetPosition = initPosition;

        targetList = new Queue<Enemy>();

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// Handles the movement of the Player
    /// </summary>
    private void HandleMovement()
    {
        // Updates the speed of the animator
        GetComponent<Animator>().SetFloat("speed", GameRuler.SPEED);

        // If the current position is not the target position
        if ((Vector2)transform.position != nextTargetPosition)
        {
            // Moves the Player
            transform.position = Vector2.MoveTowards(transform.position, nextTargetPosition, speed * GameRuler.SPEED);

            // Creates the Player Shadow
            if (iter == 4)
            {
                iter = 0;
                GameObject shadow = (GameObject)Instantiate(Resources.Load("Player_Shadow"));
                // Renders the current sprite into the Player Shadow
                shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                // Sets the position of the Player Shadow to the current
                shadow.transform.position = transform.position;
            }
            iter++;

        }
        // If the Player has to attack
        else if (attack)
        {
            if (nextEnemy.Kill())
            {
                // Resets the attack flag
                attack = false;
                // Sets the attack to the animator
                GetComponent<Animator>().SetBool("attack", true);
            }
        }
        // if there are more target positions on the queue
        else if (!GetComponent<Animator>().GetBool("attack") && targetList.Count > 0)
        {
            nextEnemy = targetList.Dequeue();
            MoveTo(nextEnemy.transform.position, true);
        }
        // If there are no more target positions on the queue
        else if (!GetComponent<Animator>().GetBool("attack"))
            MoveTo(initPosition, false, 2);
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
        this.nextTargetPosition = targetPosition;

        // Sets the default direction of the Player
        int dir = GameRuler.DIRECTION_NONE;
        // Calculates the difference between the target position and current position
        Vector2 diff = this.nextTargetPosition - (Vector2)transform.position;

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

        // If the force direction is set
        if (forceDirection >= -2 && forceDirection <= 2)
            dir = forceDirection;

        // Sets the direction to the animator
        GetComponent<Animator>().SetInteger("direction", dir);
    }
}
