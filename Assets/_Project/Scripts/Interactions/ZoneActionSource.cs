using Unity.XR.CoreUtils;
using UnityEngine;
using VRTraining.Core;

namespace VRTraining.Interactions
{
    [RequireComponent(typeof(Collider))]
    public sealed class ZoneActionSource : TrainingActionSource
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<XROrigin>() == null)
                return;

            Publish(TrainingActionType.EnterZone);
        }

        protected override void Reset()
        {
            base.Reset();

            var zoneCollider = GetComponent<Collider>();
            if (zoneCollider != null)
                zoneCollider.isTrigger = true;
        }
    }
}
