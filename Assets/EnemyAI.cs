using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Ссылки")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;

    [Header("Зрение")]
    public float sightRange = 15f;
    public float attackRange = 2.2f;
    public float fieldOfView = 100f;
    public float eyeHeight = 1.4f;
    public LayerMask obstacleMask;

    [Header("Патрулирование")]
    public float walkPointRange = 10f;

    [Header("Память")]
    public float memoryDuration = 7f;
    public float investigateRadius = 4f;

    [Header("Дополнительно")]
    public float rotationSpeed = 7f;
    public float chaseSpeed = 3.5f;
    public float patrolSpeed = 1.8f;

    // Внутренние переменные
    private Vector3 walkPoint;
    private bool walkPointSet;
    private Vector3 lastKnownPosition;
    private float memoryTimer;
    private bool isInvestigating;
    private bool isCatching;
    private bool alreadyAttacked;

    private void Awake()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();

        if (obstacleMask.value == 0)
            obstacleMask = LayerMask.GetMask("Default", "Wall", "Obstacle");

        agent.autoRepath = true;
    }

    private void Update()
    {
        if (isCatching || player == null) return;

        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            lastKnownPosition = player.position;
            memoryTimer = memoryDuration;
            isInvestigating = false;

            float dist = Vector3.Distance(transform.position, player.position);

            if (dist <= attackRange)
                AttackPlayer();
            else
                ChasePlayer();
        }
        else if (memoryTimer > 0)
        {
            memoryTimer -= Time.deltaTime;
            InvestigateLastPosition();
        }
        else
        {
            Patroling();
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);

        if (angle > fieldOfView * 0.5f) return false;
        if (Vector3.Distance(transform.position, player.position) > sightRange) return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 playerEye = player.position + Vector3.up * 1.2f;

        return !Physics.Linecast(eyePos, playerEye, obstacleMask);
    }

    private void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        SetAnimation("isRunning", true);
        SetAnimation("isWalking", false);

        RotateTowards(player.position);
    }

    private void InvestigateLastPosition()
    {
        if (!isInvestigating)
        {
            isInvestigating = true;
            agent.SetDestination(lastKnownPosition);
        }

        agent.speed = chaseSpeed * 0.85f;
        SetAnimation("isRunning", true);
        SetAnimation("isWalking", false);

        if (agent.remainingDistance < 2f && !agent.pathPending)
        {
            if (Random.value < 0.06f)
            {
                Vector3 offset = Random.insideUnitSphere * investigateRadius;
                offset.y = 0;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(lastKnownPosition + offset, out hit, investigateRadius, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }
        }

        RotateTowards(lastKnownPosition);
    }

    private void Patroling()
    {
        agent.speed = patrolSpeed;
        SetAnimation("isRunning", false);
        SetAnimation("isWalking", true);

        if (!walkPointSet || Vector3.Distance(transform.position, walkPoint) < 2f)
            SearchNewWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);
    }

    private void SearchNewWalkPoint()
    {
        walkPointSet = false;
        for (int i = 0; i < 35; i++)
        {
            Vector3 rand = Random.insideUnitSphere * walkPointRange;
            rand.y = 0;
            Vector3 target = transform.position + rand;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, walkPointRange, NavMesh.AllAreas))
            {
                walkPoint = hit.position;
                walkPointSet = true;
                return;
            }
        }
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!alreadyAttacked)
        {
            isCatching = true;
            if (anim != null) anim.SetTrigger("Attack");

            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.GetCaught(transform.position);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), 2f);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        isCatching = false;
    }

    // Открытие дверей
    public void OpenDoor(GameObject door)
    {
        agent.isStopped = true;
        SmartDoor doorScript = door.GetComponent<SmartDoor>();
        if (doorScript != null) doorScript.OpenForEnemy();
        Invoke(nameof(ResumeNavigation), 1.2f);
    }

    private void ResumeNavigation() => agent.isStopped = false;

    private void SetAnimation(string param, bool value)
    {
        if (anim != null) anim.SetBool(param, value);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}