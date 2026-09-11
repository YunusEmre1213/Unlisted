using UnityEngine;
using UnityEngine.AI;
using Backrooms.Core;
using Backrooms.Player;

namespace Backrooms.AI
{
    public class ThreatController : MonoBehaviour
    {
        [Header("Referanslar")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform eyePoint;

        [Header("Algilama Ayarlari")]
        [SerializeField] private float detectionRange = 12f;
        [SerializeField] private float detectionAngle = 70f;
        [SerializeField] private float requiredDetectionTime = 1.5f;
        [SerializeField] private float playerHeightOffset = 1f;
        [SerializeField] private float crouchDetectionMultiplier = 0.6f;
        [SerializeField] private float heavyBreathingRangeMultiplier = 1.3f;

        [Header("Vazgecme Ayarlari")]
        [SerializeField] private float loseSightTime = 5f;

        [Header("Devriye")]
        [SerializeField] private float patrolSpeed = 1.5f;
        [SerializeField] private float wanderRadius = 8f;

        [Header("Kovalama")]
        [SerializeField] private float chaseSpeed = 4.5f;

        public NavMeshAgent Agent { get; private set; }
        public Transform Player => player;
        public float DetectionRange => detectionRange;
        public float DetectionAngle => detectionAngle;
        public float RequiredDetectionTime => requiredDetectionTime;
        public float LoseSightTime => loseSightTime;
        public float PatrolSpeed => patrolSpeed;
        public float WanderRadius => wanderRadius;
        public float ChaseSpeed => chaseSpeed;

        private StateMachine stateMachine;
        private PatrolState patrolState;
        private ActiveChaseState activeChaseState;

        private PlayerMovement playerMovement;
        private PlayerBreathing playerBreathing;
        private PlayerHiding playerHiding;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();

            stateMachine = new StateMachine();
            patrolState = new PatrolState(this);
            activeChaseState = new ActiveChaseState(this);
        }

        private void Start()
        {
            if (player == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    player = playerObject.transform;
                }
            }

            if (player != null)
            {
                playerMovement = player.GetComponent<PlayerMovement>();
                playerBreathing = player.GetComponent<PlayerBreathing>();
                playerHiding = player.GetComponent<PlayerHiding>();
            }
        }

        private void Update()
        {
            stateMachine.Tick();
        }

        public void Ambush(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            gameObject.SetActive(true);

            if (Agent != null)
            {
                Agent.Warp(spawnPosition);
            }

            transform.rotation = spawnRotation;

            stateMachine.ChangeState(patrolState);
        }

        public void ChangeToPatrol()
        {
            stateMachine.ChangeState(patrolState);
        }

        public void ChangeToActiveChase()
        {
            stateMachine.ChangeState(activeChaseState);
        }

        public bool CanSeePlayer()
        {
            if (player == null || eyePoint == null) return false;
            if (playerHiding != null && playerHiding.IsHiding) return false;

            bool isCrouching = playerMovement != null && playerMovement.IsCrouching;
            float effectiveRange = isCrouching ? detectionRange * crouchDetectionMultiplier : detectionRange;
            float effectiveAngle = isCrouching ? detectionAngle * crouchDetectionMultiplier : detectionAngle;

            if (playerBreathing != null && playerBreathing.IsHeavyBreathing)
            {
                effectiveRange *= heavyBreathingRangeMultiplier;
            }

            Vector3 targetPoint = player.position + Vector3.up * playerHeightOffset;
            Vector3 toPlayer = targetPoint - eyePoint.position;
            float distance = toPlayer.magnitude;

            if (distance > effectiveRange) return false;

            Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
            float angle = Vector3.Angle(transform.forward, flatToPlayer);
            if (angle > effectiveAngle * 0.5f) return false;

            if (Physics.Raycast(eyePoint.position, toPlayer.normalized, out RaycastHit hit, effectiveRange, ~0, QueryTriggerInteraction.Ignore))
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    return false;
                }
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            if (detectionRange <= 0f) return;

            Vector3 origin = eyePoint != null ? eyePoint.position : transform.position + Vector3.up;

            Gizmos.color = Color.red;

            float halfAngle = detectionAngle * 0.5f;
            Vector3 forwardDir = transform.forward;

            Quaternion leftRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
            Quaternion rightRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);

            Vector3 leftDir = leftRotation * forwardDir;
            Vector3 rightDir = rightRotation * forwardDir;

            Gizmos.DrawRay(origin, leftDir * detectionRange);
            Gizmos.DrawRay(origin, rightDir * detectionRange);
            Gizmos.DrawRay(origin, forwardDir * detectionRange);

            int segments = 20;
            Vector3 previousPoint = origin + leftDir * detectionRange;

            for (int i = 1; i <= segments; i++)
            {
                float t = (float)i / segments;
                float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
                Vector3 currentDir = Quaternion.AngleAxis(currentAngle, Vector3.up) * forwardDir;
                Vector3 currentPoint = origin + currentDir * detectionRange;

                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }
    }
}