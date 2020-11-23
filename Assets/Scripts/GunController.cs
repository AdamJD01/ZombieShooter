//used by animation event so that the gunshot sound cannot be played again until fire gun animation ends
using UnityEngine;

public class GunController : MonoBehaviour
{
    public AudioSource steveGunshot; //is set to Steves gunshot audiosource in the editor
    public AudioSource steveGunreload; //is set to Steves gun reload audiosource in the editor


    void Start()
    {
        //meant to be empty
    }

    void Update()
    {
        //meant to be empty
    }

    public void CanShoot()
    {
        GameStats.canShoot = true; //Steve can shoot
    }

    public void PlayGunshotSound()
    {
        steveGunshot.Play(); //play the gunshot sound
    }

    public void PlayGunreloadSound()
    {
        steveGunreload.Play(); //play the gun reload sound
    }
}
