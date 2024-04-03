using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndieLINY.Event;
using NavMeshPlus.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace IndieLINY.AI.Sample
{
    [UnitCategory("IndieLINY/AI")]
    [UnitTitle("FindTargetFOV")]
    public class FindTargetFOV : UBaseEnterUpdateExit
    {
        [DoNotSerialize] public ValueInput DetectionData;
        [DoNotSerialize] public ValueInput TargetLayerMask;
        [DoNotSerialize] public ValueInput Agent;
        
        [DoNotSerialize] public ValueOutput FindedTarget;

        [DoNotSerialize] public ControlOutput Found;
        [DoNotSerialize] public ControlOutput NotFound;

        private DetectionData _data;
        private LayerMask _layerMask;
        private NavMeshAgent _agent;

        private Vector2 _desiredVelocityLasted;
        private CollisionInteraction _interaction;

        protected override ControlOutput OnEnter(Flow flow)
        {
            _data = flow.GetValue<DetectionData>(DetectionData);
            _layerMask = flow.GetValue<LayerMask>(TargetLayerMask);
            _agent = flow.GetValue<NavMeshAgent>(Agent);

            _desiredVelocityLasted = _agent.desiredVelocity;

            return null;
        }

        protected override ControlOutput OnUpdate(Flow flow)
        {
            if (Mathf.Approximately(_agent.desiredVelocity.sqrMagnitude, 0f) == false)
            {
                _desiredVelocityLasted = _agent.desiredVelocity;
            }

            _interaction = FindCloserFilterBlockWithFov(_agent.transform.position, _desiredVelocityLasted.normalized, _data.FOV, _data.MaxDistance, _layerMask.value);

            return _interaction != null ? Found : NotFound;
        }

        protected override ControlOutput OnExit(Flow flow)
        {
            return null;
        }

        protected override void OnDefinition()
        {
            DetectionData = ValueInput<DetectionData>("detection data", null);
            TargetLayerMask = ValueInput<LayerMask>("layer mask", default);
            Agent = ValueInput<NavMeshAgent>("nav agent", null);
            FindedTarget = ValueOutput("finded target", x =>
            {
                return _interaction != null ? _interaction.transform : null;
            });

            Found = ControlOutput("Found");
            NotFound = ControlOutput("Not Found");
        }
        
        

    private static Collider2D[] gOverlaps = new Collider2D[10000];
    public static AotList FindWithFov(Vector2 position, Vector2 dir, float fov, float maxDistance, int laskMask)
    {
        var ql= Quaternion.Euler(0f, 0f, fov * -0.5f);
        var qr= Quaternion.Euler(0f, 0f, fov * 0.5f);

        dir = dir.normalized;

        var leftRay = ql * dir * maxDistance;
        var rightRay = qr * dir * maxDistance;
        
        Debug.DrawRay(position, leftRay);
        Debug.DrawRay(position, rightRay);
        Debug.DrawRay(position, dir * maxDistance);

        var length = Physics2D.OverlapCircleNonAlloc(position, maxDistance, gOverlaps, laskMask);
        AotList list = new(length);
        for(int i=0; i<length; i++)
        {
            var col = gOverlaps[i];
            if (col.TryGetComponent(out CollisionInteraction interaction))
            {
                Vector2 n2o = (Vector2)col.transform.position - position;
                n2o = n2o.normalized;

                float dot = Vector2.Dot(n2o, dir);
                dot = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (dot <= fov * 0.5f)
                {
                    list.Add(interaction);
                }
            }
        }
        
        return list;
    }
    
    public static Transform CloserPoint(Vector2 point, List<(Transform, Vector2)> list)
    {
        Transform closerPoint = null;
        float minDis = Mathf.Infinity;
        foreach (var result in list)
        {
            float dis = Vector2.Distance(result.Item2, point);
            if (minDis >= dis)
            {
                minDis = dis;
                closerPoint = result.Item1;
            }
        }

        return closerPoint;

    }
    
    public static CollisionInteraction FindCloserFilterBlockWithFov(Vector2 position, Vector2 dir,float fov, float maxDistance, LayerMask layerMask)
    {
        var list = FindWithFov(position, dir, fov, maxDistance, layerMask.value);

        if (list == null) return null;

        var results = new List<Transform>(5);

        foreach (object item in list)
        {
            if (item is not CollisionInteraction interaction) continue;

            var hit = Physics2D.Raycast(position, (Vector2)interaction.transform.position - position, maxDistance,
                layerMask.value);

            if (hit && hit.transform == interaction.transform)
            {
                results.Add(hit.transform);
            }
        }


        var lastResult = CloserPoint(position, results.Select(x=>(x, (Vector2)x.position)).ToList());

        return lastResult != null ? lastResult.GetComponent<CollisionInteraction>() : null;
    }
   }

}