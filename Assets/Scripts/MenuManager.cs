using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayMenu()
    {
        SceneManager.LoadScene(AppManager.GAME_SCENE);
    }

    public void OptionsMenu()
    {

    }

    public void QuitMenu()
    {
        Application.Quit();
    }
}
