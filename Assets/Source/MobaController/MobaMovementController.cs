using System;
using UnityEngine;
using UnityEngine.AI;
using CONTEXT = MOBA.ActionContextHolder;

namespace MOBA
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(MobaInputHandler))]
    public class MobaMovementController : MobaPlayerBehaviour, IInputListener<Vector2Action>
    {
        [SerializeField] private LayerMask walkableMask;
        [SerializeField] private float stoppingSpeed;
        [SerializeField] private float stoppingDistance;
        
        private NavMeshAgent agent;
        private bool isMoving;
        
        private bool CanMove => true;
        private MobaPlayerMovementControllerConfig settings => player.movementControllerSettings;
        protected override void Awake()
        {
            base.Awake();
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (isMoving)
            {
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance + stoppingDistance)
                    {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude <= stoppingSpeed)
                        {
                            player.AnimationController.StopRunning();
                            isMoving = false;
                        }
                    }
                }
            }
        }

        private Ray GetCameraRay(Vector2 pointerPosition)
        {
            Camera cam = player.CameraController.currentCamera;
            return cam.ScreenPointToRay(new Vector3(pointerPosition.x,pointerPosition.y, 0));
        }
        public bool ValidateAction(Vector2Action action)
        {
            return action.contextId == CONTEXT.SCREEN_POSITION_CONTEXT_ID;
        }
        public void Listen(Vector2Action action)
        {
            if(!ValidateAction(action)) return;
            if (!CanMove) return;
            if (Physics.Raycast(GetCameraRay(action.value), out RaycastHit hit, Mathf.Infinity, walkableMask))
            {
                agent.destination = hit.point;
                GameObject g = Instantiate(settings.locationEffector, hit.point, Quaternion.identity);
                g.transform.localScale = settings.locationEffectorScale;
                player.AnimationController.StartRunning();
                isMoving = true;
            }
        }
    }
}
