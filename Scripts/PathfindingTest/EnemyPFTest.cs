using UnityEngine;
using UnityEngine.AI;

// This script controls an enemy character's movement using Unity's NavMesh system.
// It makes the enemy navigate towards a target by setting the target's position as the destination for the NavMeshAgent.
// Additionally, it disables automatic rotation and up-axis updates for more custom movement control.

public class EnemyPFTest : MonoBehaviour
{
    [SerializeField] Transform target;

    NavMeshAgent agent;

    private void Start() {
        // Get the NavMeshAgent component attached to the enemy character.
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update() {
        // Set the destination for the NavMeshAgent to the target's position.
        agent.SetDestination(target.position);
    }

}
