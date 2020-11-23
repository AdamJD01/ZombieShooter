//used to load scenes/levels from buttons and change music volume
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameButtons : MonoBehaviour
{
    private GameObject gameController; //the controller that travels music

    public Slider musicSlider; //music slider, is set up in the editor


    void Awake()
    {
        gameController = GameObject.Find("GameController"); //find the GameController

        AudioSource music = gameController.GetComponent<AudioSource>(); //find audiosource inside the game controller

        //stop trying to find a game controller or audiosource if there are none there 
        if (gameController == null || music == null)
        {
            return;
        }
        
        //change slider position based on the music volume
        musicSlider.value = music.volume;

        //be able to press the play button again if come from a scene/level
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Start()
    {
       //meant to be empty
    }

    void Update()
    {
        //meant to be empty
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1"); //load first scene/level if pressing the "Play" button
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); //load main menu scene/level if pressing the "X" button
    }

    public void ChangeVolume(float volume)
    {
        gameController = GameObject.Find("GameController"); //find the GameController

        AudioSource music = gameController.GetComponent<AudioSource>(); //find audiosource inside the game controller

        //stop trying to find a game controller or audiosource if there are none there 
        if (gameController == null || music == null)
        {
            return;
        }

        //change music volume based on the current volume position  
        music.volume = volume;
    }
}
