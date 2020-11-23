//a radar used to show Steve and other game objects to the player
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//setup the radar objects
public class RadarObject
{
    public Image icon
    {
        get;
        set;
    }

    public GameObject owner
    {
        get;
        set;
    }
}

public class Radar : MonoBehaviour
{
    public static List<RadarObject> radarObjects = new List<RadarObject>(); //the list of radar objects

    public Transform steveLocation; //find Steves location, is set up in the editor

    public float radarScale = 2.0f; //how far apart objects are on radar, is set up in the editor


    public static void RegisterRadarObject(GameObject g, Image i)
    {
        //add images to the radar
        Image image = Instantiate(i);

        radarObjects.Add
        (new RadarObject()
        {
            owner = g,
            icon = image,
        }
        );
    }

    public static void UnregisterRadarObject(GameObject g)
    {
        //remove images from the radar
        List<RadarObject> newRadarList = new List<RadarObject>();

        for (int i = 0; i < radarObjects.Count; i++)
        {
            if(radarObjects[i].owner == g)
            {
                Destroy(radarObjects[i].icon);
                continue;
            }

            else
            {
                newRadarList.Add(radarObjects[i]);
            }
        }

        radarObjects.RemoveRange(0, radarObjects.Count);
        radarObjects.AddRange(newRadarList);
    }

    void Start()
    {
        //meant to be empty
    }

    void Update()
    {
        if (steveLocation == null)
        {
            return;
        }

        //show how far away from Steve objects in the level are on radar, acts like a compass
        foreach (RadarObject ro in radarObjects)
        {
            Vector3 compass = ro.owner.transform.position - steveLocation.localPosition;
            float distance = Vector3.Distance(steveLocation.localPosition, ro.owner.transform.position) * radarScale;

            float deltay = Mathf.Atan2(compass.x, compass.z) * Mathf.Rad2Deg - 270 - steveLocation.eulerAngles.y;
            compass.x = distance * Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
            compass.z = distance * Mathf.Sin(deltay * Mathf.Deg2Rad);

            ro.icon.transform.SetParent(transform);

            RectTransform rt = GetComponent<RectTransform>();
            ro.icon.transform.position = new Vector3(compass.x + rt.pivot.x, compass.z + rt.pivot.y, 0) + transform.position;
        }
    }

}
