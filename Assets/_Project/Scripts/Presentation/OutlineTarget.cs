using System.Collections.Generic;
using UnityEngine;
using VRTraining.Interactions;

namespace VRTraining.Presentation
{
    /// <summary>
    /// Добавляет outline-материал к Renderer только на время подсветки.
    /// </summary>
    [RequireComponent(typeof(TrainingTarget))]
    public sealed class OutlineTarget : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Color outlineColor = new Color(1f, 0.75f, 0f, 1f);
        [SerializeField, Range(0.001f, 0.1f)] private float outlineWidth = 0.015f;

        private Material outlineMaterial;
        private bool isHighlighted;

        public TrainingTarget Target { get; private set; }

        private void Awake()
        {
            Target = GetComponent<TrainingTarget>();

            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<Renderer>(true);

            var shader = Shader.Find("VRTraining/Outline");
            if (shader == null)
            {
                Debug.LogError("Shader 'VRTraining/Outline' was not found.", this);
                return;
            }

            outlineMaterial = new Material(shader)
            {
                name = $"{name} Outline (Runtime)"
            };
            outlineMaterial.SetColor("_OutlineColor", outlineColor);
            outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
        }

        private void OnDestroy()
        {
            RemoveOutlineMaterial();

            if (outlineMaterial != null)
                Destroy(outlineMaterial);
        }

        public void SetHighlighted(bool highlighted)
        {
            if (isHighlighted == highlighted || outlineMaterial == null)
                return;

            isHighlighted = highlighted;

            if (highlighted)
                AddOutlineMaterial();
            else
                RemoveOutlineMaterial();
        }

        private void AddOutlineMaterial()
        {
            foreach (var targetRenderer in renderers)
            {
                if (targetRenderer == null)
                    continue;

                var materials = new List<Material>(targetRenderer.sharedMaterials);
                if (!materials.Contains(outlineMaterial))
                {
                    materials.Add(outlineMaterial);
                    targetRenderer.sharedMaterials = materials.ToArray();
                }
            }
        }

        private void RemoveOutlineMaterial()
        {
            if (renderers == null || outlineMaterial == null)
                return;

            foreach (var targetRenderer in renderers)
            {
                if (targetRenderer == null)
                    continue;

                var materials = new List<Material>(targetRenderer.sharedMaterials);
                if (materials.Remove(outlineMaterial))
                    targetRenderer.sharedMaterials = materials.ToArray();
            }

            isHighlighted = false;
        }
    }
}
