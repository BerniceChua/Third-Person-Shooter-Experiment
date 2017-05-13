using UnityEngine;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

namespace _Shared.Extensions {

    public static class TransformExtensions {

        /// <summary>
        /// Check if the target (targetPosition) is within the line of sight
        /// </summary>
        /// <param name="origin">origin of the Transform</param>
        /// <param name="targetPosition">Target's direction</param>
        /// <param name="fieldOfView">field of view</param>
        /// <param name="collisionMask">checks the layer mask that it's colliding with</param>
        /// <param name="offset">offset of the Transform's origin</param>
        /// <returns>yes or no</returns>
        public static bool IsInLineOfSight(this Transform origin, Vector3 targetPosition, float fieldOfView, LayerMask collisionMask, Vector3 offset) {
            //print("Inside IsInLineOfSight()");
            Vector3 dir = targetPosition - origin.position;

            /// if something is within the field of view
            if (Vector3.Angle(origin.forward, dir.normalized) < fieldOfView / 2) {
                float distanceToTarget = Vector3.Distance(origin.position, targetPosition);
                RaycastHit hit;
                /// if another object is between the target and the object (gotten by checking the layer mask)
                if (Physics.Raycast(origin.position + offset + origin.forward * .3f, dir.normalized, distanceToTarget, collisionMask)) {
                    /// return false, because something is blocking the view.
                    //print("Inside angle");
                    //print(hit.transform.name);
                    Debug.DrawLine(origin.position + offset, dir.normalized + origin.forward * distanceToTarget, Color.red);
                    return false;
                }

                return true;
            }

            /// If something is outside the field of view, return false because it's not detectable.
            return false;
        }
    }
}