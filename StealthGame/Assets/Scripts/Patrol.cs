using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    //the speed in which the enemy patrol moves towards to player
    public float speed = 0.05f;
    //the length of the line of sight cone
    public float lineOfSight = 20;
    //the width of the line of sight cone
    public float maxAngle = 30;
    //the max distance from the target this unit will move to
    public float trackRange = 10;

    public Transform player;
    public Transform head;

    bool pursuing;

    private NavMeshAgent nma;

    public GameObject[] patrolSpots;
    private int patrolIndex;

    private bool isMoving;

    [Header("shooting stuff")]
    public float shootCD;
    public float bulletSpeed;
    private float shootTImer;
    public GameObject bulletThing;
    public GameObject weapon;
    public GameObject armPivot;
    public Transform bulletShootPos;
    public float shootTimerAfterStopping;
    private float shootTimerAfterStoppingTimer;

    public float TIMERFORTHESTOPPING;
    private float actualTimerForTheStopping;


    private void Awake()
    {
        nma = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        patrolIndex = 0;
        isMoving = true;
        shootTImer = 0.0f;
        shootTimerAfterStoppingTimer = shootTimerAfterStopping;
        actualTimerForTheStopping = TIMERFORTHESTOPPING;
    }

    // Update is called once per frame
    void Update()
    {
        shootTImer -= Time.deltaTime;
        //determines the directon to travel based on this units position in contrast to the targets
        Vector3 direction = player.position - this.transform.position;
        //ignore the y axis
        direction.y = 0;
        //bases the angle to check from the head forward on the Z axis
        float angle = Vector3.Angle(direction, head.forward);

        //checks if the player has melded into the shadows
        if (Player.isMelded)
        {
            pursuing = false;
        }

        //checks if the target is in the line of sight
        if (Vector3.Distance(player.position, this.transform.position) < lineOfSight && angle < maxAngle && Player.isMelded == false)
        {
            //raycast stuff
            RaycastHit hitElNumeroUno;
            Vector3 heading = player.transform.position - bulletShootPos.transform.position;
            float distance = heading.magnitude;
            Vector3 elDirection = heading / distance;
            if (Physics.Raycast(bulletShootPos.position, elDirection, out hitElNumeroUno, 25.0f))
            {
                if (hitElNumeroUno.collider.gameObject.tag != "Player")
                {
                    pursuing = false;
                }
                else
                {
                    //slowly turns this unit to look at the target if the target is in line of sight
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                    nma.SetDestination(player.transform.position);
                    pursuing = true;
                }
            }
        }
        else
        {
            //pursuing = false;
            //isMoving = true;
        }
        if (isMoving)
        {
            nma.isStopped = false;

            //dont do this
            //if (Vector3.Distance(transform.position, player.position) <= trackRange)
            //{
            //    isMoving = false;
            //}
            if (pursuing)
            {
                //do a range check to player 
                //then shoot if in range
                //before you shoot set ismoving to false
                //after you shoot set ismoving to true

                if (shootTImer <= 0)
                {
                    shootTimerAfterStoppingTimer -= Time.deltaTime;
                    nma.isStopped = true;
                    isMoving = false;
                    if (shootTimerAfterStoppingTimer <= 0)
                    {
                        //shoot☺
                        GameObject GO = Instantiate(bulletThing, bulletShootPos.position, armPivot.transform.rotation) as GameObject;
                        GO.GetComponent<Rigidbody>().AddForce(weapon.transform.forward * bulletSpeed, ForceMode.Impulse);

                        //shoot
                        //Instantiate(bulletThing, bulletShootPos.position, Quaternion.identity);
                        shootTImer = shootCD;
                        isMoving = true;
                        shootTimerAfterStoppingTimer = shootTimerAfterStopping;
                        shootTImer = shootCD;
                        pursuing = true;
                    }
                }

                //checks if the target is outside the trackrange but still in the line of sight
                if (direction.magnitude > trackRange)
                {
                    //move on the z axix towards the target
                    this.transform.Translate(0, 0, speed);
                }
                else
                {
                    //turns of the pursuing bool if the target gets out of range
                    //pursuing = false;
                }
            }
            else
            {
                nma.SetDestination(patrolSpots[patrolIndex].transform.position);
                if (Vector3.Distance(transform.position, patrolSpots[patrolIndex].transform.position) <= 2.5f)
                {
                    //do wait timer thing under here
                    actualTimerForTheStopping -= Time.deltaTime;
                    if (actualTimerForTheStopping <= 0)
                    {
                        //then do this stuff
                        if (patrolIndex == patrolSpots.Length - 1)
                        {
                            patrolIndex = 0;
                        }
                        else
                        {
                            patrolIndex++;

                        }

                        actualTimerForTheStopping = TIMERFORTHESTOPPING;
                    }

                }
            }
        }
        else
        {
            if (Vector3.Distance(player.position, this.transform.position) < lineOfSight && angle < maxAngle && Player.isMelded == false)
            {
                //raycast stuff
                RaycastHit hitElNumeroUno;
                Vector3 heading = player.transform.position - bulletShootPos.transform.position;
                float distance = heading.magnitude;
                Vector3 elDirection = heading / distance;
                if (Physics.Raycast(bulletShootPos.position, elDirection, out hitElNumeroUno, 25.0f))
                {
                    if (hitElNumeroUno.collider.gameObject.tag != "Player")
                    {

                    }
                    else
                    {
                        //slowly turns this unit to look at the target if the target is in line of sight
                        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                        nma.SetDestination(player.transform.position);
                        pursuing = true;
                    }
                }
            }
            else
            {
                //pursuing = false;
                isMoving = true;
                return;
            }
            if (shootTImer <= 0)
            {
                shootTimerAfterStoppingTimer -= Time.deltaTime;
                nma.isStopped = true;
                isMoving = false;
                if (shootTimerAfterStoppingTimer <= 0)
                {
                    //shoot☺
                    GameObject GO = Instantiate(bulletThing, bulletShootPos.position, armPivot.transform.rotation) as GameObject;
                    GO.GetComponent<Rigidbody>().AddForce(weapon.transform.forward * bulletSpeed, ForceMode.Impulse);

                    //shoot
                    //Instantiate(bulletThing, bulletShootPos.position, Quaternion.identity);
                    shootTImer = shootCD;
                    isMoving = true;
                    shootTimerAfterStoppingTimer = shootTimerAfterStopping;
                    shootTImer = shootCD;
                    pursuing = true;
                }
            }
        }
    }
}
