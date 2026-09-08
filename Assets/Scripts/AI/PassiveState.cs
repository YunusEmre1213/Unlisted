using UnityEngine;
using Backrooms.Core;

namespace Backrooms.AI
{
    public class PassiveState : IState
    {
        private readonly ThreatController controller;
        private int currentPatrolIndex;
        private float detectionTimer;

        public PassiveState(ThreatController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            Debug.Log("[ThreatState] Pasif varlik durumuna gecildi");
            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.Passive));

            detectionTimer = 0f;
            controller.Agent.speed = controller.PatrolSpeed;
            MoveToNextPatrolPoint();
        }

        public void Tick()
        {
            if (controller.PatrolPoints != null && controller.PatrolPoints.Length > 0)
            {
                if (!controller.Agent.pathPending && controller.Agent.remainingDistance < 0.5f)
                {
                    MoveToNextPatrolPoint();
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
            Debug.Log("[ThreatState] Pasif varlik durumundan cikildi");
        }

        private void MoveToNextPatrolPoint()
        {
            if (controller.PatrolPoints == null || controller.PatrolPoints.Length == 0) return;

            controller.Agent.destination = controller.PatrolPoints[currentPatrolIndex].position;
            currentPatrolIndex = (currentPatrolIndex + 1) % controller.PatrolPoints.Length;
        }
    }
}