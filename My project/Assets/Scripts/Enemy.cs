using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth;
    public float HealthAmount { get; private set; }
    private NavMeshAgent _agent;

    private void Start()
    {
        HealthAmount = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        HealthAmount -= damage;
        Die();
    }

    private void Die()
    {
        if (HealthAmount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
