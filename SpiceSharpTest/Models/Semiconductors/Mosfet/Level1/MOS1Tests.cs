﻿using NUnit.Framework;
using System.Numerics;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;

namespace SpiceSharpTest.Models
{
    [TestFixture]
    public class MOS1Tests : Framework
    {
        /// <summary>
        /// Create a MOSFET
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="d">Drain</param>
        /// <param name="g">Gate</param>
        /// <param name="s">Source</param>
        /// <param name="b">Bulk</param>
        /// <param name="modelname">Model name</param>
        /// <param name="modelparams">Model parameters</param>
        /// <returns></returns>
        protected Mosfet1 CreateMOS1(string name, string d, string g, string s, string b,
            string modelname, string modelparams)
        {
            // Create model
            var model = new Mosfet1Model(modelname);
            ApplyParameters(model, modelparams);

            // Create mosfet
            var mos = new Mosfet1(name);
            mos.SetModel(model);
            mos.Connect(d, g, s, b);
            return mos;
        }

        [Test]
        public void When_MOS1DC_Expect_Spice3f5Reference()
        {
            /*
             * Mosfet biased by voltage sources
             * Current is expected to behave like the reference. Reference is from Spice 3f5.
             * The model is part from the ntd20n06 (OnSemi) device.
             */
            // Create circuit
            var ckt = new Circuit(
                new VoltageSource("V1", "g", "0", 0.0),
                new VoltageSource("V2", "d", "0", 0),
                CreateMOS1("M1", "d", "g", "0", "0",
                    "MM", "IS=1e-32 VTO=3.03646 LAMBDA=0 KP=5.28747 CGSO=6.5761e-06 CGDO=1e-11")
                );

            // Create simulation
            var dc = new DC("dc", new[] {
                new SweepConfiguration("V2", 0, 5, 0.5),
                new SweepConfiguration("V1", 0, 5, 0.5)
            });

            // Create exports
            Export<double>[] exports = { new RealPropertyExport(dc, "V2", "i") };

            // Create references
            var references = new double[1][];
            references[0] = new[]
            {
                -9.806842412468131e-31, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00,
                0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00,
                0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, -5.000000000000000e-13,
                -5.000000000000000e-13, -5.000000000000000e-13, -5.000000000000000e-13, -5.000000000000000e-13,
                -5.000000000000000e-13, -5.000000000000000e-13, -5.680575723780237e-01, -1.886410671900499e+00,
                -3.208278171900499e+00, -4.530145671900499e+00, -1.000000000000000e-12, -1.000000000000000e-12,
                -1.000000000000000e-12, -1.000000000000000e-12, -1.000000000000000e-12, -1.000000000000000e-12,
                -1.000000000000000e-12, -5.680575723785246e-01, -2.454468244278523e+00, -5.094688843800999e+00,
                -7.738423843800998e+00, -1.500000000000000e-12, -1.500000000000000e-12, -1.500000000000000e-12,
                -1.500000000000000e-12, -1.500000000000000e-12, -1.500000000000000e-12, -1.500000000000000e-12,
                -5.680575723790238e-01, -2.454468244279024e+00, -5.662746416179022e+00, -9.624834515701494e+00,
                -2.000000000000000e-12, -2.000000000000000e-12, -2.000000000000000e-12, -2.000000000000000e-12,
                -2.000000000000000e-12, -2.000000000000000e-12, -2.000000000000000e-12, -5.680575723795247e-01,
                -2.454468244279525e+00, -5.662746416179523e+00, -1.019289208807952e+01, -2.500000000000000e-12,
                -2.500000000000000e-12, -2.500000000000000e-12, -2.500000000000000e-12, -2.500000000000000e-12,
                -2.500000000000000e-12, -2.500000000000000e-12, -5.680575723800239e-01, -2.454468244280026e+00,
                -5.662746416180024e+00, -1.019289208808002e+01, -3.000000000000000e-12, -3.000000000000000e-12,
                -3.000000000000000e-12, -3.000000000000000e-12, -3.000000000000000e-12, -3.000000000000000e-12,
                -3.000000000000000e-12, -5.680575723805248e-01, -2.454468244280523e+00, -5.662746416180521e+00,
                -1.019289208808052e+01, -3.500000000000000e-12, -3.500000000000000e-12, -3.500000000000000e-12,
                -3.500000000000000e-12, -3.500000000000000e-12, -3.500000000000000e-12, -3.500000000000000e-12,
                -5.680575723810239e-01, -2.454468244281024e+00, -5.662746416181022e+00, -1.019289208808102e+01,
                -4.000000000000000e-12, -4.000000000000000e-12, -4.000000000000000e-12, -4.000000000000000e-12,
                -4.000000000000000e-12, -4.000000000000000e-12, -4.000000000000000e-12, -5.680575723815249e-01,
                -2.454468244281525e+00, -5.662746416181523e+00, -1.019289208808152e+01, -4.500000000000000e-12,
                -4.500000000000000e-12, -4.500000000000000e-12, -4.500000000000000e-12, -4.500000000000000e-12,
                -4.500000000000000e-12, -4.500000000000000e-12, -5.680575723820240e-01, -2.454468244282026e+00,
                -5.662746416182024e+00, -1.019289208808202e+01, -5.000000000000000e-12, -5.000000000000000e-12,
                -5.000000000000000e-12, -5.000000000000000e-12, -5.000000000000000e-12, -5.000000000000000e-12,
                -5.000000000000000e-12, -5.680575723825250e-01, -2.454468244282523e+00, -5.662746416182522e+00,
                -1.019289208808252e+01
            };

            // Run simulation
            AnalyzeDC(dc, ckt, exports, references);
        }

        [Test]
        public void When_MOS1CommonSourceAmplifierSmallSignal_Expect_Spice3f5Reference()
        {
            /*
             * Simple common source amplifier
             * Output voltage gain is expected to match reference. Reference by Spice 3f5.
             */
            // Build circuit
            var ckt = new Circuit(
                new VoltageSource("V1", "in", "0", 0.0),
                new VoltageSource("V2", "vdd", "0", 5.0),
                new Resistor("R1", "vdd", "out", 10.0e3),
                new Resistor("R2", "out", "g", 10.0e3),
                new Capacitor("Cin", "in", "g", 1e-6),
                CreateMOS1("M1", "out", "g", "0", "0",
                    "MM", "IS=1e-32 VTO=3.03646 LAMBDA=0 KP=5.28747 CGSO=6.5761e-06 CGDO=1e-11")
                );
            ckt.Entities["V1"].SetParameter("acmag", 1.0);

            // Create simulation
            var ac = new AC("ac", new DecadeSweep(10, 10e9, 5));

            // Create exports
            Export<Complex>[] exports = { new ComplexVoltageExport(ac, "out") };

            // Create references
            double[] riref =
            {
                -1.725813644006744e-03, -6.255567388468394e-01, -4.334997991949969e-03, -9.914292083082819e-01,
                -1.088870790416865e-02, -1.571263986482406e+00, -2.734921201531804e-02, -2.490104807455213e+00,
                -6.868558931531524e-02, -3.945830610208745e+00, -1.724514213823252e-01, -6.250857335307440e+00,
                -4.326808652667278e-01, -9.895562775467608e+00, -1.083718679773610e+00, -1.563829379084251e+01,
                -2.702649136180583e+00, -2.460721571760895e+01, -6.668576859467226e+00, -3.830945453134210e+01,
                -1.603760999419659e+01, -5.813162362216580e+01, -3.639300462484001e+01, -8.323207309729999e+01,
                -7.356417703660050e+01, -1.061546849651946e+02, -1.239747410542734e+02, -1.128771300396776e+02,
                -1.704839090551324e+02, -9.793908744049646e+01, -2.004160562764431e+02, -7.264486862286662e+01,
                -2.154771197776309e+02, -4.928026345664456e+01, -2.221224344754558e+02, -3.205256931555954e+01,
                -2.248834695491267e+02, -2.047502071380190e+01, -2.260018549839163e+02, -1.298284168009341e+01,
                -2.264501941354974e+02, -8.207439641802505e+00, -2.266291765967058e+02, -5.181955183396055e+00,
                -2.267005095543059e+02, -3.269540294698130e+00, -2.267289201967292e+02, -2.061484698952238e+00,
                -2.267402326137311e+02, -1.298056697906094e+00, -2.267447363683708e+02, -8.147282590010622e-01,
                -2.267465291092059e+02, -5.072375792911769e-01, -2.267472421017376e+02, -3.092289341889395e-01,
                -2.267475241460346e+02, -1.779661469529321e-01, -2.267476318976589e+02, -8.511710389733645e-02,
                -2.267476634094340e+02, -1.064055031339878e-02, -2.267476473568060e+02, 6.153924230708851e-02,
                -2.267475691320166e+02, 1.470022622053010e-01, -2.267473575512703e+02, 2.641955750327690e-01,
                -2.267468200791099e+02, 4.384147465157958e-01, -2.267454676298356e+02, 7.072623998910071e-01,
                -2.267420695496819e+02, 1.128758783561459e+00, -2.267335340265157e+02, 1.793841675558252e+00,
                -2.267120964325891e+02, 2.845900637604031e+00, -2.266582653689669e+02, 4.511350906221913e+00,
                -2.265231600192664e+02, 7.147007490994596e+00, -2.261844969618107e+02, 1.131116522055704e+01,
                -2.253382440974266e+02, 1.786070290176738e+01, -2.232401028216283e+02, 2.804520605312238e+01,
                -2.181374806913368e+02, 4.343740586524000e+01, -2.062891648647118e+02, 6.512151536164929e+01
            };
            var references = new Complex[1][];
            references[0] = new Complex[riref.Length / 2];
            for (var i = 0; i < riref.Length; i += 2)
                references[0][i / 2] = new Complex(riref[i], riref[i + 1]);

            // Run test
            AnalyzeAC(ac, ckt, exports, references);
        }

        [Test]
        public void When_MOS1SwitchTransient_Expect_Spice3f5Reference()
        {
            // Create circuit
            var ckt = new Circuit(
                new VoltageSource("V1", "in", "0", new Pulse(1, 5, 1e-6, 1e-9, 0.5e-6, 2e-6, 6e-6)),
                new VoltageSource("Vsupply", "vdd", "0", 5),
                new Resistor("R1", "out", "vdd", 1.0e3),
                CreateMOS1("M1", "out", "in", "0", "0",
                    "MM", "IS=1e-32 VTO=3.03646 LAMBDA=0 KP=5.28747 CGSO=6.5761e-06 CGDO=1e-11")
                );

            // Create simulation
            var tran = new Transient("tran", 1e-9, 10e-6);

            // Create exports
            Export<double>[] exports = { new RealVoltageExport(tran, "out") };

            // Create references
            var references = new double[1][];
            references[0] = new[]
            {
                4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00,
                4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00,
                4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00,
                4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00,
                4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00,
                4.999999995000000e+00, 5.003960391035683e+00, 5.004038814719480e+00, 5.003961923904320e+00,
                5.822926169346066e-04, 4.810560374122478e-04, 4.816080793679862e-04, 4.816080824751261e-04,
                4.816080793679973e-04, 4.816080824751207e-04, 4.816080793680000e-04, 4.816080824751193e-04,
                4.816080793680006e-04, 4.816080824751191e-04, 4.816080793680011e-04, 4.816080824751189e-04,
                4.816080793680010e-04, 4.816080824751191e-04, 4.816080793680009e-04, 4.816080824751189e-04,
                4.816080793680010e-04, 4.816080824751189e-04, 4.816080793680010e-04, 4.816080824751189e-04,
                4.816080793680010e-04, 4.816080824751189e-04, 4.816080793680010e-04, 4.816080824751188e-04,
                5.243402581575843e-04, 6.374704766839068e-04, 1.121520168458353e-03, -1.720170200560100e+01,
                4.999968188570249e+00, 5.000015800431791e+00, 4.999968190756332e+00, 4.999999991842038e+00,
                4.999999998157336e+00, 4.999999991842977e+00, 4.999999998156865e+00, 4.999999991843213e+00,
                4.999999998156724e+00, 4.999999991843340e+00, 4.999999998156598e+00, 4.999999991843466e+00,
                4.999999998156472e+00, 4.999999991843592e+00, 4.999999998156345e+00, 4.999999991843718e+00,
                4.999999998156220e+00, 4.999999991843845e+00, 4.999999998156093e+00, 4.999999991843971e+00,
                4.999999998155968e+00, 4.999999991844097e+00, 4.999999998155841e+00, 4.999999991844228e+00,
                5.003960391004448e+00, 5.004038814750084e+00, 5.003961923874295e+00, 5.822926152947390e-04,
                4.810560374114067e-04, 4.816080793679895e-04, 4.816080824751229e-04, 4.816080793680006e-04,
                4.816080824751175e-04, 4.816080793680033e-04, 4.816080824751161e-04, 4.816080793680040e-04,
                4.816080824751158e-04, 4.816080793680041e-04, 4.816080824751156e-04, 4.816080793680043e-04,
                4.816080824751156e-04, 4.816080793680043e-04, 4.816080824751157e-04, 4.816080793680042e-04,
                4.816080824751155e-04, 4.816080793680042e-04, 4.816080824751155e-04, 4.816080793680042e-04,
                4.816080824751155e-04, 4.816080793680042e-04, 4.816080824751157e-04, 5.243402581575805e-04,
                6.374704766839011e-04, 1.121520168458326e-03, -1.720170200561121e+01, 4.999968188570249e+00,
                5.000015800431791e+00, 4.999968190756332e+00, 4.999999991842038e+00, 4.999999998157336e+00,
                4.999999991842977e+00, 4.999999998156865e+00, 4.999999991843213e+00, 4.999999998156719e+00
            };

            var oldt = 0.0;
            tran.ExportSimulationData += (sender, args) =>
            {
                var t = args.Time;
                var output = args.GetVoltage("out");
                oldt = t;
            };

            // Run test
            AnalyzeTransient(tran, ckt, exports, references);
        }

        [Test]
        public void When_MOS1CommonSourceAmplifierNoise_Expect_Spice3f5Reference()
        {
            // Create circuit
            var ckt = new Circuit(
                new VoltageSource("V1", "in", "0", 0.0),
                new VoltageSource("V2", "vdd", "0", 5.0),
                new Resistor("R1", "vdd", "out", 10e3),
                new Resistor("R2", "out", "g", 10e3),
                new Capacitor("Cin", "in", "g", 1e-6),
                CreateMOS1("M1", "out", "g", "0", "0",
                    "MM", "IS = 1e-32 VTO = 3.03646 LAMBDA = 0 KP = 5.28747 CGSO = 6.5761e-06 CGDO = 1e-11 KF = 1e-25")
                );
            ckt.Entities["V1"].SetParameter("acmag", 1.0);
            ckt.Entities["M1"].SetParameter("w", 100e-6);
            ckt.Entities["M1"].SetParameter("l", 100e-6);

            // Create simulation, exports and references
            var noise = new Noise("noise", "out", "V1", new DecadeSweep(10, 10e9, 10));
            Export<double>[] exports = { new InputNoiseDensityExport(noise), new OutputNoiseDensityExport(noise) };
            var references = new double[2][];
            references[0] = new[]
            {
                2.815651317421889e-12, 1.645027613662841e-12, 1.010218811439069e-12, 6.538511835765611e-13,
                4.448954302782106e-13, 3.160638431808556e-13, 2.323481430226209e-13, 1.751824608495907e-13,
                1.344515862942468e-13, 1.044423847431228e-13, 8.178038471381158e-14, 6.436837445436251e-14,
                5.083280290976165e-14, 4.022913978256862e-14, 3.188055911876120e-14, 2.528625121005242e-14,
                2.006686143767511e-14, 1.593030775258497e-14, 1.264921826655891e-14, 1.004530985270185e-14,
                7.978132791490454e-15, 6.336708373912842e-15, 5.033176185391673e-15, 3.997893232665373e-15,
                3.175613873077495e-15, 2.522491942006981e-15, 2.003717769837784e-15, 1.591650381587094e-15,
                1.264338456987390e-15, 1.004347798555624e-15, 7.978311162761375e-16, 6.337897136092530e-16,
                5.034873162956860e-16, 3.999846069863884e-16, 3.177695668556620e-16, 2.524638826984041e-16,
                2.005897565348039e-16, 1.593846853472544e-16, 1.266543401716210e-16, 1.006557062235251e-16,
                8.000425902841744e-17, 6.360023242564495e-17, 5.057005148155293e-17, 4.021981116261989e-17,
                3.199832321658799e-17, 2.546776331071195e-17, 2.028035524788774e-17, 1.615985059334290e-17,
                1.288681742566722e-17, 1.028695477987395e-17, 8.221810481485991e-18, 6.581408061120865e-18,
                5.278390105156135e-18, 4.243366154135094e-18, 3.421217407310980e-18, 2.768161445242110e-18,
                2.249420656137898e-18, 1.837370201113151e-18, 1.510066890721238e-18, 1.250080630061784e-18,
                1.043566202644526e-18, 8.795259621097400e-19, 7.492241674473541e-19, 6.457217729274123e-19,
                5.635068986080232e-19, 4.982013026270218e-19, 4.463272238560322e-19, 4.051221784377176e-19,
                3.723918474462766e-19, 3.463932214025209e-19, 3.257417786627270e-19, 3.093377545919019e-19,
                2.963075750859589e-19, 2.859573355641487e-19, 2.777358480182294e-19, 2.712052882386572e-19,
                2.660178800750258e-19, 2.618973750817693e-19, 2.586243412714054e-19, 2.560244775456920e-19,
                2.539593315022365e-19, 2.523189263006598e-19, 2.510159039337331e-19, 2.499808729981372e-19,
                2.491587131957750e-19, 2.485056397337421e-19, 2.479868712390628e-19, 2.475747769129639e-19,
                2.472474041220591e-19, 2.469873078063680e-19, 2.467806190347303e-19
            };
            references[1] = new[]
            {
                1.101832532914486e-12, 1.020253230193121e-12, 9.929952548173648e-13, 1.018604943979157e-12,
                1.098441958732390e-12, 1.236749625666466e-12, 1.440876787782532e-12, 1.721660933077368e-12,
                2.093988925590930e-12, 2.577557075810235e-12, 3.197854860291897e-12, 3.987392558889523e-12,
                4.987174542006099e-12, 6.248371072384571e-12, 7.834031434867882e-12, 9.820452019717962e-12,
                1.229736153915473e-11, 1.536524524926090e-11, 1.912668338451432e-11, 2.366640647887245e-11,
                2.901240489602347e-11, 3.507047218951613e-11, 4.153391472507595e-11, 4.779958110319135e-11,
                5.297084398705647e-11, 5.605061300765190e-11, 5.632647597529333e-11, 5.373019247628322e-11,
                4.887531438615962e-11, 4.273855574886992e-11, 3.625653556498441e-11, 3.009145926495366e-11,
                2.459985331204318e-11, 1.990800234848295e-11, 1.600470036830737e-11, 1.281197788956573e-11,
                1.022843301800413e-11, 8.152046737396919e-12, 6.490451390928037e-12, 5.164410386114776e-12,
                4.107982887713966e-12, 3.267266582522150e-12, 2.598675415452822e-12, 2.067199850497022e-12,
                1.644835756571984e-12, 1.309240679081301e-12, 1.042618193670920e-12, 8.308074217267168e-13,
                6.625475993808368e-13, 5.288877753997852e-13, 4.227148424629126e-13, 3.383770965728897e-13,
                2.713844460468793e-13, 2.181698910811493e-13, 1.758998656630966e-13, 1.423234892106491e-13,
                1.156527739427722e-13, 9.446744565262983e-14, 7.763932783504480e-14, 6.427227167499369e-14,
                5.365443763032955e-14, 4.522038977757301e-14, 3.852098548440520e-14, 3.319945751360530e-14,
                2.897241513585387e-14, 2.561475263262926e-14, 2.294766157361505e-14, 2.082910849476789e-14,
                1.914627086303746e-14, 1.780952846972553e-14, 1.674769018959033e-14, 1.590420177688972e-14,
                1.523413253099298e-14, 1.470178001275520e-14, 1.427876470269168e-14, 1.394251226156354e-14,
                1.367504118754215e-14, 1.346198912774333e-14, 1.329182255520127e-14, 1.315518232604495e-14,
                1.304432183279279e-14, 1.295259477360910e-14, 1.287394532117294e-14, 1.280234382749476e-14,
                1.273109557296487e-14, 1.265192982980225e-14, 1.255375930538705e-14, 1.242100993671518e-14,
                1.223151914363468e-14, 1.195431702088460e-14, 1.154835357346553e-14
            };

            // Run test
            AnalyzeNoise(noise, ckt, exports, references);
        }
    }
}
