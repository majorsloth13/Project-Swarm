using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject GameMenu;
    public static bool gameActive;
    #region Custom Methods

    // Loads the given game scene.

    public void PlayGame()
    {

        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }

    public void PlayAnimation()
    {
        SceneManager.LoadScene(2);
    }

    //public void MainMenuBack()
    //{
    //SceneManager.LoadScene(0);

    //}








    // Called to quit the game.

    public void QuitGame()
    {
        Application.Quit(); // Closes the built application.
        Debug.Log("Quit");
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //MainMenuBack();
            ToggleMenu();
            Debug.Log("mainMEnu works");

        }
    }

    public void ToggleMenu()
    {
        Debug.Log("mainMEnu works");

        bool currentState = GameMenu.activeSelf;
        GameMenu.SetActive(!currentState);

        if (currentState != true)
        {
            gameActive = false;

            Time.timeScale = 0f;
        }
        else
        {
            gameActive = true;
            Time.timeScale = 1f;
        }

    }
}
