using UnityEngine;

public class WaspEnemyAI : MonoBehaviour // <--- Name ge�ndert
{
    // Rigidbody statt NavMeshAgent
    private Rigidbody rb;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    // Bewegungsvariablen
    public float speed = 5f; // Geschwindigkeit f�r die Wespe

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    private Vector3 initialPosition; // Startpunkt f�r die Patrouille

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public float projectileSpeed;
    public float projectileSpawnOffset;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        // Rigidbody-Komponente holen
        rb = GetComponent<Rigidbody>();
        // Startposition speichern, damit die Wespe nicht davonfliegt
        initialPosition = transform.position;

        // Finde das Spieler-Objekt automatisch anhand seines Tags
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            // Eine Fehlermeldung ausgeben, falls kein Spieler gefunden wurde
            Debug.LogError("FEHLER: Spieler-Objekt mit dem Tag 'Player' konnte in der Szene nicht gefunden werden.");
        }
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

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            // Bewege die Wespe in Richtung des walkPoint
            Vector3 directionToWalkPoint = (walkPoint - transform.position).normalized;
            rb.linearVelocity = directionToWalkPoint * speed;
        }

        // Walkpoint reached
        if (Vector3.Distance(transform.position, walkPoint) < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        // Berechne einen zuf�lligen Punkt im 3D-Raum um die Startposition
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange / 2, walkPointRange / 2);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(initialPosition.x + randomX, initialPosition.y + randomY, initialPosition.z + randomZ);

        walkPointSet = true;
    }

    private void ChasePlayer()
    {
        // Berechne die Richtung zum Spieler
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // Setze die Geschwindigkeit des Rigidbodys, um den Spieler zu verfolgen
        rb.linearVelocity = directionToPlayer * speed;
    }

    private void AttackPlayer()
    {
        // Stoppe die Bewegung, um anzugreifen
        rb.linearVelocity = Vector3.zero;

        if (!alreadyAttacked)
        {
            Vector3 spawnPosition = transform.position - transform.forward * projectileSpawnOffset;
            Rigidbody projectileRb = Instantiate(projectile, spawnPosition, transform.rotation).GetComponent<Rigidbody>();
            projectileRb.AddForce(-projectileRb.transform.forward * projectileSpeed, ForceMode.Impulse);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    // Die restlichen Methoden bleiben unver�ndert
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