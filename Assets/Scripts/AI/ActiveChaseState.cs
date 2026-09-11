using UnityEngine;
using Backrooms.Core;

namespace Backrooms.AI
{
    public class ActiveChaseState : IState
    {
        private readonly ThreatController controller;
        private Vector3 lastKnownPosition;
        private float sightLostTimer;

        public ActiveChaseState(ThreatController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            Debug.Log("[ThreatState] Aktif kovalama basladi");
            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.ActiveChase));

            sightLostTimer = 0f;
            controller.Agent.speed = controller.ChaseSpeed;

            if (controller.Player != null)
            {
                lastKnownPosition = controller.Player.position;
            }
        }

        public void Tick()
        {
            if (controller.CanSeePlayer())
            {
                sightLostTimer = 0f;
                lastKnownPosition = controller.Player.position;
                controller.Agent.destination = lastKnownPosition;
            }
            else
            {
                sightLostTimer += Time.deltaTime;

                if (sightLostTimer >= controller.ChaseGraceTime)
                {
                    controller.ChangeToSearch(lastKnownPosition);
                }
            }
        }

        public void Exit()
        {
            Debug.Log("[ThreatState] Aktif kovalama sona erdi");
        }
    }
}