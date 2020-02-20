using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuler : MonoBehaviour
{
    [SerializeField]
    private float speed; // DEBUG

    private GameObject player;

    public static int GAMESTATUS;

    public static int GAME_STATUS_STOP = -1;
    public static int GAME_STATUS_OVER = 0;
    public static int GAME_STATUS_LIVE = 1;

    public static int DIRECTION_RIGHT = -2;
    public static int DIRECTION_LEFT = -1;
    public static int DIRECTION_NONE = 0;
    public static int DIRECTION_BOTTOM = 1;
    public static int DIRECTION_TOP = 2;

    public static float SPEED = 1f;

    // Posible init positions of the Enemies
    private readonly Vector2[] InitPositions = new Vector2[]
    {
        new Vector2(-5.78f, 3.5f),new Vector2(-5.78f, 1),new Vector2(-5.78f, -1.5f),
        new Vector2(5.78f, 3.5f),new Vector2(5.78f, 1),new Vector2(5.78f, -1.5f)
    };
    // Posible target positions of the Enemies
    private readonly Vector2[] TargetPositions = new Vector2[]
    {
        new Vector2(-1.40f, 3.5f),new Vector2(-1.40f, 1),new Vector2(-1.40f, -1.5f),
        new Vector2(1.40f, 3.5f),new Vector2(1.40f, 1),new Vector2(1.40f, -1.5f)
    };
    // Total number of Enemies of the stage
    private int nEnemies;
    // Stores the click order of the Enemies
    private List<Minion> clickOrder;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        clickOrder = new List<Minion>();
        SPEED = speed;
        GAMESTATUS = GAME_STATUS_LIVE;
        GenerateStage(50);
    }

    // Update is called once per frame
    void Update()
    {
        SPEED = speed; // DEBUG

        // If all the Enemies have been clicked
        if (clickOrder.Count == nEnemies)
        {
            // Adds all the Enemies to the Player target list
            foreach (Minion enemy in clickOrder)
                player.GetComponent<Player>().AddMinion(enemy);
            nEnemies = 0;
        }
    }

    /// <summary>
    /// Generates a pseudorandom stage
    /// </summary>
    /// <param name="delayMilis">The initial delay of the stage</param>
    private void GenerateStage(int delayMilis)
    {
        // Generates a random number of Enemies
        nEnemies = Random.Range(3, 6);
        // List of possible values
        List<int> positions = new List<int>(new int[] { 0, 1, 2, 3, 4, 5 });
        // List of possible target values
        List<int> targetPositions = new List<int>(new int[] { 0, 1, 2, 3, 4, 5 });
        for (int ii = 0; ii < nEnemies; ii++)
        {
            // Loads the Minion prefab
            GameObject enemy = (GameObject)Instantiate(Resources.Load("Minion"));
            // Sets the speed of the Minion
            enemy.GetComponent<Minion>().Speed = 0.5f;
            // Sets the enter delay of the Minion
            enemy.GetComponent<Minion>().Delay = delayMilis;
            // Sets the order click of the Minion
            enemy.GetComponent<Minion>().Index = ii;
            // Randomizes a position and removes it from the list so it cannot be repeated
            int pos = positions[Random.Range(0, positions.Count)];
            // Randomizes a target position and removes it from the list so it cannot be repeated
            int targetPos = targetPositions[Random.Range(0, targetPositions.Count)];
            positions.Remove(pos);
            targetPositions.Remove(targetPos);
            // Sets the initial position from the initposition array
            enemy.GetComponent<Minion>().InitPosition = InitPositions[pos];
            // Sets the target position from the targetposition array
            enemy.GetComponent<Minion>().AddTargetPosition(TargetPositions[targetPos]);
            // Adds a delay for the next Minion
            delayMilis += 20;
        }
    }

    /// <summary>
    /// Adds the Minion to the click order list
    /// </summary>
    /// <param name="enemy">The Minion to add</param>
    public void NotifyClick(Minion enemy)
    {
        clickOrder.Add(enemy);
    }
}
