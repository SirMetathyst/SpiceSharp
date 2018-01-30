﻿using System;
using SpiceSharp.Circuits;
using SpiceSharp.Components.Semiconductors;
using SpiceSharp.Attributes;
using SpiceSharp.Sparse;
using SpiceSharp.Simulations;
using SpiceSharp.Behaviors;

namespace SpiceSharp.Components.BipolarBehaviors
{
    /// <summary>
    /// General behavior for <see cref="BipolarJunctionTransistor"/>
    /// </summary>
    public class LoadBehavior : Behaviors.LoadBehavior, IConnectedBehavior
    {
        /// <summary>
        /// Necessary behaviors
        /// </summary>
        BaseParameters bp;
        ModelBaseParameters mbp;
        TemperatureBehavior temp;
        ModelTemperatureBehavior modeltemp;

        /// <summary>
        /// Methods
        /// </summary>
        [PropertyName("vbe"), PropertyInfo("B-E voltage")]
        public double Vbe { get; protected set; }
        [PropertyName("vbc"), PropertyInfo("B-C voltage")]
        public double Vbc { get; protected set; }
        [PropertyName("cc"), PropertyInfo("Current at collector node")]
        public double Cc { get; protected set; }
        [PropertyName("cb"), PropertyInfo("Current at base node")]
        public double Cb { get; protected set; }
        [PropertyName("gpi"), PropertyInfo("Small signal input conductance - pi")]
        public double Gpi { get; protected set; }
        [PropertyName("gmu"), PropertyInfo("Small signal conductance - mu")]
        public double Gmu { get; protected set; }
        [PropertyName("gm"), PropertyInfo("Small signal transconductance")]
        public double Gm { get; protected set; }
        [PropertyName("go"), PropertyInfo("Small signal output conductance")]
        public double Go { get; protected set; }
        public double Gx { get; protected set; }

        /// <summary>
        /// Nodes
        /// </summary>
        int collectorNode, baseNode, emitterNode, substrateNode;
        public int CollectorPrimeNode { get; private set; }
        public int BasePrimeNode { get; private set; }
        public int EmitterPrimeNode { get; private set; }
        protected MatrixElement CollectorCollectorPrimePtr { get; private set; }
        protected MatrixElement BaseBasePrimePtr { get; private set; }
        protected MatrixElement EmitterEmitterPrimePtr { get; private set; }
        protected MatrixElement CollectorPrimeCollectorPtr { get; private set; }
        protected MatrixElement CollectorPrimeBasePrimePtr { get; private set; }
        protected MatrixElement CollectorPrimeEmitterPrimePtr { get; private set; }
        protected MatrixElement BasePrimeBasePtr { get; private set; }
        protected MatrixElement BasePrimeCollectorPrimePtr { get; private set; }
        protected MatrixElement BasePrimeEmitterPrimePtr { get; private set; }
        protected MatrixElement EmitterPrimeEmitterPtr { get; private set; }
        protected MatrixElement EmitterPrimeCollectorPrimePtr { get; private set; }
        protected MatrixElement EmitterPrimeBasePrimePtr { get; private set; }
        protected MatrixElement CollectorCollectorPtr { get; private set; }
        protected MatrixElement BaseBasePtr { get; private set; }
        protected MatrixElement EmitterEmitterPtr { get; private set; }
        protected MatrixElement CollectorPrimeCollectorPrimePtr { get; private set; }
        protected MatrixElement BasePrimeBasePrimePtr { get; private set; }
        protected MatrixElement EmitterPrimeEmitterPrimePtr { get; private set; }
        protected MatrixElement SubstrateSubstratePtr { get; private set; }
        protected MatrixElement CollectorPrimeSubstratePtr { get; private set; }
        protected MatrixElement SubstrateCollectorPrimePtr { get; private set; }
        protected MatrixElement BaseCollectorPrimePtr { get; private set; }
        protected MatrixElement CollectorPrimeBasePtr { get; private set; }

        /// <summary>
        /// Shared parameters
        /// </summary>
        public double Cbe { get; protected set; }
        public double Gbe { get; protected set; }
        public double Cbc { get; protected set; }
        public double Gbc { get; protected set; }
        public double Qb { get; protected set; }
        public double DqbDvc { get; protected set; }
        public double DqbDve { get; protected set; }

        /// <summary>
        /// Event called when excess phase calculation is needed
        /// </summary>
        public event EventHandler<ExcessPhaseEventArgs> ExcessPhaseCalculation;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        public LoadBehavior(Identifier name) : base(name) { }

        /// <summary>
        /// Setup behavior
        /// </summary>
        /// <param name="provider">Data provider</param>
        public override void Setup(SetupDataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            // Get parameters
            bp = provider.GetParameterSet<BaseParameters>(0);
            mbp = provider.GetParameterSet<ModelBaseParameters>(1);

            // Get behaviors
            temp = provider.GetBehavior<TemperatureBehavior>(0);
            modeltemp = provider.GetBehavior<ModelTemperatureBehavior>(1);
        }

        /// <summary>
        /// Connect
        /// </summary>
        /// <param name="pins">Pins</param>
        public void Connect(params int[] pins)
        {
            if (pins == null)
                throw new ArgumentNullException(nameof(pins));
            if (pins.Length != 4)
                throw new Diagnostics.CircuitException("Pin count mismatch: 4 pins expected, {0} given".FormatString(pins.Length));
            collectorNode = pins[0];
            baseNode = pins[1];
            emitterNode = pins[2];
            substrateNode = pins[3];
        }

        /// <summary>
        /// Get matrix pointers
        /// </summary>
        /// <param name="nodes">Nodes</param>
        /// <param name="matrix">Matrix</param>
        public override void GetMatrixPointers(Nodes nodes, Matrix matrix)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            // Add a series collector node if necessary
            if (mbp.CollectorResistance.Value > 0)
                CollectorPrimeNode = nodes.Create(Name.Grow("#col")).Index;
            else
                CollectorPrimeNode = collectorNode;

            // Add a series base node if necessary
            if (mbp.BaseResist.Value > 0)
                BasePrimeNode = nodes.Create(Name.Grow("#base")).Index;
            else
                BasePrimeNode = baseNode;

            // Add a series emitter node if necessary
            if (mbp.EmitterResistance.Value > 0)
                EmitterPrimeNode = nodes.Create(Name.Grow("#emit")).Index;
            else
                EmitterPrimeNode = emitterNode;

            // Get matrix pointers
            CollectorCollectorPrimePtr = matrix.GetElement(collectorNode, CollectorPrimeNode);
            BaseBasePrimePtr = matrix.GetElement(baseNode, BasePrimeNode);
            EmitterEmitterPrimePtr = matrix.GetElement(emitterNode, EmitterPrimeNode);
            CollectorPrimeCollectorPtr = matrix.GetElement(CollectorPrimeNode, collectorNode);
            CollectorPrimeBasePrimePtr = matrix.GetElement(CollectorPrimeNode, BasePrimeNode);
            CollectorPrimeEmitterPrimePtr = matrix.GetElement(CollectorPrimeNode, EmitterPrimeNode);
            BasePrimeBasePtr = matrix.GetElement(BasePrimeNode, baseNode);
            BasePrimeCollectorPrimePtr = matrix.GetElement(BasePrimeNode, CollectorPrimeNode);
            BasePrimeEmitterPrimePtr = matrix.GetElement(BasePrimeNode, EmitterPrimeNode);
            EmitterPrimeEmitterPtr = matrix.GetElement(EmitterPrimeNode, emitterNode);
            EmitterPrimeCollectorPrimePtr = matrix.GetElement(EmitterPrimeNode, CollectorPrimeNode);
            EmitterPrimeBasePrimePtr = matrix.GetElement(EmitterPrimeNode, BasePrimeNode);
            CollectorCollectorPtr = matrix.GetElement(collectorNode, collectorNode);
            BaseBasePtr = matrix.GetElement(baseNode, baseNode);
            EmitterEmitterPtr = matrix.GetElement(emitterNode, emitterNode);
            CollectorPrimeCollectorPrimePtr = matrix.GetElement(CollectorPrimeNode, CollectorPrimeNode);
            BasePrimeBasePrimePtr = matrix.GetElement(BasePrimeNode, BasePrimeNode);
            EmitterPrimeEmitterPrimePtr = matrix.GetElement(EmitterPrimeNode, EmitterPrimeNode);
            SubstrateSubstratePtr = matrix.GetElement(substrateNode, substrateNode);
            CollectorPrimeSubstratePtr = matrix.GetElement(CollectorPrimeNode, substrateNode);
            SubstrateCollectorPrimePtr = matrix.GetElement(substrateNode, CollectorPrimeNode);
            BaseCollectorPrimePtr = matrix.GetElement(baseNode, CollectorPrimeNode);
            CollectorPrimeBasePtr = matrix.GetElement(CollectorPrimeNode, baseNode);
        }
        
        /// <summary>
        /// Unsetup
        /// </summary>
        public override void Unsetup()
        {
            // Remove references
            CollectorCollectorPrimePtr = null;
            BaseBasePrimePtr = null;
            EmitterEmitterPrimePtr = null;
            CollectorPrimeCollectorPtr = null;
            CollectorPrimeBasePrimePtr = null;
            CollectorPrimeEmitterPrimePtr = null;
            BasePrimeBasePtr = null;
            BasePrimeCollectorPrimePtr = null;
            BasePrimeEmitterPrimePtr = null;
            EmitterPrimeEmitterPtr = null;
            EmitterPrimeCollectorPrimePtr = null;
            EmitterPrimeBasePrimePtr = null;
            CollectorCollectorPtr = null;
            BaseBasePtr = null;
            EmitterEmitterPtr = null;
            CollectorPrimeCollectorPrimePtr = null;
            BasePrimeBasePrimePtr = null;
            EmitterPrimeEmitterPrimePtr = null;
            SubstrateSubstratePtr = null;
            CollectorPrimeSubstratePtr = null;
            SubstrateCollectorPrimePtr = null;
            BaseCollectorPrimePtr = null;
            CollectorPrimeBasePtr = null;
        }

        /// <summary>
        /// Execute behavior
        /// </summary>
        /// <param name="simulation">Base simulation</param>
        public override void Load(BaseSimulation simulation)
        {
            if (simulation == null)
                throw new ArgumentNullException(nameof(simulation));

            var state = simulation.State;
            double vt;
            double ceqcs, ceqbx, csat, rbpr, rbpi, gcpr, gepr, oik, c2, vte, oikr, c4, vtc, xjrb, vbe, vbc;
            bool icheck;
            double vce;
            bool ichk1;
            double vtn, evbe, gben, evben, cben, evbc, gbcn, evbcn, cbcn, q1, q2, arg, sqarg, cc, cex,
                gex, arg1, arg2, cb, gx, gpi, gmu, go, gm;
            double ceqbe, ceqbc;

            vt = bp.Temperature * Circuit.KOverQ;

            ceqcs = 0;
            ceqbx = 0;

            /* 
			 * dc model paramters
			 */
            csat = temp.TSatCur * bp.Area;
            rbpr = mbp.MinimumBaseResistance / bp.Area;
            rbpi = mbp.BaseResist / bp.Area - rbpr;
            gcpr = modeltemp.CollectorConduct * bp.Area;
            gepr = modeltemp.EmitterConduct * bp.Area;
            oik = modeltemp.InverseRollOffForward / bp.Area;
            c2 = temp.TBEleakCur * bp.Area;
            vte = mbp.LeakBEEmissionCoefficient * vt;
            oikr = modeltemp.InverseRollOffReverse / bp.Area;
            c4 = temp.TBCleakCur * bp.Area;
            vtc = mbp.LeakBCEmissionCoefficient * vt;
            xjrb = mbp.BaseCurrentHalfResist * bp.Area;

            /* 
			* initialization
			*/
            icheck = false;
            if (state.Init == State.InitializationStates.InitJct && state.Domain == State.DomainType.Time && state.UseDC && state.UseIC)
            {
                vbe = mbp.BipolarType * bp.InitialVbe;
                vce = mbp.BipolarType * bp.InitialVce;
                vbc = vbe - vce;
            }
            else if (state.Init == State.InitializationStates.InitJct && !bp.Off)
            {
                vbe = temp.TVcrit;
                vbc = 0;
            }
            else if (state.Init == State.InitializationStates.InitJct || (state.Init == State.InitializationStates.InitFix && bp.Off))
            {
                vbe = 0;
                vbc = 0;
            }
            else
            {
                /* 
                 * compute new nonlinear branch voltages
                 */
                vbe = mbp.BipolarType * (state.Solution[BasePrimeNode] - state.Solution[EmitterPrimeNode]);
                vbc = mbp.BipolarType * (state.Solution[BasePrimeNode] - state.Solution[CollectorPrimeNode]);

                /* 
				 * limit nonlinear branch voltages
				 */
                ichk1 = true;
                vbe = Semiconductor.DEVpnjlim(vbe, Vbe, vt, temp.TVcrit, ref icheck);
                vbc = Semiconductor.DEVpnjlim(vbc, Vbc, vt, temp.TVcrit, ref ichk1);
                if (ichk1 == true)
                    icheck = true;
            }

            /* 
			 * determine dc current and derivitives
			 */
            vtn = vt * mbp.EmissionCoefficientForward;
            if (vbe > -5 * vtn)
            {
                evbe = Math.Exp(vbe / vtn);
                Cbe = csat * (evbe - 1) + state.Gmin * vbe;
                Gbe = csat * evbe / vtn + state.Gmin;
                if (c2 == 0)
                {
                    cben = 0;
                    gben = 0;
                }
                else
                {
                    evben = Math.Exp(vbe / vte);
                    cben = c2 * (evben - 1);
                    gben = c2 * evben / vte;
                }
            }
            else
            {
                Gbe = -csat / vbe + state.Gmin;
                Cbe = Gbe * vbe;
                gben = -c2 / vbe;
                cben = gben * vbe;
            }

            vtn = vt * mbp.EmissionCoefficientReverse;
            if (vbc > -5 * vtn)
            {
                evbc = Math.Exp(vbc / vtn);
                Cbc = csat * (evbc - 1) + state.Gmin * vbc;
                Gbc = csat * evbc / vtn + state.Gmin;
                if (c4 == 0)
                {
                    cbcn = 0;
                    gbcn = 0;
                }
                else
                {
                    evbcn = Math.Exp(vbc / vtc);
                    cbcn = c4 * (evbcn - 1);
                    gbcn = c4 * evbcn / vtc;
                }
            }
            else
            {
                Gbc = -csat / vbc + state.Gmin;
                Cbc = Gbc * vbc;
                gbcn = -c4 / vbc;
                cbcn = gbcn * vbc;
            }

            /* 
			 * determine base charge terms
			 */
            q1 = 1 / (1 - modeltemp.InvEarlyVoltForward * vbc - modeltemp.InvEarlyVoltReverse * vbe);
            if (oik == 0 && oikr == 0)
            {
                Qb = q1;
                DqbDve = q1 * Qb * modeltemp.InvEarlyVoltReverse;
                DqbDvc = q1 * Qb * modeltemp.InvEarlyVoltForward;
            }
            else
            {
                q2 = oik * Cbe + oikr * Cbc;
                arg = Math.Max(0, 1 + 4 * q2);
                sqarg = 1;
                if (arg != 0)
                    sqarg = Math.Sqrt(arg);
                Qb = q1 * (1 + sqarg) / 2;
                DqbDve = q1 * (Qb * modeltemp.InvEarlyVoltReverse + oik * Gbe / sqarg);
                DqbDvc = q1 * (Qb * modeltemp.InvEarlyVoltForward + oikr * Gbc / sqarg);
            }

            // Excess phase calculation
            ExcessPhaseEventArgs ep = new ExcessPhaseEventArgs()
            {
                cc = 0.0,
                cex = Cbe,
                gex = Gbe,
                qb = Qb
            };
            ExcessPhaseCalculation?.Invoke(this, ep);
            cc = ep.cc;
            cex = ep.cex;
            gex = ep.gex;

            /* 
			 * determine dc incremental conductances
			 */
            cc = cc + (cex - Cbc) / Qb - Cbc / temp.TBetaR - cbcn;
            cb = Cbe / temp.TBetaF + cben + Cbc / temp.TBetaR + cbcn;
            gx = rbpr + rbpi / Qb;
            if (xjrb != 0)
            {
                arg1 = Math.Max(cb / xjrb, 1e-9);
                arg2 = (-1 + Math.Sqrt(1 + 14.59025 * arg1)) / 2.4317 / Math.Sqrt(arg1);
                arg1 = Math.Tan(arg2);
                gx = rbpr + 3 * rbpi * (arg1 - arg2) / arg2 / arg1 / arg1;
            }
            if (gx != 0)
                gx = 1 / gx;
            gpi = Gbe / temp.TBetaF + gben;
            gmu = Gbc / temp.TBetaR + gbcn;
            go = (Gbc + (cex - Cbc) * DqbDvc / Qb) / Qb;
            gm = (gex - (cex - Cbc) * DqbDve / Qb) / Qb - go;

            /* 
			 * check convergence
			 */
            if (state.Init != State.InitializationStates.InitFix || !bp.Off)
            {
                if (icheck)
                    state.IsCon = false;
            }

            Vbe = vbe;
            Vbc = vbc;
            Cc = cc;
            Cb = cb;
            Gpi = gpi;
            Gmu = gmu;
            Gm = gm;
            Go = go;
            Gx = gx;

            /* 
			 * load current excitation vector
			 */
            ceqbe = mbp.BipolarType * (cc + cb - vbe * (gm + go + gpi) + vbc * go);
            ceqbc = mbp.BipolarType * (-cc + vbe * (gm + go) - vbc * (gmu + go));

            state.Rhs[baseNode] += (-ceqbx);
            state.Rhs[CollectorPrimeNode] += (ceqcs + ceqbx + ceqbc);
            state.Rhs[BasePrimeNode] += (-ceqbe - ceqbc);
            state.Rhs[EmitterPrimeNode] += (ceqbe);
            state.Rhs[substrateNode] += (-ceqcs);

            /* 
			 * load y matrix
			 */
            CollectorCollectorPtr.Add(gcpr);
            BaseBasePtr.Add(gx);
            EmitterEmitterPtr.Add(gepr);
            CollectorPrimeCollectorPrimePtr.Add(gmu + go + gcpr);
            BasePrimeBasePrimePtr.Add(gx + gpi + gmu);
            EmitterPrimeEmitterPrimePtr.Add(gpi + gepr + gm + go);
            CollectorCollectorPrimePtr.Add(-gcpr);
            BaseBasePrimePtr.Add(-gx);
            EmitterEmitterPrimePtr.Add(-gepr);
            CollectorPrimeCollectorPtr.Add(-gcpr);
            CollectorPrimeBasePrimePtr.Add(-gmu + gm);
            CollectorPrimeEmitterPrimePtr.Add(-gm - go);
            BasePrimeBasePtr.Add(-gx);
            BasePrimeCollectorPrimePtr.Add(-gmu);
            BasePrimeEmitterPrimePtr.Add(-gpi);
            EmitterPrimeEmitterPtr.Add(-gepr);
            EmitterPrimeCollectorPrimePtr.Add(-go);
            EmitterPrimeBasePrimePtr.Add(-gpi - gm);
        }

        /// <summary>
        /// Check if the BJT is convergent
        /// </summary>
        /// <param name="simulation">Base simulation</param>
        /// <returns></returns>
        public override bool IsConvergent(BaseSimulation simulation)
        {
			if (simulation == null)
				throw new ArgumentNullException(nameof(simulation));

            var state = simulation.State;
            var config = simulation.BaseConfiguration;

            double vbe, vbc, delvbe, delvbc, cchat, cbhat, cc, cb;

            vbe = mbp.BipolarType * (state.Solution[BasePrimeNode] - state.Solution[EmitterPrimeNode]);
            vbc = mbp.BipolarType * (state.Solution[BasePrimeNode] - state.Solution[CollectorPrimeNode]);
            delvbe = vbe - Vbe;
            delvbc = vbc - Vbe;
            cchat = Cc + (Gm + Go) * delvbe - (Go + Gmu) * delvbc;
            cbhat = Cb + Gpi * delvbe + Gmu * delvbc;
            cc = Cc;
            cb = Cb;

            /*
             *   check convergence
             */
            // NOTE: access configuration in some way here!
            double tol = config.RelTolerance * Math.Max(Math.Abs(cchat), Math.Abs(cc)) + config.AbsTolerance;
            if (Math.Abs(cchat - cc) > tol)
            {
                state.IsCon = false;
                return false;
            }

            tol = config.RelTolerance * Math.Max(Math.Abs(cbhat), Math.Abs(cb)) + config.AbsTolerance;
            if (Math.Abs(cbhat - cb) > tol)
            {
                state.IsCon = false;
                return false;
            }
            return true;
        }
    }
}
