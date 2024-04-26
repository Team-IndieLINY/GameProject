using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using IndieLINY.Event;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IndieLINY
{
    [RequireComponent(typeof(CollisionInteraction))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerControllerData _data;
        [SerializeField] private SteminaController _steminaController;
        private SkeletonAnimation _skeletonAnimation;
        [SerializeField] private SkeletonDataAsset _frontSkeletonDataAsset;
        [SerializeField] private SkeletonDataAsset _backSkeletonDataAsset;
        
        public PlayerControllerData ControllerData => _data;
        public SteminaController SteminaController => _steminaController;

        public Rigidbody2D Rigid2D;

        public bool IsStopped { get; set; }

        private Vector2 _currentDir = new Vector2();

        private void Awake()
        {
            Interaction = GetComponentInChildren<CollisionInteraction>();
            _skeletonAnimation = GetComponent<SkeletonAnimation>();

            Interaction.SetContractInfo(ActorContractInfo.Create(
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
            AnimateMoving();
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

            CollisionInteractionUtil
                .CreateState()
                .Bind<IBOSample>(sample =>
                {
                    if (Input.GetKeyDown(KeyCode.E) && BoxInventory.Instance.IsOpened() is false)
                    {
                        sample.OpenInventory();
                    }
                })
                .Execute<ObjectContractInfo>(collider.gameObject);


            // TODO: 확인시 주석 제거. 위 코드와 아래 코드 중 원하는 방식 사용
            //if (collider.TryGetComponent<CollisionInteraction>(out var ttInteraction))
            //{
            //    if (ttInteraction.TryGetContractInfo(out ObjectContractInfo info) &&
            //        info.TryGetBehaviour(out IBOSample sample))
            //    {
            //        if (Input.GetKeyDown(KeyCode.E) && BoxInventory.Instance.IsOpened() is false)
            //        {
            //            sample.OpenInventory();
            //        }
            //    }
            //}
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

                _steminaController?.Decrease(
                    ControllerData.DecreaseEndurancePerSec);
                
                if (_steminaController.GetStemina() <= 0)
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
                _steminaController?.Increase(ControllerData.IncreaseEndurancePerSec);
            }
            
            if (BoxInventory.Instance.IsOpened())
            {
                currentSpeed = 0f;
            }

            if (IsStopped) return;

            Rigid2D.velocity = dir.normalized * currentSpeed;

        }

        private void AnimateMoving()
        {
            var dir = new Vector2();
            dir.x = Input.GetAxisRaw("Horizontal");
            dir.y = Input.GetAxisRaw("Vertical");

            if (dir == _currentDir)
            {
                return;
            }

            if (dir is { x: 0, y: 0 })
            {
                if (_skeletonAnimation.skeletonDataAsset.name == "character_back_resource_SkeletonData")
                {
                    _skeletonAnimation.AnimationState.SetAnimation(0, "backidle", true);
                }
                else if (_skeletonAnimation.skeletonDataAsset.name == "character_resource_SkeletonData")
                {
                    _skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
                }
            }
            else
            {
                if (dir is { x: <= 0, y: <= 0 })
                {
                    _skeletonAnimation.skeletonDataAsset = _frontSkeletonDataAsset;
                    _skeletonAnimation.initialFlipX = false;
                    _skeletonAnimation.Initialize(true);
                    
                    _skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
                }
                else if (dir is { x: >= 0, y: <= 0 })
                {
                    _skeletonAnimation.skeletonDataAsset = _frontSkeletonDataAsset;
                    _skeletonAnimation.initialFlipX = true;
                    _skeletonAnimation.Initialize(true);
                    
                    _skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
                }
                else if (dir is { x: <= 0, y: >= 0 })
                {
                    _skeletonAnimation.skeletonDataAsset = _backSkeletonDataAsset;
                    _skeletonAnimation.initialFlipX = false;
                    _skeletonAnimation.Initialize(true);

                    _skeletonAnimation.AnimationState.SetAnimation(0, "backwalk", true);
                }
                else if (dir is { x: >= 0, y: >= 0 })
                {
                    _skeletonAnimation.skeletonDataAsset = _backSkeletonDataAsset;
                    _skeletonAnimation.initialFlipX = true;
                    _skeletonAnimation.Initialize(true);
                    
                    _skeletonAnimation.AnimationState.SetAnimation(0, "backwalk", true);
                }
            }
            
            _currentDir = dir;
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