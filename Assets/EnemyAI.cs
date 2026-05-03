using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;

    [Header("Настройки")]
    public float patrolRadius = 8f;
    public float waitTime = 2f;
    public float chaseRange = 12f;
    public float attackRange = 2.2f;
    public int fearDamage = 25;

    private bool isDead = false;
    private bool isCatching = false;
    private Vector3 patrolPoint;
    private float timer = 0f;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
        if (player == null) 
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        GetNewPatrolPoint();
    }

    void Update()
    {
        if (isDead || isCatching || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            CatchPlayer();
            return;
        }

        if (dist <= chaseRange)
        {
            // Поворот к игроку
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 360f * Time.deltaTime);
            }
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        anim.SetBool("isRunning", true);
    }

    void Patrol()
    {
        anim.SetBool("isRunning", false);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                timer = 0f;
                GetNewPatrolPoint();
            }
        }
    }

    void GetNewPatrolPoint()
    {
        Vector3 randDir = Random.insideUnitSphere * patrolRadius;
        randDir.y = 0;
        randDir += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randDir, out hit, patrolRadius * 1.5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void CatchPlayer()
    {
        if (isCatching) return;
        isCatching = true;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        dirToPlayer.y = 0;
        if (dirToPlayer != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dirToPlayer);

        if (anim != null) anim.SetTrigger("Attack");

        // Вызываем кат-сцену у игрока
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null) pc.GetCaught(transform.position);
    }

    public void Die()
    {
        isDead = true;
        if (anim != null) anim.SetBool("isDead", true);
        if (agent) agent.isStopped = true;
        Destroy(gameObject, 3.5f);
    }
}