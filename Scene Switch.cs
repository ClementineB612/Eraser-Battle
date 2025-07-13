using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingGame : MonoBehaviour
{
    public void LoadGame()
    {
        Application.LoadLevel("Game-Score");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void QuitToMenu()
    {
        Application.LoadLevel("GameStart");
    }
    public void Rule()
    {
        Application.LoadLevel("Rule");
    }
}