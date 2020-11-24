using Mvvm;
using System;
using System.Diagnostics;

namespace PipePressureDrop.Model
{
    // 单位制mks
    internal class Pipeline : BindableBase
    {
        private double _massFlow;
        public double MassFlow
        {
            get => _massFlow;
            set => _massFlow = value / 3600; // kg/h → kg/s
        }

        private double _viscosity;
        public double Viscosity
        {
            get => _viscosity;
            set => _viscosity = value / 1000; // cp → pa.s
        }
        
        private double _innerDiameter;
        public double InnerDiameter
        {
            get => _innerDiameter;
            set => _innerDiameter = value / 1000; // mm → m
        }

        private double _absoluteRoughness;
        public double AbsoluteRoughness
        {
            get => _absoluteRoughness;
            set => _absoluteRoughness = value / 1000; // mm → m
        }
        public double MarginFactor { get; set; }
        public double FrictionResistance { get; set; }
        public double Density { get; set; } // kg/m³
        public double Velocity { get; set; } // m/s
        public double ReynoldsNumber { get; set; }
        public double FrictionFactor { get; set; }
        public double FittingEquivalentLength { get; set; } // m
        public double PipeLength { get; set; } // m
        public double ElevationChange { get; set; } // m
        public int ElbowAndTee { get; set; }
        public int GlobeValve { get; set; }
        public int CheckValve { get; set; }

        /// <summary>
        /// 计算管线压力降
        /// </summary>
        /// <returns>kPa</returns>
        public void CalculatePipePressureDrop()
        {
            Velocity = CalculateVelocity(MassFlow, Density, InnerDiameter);
            ReynoldsNumber = CalculateReynoldsNumber(Density, InnerDiameter, Velocity, Viscosity);
            FrictionFactor = CalculateFrictionFactor(AbsoluteRoughness, InnerDiameter, ReynoldsNumber);
            FittingEquivalentLength = CalculateFittingEquivalentLength(InnerDiameter, ElbowAndTee, GlobeValve, CheckValve);
            FrictionResistance = CalculateFrictionalResistance(
                InnerDiameter,
                PipeLength  + FittingEquivalentLength,
                Density,
                Velocity,
                FrictionFactor);
            FrictionResistance *= MarginFactor;
            FrictionResistance += Density * 9.8 * ElevationChange;
            FrictionResistance = Math.Ceiling(FrictionResistance / 1000);

            Debug.WriteLine(FrictionResistance);
        }

        /// <summary>
        /// 计算流速
        /// </summary>
        /// <param name="massFlow">kg/s</param>
        /// <param name="density">kg/m³</param>
        /// <param name="innerDiameter">m</param>
        /// <returns>m/s</returns>
        protected double CalculateVelocity(double massFlow, double density, double innerDiameter)
        {
            Debug.WriteLine(massFlow / density / (0.785 * innerDiameter * innerDiameter));
            return massFlow / density / (0.785 * innerDiameter * innerDiameter);
        }

        /// <summary>
        /// 计算雷诺数
        /// </summary>
        /// <param name="density">kg/m³</param>
        /// <param name="innerDiameter">m</param>
        /// <param name="velocity">m/s</param>
        /// <param name="viscosity">Pa.s</param>
        /// <returns>无量纲</returns>
        protected double CalculateReynoldsNumber(double density, double innerDiameter, double velocity, double viscosity)
        {
            Debug.WriteLine(density * innerDiameter * velocity / viscosity);
            return density * innerDiameter * velocity / viscosity;
        }

        /// <summary>
        /// 计算管阀件当量长度
        /// </summary>
        /// <param name="innerDiameter">m</param>
        /// <param name="elbowAndTee">无量纲</param>
        /// <param name="globeValve">无量纲</param>
        /// <param name="checkValve">无量纲</param>
        /// <returns>m</returns>
        protected double CalculateFittingEquivalentLength(double innerDiameter, int elbowAndTee, int globeValve, int checkValve)
        {
            Debug.WriteLine(innerDiameter * ((elbowAndTee * 30) + (globeValve * 340) + (checkValve * 100)));
            return innerDiameter * ((elbowAndTee * 30) + (globeValve * 340) + (checkValve * 100));
        }

        /// <summary>
        /// 计算达西摩阻系数
        /// </summary>
        /// <param name="absoluteRoughness">m</param>
        /// <param name="innerDiameter">m</param>
        /// <param name="reynoldsNumber">无量纲</param>
        /// <returns>无量纲</returns>
        /// <demo>ColeBrook(0.15, 315, 125000)</demo>
        /// 来源文献：https://arxiv.org/pdf/0810.5564.pdf http://www.docin.com/p-1773318871.html
        protected double CalculateFrictionFactor(double absoluteRoughness, double innerDiameter, double reynoldsNumber)
        {
            if (reynoldsNumber < 2300)
            {
                return reynoldsNumber / 64;
            }
            
            var k = absoluteRoughness / innerDiameter;
            const double T = 0.333333333333333333;
            var x1 = k * reynoldsNumber * 0.123968186335417556;
            var x2 = Math.Log(reynoldsNumber) - 0.779397488455682028;
            var f = x2 - 0.2d;

            var e = (Math.Log(x1 + f) - 0.2d) / (1.0d + x1 + f);
            f -= (1.0d + x1 + f + 0.5d * e) * e * (x1 + f) / (1.0d + x1 + f + e * (1.0d + e * T));
            if ((x1 + x2) < 5.7d)
            {
                e = (Math.Log(x1 + f) + f - x2) / (1.0d + x1 + f);
                f -= (1.0d + x1 + f + 0.5d * e) * e * (x1 + f) / (1.0d + x1 + f + e * (1.0d + e * T));
            }
            f = 1.151292546497022842 / f;

            Debug.WriteLine(f * f);
            return f * f;
        }

        /// <summary>
        /// 计算管道摩擦阻力
        /// </summary>
        /// <param name="innerDiameter">m</param>
        /// <param name="pipelineLength">m</param>
        /// <param name="density">kg/m³</param>
        /// <param name="velocity">m/s</param>
        /// <param name="frictionFactor">无量纲</param>
        /// <returns>Pa</returns>
        private static double CalculateFrictionalResistance(double innerDiameter, double pipelineLength, double density, double velocity, double frictionFactor)
        {
            Debug.WriteLine(frictionFactor * (pipelineLength / innerDiameter) * (velocity * velocity * density / 2));
            return frictionFactor * (pipelineLength / innerDiameter) * (velocity * velocity * density / 2);
        }
    }
}