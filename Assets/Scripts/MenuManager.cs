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
        GetComponent<AudioSource>().volume = AppManager.VOLUME;

        GameObject.Find("Background").GetComponent<Image>().material.mainTextureOffset += new Vector2(0.0005f, 0);
    }

    /// <summary>
    /// Handles the behaviour of the button Play
    /// </summary>
    public void OnPlayClicked()
    {
        SceneManager.LoadScene(AppManager.SCENE_GAME);
    }

    /// <summary>
    /// Handles the behaviour of the button Options
    /// </summary>
    public void OnOptionsClicked()
    {

    }

    /// <summary>
    /// Handles the behaviour of the button Quit
    /// </summary>
    public void OnQuitClicked()
    {
        Application.Quit();
    }

    /// <summary>
    /// Handles the biehaviour of the button Sound
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
