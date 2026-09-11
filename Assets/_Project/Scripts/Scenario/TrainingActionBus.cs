using System;
using UnityEngine;
using VRTraining.Core;

namespace VRTraining.Scenario
{
    /// <summary>
    /// Локальная для сцены шина действий. Не использует static и не хранит глобальное состояние.
    /// </summary>
    public sealed class TrainingActionBus : MonoBehaviour
    {
        public event Action<TrainingAction> ActionRaised;

        public void Raise(TrainingAction action)
        {
            ActionRaised?.Invoke(action);
        }
    }
}
