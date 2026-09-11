using System;
using UnityEngine;
using VRTraining.Core;
using VRTraining.Scenario;

namespace VRTraining.Presentation
{
    /// <summary>
    /// Отображает только визуальное состояние конвейера и не содержит правил сценария.
    /// </summary>
    public sealed class ConveyorVisualPresenter : MonoBehaviour
    {
        [Header("Scenario")]
        [SerializeField] private ScenarioController scenarioController;
        [SerializeField] private string stopButtonTargetId = "conveyor.stop-button";
        [SerializeField] private string powerSwitchTargetId = "conveyor.power-switch";
        [SerializeField] private string cardSocketTargetId = "conveyor.card-socket";

        [Header("Rollers")]
        [SerializeField] private Transform rollersRoot;
        [SerializeField] private Transform[] rollers;
        [SerializeField] private Vector3 localRotationAxis = Vector3.up;
        [SerializeField] private float rotationSpeed = 120f;

        [Header("Indicators")]
        [SerializeField] private Renderer powerIndicator;
        [SerializeField] private Color poweredColor = Color.green;
        [SerializeField] private Color stoppedColor = Color.red;
        [SerializeField] private Color powerOffColor = Color.gray;
        [SerializeField] private Renderer lockIndicator;
        [SerializeField] private Color unlockedColor = Color.red;
        [SerializeField] private Color lockedColor = Color.green;

        [Header("Power Lever")]
        [SerializeField] private Transform leverPivot;
        [SerializeField] private float leverOffAngleX = -70f;
        [SerializeField, Min(0f)] private float leverRotationDuration = 0.35f;

        private MaterialPropertyBlock propertyBlock;
        private int baseColorId;
        private int colorId;
        private int emissionColorId;
        private bool isRunning = true;
        private bool isLeverMoving;
        private Quaternion leverStartRotation;
        private Quaternion leverTargetRotation;
        private float leverRotationElapsed;

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            baseColorId = Shader.PropertyToID("_BaseColor");
            colorId = Shader.PropertyToID("_Color");
            emissionColorId = Shader.PropertyToID("_EmissionColor");
        }

        private void OnEnable()
        {
            if (scenarioController != null)
                scenarioController.ActionEvaluated += HandleEvaluation;
        }

        private void Start()
        {
            CollectRollersIfNeeded();
            SetColor(powerIndicator, poweredColor);
            SetColor(lockIndicator, unlockedColor);
        }

        private void Update()
        {
            if (isRunning && rollers != null)
            {
                var rotation = localRotationAxis.normalized * (rotationSpeed * Time.deltaTime);
                foreach (var roller in rollers)
                {
                    if (roller != null)
                        roller.Rotate(rotation, Space.Self);
                }
            }

            UpdateLeverRotation();
        }

        private void OnDisable()
        {
            if (scenarioController != null)
                scenarioController.ActionEvaluated -= HandleEvaluation;
        }

        private void HandleEvaluation(ScenarioEvaluation evaluation)
        {
            if (evaluation.Feedback != ScenarioFeedbackType.Correct)
                return;

            var targetId = evaluation.Action.TargetId;

            if (string.Equals(targetId, stopButtonTargetId, StringComparison.Ordinal))
            {
                isRunning = false;
                SetColor(powerIndicator, stoppedColor);
            }
            else if (string.Equals(targetId, powerSwitchTargetId, StringComparison.Ordinal))
            {
                SetColor(powerIndicator, powerOffColor);
                BeginLeverRotation();
            }
            else if (string.Equals(targetId, cardSocketTargetId, StringComparison.Ordinal))
            {
                SetColor(lockIndicator, lockedColor);
            }
        }

        private void BeginLeverRotation()
        {
            if (leverPivot == null)
                return;

            leverStartRotation = leverPivot.localRotation;
            var currentAngles = leverPivot.localEulerAngles;
            leverTargetRotation = Quaternion.Euler(
                leverOffAngleX,
                currentAngles.y,
                currentAngles.z);
            leverRotationElapsed = 0f;
            isLeverMoving = true;
        }

        private void UpdateLeverRotation()
        {
            if (!isLeverMoving || leverPivot == null)
                return;

            leverRotationElapsed += Time.deltaTime;
            var progress = leverRotationDuration <= 0f
                ? 1f
                : Mathf.Clamp01(leverRotationElapsed / leverRotationDuration);

            leverPivot.localRotation = Quaternion.Slerp(
                leverStartRotation,
                leverTargetRotation,
                progress);

            if (progress >= 1f)
                isLeverMoving = false;
        }

        private void SetColor(Renderer targetRenderer, Color color)
        {
            if (targetRenderer == null)
                return;

            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(baseColorId, color);
            propertyBlock.SetColor(colorId, color);
            propertyBlock.SetColor(emissionColorId, color);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }

        private void CollectRollersIfNeeded()
        {
            if ((rollers != null && rollers.Length > 0) || rollersRoot == null)
                return;

            rollers = new Transform[rollersRoot.childCount];
            for (var index = 0; index < rollersRoot.childCount; index++)
                rollers[index] = rollersRoot.GetChild(index);
        }
    }
}
