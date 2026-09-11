using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Scenario.Definitions
{
    [Serializable]
    public sealed class StepGroupDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private string title;
        [SerializeField, TextArea(2, 5)] private string instruction;
        [SerializeField] private List<StepDefinition> steps = new List<StepDefinition>();

        public string Id => id;
        public string Title => title;
        public string Instruction => instruction;
        public IReadOnlyList<StepDefinition> Steps => steps;
    }
}
