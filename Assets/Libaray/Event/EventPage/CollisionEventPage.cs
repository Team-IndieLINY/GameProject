using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Utils.Log;

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
                XLog.LogDebug("--", "default");
            });
        }
    } 
}