using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour
{
    [SerializeField] private float distanceToWalk = 20f;

    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private float _distanceToPlayer = 0;

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
    }

    private void Update()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (_distanceToPlayer <= distanceToWalk)
        {
            _agent.enabled = true;
            _agent.SetDestination(_playerTransform.position);
        }
        else if (_distanceToPlayer > distanceToWalk)
        {
            _agent.enabled = false;
        }
    }
}
