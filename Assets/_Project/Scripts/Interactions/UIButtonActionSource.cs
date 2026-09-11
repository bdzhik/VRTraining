using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core;

namespace VRTraining.Interactions
{
    [RequireComponent(typeof(Button))]
    public sealed class UIButtonActionSource : TrainingActionSource
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (button == null)
                button = GetComponent<Button>();

            button.onClick.AddListener(HandleClicked);
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);
        }

        private void HandleClicked()
        {
            Publish(TrainingActionType.PressUIButton);
        }
    }
}
