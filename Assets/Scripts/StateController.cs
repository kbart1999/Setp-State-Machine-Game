﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class StateController : MonoBehaviour {

    public GameObject navPointsParent;
    public State currentState;
    public GameObject[] navPoints;
    public GameObject enemyToChase;
    public int navPointNum = 0;
    public float remainingDistance;
    public Transform destination;
    public UnityStandardAssets.Characters.ThirdPerson.AICharacterControl ai;
    public Renderer[] childrenRend;
    public GameObject[] enemies;
    public float detectionRange;
    public GameObject wanderP;
    public GameObject newNavPoint;
    public int damage = 25;
    public bool hasAttacked;
    public bool canAttack;
    public NavMeshAgent agent;

    [Header("A.I Settings")]
    public float maximumDetectionAngle = 50;
    public float minimumDetectionAngle = -50;

    //public UnityStandardAssets.Characters.ThirdPerson.AICharacterControl ai;



    void Start()
    {
        ai = GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>();
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();



        childrenRend = GetComponentsInChildren<Renderer>();
        agent.speed = .5f;
        SetState(new PatrolState(this));
        hasAttacked = false;
        canAttack = true;
    }

    void Update()
    {
        currentState.CheckTransitions();
        currentState.Act();
    }
    public Transform GetNextNavPoint()
    {
        navPointNum += 1;
        if (navPointNum >= navPoints.Length) navPointNum = 0;
        //navPointNum = (navPointNum + 1) % navPoints.Length;
        return navPoints[navPointNum].transform;
    }
    public Transform GetWanderPoint()
    {
        Vector3 wanderPoint = new Vector3(Random.Range(-2f, 2f), 1.5f, Random.Range(-2f, 2f));
        GameObject go = Instantiate(wanderP, wanderPoint, Quaternion.identity);
        return go.transform;
    }
    public Vector3 GetRandomPoint()
    {
        Vector3 ran = new Vector3(Random.Range(-detectionRange, detectionRange), 1.5f, Random.Range(-detectionRange, detectionRange));
        return ran;
    }

    public void AddNavPoint(Vector3 pos)
    {
        GameObject go = Instantiate(newNavPoint, pos, Quaternion.identity);
        go.transform.SetParent(navPointsParent.transform);
       
    }

    public void ChangeColor(Color color)
    {
        foreach(Renderer r in childrenRend)
        {
            foreach(Material m in r.materials)
            {
                m.color = color;
            }
        }
    }
    public bool CheckIfInRange(string tag)
    {
        enemies = GameObject.FindGameObjectsWithTag(tag);
        if (enemies != null)
        {
            foreach (GameObject g in enemies)
            {
                if (Vector3.Distance(g.transform.position, transform.position) < detectionRange)
                {
                    enemyToChase = g;
                    return true;
                }
            }
        }
        return false;
    }

    public void SetState(State state)
    {
        if(currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = state;
        gameObject.name = "AI agent in state " + state.GetType().Name;

        if(currentState != null)
        {
            currentState.OnStateEnter();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(maximumDetectionAngle, transform.up) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(minimumDetectionAngle, transform.up) * transform.forward * detectionRange;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(canAttack == true)
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
                hasAttacked = true;
                canAttack = false;
            }
        }
        
    }
}
