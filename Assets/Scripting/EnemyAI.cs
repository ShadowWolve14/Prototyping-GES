using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;
    public float projectileSpeed;
    public float projectileSpawnOffset;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // Geschwindigkeit für die Drehung
    public float rotationSpeed = 10f;

    private void Awake()
    {
        player = GameObject.Find("Bee").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // Statt eine Drehung zu berechnen UND DANN zu spiegeln,
            // berechnen wir direkt die Drehung für die entgegengesetzte Richtung (-direction).
            // Das berücksichtigt automatisch die Neigung auf der X-Achse.
            Quaternion targetRotation = Quaternion.LookRotation(-direction);

            // Der Rest bleibt gleich: Flüssig zur neuen Drehung interpolieren.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        RotateTowards(agent.velocity.normalized);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        // Berechne einen zufälligen Punkt im Patrouillenradius
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Finde den nächstgelegenen Punkt auf dem NavMesh zu unserem zufälligen Punkt
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, walkPointRange, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);

        // NEU: Benutze den echten 3D-Vektor zum Spieler für die Ausrichtung.
        // Genau wie in der AttackPlayer-Methode.
        Vector3 directionToPlayer = player.position - transform.position;
        RotateTowards(directionToPlayer.normalized);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        Vector3 directionToPlayer = player.position - transform.position;
        RotateTowards(directionToPlayer.normalized);

        if (!alreadyAttacked)
        {
            Vector3 spawnPosition = transform.position - transform.forward * projectileSpawnOffset;
            Rigidbody rb = Instantiate(projectile, spawnPosition, transform.rotation).GetComponent<Rigidbody>();
            rb.AddForce(-rb.transform.forward * projectileSpeed, ForceMode.Impulse);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}
