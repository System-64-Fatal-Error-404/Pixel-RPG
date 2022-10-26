using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool newGame;
    [SerializeField] private TextMeshProUGUI text;

    //Sends the player to the game scene
    public void PlayGame()
    {
        if (text.text == "Jaguar")
        {
            SceneManager.LoadScene("Jaguar");
        }
        else if (text.text == "Eagle")
        {
            SceneManager.LoadScene("Eagle");
        }
        else
        {
            SceneManager.LoadScene("Jaguar");
        }
        
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

    public void GoToMainMenu() // pretty self explanatory
    {
        SceneManager.LoadScene("Main Title");
    }
}


