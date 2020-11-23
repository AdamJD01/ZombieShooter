//used to determine if and when the main menu should load
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteveModel : MonoBehaviour
{
    //public AudioSource steveModelDie;
    //public AudioSource steveModelVictory;


    void Start()
    {
        if (GameStats.wonGame)
        {
            Invoke("SwitchToMainMenu", 18); //go to main menu and wait until victory stuff is done if the player has won 
        }

        else if (GameStats.isDead)
        {
            Invoke("SwitchToMainMenu", 10); //go to main menu and wait until death stuff is done if the player has died 
        }
    }

    void Update()
    {
        //meant to be empty
    }

    void SwitchToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); //load the main menu scene/level
    }

    /*
    public void PlaySteveModelDieSound()
    {
        steveModelDie.Play();
    }
    */

    /*
    public void PlaySteveModelVictoryMusic()
    {
        steveModelVictory.Play();
    }
    */
}
