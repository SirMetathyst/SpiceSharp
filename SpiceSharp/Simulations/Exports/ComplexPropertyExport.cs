﻿using System;
using System.Numerics;
using SpiceSharp.Behaviors;

namespace SpiceSharp.Simulations
{
    public class ComplexPropertyExport : Export<Complex>
    {
        /// <summary>
        /// Gets the name of the entity
        /// </summary>
        public Identifier EntityName { get; }

        /// <summary>
        /// Gets the property name
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="simulation">Simulation</param>
        /// <param name="entityName">Entity name</param>
        /// <param name="propertyName">Property name</param>
        public ComplexPropertyExport(Simulation simulation, Identifier entityName, string propertyName)
            : base(simulation)
        {
            EntityName = entityName ?? throw new ArgumentNullException(nameof(entityName));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        protected override void Initialize(object sender, InitializeSimulationEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var eb = e.Behaviors.GetEntityBehaviors(EntityName);
            Func<ComplexState, Complex> stateExtractor = null;

            // Get the necessary behavior in order:
            // 1) First try transient analysis
            var behavior = eb.Get<FrequencyBehavior>();
            if (behavior != null)
                stateExtractor = behavior.CreateACExport(PropertyName);

            // Create the extractor
            if (stateExtractor != null)
            {
                var state = Simulation.States.Get<ComplexState>();
                Extractor = () => stateExtractor(state);
            }
        }
    }
}
