using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
