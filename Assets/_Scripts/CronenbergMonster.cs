using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CronenbergMonster : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public Animator animator;

    public float lastShotTime;

    private Vector3 retreatTarget;

    public AudioSource source;
    public AudioClip hitClip; 


    public Transform[] fleePoints;

    public void Init()
    {
        float highestDistance = float.MinValue;
        Transform furtherstTarget = null;


        foreach (Transform point in fleePoints)
        {
            float dist = Vector3.Distance(PlayerController.Instance.transform.position, point.position);

            if (dist > highestDistance)
            {
                highestDistance = dist;
                furtherstTarget = point;
            }

        }

            if (furtherstTarget)
                retreatTarget = furtherstTarget.position;

            agent.Warp(retreatTarget);
    }

    public void Update()
    {
        agent.speed = GetSpeed();

        if (Time.time > lastShotTime)
        {
            PlayerController.Instance.obsticle.enabled = false;
            agent.SetDestination(target.position);
        }
        else
        {
            PlayerController.Instance.obsticle.enabled = true;
            agent.SetDestination(retreatTarget);
        }


        animator.SetBool("Moving", true);
    }

    public void Flee()
    {
        float highestDistance = float.MinValue;
        Transform furtherstTarget = null;

        foreach (Transform point in fleePoints)
        {
            float dist = Vector3.Distance(PlayerController.Instance.transform.position, point.position);

            if (dist > highestDistance)
            {
                highestDistance = dist;
                furtherstTarget = point; 
             }


            if (furtherstTarget)
                retreatTarget = furtherstTarget.position; 
        }

    }

    public void Shot()
    {
        if (Time.time > lastShotTime)
        {
            animator.SetTrigger("Hit");
            source.PlayOneShot(hitClip);
            lastShotTime = Time.time + GetShotWaitTime();
            Flee();
        }
    }


    public float GetSpeed()
    {
        float speed = 0f;

        if (Time.time > lastShotTime)
        {
            switch (GameManager.Instance.samplesCollected)
            {
                case 0:
                    speed = 0f;
                    break;
                case 1:
                    speed = 2f;
                    break;
                case 2:
                    speed = 3f;
                    break;
                case 3:
                    speed = 3f;
                    break;
                case 4:
                    speed =  5f;
                    break;
                case 5:
                    speed = 7;
                    break;
                case 6:
                    speed = 10f;
                    break;
                default:
                    break;
            }
        }
        else
        {
            speed = 15f; 
        }

        return speed; 
    }

    public float GetShotWaitTime()
    {
        float waitTime = 0f;


        switch (GameManager.Instance.samplesCollected)
        {
            case 0:
                waitTime = 10f;
                break;
            case 1:
                waitTime = 6f;
                break;
            case 2:
                waitTime = 5f;
                break;
            case 3:
                waitTime = 4f;
                break;
            case 4:
                waitTime = 4f;
                break;
            case 5:
                waitTime = 3f;
                break;
            case 6:
                waitTime = 3f;
                break;
            default:
                break;
        }

        return waitTime; 
    }
}
