using UnityEngine;
using VRTraining.Core;
using VRTraining.Scenario;

namespace VRTraining.Presentation
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioFeedbackPresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioController scenarioController;
        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;
        [SerializeField] private AudioClip sequenceViolationClip;

        private AudioSource audioSource;
        private AudioClip generatedSuccessClip;
        private AudioClip generatedErrorClip;
        private AudioClip generatedSequenceViolationClip;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            generatedSuccessClip = successClip == null
                ? CreateToneClip("Success Tone", 880f, 0.14f)
                : null;
            generatedErrorClip = errorClip == null
                ? CreateToneClip("Error Tone", 220f, 0.22f)
                : null;
            generatedSequenceViolationClip = sequenceViolationClip == null
                ? CreateToneClip("Sequence Violation Tone", 140f, 0.35f)
                : null;
        }

        private void OnEnable()
        {
            if (scenarioController != null)
                scenarioController.ActionEvaluated += HandleEvaluation;
        }

        private void OnDisable()
        {
            if (scenarioController != null)
                scenarioController.ActionEvaluated -= HandleEvaluation;
        }

        private void OnDestroy()
        {
            DestroyGeneratedClip(generatedSuccessClip);
            DestroyGeneratedClip(generatedErrorClip);
            DestroyGeneratedClip(generatedSequenceViolationClip);
        }

        private void HandleEvaluation(ScenarioEvaluation evaluation)
        {
            AudioClip clip;

            switch (evaluation.Feedback)
            {
                case ScenarioFeedbackType.Correct:
                    clip = successClip != null ? successClip : generatedSuccessClip;
                    break;
                case ScenarioFeedbackType.Error:
                    clip = errorClip != null ? errorClip : generatedErrorClip;
                    break;
                case ScenarioFeedbackType.SequenceViolation:
                    clip = sequenceViolationClip != null
                        ? sequenceViolationClip
                        : generatedSequenceViolationClip;
                    break;
                default:
                    clip = null;
                    break;
            }

            if (clip != null)
                audioSource.PlayOneShot(clip);
        }

        private static AudioClip CreateToneClip(string clipName, float frequency, float duration)
        {
            const int sampleRate = 44100;
            var sampleCount = Mathf.CeilToInt(sampleRate * duration);
            var samples = new float[sampleCount];

            for (var sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
            {
                var time = (float)sampleIndex / sampleRate;
                var fade = 1f - (float)sampleIndex / sampleCount;
                samples[sampleIndex] = Mathf.Sin(2f * Mathf.PI * frequency * time) * fade * 0.25f;
            }

            var clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static void DestroyGeneratedClip(AudioClip clip)
        {
            if (clip != null)
                Destroy(clip);
        }
    }
}
