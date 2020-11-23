//the behaviour of enemies (Zombies) 
using UnityEngine;
using UnityEngine.AI; 

public class Zombie : MonoBehaviour
{
    //variables
    public Animator zombieAnimator; //is set to each Zombies animator in the editor

    public NavMeshAgent zombieNavMeshAgent; //is set to each Zombies navmesh agent in the editor

    public float walkingSpeed; //walking speed for each Zombie, can be changed in the editor (0 is default)
    public float runningSpeed; //running speed for each Zombie, can be changed in the editor (0 is default)
    public float damageAmount = 10; //damageAmount each Zombie does to Steve, can be changed in the editor
   
    public int shotsToKill = 1; //shots needed to kill a Zombie, can be changed in the editor

    public GameObject steveGameObject; //is set to Steve in the editor
    public GameObject zombieRagdoll; //is set to ragdoll in the editor

    private Steve steve;

    private AudioSource[] zombieSounds;

    //set up the Zombie states
    private enum State
    {
        Idle,
        Wander,
        Chase,
        Attack,
        Dead,
    };
    private State state;


    void Start()
    {
        state = State.Idle; //go to idle state straight away

        steve = FindObjectOfType<Steve>(); //call Steve

        zombieSounds = GetComponents<AudioSource>(); //find the Zombie sounds
    }

    void Update()
    {
        //find Steve
        if (steveGameObject == null && GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
        {
            steveGameObject = GameObject.FindWithTag("Player");

            //try to turn volume off 
            foreach (AudioSource s in zombieSounds)
            {
                s.volume = 0;
            }

            return;
        }

        //choose which state to be in
        switch (state) 
        {
            case State.Idle:
                /*
                foreach (AudioSource s in zombieSounds)
                {
                    s.volume = 0;
                }
                */

                if (CanSeeSteve())
                {
                    state = State.Chase; //chase Steve if Zombie can see him
                }
                else if ((Random.Range(0, 5000) < 20))
                {
                    state = State.Wander; //go back and fore between states wander and idle
                }
                break;

            case State.Wander:
                if (!zombieNavMeshAgent.hasPath)
                {
                    /*
                    foreach (AudioSource s in zombieSounds)
                    {
                        s.volume = 0;
                    }
                    */

                    //make Zombies walk in a random direction instead of a straight line
                    float XPosition = transform.localPosition.x + Random.Range(-5, 5);
                    float ZPosition = transform.localPosition.z + Random.Range(-5, 5);
                    float YPosition = Terrain.activeTerrain.SampleHeight(new Vector3(XPosition, 0, ZPosition));

                    //set the zombies destination
                    Vector3 destination = new Vector3(XPosition, YPosition, ZPosition);
                    zombieNavMeshAgent.SetDestination(destination);

                    //turn on the animation
                    TurnOffBools();
                    zombieNavMeshAgent.speed = walkingSpeed;
                    zombieAnimator.SetBool("isWalking", true);
                }
                else if (CanSeeSteve())
                {
                    state = State.Chase; //chase Steve if Zombie can see him
                }
                else if ((Random.Range(0, 5000) < 20))
                {
                    state = State.Idle; //go back and fore between states wander and idle
                    TurnOffBools();
                    zombieNavMeshAgent.ResetPath();
                }
                break;

            case State.Chase:
                if(GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
                {
                    TurnOffBools();
                    zombieNavMeshAgent.speed = 0; //stop moving if Steve has fainted, is dead or won
                    state = State.Wander; //go back to the wander state if Steve has fainted, is dead or won

                    //try to turn volume off 
                    foreach (AudioSource s in zombieSounds)
                    {
                        s.volume = 0;
                    }

                    return;
                }
                //set the zombies destination
                zombieNavMeshAgent.SetDestination(steveGameObject.transform.localPosition);

                //turn on the animation
                TurnOffBools();
                zombieNavMeshAgent.speed = runningSpeed;
                zombieAnimator.SetBool("isRunning", true);

                /*
                foreach (AudioSource s in zombieSounds)
                {
                    s.volume = 1;
                }
                */

                if ((zombieNavMeshAgent.remainingDistance <= zombieNavMeshAgent.stoppingDistance) && !zombieNavMeshAgent.pathPending)
                {
                    state = State.Attack; //attack Steve if near enough to him
                }
                else if (LostSteve())
                {
                    TurnOffBools();
                    state = State.Wander; //go to the wander state
                    zombieNavMeshAgent.ResetPath();
                }
                break;

            case State.Attack:
                if (GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
                {
                    TurnOffBools();
                    state = State.Wander; //go back to the wander state if Steve has fainted, is dead or won

                    //try to turn volume off 
                    foreach (AudioSource s in zombieSounds)
                    {
                        s.volume = 0;
                    }

                    return;
                }
                TurnOffBools();
                zombieAnimator.SetBool("isAttacking", true); //turn on the animation
                Vector3 Steve2D = new Vector3(steveGameObject.transform.localPosition.x, 0, steveGameObject.transform.localPosition.z);
                transform.LookAt(Steve2D); //stop Zombie leaning on it's back when Steve gets near
                
                /*
                foreach (AudioSource s in zombieSounds)
                {
                    s.volume = 1;
                }
                */

                if (DistanceToSteve() > zombieNavMeshAgent.stoppingDistance + 2)
                {
                    TurnOffBools();
                    state = State.Chase; //go back to the chasing state if Steve is out of range for attacking
                }
                break;

            case State.Dead:
                Destroy(zombieNavMeshAgent);

                //turn sounds off if the Zombie is dead
                foreach(AudioSource s in zombieSounds)
                {
                    s.volume = 0;
                }

                GetComponent<SinkZombie>().StartToSink(); //start to sink the dead Zombie
                break;

            default:
                break;
        }
    }

    protected float DistanceToSteve()
    {
        if(GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
        {
            return Mathf.Infinity;
        }
        return Vector3.Distance(steveGameObject.transform.localPosition, transform.localPosition); //distance from Zombie to Steve if not fainted, dead or won
    }

    protected bool CanSeeSteve()
    {
        if (DistanceToSteve() < 10)
        {
            return true; //Zombie can see Steve
        }

        return false;
    }

    public void DamageSteve()
    {
        if(steve == null)
        {
            return;
        }

        steve.SteveTakeDamage(damageAmount); //damage Steve if he is in the level
    }

    protected bool LostSteve()
    {
        if (DistanceToSteve() > 20)
        {
            return true; //Zombie has lost steve
        }

        return false;
    }

    protected void TurnOffBools()
    {
        //turn off the animations
        zombieAnimator.SetBool("isWalking", false);
        zombieAnimator.SetBool("isRunning", false);
        zombieAnimator.SetBool("isAttacking", false);
        zombieAnimator.SetBool("isDead", false);
    }

    public void KillZombieAnimation()
    {
        //Zombie is dead
        TurnOffBools();
        zombieAnimator.SetBool("isDead", true);
        state = State.Dead;
    }
}
