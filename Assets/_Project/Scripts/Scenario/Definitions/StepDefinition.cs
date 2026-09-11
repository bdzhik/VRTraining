using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Scenario.Definitions
{
    [Serializable]
    public sealed class StepDefinition
    {
        [SerializeField] private string id;
        [SerializeField, TextArea(2, 4)] private string description;
        [SerializeField] private List<ExpectedActionDefinition> expectedActions =
            new List<ExpectedActionDefinition>();

        public string Id => id;
        public string Description => description;
        public IReadOnlyList<ExpectedActionDefinition> ExpectedActions => expectedActions;
    }
}
