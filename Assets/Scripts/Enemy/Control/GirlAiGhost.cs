﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace LoneWolfStudios.Control
{
    public class GirlAiGhost : MonoBehaviour
    {

        public float wanderRadius;
        public float wanderTimer, fieldOfView = 110f, range;
        Vector3 playerLastInSight;
        [SerializeField]
        private Transform target;
        [SerializeField]
        private NavMeshAgent agent;
       // [SerializeField]
        private float timer;
        private GameObject player;
        [SerializeField]
        private bool cooldown;

        [SerializeField]
        private Animator Girl_animator;
        [SerializeField]
        private int No_of_hits,Hits_taken;
        [SerializeField]
        private float attack_distance;
        [SerializeField]
        private float Cooldown_period;


        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            timer = wanderTimer;
            Hits_taken = 0;
           //Tracker because gameobject being tracked by navmesh should be grounded
            player = GameObject.FindWithTag("Tracker");
            cooldown = false;
        }
        private void Update()
        {
            if (!cooldown)
            {

                if (isinFrontOFMe(player))
                {
                    // Debug.Log("chaising");
                    agent.SetDestination(player.transform.position);
                    Animations(2, 0);
                    Attack();
                }

                else if (!isinFrontOFMe(player))
                {
                    //   Debug.Log("patoling");
                    Animations(1, 0);
                    timer += Time.deltaTime;

                    if (timer >= wanderTimer)
                    {

                        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                        agent.SetDestination(newPos);
                        timer = 0;
                    }
                }
            }
        }
        bool isinFrontOFMe(GameObject player)
        {

            Vector3 direction = player.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            if (angle < fieldOfView * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, range))
                {
                    Debug.DrawRay(transform.position, direction, Color.black);
                    return true;

                }
                else
                    return false;
            }
            else
                return false;
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

            return navHit.position;
        }
        /* walk state -1 is idle 
         * walk state 2 is chasing
         * walk state 1 is walking 
         */
        private void Animations(int walk_state , int hit_state)
        {
            Girl_animator.SetInteger("Hit_State", hit_state);
            Girl_animator.SetInteger("Walk_State", walk_state);
        }

        private void Attack()
        {
            if (Hits_taken < No_of_hits)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < attack_distance)
                {
                    cooldown = true;
                    Hits_taken++;
                    Animations(-1, 1);
                    Invoke("ReActivate", Cooldown_period);
                }
            }
            else
            {
                Animations(2, 2);
              //  Debug.Log("kill;");
            }
        }

        void ReActivate()
        {
            cooldown = false;
        }
    }
}