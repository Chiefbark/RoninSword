using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public override bool Kill(int expectedIndex)
    {
        return true;
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
        blood.transform.localPosition = new Vector2(0, 0.2f);
    }
}
