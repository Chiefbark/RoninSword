using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Material m in GetComponent<Renderer>().materials)
            m.mainTextureOffset += new Vector2(0, 0.01f * GameRuler.SPEED);
    }
}
