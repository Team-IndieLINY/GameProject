using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.Event
{
    public static partial class EventPage
    {
        [EventHandler(typeof(CollisionEventCommand))]
        public static void CollisionEventHandler(IEventCommand command)
        {
            if (command is not CollisionEventCommand cmd) return;

            cmd.groups.ForEach(x =>
            {
                x.ForEach(y => Debug.Log(y));
                Debug.Log("--");
            });
        }
    } 
}