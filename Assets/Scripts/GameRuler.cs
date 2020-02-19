using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuler : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private GameObject player;

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
    private List<Enemy> clickOrder;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        clickOrder = new List<Enemy>();
        SPEED = speed;
        GenerateStage(50);
    }

    // Update is called once per frame
    void Update()
    {
        SPEED = speed;
        if (clickOrder.Count == nEnemies)
        {
            foreach (Enemy enemy in clickOrder)
                player.GetComponent<Player>().AddEnemy(enemy);
            nEnemies = 0;
        }
    }

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
            // Loads the Enemy prefab
            GameObject enemy = (GameObject)Instantiate(Resources.Load("Enemy"));
            // Sets the speed of the Enemy
            enemy.GetComponent<Enemy>().Speed = 0.5f;
            // Sets the enter delay of the Enemy
            enemy.GetComponent<Enemy>().Delay = delayMilis;
            // Sets the order click of the Enemy
            enemy.GetComponent<Enemy>().Index = ii;
            // Randomizes a position and removes it from the list so it cannot be repeated
            int pos = positions[Random.Range(0, positions.Count)];
            // Randomizes a target position and removes it from the list so it cannot be repeated
            int targetPos = targetPositions[Random.Range(0, targetPositions.Count)];
            positions.Remove(pos);
            targetPositions.Remove(targetPos);
            // Sets the initial position from the initposition array
            enemy.GetComponent<Enemy>().InitPosition = InitPositions[pos];
            // Sets the target position from the targetposition array
            enemy.GetComponent<Enemy>().TargetPosition = TargetPositions[targetPos];
            // Adds a delay for the next Enemy
            delayMilis += 20;
        }
    }

    public void NotifyClick(Enemy enemy)
    {
        clickOrder.Add(enemy);
    }
}
