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
            bool hasArrived = !controller.Agent.pathPending && controller.Agent.remainingDistance < 0.5f;

            if (hasArrived)
            {
                waitTimer -= Time.deltaTime;
                controller.transform.Rotate(Vector3.up, controller.LookAroundSpeed * Time.deltaTime);

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
                    return;
                }
            }
            else
            {
                detectionTimer = 0f;
            }

            if (controller.CanHearPlayer())
            {
                controller.ChangeToSearch(controller.Player.position);
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