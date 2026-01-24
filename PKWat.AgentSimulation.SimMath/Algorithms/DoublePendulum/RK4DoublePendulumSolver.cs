using PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKWat.AgentSimulation.SimMath.Algorithms.DoublePendulum;

public record DoublePendulumState(double Theta1, double Omega1, double Theta2, double Omega2);

public class RK4DoublePendulumSolver : IDoublePendulumSolver
{
    private readonly RungeKuttaMethod _rk4Method = new();

    public DoublePendulumState CalculateNextState(DoublePendulumState currentState, double dt, double g, double L1, double L2, double m1, double m2)
    {
        // 1. Pack state into array: [theta1, omega1, theta2, omega2]
        // Index mapping: 0->t1, 1->w1, 2->t2, 3->w2
        double[] state =
        [
            currentState.Theta1,
            currentState.Omega1,
            currentState.Theta2,
            currentState.Omega2
        ];

        // 2. Define derivative function (Equations of Motion from Lagrangian Mechanics)
        Func<double, double[], double[]> derivatives = (t, s) =>
        {
            double theta1 = s[0];
            double omega1 = s[1];
            double theta2 = s[2];
            double omega2 = s[3];

            // Common terms to optimize calculation
            double deltaTheta = theta1 - theta2;
            double sinDelta = Math.Sin(deltaTheta);
            double cosDelta = Math.Cos(deltaTheta);

            // Denominator is almost the same for both equations (mass matrix determinant part)
            // den = L1 * (2*m1 + m2 - m2 * cos(2*theta1 - 2*theta2))
            double denominatorCommon = 2.0 * m1 + m2 - m2 * Math.Cos(2.0 * theta1 - 2.0 * theta2);
            double denominator1 = L1 * denominatorCommon;
            double denominator2 = L2 * denominatorCommon;

            // Equation for angular acceleration 1 (d_omega1)
            double num1 = -g * (2.0 * m1 + m2) * Math.Sin(theta1);
            double num2 = -m2 * g * Math.Sin(theta1 - 2.0 * theta2);
            double num3 = -2.0 * sinDelta * m2 * (omega2 * omega2 * L2 + omega1 * omega1 * L1 * cosDelta);

            double d_omega1 = (num1 + num2 + num3) / denominator1;

            // Equation for angular acceleration 2 (d_omega2)
            double num4 = 2.0 * sinDelta;
            double num5 = omega1 * omega1 * L1 * (m1 + m2);
            double num6 = g * (m1 + m2) * Math.Cos(theta1);
            double num7 = omega2 * omega2 * L2 * m2 * cosDelta;

            double d_omega2 = (num4 * (num5 + num6 + num7)) / denominator2;

            // Return derivatives: [d_theta1, d_omega1, d_theta2, d_omega2]
            return [omega1, d_omega1, omega2, d_omega2];
        };

        // 3. Solve using generic RK4
        double[] newState = _rk4Method.CalculateNextState(
            0, // Time is not explicitly used in equations (autonomous system), so passing 0 is fine if only dt matters
            state,
            dt,
            derivatives
        );

        // 4. Unpack back to object AND NORMALIZE ANGLES
        // Tutaj jest bezpiecznie, bo RK4 już zakończyło pracę
        double newTheta1 = NormalizeAngle(newState[0]);
        double newOmega1 = newState[1];
        double newTheta2 = NormalizeAngle(newState[2]);
        double newOmega2 = newState[3];

        return new DoublePendulumState(newTheta1, newOmega1, newTheta2, newOmega2);
    }

    private double NormalizeAngle(double angle)
    {
        // Odejmujemy lub dodajemy 2*PI tak długo, aż trafimy w zakres [-PI, PI]
        while (angle > Math.PI) angle -= 2.0 * Math.PI;
        while (angle <= -Math.PI) angle += 2.0 * Math.PI;
        return angle;
    }
}
