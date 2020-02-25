using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameRuler : MonoBehaviour
{
    [SerializeField]
    private GameObject Canvas;
    [SerializeField]
    private GameObject Menus;
    [SerializeField]
    private GameObject GameOver;
    [SerializeField]
    private GameObject Help;

    private GameObject Player;

    public static int GAMESTATUS = 2;
    private int prevGameStatus;

    public static int GAME_STATUS_STOP = -1;
    public static int GAME_STATUS_OVER = 0;
    public static int GAME_STATUS_LIVE = 1;
    public static int GAME_STATUS_LOADING = 2;

    public static int DIRECTION_RIGHT = -2;
    public static int DIRECTION_LEFT = -1;
    public static int DIRECTION_NONE = 0;
    public static int DIRECTION_BOTTOM = 1;
    public static int DIRECTION_TOP = 2;

    public static int MESSAGE_DEFAULT = 0;
    private bool isMessageDefaultShown; // If the default help message has been shown
    public static int MESSAGE_BOSS = 1;
    private bool isMessageBossShown;    // If the boss help message has been shown

    //TODO: implement slow motion
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
    // Total number of Enemies that has been placed
    private int enemiesPlaced;
    // Stores the click order of the Enemies
    private List<Enemy> clickOrder = new List<Enemy>();

    // If the stage has a boss
    private bool hasBoss;
    // If the stage has a feint
    private bool hasFeint;
    // If the stage has end
    private bool isStageEnd;
    // Count of stages
    private int stages = 0;

    // Start is called before the first frame update
    void Start()
    {
        Player = Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        prevGameStatus = GAMESTATUS;

        GenerateStage(50);
        isStageEnd = false;

        if (AppManager.VOLUME == AppManager.VOLUME_MIN)
            GameObject.Find("Sound").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/volume_off");
    }

    // Update is called once per frame
    void Update()
    {
        GetComponents<AudioSource>()[0].volume = AppManager.VOLUME;
        GetComponents<AudioSource>()[1].volume = AppManager.VOLUME;
        GetComponents<AudioSource>()[0].pitch = SPEED;
        GetComponents<AudioSource>()[1].pitch = SPEED;

        // Detects whenever the game status changes
        if (GAMESTATUS != prevGameStatus)
            OnGameStatusChanged(GAMESTATUS);

        prevGameStatus = GAMESTATUS;

        // If the stage has end
        if (isStageEnd)
        {
            GAMESTATUS = GAME_STATUS_LOADING;
            // Generates the next stage
            GenerateStage(150);
            isStageEnd = false;

            // If the message dialog of the boss has not been shown
            if (hasBoss && !isMessageBossShown)
            {
                ShowHelpDialog(MESSAGE_BOSS, 2);
                isMessageBossShown = true;
            }
        }
        else
        {
            int count = nEnemies;
            if (hasBoss)
                count *= 2;
            // If all the Enemies have been clicked
            if (clickOrder.Count == count)
            {
                // Adds all the Enemies to the Player target list
                foreach (Enemy enemy in clickOrder)
                    Player.GetComponent<Player>().AddEnemy(enemy);
                nEnemies = 0;
                clickOrder.Clear();
            }
        }
    }

    /// <summary>
    /// Generates a pseudorandom stage
    /// </summary>
    /// <param name="delayMilis">The initial delay of the stage</param>
    private void GenerateStage(int delayMilis)
    {
        // Generates a random number of Enemies
        nEnemies = Random.Range(3, stages == 0 ? 4 : hasBoss ? 4 : 6);
        // List of possible values
        List<int> positions = new List<int>(new int[] { 0, 1, 2, 3, 4, 5 });
        // List of possible target values
        List<int> targetPositions = new List<int>(new int[] { 0, 1, 2, 3, 4, 5 });

        // Creates a Boss
        if (stages > 0 && stages % 3 == 0)
        {
            // Loads the Boss prefab
            GameObject boss = Instantiate(Resources.Load<GameObject>("Prefabs/Boss"));
            // Sets the speed of the Boss
            boss.GetComponent<Boss>().Speed = 0.1f;
            // Sets the enter delay of the Boss
            boss.GetComponent<Boss>().Delay = delayMilis - 20;
            boss.GetComponent<Boss>().MaxClick = nEnemies;
            // Sets the initial position from the initposition array
            boss.GetComponent<Boss>().InitPosition = new Vector2(0, 7);
            // Sets the target position from the targetposition array
            boss.GetComponent<Boss>().AddTargetPosition(Vector2.zero);
            hasBoss = true;
        }
        else
            hasBoss = false;

        // Creates a Minion doing a feint
        if (stages > 4 && Random.Range(1, 4) == 1)
        {
            GameObject enemy = Instantiate(Resources.Load<GameObject>("Prefabs/Minion"));
            // Sets the speed of the Minion
            enemy.GetComponent<Minion>().Speed = 0.25f;
            // Sets the enter delay of the Minion
            enemy.GetComponent<Minion>().Delay = delayMilis;
            // Randomizes a position and removes it from the list so it cannot be repeated
            int pos = positions[Random.Range(0, positions.Count)];
            positions.Remove(pos);
            // Sets the initial position from the initposition array
            enemy.GetComponent<Minion>().InitPosition = InitPositions[pos];
            // Randomizes a target position and removes it from the list so it cannot be repeated
            int targetPos1 = targetPositions[Random.Range(0, targetPositions.Count)];
            targetPositions.Remove(targetPos1);
            // Sets the target position from the targetposition array
            enemy.GetComponent<Minion>().AddTargetPosition(TargetPositions[targetPos1]);
            // Randomizes a target position
            Vector2 targetPos2 = InitPositions[Random.Range(0, InitPositions.Length)];
            // Sets the second target position from the targetposition array
            enemy.GetComponent<Minion>().AddTargetPosition(targetPos2);
            hasFeint = true;
        }
        else
            hasFeint = false;

        int index = 0;
        for (int ii = 0; ii < nEnemies; ii++)
        {
            // Loads the Minion prefab
            GameObject enemy = Instantiate(Resources.Load<GameObject>("Prefabs/Minion"));
            // Sets the speed of the Minion
            enemy.GetComponent<Minion>().Speed = 0.5f;
            // Sets the enter delay of the Minion
            enemy.GetComponent<Minion>().Delay = delayMilis;
            // Sets the order click of the Minion
            enemy.GetComponent<Minion>().Index = index;
            // Randomizes a position and removes it from the list so it cannot be repeated
            int pos = positions[Random.Range(0, positions.Count)];
            positions.Remove(pos);
            // Sets the initial position from the initposition array
            enemy.GetComponent<Minion>().InitPosition = InitPositions[pos];
            // Randomizes a target position and removes it from the list so it cannot be repeated
            int targetPos = targetPositions[Random.Range(0, targetPositions.Count)];
            targetPositions.Remove(targetPos);
            // Sets the target position from the targetposition array
            enemy.GetComponent<Minion>().AddTargetPosition(TargetPositions[targetPos]);
            // Adds a delay for the next Enemy
            delayMilis += 20;

            index += hasBoss ? 2 : 1;
        }
        stages++;
        GameObject.Find("Stage").GetComponent<Text>().text = "Stage " + stages;
    }

    /// <summary>
    /// Adds the Enemy to the click order list
    /// </summary>
    /// <param name="enemy">The Enemy to add</param>
    public void NotifyClick(Enemy enemy)
    {
        clickOrder.Add(enemy);
    }

    /// <summary>
    /// Notifies the GameRuler that the stage has end
    /// </summary>
    public void NotifyStageEnd()
    {
        isStageEnd = true;
        enemiesPlaced = 0;
    }

    /// <summary>
    /// Notifies the GameRuler that the Enemy has been placed
    /// </summary>
    public void NotifyEnemyPlaced()
    {
        enemiesPlaced++;
        if (enemiesPlaced == nEnemies + (hasFeint ? 1 : 0))
        {
            GAMESTATUS = GAME_STATUS_LIVE;
            if (!isMessageDefaultShown && !hasBoss)
            {
                ShowHelpDialog(MESSAGE_DEFAULT);
                isMessageDefaultShown = true;
            }
        }
    }

    /// <summary>
    /// Shows the help dialog
    /// </summary>
    /// <param name="msgType">If the message is the default or the boss one</param>
    /// <param name="delay">The delay oat which the dilog will show</param>
    private void ShowHelpDialog(int msgType, float delay = 0)
    {
        StartCoroutine(HelpDialog(msgType, delay));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msgType"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator HelpDialog(int msgType, float delay)
    {
        yield return new WaitForSeconds(delay);
        GAMESTATUS = GAME_STATUS_STOP;
        Help.SetActive(true);
        string text = "";
        if (msgType == MESSAGE_DEFAULT)
            text = "The best way to overcome these guys is to kill them in order";
        if (msgType == MESSAGE_BOSS)
            text = "Hmmm, it seems like the big boss is coming. It may take a few more hits than usual...\n(enemy, boss, enemy, boss, ...)";
        GameObject.Find("HelpMessage").GetComponent<Text>().text = text;
    }

    /// <summary>
    /// Hides the help dialog
    /// </summary>
    public void HideHelpDialog()
    {
        GAMESTATUS = GAME_STATUS_LIVE;
        Help.SetActive(false);
    }

    private void OnGameStatusChanged(int newStatus)
    {
        if (GAMESTATUS == GAME_STATUS_OVER)
        {
            GetComponents<AudioSource>()[1].Play();
            GameOver.SetActive(true);
        }
    }

    /// <summary>
    /// Handles the behaviour of the button Sound
    /// </summary>
    public void OnVolumeClicked()
    {
        Canvas.GetComponent<AudioSource>().Play();
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

    /// <summary>
    /// Handles the behaviour of the button Pause
    /// </summary>
    public void OnPauseClicked()
    {
        if (GAMESTATUS != GAME_STATUS_OVER)
        {
            Canvas.GetComponent<AudioSource>().Play();
            Menus.SetActive(true);
            GAMESTATUS = GAME_STATUS_STOP;
        }
    }

    /// <summary>
    /// Handles the behaviour of the button Resume
    /// </summary>
    public void OnResumeClicked()
    {
        Canvas.GetComponent<AudioSource>().Play();
        Menus.SetActive(false);
        GAMESTATUS = GAME_STATUS_LIVE;
    }

    /// <summary>
    /// Handles the behaviour of the button Restart
    /// </summary>
    public void OnRestartClicked()
    {
        Canvas.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(AppManager.SCENE_GAME);
        GAMESTATUS = GAME_STATUS_LIVE;
    }

    /// <summary>
    /// Handles the behaviour of the button Quit
    /// </summary>
    public void OnQuitClicked()
    {
        Canvas.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(AppManager.SCENE_MENU);
        GAMESTATUS = GAME_STATUS_LIVE;
    }
}
