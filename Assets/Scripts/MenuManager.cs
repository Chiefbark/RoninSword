using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<AudioSource>().volume = AppManager.VOLUME_MUSIC;

        GameObject.Find("Background").GetComponent<Image>().material.mainTextureOffset += new Vector2(0.0005f, 0);
    }

    public void PlayMenu()
    {
        SceneManager.LoadScene(AppManager.SCENE_GAME);
    }

    public void OptionsMenu()
    {

    }

    public void QuitMenu()
    {
        Application.Quit();
    }
}
