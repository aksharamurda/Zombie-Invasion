using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour {

    [HideInInspector]
    public Health health;
    [HideInInspector]
    public Animator animator;

    public float speedMove = 0.25f;
    public float stopDistance = 1.5f;
    public float damage = 10;

    private NavMeshAgent navAgent;
    private Transform player;

    public bool isDead;
    public bool isHit;
    public float attackRate = 1;
    private float nextAttackTime;

    public int deathAnimSize = 4;
    public int hitAnimSize = 2;
    public int attackAnimSize = 2;
    public int walkAnimSize = 2;

    private int currentDeathType;
    private int currentHitType;
    private int currentAttackType;
    private int currentWalkType;

    void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        player = (FindObjectOfType(typeof(PlayerController)) as PlayerController).transform;

        navAgent.stoppingDistance = stopDistance;
        navAgent.SetDestination(player.position);

        currentAttackType = Random.Range(0, attackAnimSize);
        currentWalkType = Random.Range(0, walkAnimSize);
        animator.SetFloat("WalkType", currentWalkType);
    }

    public void Update()
    {

        if (isDead)
            return;

        navAgent.speed = speedMove;

        if (navAgent.remainingDistance < navAgent.stoppingDistance)
        {
            navAgent.isStopped = true;

            if(Time.time > nextAttackTime)
            {
                nextAttackTime = Time.time + attackRate;
                animator.SetTrigger("OnAttack");
                animator.SetFloat("AttackType", currentAttackType);
            }

        }

        if (health.healthAmount <= 0)
        {
            isDead = true;
            navAgent.isStopped = true;
            navAgent.speed = 0;
            animator.SetBool("isDead", isDead);

            animator.SetTrigger("OnDeath");
            currentDeathType = Random.Range(0, deathAnimSize);
            animator.SetFloat("DeathType", currentDeathType);

            health.healthAmount = 0;
            Destroy(gameObject, 3.5f);
        }
    }

    public void AttackAction()
    {
        Debug.Log("Enemy : Attack Player!");
        player.SendMessage("OnHitTaken", damage, SendMessageOptions.DontRequireReceiver);
    }

    public virtual void OnHitArea(float damage)
    {
        if (isDead)
            return;

        health.OnHitTaken(damage);

        StartCoroutine(OnHitStop());

        if (!isHit)
            StartCoroutine(OnWaitToHitFx());

    }

    IEnumerator OnWaitToHitFx()
    {
        isHit = true;
        animator.SetTrigger("OnHit");
        currentHitType = Random.Range(0, hitAnimSize);
        animator.SetFloat("HitType", currentHitType);
        yield return new WaitForSeconds(1f);
        isHit = false;
    }

    IEnumerator OnHitStop()
    {
        navAgent.isStopped = true;
        yield return new WaitForSeconds(1f);
        navAgent.isStopped = false;
    }

}
