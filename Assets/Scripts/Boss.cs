using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    // Maximum number of clicks on the Boss (number of Minions in game)
    public int MaxClick
    {
        get => maxClick;
        set
        {
            maxClick = value;
            for (int ii = 1; ii < maxClick * 2; ii += 2)
                Indexes.Enqueue(ii);
        }
    }
    private int maxClick;
    // The indexes of the Boss, indicates the order
    public Queue<int> Indexes = new Queue<int>();
    private int nClick; // Click counter

    public override bool Kill(int expectedIndex)
    {
        int index = Indexes.Dequeue();
        // If both indexes are equal
        if (expectedIndex == index)
        {
            // Delays the blood effect
            // Once the blood effect is gone, it will active the death behaviour
            StartCoroutine(Blood());
            return true;
        }
        // Enables the attack flag
        Attack = true;
        return false;
    }

    /// <summary>
    /// Creates the blood effect
    /// </summary>
    /// <returns></returns>
    private IEnumerator Blood()
    {
        yield return new WaitForSeconds(0.2f * (1 / GameRuler.SPEED));
        // Loads and places the blood effect
        GameObject blood = (GameObject)Instantiate(Resources.Load("blood"));
        blood.GetComponent<Animator>().SetFloat("speed", GameRuler.SPEED);
        blood.transform.SetParent(transform);
        blood.transform.localPosition = new Vector2(0, 0.1f);
    }

    protected override void OnMouseDown()
    {
        if (GameRuler.GAMESTATUS == GameRuler.GAME_STATUS_LIVE)
        {
            nClick++;
            // Notifies the GameRuler that this Enemy has been clicked
            GameObject.Find("GameRuler").GetComponent<GameRuler>().NotifyClick(this);
            // Loads and places the select effect
            GameObject select = (GameObject)Instantiate(Resources.Load("select"));
            select.transform.SetParent(transform);
            select.transform.localPosition = new Vector2(0, -0.3f);
            if (nClick == MaxClick)
                // Destroys the collider so the Enemy cannot be clicked again
                Destroy(GetComponent<BoxCollider2D>());
        }
    }
}
