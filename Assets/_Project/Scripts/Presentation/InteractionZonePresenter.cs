using System;
using System.Collections.Generic;
using UnityEngine;
using VRTraining.Core;
using VRTraining.Interactions;
using VRTraining.Scenario;

namespace VRTraining.Presentation
{
    /// <summary>
    /// Показывает только ожидаемую зону, сохраняя триггеры всех зон активными.
    /// Это позволяет сценарию обнаружить вход в неправильную зону как нарушение.
    /// Управляет представлением сцены и не содержит правил прохождения сценария.
    /// </summary>
    public sealed class InteractionZonePresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioController scenarioController;
        [SerializeField] private ZoneActionSource[] zones;

        private readonly Dictionary<ZoneActionSource, Renderer[]> renderersByZone =
            new Dictionary<ZoneActionSource, Renderer[]>();

        private void Awake()
        {
            if (zones == null || zones.Length == 0)
                zones = GetComponentsInChildren<ZoneActionSource>(true);

            foreach (var zone in zones)
            {
                if (zone != null)
                    renderersByZone[zone] = zone.GetComponentsInChildren<Renderer>(true);
            }
        }

        private void OnEnable()
        {
            if (scenarioController != null)
                scenarioController.StateChanged += Refresh;
        }

        private void Start()
        {
            Refresh();
        }

        private void OnDisable()
        {
            if (scenarioController != null)
                scenarioController.StateChanged -= Refresh;
        }

        private void Refresh()
        {
            var expectedAction = scenarioController != null
                ? scenarioController.CurrentExpectedAction
                : null;

            var expectedZoneId = expectedAction != null &&
                                 expectedAction.ActionType == TrainingActionType.EnterZone
                ? expectedAction.TargetId
                : null;

            foreach (var zone in zones)
            {
                if (zone == null)
                    continue;

                var target = zone.GetComponent<TrainingTarget>();
                var shouldBeVisible = target != null &&
                                      string.Equals(
                                          target.TargetId,
                                          expectedZoneId,
                                          StringComparison.Ordinal);

                // Collider и источник действия работают постоянно. Выключается
                // только визуальная подсказка зоны, а не само распознавание входа.
                if (!zone.gameObject.activeSelf)
                    zone.gameObject.SetActive(true);

                if (!renderersByZone.TryGetValue(zone, out var renderers))
                    continue;

                foreach (var zoneRenderer in renderers)
                {
                    if (zoneRenderer != null)
                        zoneRenderer.enabled = shouldBeVisible;
                }
            }
        }
    }
}
