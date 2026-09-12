using UnityEngine;
using UnityEngine.AI;
using Backrooms.Core;

namespace Backrooms.AI
{
    public class SearchState : IState
    {
        private readonly ThreatController controller;
        private Vector3 searchOrigin;
        private float searchTimer;
        private float nextLookTimer;

        public SearchState(ThreatController controller)
        {
            this.controller = controller;
        }

        public void SetSearchOrigin(Vector3 origin)
        {
            searchOrigin = origin;
        }

        public void Enter()
        {
            Debug.Log("[ThreatState] Son bilinen konuma gidiliyor, arama basladi");
            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.Searching));

            searchTimer = 0f;
            nextLookTimer = 0f;
            controller.Agent.speed = controller.SearchSpeed;
            controller.Agent.destination = searchOrigin;
        }

        public void Tick()
        {
            searchTimer += Time.deltaTime;

            if (controller.CanSeePlayer())
            {
                controller.ChangeToActiveChase();
                return;
            }

            bool hasArrived = !controller.Agent.pathPending && controller.Agent.remainingDistance < 0.5f;

            if (hasArrived)
            {
                nextLookTimer -= Time.deltaTime;

                if (nextLookTimer <= 0f)
                {
                    PickNearbySearchPoint();
                    nextLookTimer = Random.Range(1.5f, 3f);
                }
            }

            if (searchTimer >= controller.LoseSightTime)
            {
                controller.ChangeToPatrol();
            }
        }

        public void Exit()
        {
            Debug.Log("[ThreatState] Arama sona erdi");
        }

        private void PickNearbySearchPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * controller.SearchRadius;
            randomDirection += searchOrigin;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, controller.SearchRadius, NavMesh.AllAreas))
            {
                controller.Agent.destination = hit.position;
            }
        }
    }
}