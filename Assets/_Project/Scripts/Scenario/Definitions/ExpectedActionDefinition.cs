using System;
using UnityEngine;
using VRTraining.Core;

namespace VRTraining.Scenario.Definitions
{
    [Serializable]
    public sealed class ExpectedActionDefinition
    {
        [SerializeField] private TrainingActionType actionType;
        [SerializeField] private string targetId;

        public TrainingActionType ActionType => actionType;
        public string TargetId => targetId;

        public bool Matches(TrainingAction action)
        {
            return actionType == action.Type &&
                   string.Equals(targetId, action.TargetId, StringComparison.Ordinal);
        }
    }
}
