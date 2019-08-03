﻿using SpiceSharp.Attributes;

namespace SpiceSharp.Components.CommonBehaviors
{
    /// <summary>
    /// Base parameters for an independent source.
    /// </summary>
    public class IndependentSourceParameters : ParameterSet
    {
        /// <summary>
        /// The time-dependent waveform of the source.
        /// </summary>
        [ParameterName("waveform"), ParameterInfo("The waveform object for this source")]
        public Waveform Waveform { get; set; }

        /// <summary>
        /// The DC value of the source.
        /// </summary>
        [ParameterName("dc"), ParameterInfo("D.C. source value")]
        public GivenParameter<double> DcValue { get; } = new GivenParameter<double>();

        /// <summary>
        /// Constructor
        /// </summary>
        public IndependentSourceParameters()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dc">DC value</param>
        public IndependentSourceParameters(double dc)
        {
            DcValue.Value = dc;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="waveform">Waveform</param>
        public IndependentSourceParameters(Waveform waveform)
        {
            Waveform = waveform;
        }
    }
}
