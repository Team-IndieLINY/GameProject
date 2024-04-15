using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using IndieLINY.Event;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IndieLINY
{
    [RequireComponent(typeof(CollisionInteraction))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerControllerData _data;
        [SerializeField] private SteminaController _steminaController;

        public PlayerControllerData ControllerData => _data;

        public Rigidbody2D Rigid2D;

        public bool IsStopped { get; set; }

        private void Awake()
        {
            Interaction = GetComponentInChildren<CollisionInteraction>();

            Interaction.SetContractInfo(ActorContractInfo.Create(
                transform,
                () => gameObject == false
            ), this);
        }

        private void Start()
        {
            if (Rigid2D == false)
                Rigid2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Move();

            WorldInteraction();
            SelfInteraction();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var instance = PlayerInventory.Instance;
                
                if (instance.IsOpened())
                {
                    instance.CloseInventory();
                    BoxInventory.Instance.CloseInventory();
                }
                else
                {
                    instance.OpenInventory();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SceneManager.LoadScene("Test");
            }
        }

        private bool _isOpen = false;

        private void SelfInteraction()
        {
        }

        private void WorldInteraction()
        {
            var cols = Physics2D.OverlapCircleAll(transform.position, ControllerData.WorldInteractionRadius);
            Collider2D collider = null;
            float dis = Mathf.Infinity;
            foreach (var col in cols)
            {
                if (col.TryGetComponent<CollisionInteraction>(out var tInteraction))
                {
                    if (tInteraction == this.Interaction) continue;
                    float tempDis = Vector2.Distance(col.transform.position, transform.position);
                    if (tempDis <= dis)
                    {
                        collider = col;
                        dis = tempDis;
                    }
                }
            }

            if (collider == null) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics2D.RaycastAll(
                ray.origin,
                ray.direction,
                Mathf.Infinity
            );

            if (collider.TryGetComponent<CollisionInteraction>(out var ttInteraction))
            {
                if (ttInteraction.TryGetContractInfo(out ObjectContractInfo info) &&
                    info.TryGetBehaviour(out IBOSample sample))
                {
                    if (Input.GetKeyDown(KeyCode.E) && BoxInventory.Instance.IsOpened() is false)
                    {
                        sample.OpenInventory();
                    }
                }
            }
        }


        private void Move()
        {
            var dir = new Vector2()
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };

            float currentSpeed = 0f;
            bool isSprint = false;

            if (Input.GetKey(KeyCode.LeftControl) && dir.sqrMagnitude >= 0.001f)
            {
                currentSpeed = ControllerData.CrouchSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftShift) && dir.sqrMagnitude >= 0.001f)
            {
                currentSpeed = ControllerData.SprintSpeed;

                _steminaController?.Decrease(ESteminaType.Endurance,
                    ControllerData.DecreaseEndurancePerSec * Time.deltaTime);
                
                if (_steminaController.GetStemina(ESteminaType.Endurance) <= 0f)
                {
                    currentSpeed = ControllerData.WalkSpeed;
                }

                isSprint = true;
            }
            else if (dir.sqrMagnitude >= 0.001f)
            {
                currentSpeed = ControllerData.WalkSpeed;
            }

            if (isSprint is false)
            {
                _steminaController?.Increase(ESteminaType.Endurance,
                    ControllerData.IncreaseEndurancePerSec * Time.deltaTime);
            }
            
            if (BoxInventory.Instance.IsOpened())
            {
                currentSpeed = 0f;
            }

            if (IsStopped) return;

            Rigid2D.velocity = dir.normalized * currentSpeed;
        }

        #region ActorBehviour

        public CollisionInteraction Interaction { get; private set; }

        #endregion


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, ControllerData.WorldInteractionRadius);
        }
    }
}