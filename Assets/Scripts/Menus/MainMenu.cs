using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool newGame;

    //Sends the player to the game scene
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
        newGame = true;
    }

  //  public void Tutorial() // If we have a tutorial I can re-add this
  //  {
   //     SceneManager.LoadScene("Tutorial");
   // }

    // function to close the game
    public void QuitGame()
    {
        Application.Quit(); // this only works when the game is built
        Debug.Log("Exiting Game..."); // use this to show functionality in unity editor
    }
}


