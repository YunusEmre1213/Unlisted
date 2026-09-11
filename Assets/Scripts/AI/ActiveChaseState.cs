using UnityEngine;
using Backrooms.Core;

namespace Backrooms.AI
{
    public class ActiveChaseState : IState
    {
        private readonly ThreatController controller;
        private float loseSightTimer;

        public ActiveChaseState(ThreatController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            Debug.Log("[ThreatState] Aktif kovalama basladi");
            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.ActiveChase));

            loseSightTimer = 0f;
            controller.Agent.speed = controller.ChaseSpeed;
        }

        public void Tick()
        {
            if (controller.Player != null)
            {
                controller.Agent.destination = controller.Player.position;
            }

            if (controller.CanSeePlayer())
            {
                loseSightTimer = 0f;
            }
            else
            {
                loseSightTimer += Time.deltaTime;

                if (loseSightTimer >= controller.LoseSightTime)
                {
                    controller.ChangeToPatrol();
                }
            }
        }

        public void Exit()
        {
            Debug.Log("[ThreatState] Aktif kovalama sona erdi");
            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.Passive));
        }
    }
}