using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
#endif

namespace VRTraining.Infrastructure
{
    /// <summary>
    /// Переподключает сохраняемый между сценами XR Interaction Simulator
    /// к XR Origin текущей сцены. Используется только при запуске в Editor.
    /// </summary>
    public sealed class XRSimulatorSceneRebinder : MonoBehaviour
    {
        private void Start()
        {
#if UNITY_EDITOR
            var simulator = XRInteractionSimulator.instance;
            if (simulator == null)
                return;

            // Повторный OnEnable заставляет симулятор найти новые камеру,
            // XR Origin и контроллеры после смены сцены.
            simulator.enabled = false;
            simulator.enabled = true;
#endif
        }
    }
}
