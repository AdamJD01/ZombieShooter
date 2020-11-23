//used to save the music volume between scenes/levels 
using UnityEngine;

public class MusicTraveller : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController"); //find all GameControllers

        //only have one game controller in the scene/level
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        //travel controller through scenes/levels
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
       //meant to be empty 
    }

    void Update()
    {
        //meant to be empty
    }
}
