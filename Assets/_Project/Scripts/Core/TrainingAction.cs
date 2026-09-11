using System;

namespace VRTraining.Core
{
    /// <summary>
    /// Независимое от Unity-представления сообщение о действии пользователя.
    /// </summary>
    public readonly struct TrainingAction : IEquatable<TrainingAction>
    {
        public TrainingAction(TrainingActionType type, string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
                throw new ArgumentException("Target id cannot be empty.", nameof(targetId));

            Type = type;
            TargetId = targetId.Trim();
        }

        public TrainingActionType Type { get; }
        public string TargetId { get; }

        public bool Equals(TrainingAction other)
        {
            return Type == other.Type &&
                   string.Equals(TargetId, other.TargetId, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is TrainingAction other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Type * 397) ^ (TargetId != null ? TargetId.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{Type}: {TargetId}";
        }
    }
}
