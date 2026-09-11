using UnityEngine;
using UnityEngine.AI;
using Backrooms.Core;

namespace Backrooms.AI
{
    public class PatrolState : IState
    {
        private readonly ThreatController controller;
        private float detectionTimer;
        private float waitTimer;

        public PatrolState(ThreatController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            Debug.Log("[ThreatState] Devriyeye basladi");
            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.Passive));

            detectionTimer = 0f;
            controller.Agent.speed = controller.PatrolSpeed;
            PickNewWanderPoint();
        }

        public void Tick()
        {
            if (!controller.Agent.pathPending && controller.Agent.remainingDistance < 0.5f)
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    PickNewWanderPoint();
                }
            }

            if (controller.CanSeePlayer())
            {
                detectionTimer += Time.deltaTime;

                if (detectionTimer >= controller.RequiredDetectionTime)
                {
                    controller.ChangeToActiveChase();
                }
            }
            else
            {
                detectionTimer = 0f;
            }
        }

        public void Exit()
        {
            Debug.Log("[ThreatState] Devriye sona erdi");
        }

        private void PickNewWanderPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * controller.WanderRadius;
            randomDirection += controller.transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, controller.WanderRadius, NavMesh.AllAreas))
            {
                controller.Agent.destination = hit.position;
            }

            waitTimer = Random.Range(2f, 5f);
        }
    }
}