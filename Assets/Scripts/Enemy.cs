using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector2 InitPosition { get; set; }
    public Vector2 TargetPosition { get; set; }

    public float Speed { get; set; }
    public int Index { get; set; }

    private int delay;
    public int Delay { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        delay = 0;
        transform.position = InitPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (delay < Delay)
            delay += 1;
        if (TargetPosition != null && delay >= Delay && TargetPosition != (Vector2)transform.position)
            transform.position = Vector2.MoveTowards(transform.position, TargetPosition, Speed * GameRuler.SPEED);
    }

    public bool Kill()
    {
        StartCoroutine(Blood());
        return true;
    }

    IEnumerator Blood()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject blood = (GameObject)Instantiate(Resources.Load("blood"));
        blood.transform.SetParent(transform);
        blood.transform.localPosition = new Vector2(0, 0.2f);
    }

    private void OnMouseDown()
    {
        GameObject.Find("GameRuler").GetComponent<GameRuler>().NotifyClick(this);
        GameObject select = (GameObject)Instantiate(Resources.Load("select"));
        select.transform.SetParent(transform);
        select.transform.localPosition = new Vector2(0, -0.3f);
        Destroy(GetComponent<BoxCollider2D>());
    }
}
