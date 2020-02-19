using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BasicScriptBehaviour
{
    public Vector2 InitPosition { get; set; }
    public Vector2 TargetPosition { get; set; }

    public float Speed { get; set; }
    public int Index { get; set; }

    private int delay;
    public int Delay { get; set; }

    protected override void OnStart()
    {
        delay = 0;
        transform.position = InitPosition;
    }

    protected override void StatusLiveBehaviour()
    {
        if (delay < Delay)
            delay += 1;
        if (TargetPosition != null && delay >= Delay && TargetPosition != (Vector2)transform.position)
            transform.position = Vector2.MoveTowards(transform.position, TargetPosition, Speed * GameRuler.SPEED);
    }

    protected override void StatusStopBehaviour()
    {
        // TODO: stop select & blood animation
    }

    protected override void StatusOverBehaviour()
    {
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
            // Kills the Enemy
            StartCoroutine(Blood());
            return true;
        }
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
