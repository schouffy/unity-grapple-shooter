using System;
using System.Collections;
using UnityEngine;

public class BaseFlyingEnemy : EnemyAI
{
    public EnemyStatus Status;

    protected Vector3 _startingPoint;
    protected Vector3 _currentPatrollingDestination;
    public float MaxPatrolDistanceFromStartingPoint;
    public float PatrolSpeed;
    public float DetectionRange;
    public LayerMask AvoidTheseWhenPatrolling;

    protected override void Start()
    { 
        base.Start();

        _startingPoint = transform.position;
        _currentPatrollingDestination = transform.position;
    }

    protected Vector3 GetRandomPointAround(Vector3 center, float maxRadius)
    {
        int maxAttempts = 10;
        int i = 0;
        while (i < maxAttempts)
        {
            ++i;
            var candidate = center + UnityEngine.Random.insideUnitSphere * maxRadius;
            bool isFreeArea = Physics.OverlapSphere(candidate, 4f, AvoidTheseWhenPatrolling.value).Length == 0;
            if (!isFreeArea)
                continue;
            bool canBeReached = !Physics.SphereCast(transform.position, 1f, candidate - transform.position, out var hitInfo, 
                (candidate - transform.position).magnitude, AvoidTheseWhenPatrolling.value);
            if (canBeReached)
            {
                Debug.DrawLine(transform.position, candidate, Color.red, 5f);
                return candidate;
            }
        }
        return center;
    }

    protected void Idle()
    {
        if (Vector3.Distance(_playerAimPoint.position, transform.position) < DetectionRange)
        {
            var seesPlayer = Physics.Raycast(transform.position, _playerAimPoint.position - transform.position, out var hitInfo, DetectionRange);
            if (seesPlayer && hitInfo.collider.tag == Constants.PlayerTag)
            {
                Status = EnemyStatus.Attacking;
                StartCoroutine(Attack());
                return;
            }
        }

        if (Vector3.Distance(_startingPoint, transform.position) > MaxPatrolDistanceFromStartingPoint
            || Vector3.Distance(_currentPatrollingDestination, transform.position) < 1)
        {
            // Get a new destination around the starting point
            _currentPatrollingDestination = GetRandomPointAround(_startingPoint, MaxPatrolDistanceFromStartingPoint);
        }

        // move to _currentPatrollingDestination
        transform.position = Vector3.MoveTowards(transform.position, _currentPatrollingDestination, PatrolSpeed * Time.deltaTime);
        transform.LookAt(_currentPatrollingDestination);
    }

    protected virtual IEnumerator Attack()
    {
        throw new Exception("Must be overriden");
    }
}
