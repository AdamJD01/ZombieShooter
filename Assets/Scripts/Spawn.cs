//used to spawn enemies (Zombies) in a level
using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{
    //variables
    public GameObject zombieGameObject; //is set to a Zombie in the editor

    public int numberOfZombies; //the amount of Zombies to spawn, number can be changed in the editor (0 is default)

    public float spawnRadius; //the radius that Zombies spawn around this object, number can be changed in editor (0 is default)

    public bool spawnAtStart; //if the Zombies should spawn when level is loaded or not, bool can be set true/false in editor


    void Start()
    {
        //create the Zombies instantly if set to true
        if (spawnAtStart)
        {
            SpawnAllZombies(); 
        }
    }

    void Update()
    {
        //meant to be empty
    }

    void OnTriggerEnter(Collider collider)
    {
        //create zombies when Steve is in range if set to false and only if Steve is the one touching it
        if (!spawnAtStart && collider.gameObject.tag == "Player")
        {
            SpawnAllZombies();
        }
    }

    protected void SpawnAllZombies()
    {
        for (int i = 0; i < numberOfZombies; i++)
        {
            Vector3 randomPosition = transform.localPosition + Random.insideUnitSphere * spawnRadius; //make the position random within the spawn radius
            NavMeshHit navMeshHit;

            if (NavMesh.SamplePosition(randomPosition, out navMeshHit, 10.0f, NavMesh.AllAreas))
            {
                Instantiate(zombieGameObject, navMeshHit.position, Quaternion.identity); //create a copy of the zombie if safe to do so
            }

            else
            {
                i--; //if not safe (such as the zombie spawn position is out of bounds) then wait until it is safe
            }
        }
    }
}
