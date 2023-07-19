using UnityEngine;

using Zlitz.AI.BehaviourTrees.Runtime;

public class AgentBehaviour : MonoBehaviour
{
    [SerializeField]
    private float m_trackDistance = 5.0f;

    [SerializeField]
    private float m_reachDistance = 0.5f;

    [SerializeField]
    private float m_speed = 3.0f;

    [SerializeField]
    private TargetBehaviour m_target;

    private Rigidbody m_rigidBody;

    private Vector3? m_destination;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        m_destination = null;
    }

    private void FixedUpdate()
    {
        MoveToDestination();
    }

    private void MoveToDestination()
    {
        if (m_destination.HasValue)
        {
            Vector3 destination = m_destination.Value;
            destination.y = transform.position.y;

            float distanceToDestination = Vector3.Distance(destination, transform.position);
            if (distanceToDestination > m_reachDistance)
            {
                Vector3 direction = destination - transform.position;
                direction = direction.normalized;
                m_rigidBody.velocity = m_speed * direction;
            }
            else
            {
                m_destination = null;
            }
        }
    }

    private bool DestinationReached()
    {
        if (!m_destination.HasValue)
        {
            return true;
        }

        Vector3 destination = m_destination.Value;
        destination.y = transform.position.y;

        return Vector3.Distance(destination, transform.position) <= m_reachDistance;
    }

    #region Agent Behaviours

    private Vector3 m_targetPosition;

    private bool IsTargetInRange()
    {
        Vector3 targetPosition = m_target.transform.position;
        targetPosition.y = transform.position.y;

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        return distanceToTarget <= m_trackDistance;
    }

    private BehaviourResult TargetInRange()
    {
        if (IsTargetInRange())
        {
            m_targetPosition = m_target.transform.position;
            return BehaviourResult.Successful;
        }

        return BehaviourResult.Failed;
    }

    private BehaviourResult GoToTarget()
    {
        m_destination = m_targetPosition;
        if (DestinationReached())
        {
            m_target.OnTargetReach();
            return BehaviourResult.Successful;
        }
        return BehaviourResult.Running;
    }

    private Vector3 m_wanderPosition;

    private void GetRandomWanderTarget()
    {
        m_wanderPosition = new Vector3(Random.Range(-9.0f, 9.0f), transform.position.y, Random.Range(-9.0f, 9.0f));
    }

    private BehaviourResult GoToWanderTarget()
    {
        if (IsTargetInRange())
        {
            return BehaviourResult.Failed;
        }

        m_destination = m_wanderPosition;
        if (DestinationReached())
        {
            return BehaviourResult.Successful;
        }
        return BehaviourResult.Running;
    }

    #endregion
}
