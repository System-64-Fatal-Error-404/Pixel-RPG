using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public bool newGame;
    public TextMeshProUGUI text;

    //Sends the player to the game scene
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGame (Do Not Touch)");
        
        newGame = true;
    }

    public void PlaySelected()
    {
        if (text.text == "Jaguar")
        {
            GotoJaguar();
        }
        
        if (text.text == "Eagle")
        {
            GoToEagle();
        }
        
        if (text.text == "Custom Level")
        {
            PlayGame();
        }
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

    public void GoToEagle()
    {
        SceneManager.LoadScene("Eagle");
    }

    public void GotoJaguar()
    {
        SceneManager.LoadScene("Jaguar");
    }
}


