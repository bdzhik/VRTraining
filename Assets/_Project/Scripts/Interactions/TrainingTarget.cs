using UnityEngine;

namespace VRTraining.Interactions
{
    /// <summary>
    /// Стабильный идентификатор интерактивного объекта, независимый от имени GameObject.
    /// </summary>
    public sealed class TrainingTarget : MonoBehaviour
    {
        [SerializeField] private string targetId;

        public string TargetId => targetId;

        private void OnValidate()
        {
            if (targetId != null)
                targetId = targetId.Trim();
        }
    }
}
