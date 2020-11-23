//used to show the radar image
using UnityEngine;
using UnityEngine.UI;

public class RadarImage : MonoBehaviour
{
    public Image radarImage; //image to show on radar, is set up in the editor


    void Start()
    {
        Radar.RegisterRadarObject(gameObject, radarImage); //register object to the radar
    }

    void Update()
    {
        //meant to be empty
    }

    void OnDestroy()
    {
        Radar.UnregisterRadarObject(gameObject); //unregister object from the radar
    }
}
