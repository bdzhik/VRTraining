using UnityEngine;
using VRTraining.Core;
using VRTraining.Scenario;

namespace VRTraining.Interactions
{
    public abstract class TrainingActionSource : MonoBehaviour
    {
        [SerializeField] private TrainingActionBus actionBus;
        [SerializeField] private TrainingTarget target;

        protected void Publish(TrainingActionType actionType)
        {
            if (target == null)
            {
                Debug.LogError("Training Target is not assigned.", this);
                return;
            }

            Publish(actionType, target.TargetId);
        }

        protected void Publish(TrainingActionType actionType, string targetId)
        {
            if (actionBus == null)
            {
                Debug.LogError("Action Bus is not assigned.", this);
                return;
            }

            if (string.IsNullOrWhiteSpace(targetId))
            {
                Debug.LogError("Training Target id is empty.", this);
                return;
            }

            actionBus.Raise(new TrainingAction(actionType, targetId));
        }

        protected virtual void Reset()
        {
            target = GetComponent<TrainingTarget>();
        }
    }
}
