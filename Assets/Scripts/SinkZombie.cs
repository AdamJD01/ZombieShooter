//removes Zombies from a scene if they are dead
using UnityEngine;

public class SinkZombie : MonoBehaviour
{
    private float destroyHeight; //when the zombie gets destroyed
    public float delayTime = 10; //the default delayTime value, can be changed in the editor


    void Start()
    {
        if (gameObject.tag == "Ragdoll")
        {
            Invoke("StartToSink", 5); //start the sinking process
        }
    }

    void Update()
    {
        //meant to be empty
    }

    public void StartToSink()
    {
        destroyHeight = Terrain.activeTerrain.SampleHeight(transform.localPosition) - 5;
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            Destroy(c); //turn off colliders
        }

        InvokeRepeating("SinkIntoGround", delayTime, 0.5f); //keep repeating until zombie is gone
    }

    void SinkIntoGround()
    {
        transform.Translate(0, -0.01f, 0); //slowly goes into the ground

        if(transform.localPosition.y < destroyHeight)
        {
            Destroy(gameObject); //destroy the zombie
        }
    }
}
