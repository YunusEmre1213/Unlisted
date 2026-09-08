using UnityEngine;
using UnityEngine.AI;
using Backrooms.Core;

namespace Backrooms.AI
{
    public class ThreatController : MonoBehaviour
    {
        [Header("Referanslar")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform eyePoint;
        [SerializeField] private Transform[] patrolPoints;

        [Header("Algilama Ayarlari")]
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float detectionAngle = 60f;
        [SerializeField] private float requiredDetectionTime = 1.5f;
        [SerializeField] private float playerHeightOffset = 1f;

        [Header("Vazgecme Ayarlari")]
        [SerializeField] private float loseSightTime = 4f;

        [Header("Hareket Hizlari")]
        [SerializeField] private float patrolSpeed = 1.5f;
        [SerializeField] private float chaseSpeed = 4f;

        public NavMeshAgent Agent { get; private set; }
        public Transform Player => player;
        public Transform[] PatrolPoints => patrolPoints;
        public float DetectionRange => detectionRange;
        public float DetectionAngle => detectionAngle;
        public float RequiredDetectionTime => requiredDetectionTime;
        public float LoseSightTime => loseSightTime;
        public float PatrolSpeed => patrolSpeed;
        public float ChaseSpeed => chaseSpeed;

        private StateMachine stateMachine;

        private FakeClueState fakeClueState;
        private PassiveState passiveState;
        private ActiveChaseState activeChaseState;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();

            stateMachine = new StateMachine();
            fakeClueState = new FakeClueState();
            passiveState = new PassiveState(this);
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

            ChangeState(passiveState);
        }

        private void Update()
        {
            stateMachine.Tick();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeState(fakeClueState);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeState(passiveState);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeState(activeChaseState);
            }
        }

        public void ChangeState(IState newState)
        {
            stateMachine.ChangeState(newState);
        }

        public void ChangeToPassive()
        {
            ChangeState(passiveState);
        }

        public void ChangeToActiveChase()
        {
            ChangeState(activeChaseState);
        }

        public bool CanSeePlayer()
        {
            if (player == null || eyePoint == null) return false;

            Vector3 targetPoint = player.position + Vector3.up * playerHeightOffset;
            Vector3 toPlayer = targetPoint - eyePoint.position;
            float distance = toPlayer.magnitude;

            if (distance > detectionRange) return false;

            Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
            float angle = Vector3.Angle(transform.forward, flatToPlayer);
            if (angle > detectionAngle * 0.5f) return false;

            if (Physics.Raycast(eyePoint.position, toPlayer.normalized, out RaycastHit hit, detectionRange, ~0, QueryTriggerInteraction.Ignore))
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