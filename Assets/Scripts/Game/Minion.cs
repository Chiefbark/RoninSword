﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Enemy
{
    // The index of the Minion, indicates the order
    public int Index { get; set; }

    public override bool Kill(int expectedIndex)
    {
        // If both indexes are equal
        if (expectedIndex == Index)
        {
            // Delays the blood effect
            // Once the blood effect is gone, it will active the death behaviour
            StartCoroutine(Blood());
            return true;
        }
        // Enables the attack flag
        Attack = true;
        // Playes the sound effect asociated
        GetComponent<AudioSource>().Play();
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
        GameObject blood = (GameObject)Instantiate(Resources.Load("Prefabs/blood"));
        blood.GetComponent<Animator>().SetFloat("speed", GameRuler.SPEED);
        blood.transform.SetParent(transform);
        blood.transform.localPosition = Vector2.zero;
    }
}
