﻿using SpiceSharp.Circuits;
using SpiceSharp.Behaviors.ISRC;
using SpiceSharp.Components.ISRC;

namespace SpiceSharp.Components
{
    /// <summary>
    /// An independent current source
    /// </summary>
    [SpicePins("I+", "I-"), IndependentSource, ConnectedAttribute()]
    public class Currentsource : Component
    {
        /// <summary>
        /// Nodes
        /// </summary>
        public int ISRCposNode { get; private set; }
        public int ISRCnegNode { get; private set; }

        /// <summary>
        /// Constants
        /// </summary>
        private const int ISRCpinCount = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the current source</param>
        public Currentsource(Identifier name) 
            : base(name, ISRCpinCount)
        {
            // Add parameters
            Parameters.Add(new BaseParameters());
            Parameters.Add(new AcParameters());

            // Add factories
            AddFactory(typeof(LoadBehavior), () => new LoadBehavior(Name));
            AddFactory(typeof(AcBehavior), () => new AcBehavior(Name));
            AddFactory(typeof(AcceptBehavior), () => new AcceptBehavior(Name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the current source</param>
        /// <param name="pos">The positive node</param>
        /// <param name="neg">The negative node</param>
        /// <param name="dc">The DC value</param>
        public Currentsource(Identifier name, Identifier pos, Identifier neg, double dc)
            : base(name, ISRCpinCount)
        {
            // Add parameters
            Parameters.Add(new BaseParameters(dc));
            Parameters.Add(new AcParameters());

            // Add factories
            AddFactory(typeof(LoadBehavior), () => new LoadBehavior(Name));
            AddFactory(typeof(AcBehavior), () => new AcBehavior(Name));
            AddFactory(typeof(AcceptBehavior), () => new AcceptBehavior(Name));

            // Connect
            Connect(pos, neg);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the current source</param>
        /// <param name="pos">The positive node</param>
        /// <param name="neg">The negative node</param>
        /// <param name="w">The Waveform-object</param>
        public Currentsource(Identifier name, Identifier pos, Identifier neg, Waveform w)
            : base(name, ISRCpinCount)
        {
            // Add parameters
            Parameters.Add(new BaseParameters(w));
            Parameters.Add(new AcParameters());

            // Add factories
            AddFactory(typeof(LoadBehavior), () => new LoadBehavior(Name));
            AddFactory(typeof(AcBehavior), () => new AcBehavior(Name));
            AddFactory(typeof(AcceptBehavior), () => new AcceptBehavior(Name));

            // Connect
            Connect(pos, neg);
        }

        /// <summary>
        /// Setup the current source
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Setup(Circuit ckt)
        {
            var nodes = BindNodes(ckt);
            ISRCposNode = nodes[0].Index;
            ISRCnegNode = nodes[1].Index;
        }
    }
}
