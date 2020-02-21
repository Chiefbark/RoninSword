﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRuler : MonoBehaviour
{
    [SerializeField]
    private float speed; // DEBUG

    private GameObject player;

    public static int GAMESTATUS;
    private int prevGameStatus;

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
    private List<Enemy> clickOrder;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        clickOrder = new List<Enemy>();
        SPEED = speed;
        GAMESTATUS = GAME_STATUS_LIVE;
        prevGameStatus = GAMESTATUS;

        GenerateStage(50);

        if (AppManager.VOLUME == AppManager.VOLUME_MIN)
            GameObject.Find("Sound").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/volume_off");
    }

    // Update is called once per frame
    void Update()
    {
        SPEED = speed; // DEBUG

        GetComponents<AudioSource>()[0].volume = AppManager.VOLUME;
        GetComponents<AudioSource>()[1].volume = AppManager.VOLUME;
        GetComponents<AudioSource>()[0].pitch = SPEED;
        GetComponents<AudioSource>()[1].pitch = SPEED;

        // Detects whenever the game status changes
        if (GAMESTATUS != prevGameStatus)
            OnGameStatusChanged(GAMESTATUS);

        prevGameStatus = GAMESTATUS;

        // If all the Enemies have been clicked
        if (clickOrder.Count == nEnemies * 2)
        {
            // Adds all the Enemies to the Player target list
            foreach (Enemy enemy in clickOrder)
                player.GetComponent<Player>().AddEnemy(enemy);
            nEnemies = 0;
        }
    }

    /// <summary>
    /// Generates a pseudorandom stage
    /// </summary>
    /// <param name="delayMilis">The initial delay of the stage</param>
    private void GenerateStage(int delayMilis)
    {
        // TODO: improve radnom stage generator
        // TODO: remove hardcoded values for boss
        // Generates a random number of Enemies
        nEnemies = Random.Range(3, 3);
        // List of possible values
        List<int> positions = new List<int>(new int[] { 0, 1, 2, 3, 4, 5 });
        // List of possible target values
        List<int> targetPositions = new List<int>(new int[] { 0, 1, 2, 3, 4, 5 });

        GameObject boss = (GameObject)Instantiate(Resources.Load("Prefabs/Boss"));
        boss.GetComponent<Boss>().Speed = 0.1f;
        boss.GetComponent<Boss>().Delay = delayMilis - 20;
        boss.GetComponent<Boss>().MaxClick = nEnemies;
        boss.GetComponent<Boss>().InitPosition = new Vector2(0, 7);
        boss.GetComponent<Boss>().AddTargetPosition(Vector2.zero);

        int index = 0;
        for (int ii = 0; ii < nEnemies; ii++)
        {
            // Loads the Enemy prefab
            GameObject enemy = (GameObject)Instantiate(Resources.Load("Prefabs/Minion"));
            // Sets the speed of the Enemy
            enemy.GetComponent<Minion>().Speed = 0.5f;
            // Sets the enter delay of the Enemy
            enemy.GetComponent<Minion>().Delay = delayMilis;
            // Sets the order click of the Enemy
            enemy.GetComponent<Minion>().Index = index;
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
            // Adds a delay for the next Enemy
            delayMilis += 20;
            index += 2;
        }
    }

    /// <summary>
    /// Adds the Enemy to the click order list
    /// </summary>
    /// <param name="enemy">The Enemy to add</param>
    public void NotifyClick(Enemy enemy)
    {
        clickOrder.Add(enemy);
    }

    private void OnGameStatusChanged(int newStatus)
    {
        if (GAMESTATUS == GAME_STATUS_OVER)
        {
            GetComponents<AudioSource>()[1].Play();
        }
    }

    /// <summary>
    /// Handles the behaviour of the button Sound
    /// </summary>
    public void OnVolumeClicked()
    {
        if (AppManager.VOLUME == AppManager.VOLUME_MAX)
        {
            AppManager.VOLUME = AppManager.VOLUME_MIN;
            GameObject.Find("Sound").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/volume_off");
        }
        else
        {
            AppManager.VOLUME = AppManager.VOLUME_MAX;
            GameObject.Find("Sound").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/volume_up");
        }
    }
}
