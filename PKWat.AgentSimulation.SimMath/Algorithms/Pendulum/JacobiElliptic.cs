namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

/// <summary>
/// Static helper class for calculating Jacobi Elliptic functions and Complete Elliptic Integrals.
/// </summary>
public static class JacobiElliptic
{
    private const double AGM_TOLERANCE = 1e-12;
    private const int MAX_ITERATIONS = 100;

    /// <summary>
    /// Computes the Complete Elliptic Integral of the First Kind K(k) using the Arithmetic-Geometric Mean (AGM) method.
    /// </summary>
    /// <param name="k">The elliptic modulus (0 &lt;= k &lt; 1)</param>
    /// <returns>K(k)</returns>
    public static double CompleteEllipticIntegralFirstKind(double k)
    {
        if (k < 0 || k >= 1)
        {
            throw new ArgumentException("Elliptic modulus k must be in the range [0, 1)", nameof(k));
        }

        if (Math.Abs(k) < 1e-10)
        {
            return Math.PI / 2;
        }

        double a = 1.0;
        double g = Math.Sqrt(1 - k * k);
        double aPrev;

        for (int i = 0; i < MAX_ITERATIONS; i++)
        {
            aPrev = a;
            double aNext = (a + g) / 2;
            double gNext = Math.Sqrt(a * g);
            
            a = aNext;
            g = gNext;

            if (Math.Abs(a - aPrev) < AGM_TOLERANCE)
            {
                return Math.PI / (2 * a);
            }
        }

        return Math.PI / (2 * a);
    }

    /// <summary>
    /// Computes the Jacobi Elliptic Sine function sn(u, k) using the descending Landen transformation.
    /// </summary>
    /// <param name="u">The argument</param>
    /// <param name="k">The elliptic modulus (0 &lt;= k &lt; 1)</param>
    /// <returns>sn(u, k)</returns>
    public static double JacobiSn(double u, double k)
    {
        if (k < 0 || k >= 1)
        {
            throw new ArgumentException("Elliptic modulus k must be in the range [0, 1)", nameof(k));
        }

        if (Math.Abs(k) < 1e-10)
        {
            return Math.Sin(u);
        }

        if (Math.Abs(k - 1) < 1e-10)
        {
            return Math.Tanh(u);
        }

        // Use the descending Landen transformation
        List<double> kValues = new List<double> { k };
        double kn = k;

        // Forward transformation to reduce k to a small value
        for (int i = 0; i < MAX_ITERATIONS; i++)
        {
            double kNext = (1 - Math.Sqrt(1 - kn * kn)) / (1 + Math.Sqrt(1 - kn * kn));
            kValues.Add(kNext);
            
            if (Math.Abs(kNext) < 1e-10)
            {
                break;
            }
            
            kn = kNext;
        }

        // Start with the approximation sn(u_n, k_n) ≈ sin(u_n) for small k_n
        int n = kValues.Count - 1;
        double phi = u * Math.Pow(2, n);
        
        // Backward transformation
        for (int i = n - 1; i >= 0; i--)
        {
            double ki = kValues[i];
            phi = (2 / (1 + Math.Sqrt(1 - ki * ki))) * Math.Atan(Math.Sqrt(1 - ki * ki) * Math.Tan(phi) / (1 + Math.Cos(phi) * Math.Sqrt(1 - ki * ki)));
        }

        return Math.Sin(phi);
    }

    /// <summary>
    /// Alternative implementation of Jacobi sn using the arithmetic-geometric mean method.
    /// This is more numerically stable for certain parameter ranges.
    /// </summary>
    /// <param name="u">The argument</param>
    /// <param name="k">The elliptic modulus (0 &lt;= k &lt; 1)</param>
    /// <returns>sn(u, k)</returns>
    public static double JacobiSnAGM(double u, double k)
    {
        if (k < 0 || k >= 1)
        {
            throw new ArgumentException("Elliptic modulus k must be in the range [0, 1)", nameof(k));
        }

        if (Math.Abs(k) < 1e-10)
        {
            return Math.Sin(u);
        }

        if (Math.Abs(k - 1) < 1e-10)
        {
            return Math.Tanh(u);
        }

        // Store transformation values
        double[] a = new double[MAX_ITERATIONS];
        double[] g = new double[MAX_ITERATIONS];
        double[] c = new double[MAX_ITERATIONS];

        a[0] = 1.0;
        g[0] = Math.Sqrt(1 - k * k);
        c[0] = k;

        int N = 0;

        // Forward AGM iteration
        for (int i = 0; i < MAX_ITERATIONS - 1; i++)
        {
            a[i + 1] = (a[i] + g[i]) / 2;
            g[i + 1] = Math.Sqrt(a[i] * g[i]);
            c[i + 1] = (a[i] - g[i]) / 2;

            N = i + 1;

            if (Math.Abs(c[i + 1]) < AGM_TOLERANCE)
            {
                break;
            }
        }

        // Backward substitution
        double phi = Math.Pow(2, N) * a[N] * u;

        for (int i = N; i >= 0; i--)
        {
            phi = (phi + Math.Asin(c[i] * Math.Sin(phi) / a[i])) / 2;
        }

        return Math.Sin(phi);
    }

    /// <summary>
    /// Computes the Jacobi Elliptic Cosine function cn(u, k) using the AGM method.
    /// </summary>
    /// <param name="u">The argument</param>
    /// <param name="k">The elliptic modulus (0 &lt;= k &lt; 1)</param>
    /// <returns>cn(u, k)</returns>
    public static double JacobiCn(double u, double k)
    {
        if (k < 0 || k >= 1)
        {
            throw new ArgumentException("Elliptic modulus k must be in the range [0, 1)", nameof(k));
        }

        if (Math.Abs(k) < 1e-10)
        {
            return Math.Cos(u);
        }

        if (Math.Abs(k - 1) < 1e-10)
        {
            return 1.0 / Math.Cosh(u);
        }

        // Store transformation values
        double[] a = new double[MAX_ITERATIONS];
        double[] g = new double[MAX_ITERATIONS];
        double[] c = new double[MAX_ITERATIONS];

        a[0] = 1.0;
        g[0] = Math.Sqrt(1 - k * k);
        c[0] = k;

        int N = 0;

        // Forward AGM iteration
        for (int i = 0; i < MAX_ITERATIONS - 1; i++)
        {
            a[i + 1] = (a[i] + g[i]) / 2;
            g[i + 1] = Math.Sqrt(a[i] * g[i]);
            c[i + 1] = (a[i] - g[i]) / 2;

            N = i + 1;

            if (Math.Abs(c[i + 1]) < AGM_TOLERANCE)
            {
                break;
            }
        }

        // Backward substitution
        double phi = Math.Pow(2, N) * a[N] * u;

        for (int i = N; i >= 0; i--)
        {
            phi = (phi + Math.Asin(c[i] * Math.Sin(phi) / a[i])) / 2;
        }

        return Math.Cos(phi);
    }

    /// <summary>
    /// Computes the Jacobi Elliptic Delta Amplitude function dn(u, k) using the AGM method.
    /// </summary>
    /// <param name="u">The argument</param>
    /// <param name="k">The elliptic modulus (0 &lt;= k &lt; 1)</param>
    /// <returns>dn(u, k)</returns>
    public static double JacobiDn(double u, double k)
    {
        if (k < 0 || k >= 1)
        {
            throw new ArgumentException("Elliptic modulus k must be in the range [0, 1)", nameof(k));
        }

        if (Math.Abs(k) < 1e-10)
        {
            return 1.0;
        }

        if (Math.Abs(k - 1) < 1e-10)
        {
            return 1.0 / Math.Cosh(u);
        }

        // Store transformation values
        double[] a = new double[MAX_ITERATIONS];
        double[] g = new double[MAX_ITERATIONS];
        double[] c = new double[MAX_ITERATIONS];

        a[0] = 1.0;
        g[0] = Math.Sqrt(1 - k * k);
        c[0] = k;

        int N = 0;

        // Forward AGM iteration
        for (int i = 0; i < MAX_ITERATIONS - 1; i++)
        {
            a[i + 1] = (a[i] + g[i]) / 2;
            g[i + 1] = Math.Sqrt(a[i] * g[i]);
            c[i + 1] = (a[i] - g[i]) / 2;

            N = i + 1;

            if (Math.Abs(c[i + 1]) < AGM_TOLERANCE)
            {
                break;
            }
        }

        // Backward substitution
        double phi = Math.Pow(2, N) * a[N] * u;

        for (int i = N; i >= 0; i--)
        {
            phi = (phi + Math.Asin(c[i] * Math.Sin(phi) / a[i])) / 2;
        }

        return Math.Sqrt(1 - k * k * Math.Sin(phi) * Math.Sin(phi));
    }

    /// <summary>
    /// Computes all three Jacobi elliptic functions sn, cn, and dn simultaneously.
    /// This is more efficient than calling each function separately.
    /// </summary>
    /// <param name="u">The argument</param>
    /// <param name="k">The elliptic modulus (0 &lt;= k &lt; 1)</param>
    /// <returns>A tuple containing (sn, cn, dn)</returns>
    public static (double sn, double cn, double dn) CalculateAll(double u, double k)
    {
        if (k < 0 || k >= 1)
        {
            throw new ArgumentException("Elliptic modulus k must be in the range [0, 1)", nameof(k));
        }

        if (Math.Abs(k) < 1e-10)
        {
            double sinU = Math.Sin(u);
            double cosU = Math.Cos(u);
            return (sinU, cosU, 1.0);
        }

        if (Math.Abs(k - 1) < 1e-10)
        {
            double tanhU = Math.Tanh(u);
            double sechU = 1.0 / Math.Cosh(u);
            return (tanhU, sechU, sechU);
        }

        // Store transformation values
        double[] a = new double[MAX_ITERATIONS];
        double[] g = new double[MAX_ITERATIONS];
        double[] c = new double[MAX_ITERATIONS];

        a[0] = 1.0;
        g[0] = Math.Sqrt(1 - k * k);
        c[0] = k;

        int N = 0;

        // Forward AGM iteration
        for (int i = 0; i < MAX_ITERATIONS - 1; i++)
        {
            a[i + 1] = (a[i] + g[i]) / 2;
            g[i + 1] = Math.Sqrt(a[i] * g[i]);
            c[i + 1] = (a[i] - g[i]) / 2;

            N = i + 1;

            if (Math.Abs(c[i + 1]) < AGM_TOLERANCE)
            {
                break;
            }
        }

        // Backward substitution
        double phi = Math.Pow(2, N) * a[N] * u;

        // POPRAWKA: Zatrzymaj się na 1. Krok 0 to nasze dane wejściowe, nie używamy ich do redukcji.
        for (int i = N; i >= 1; i--)
        {
            phi = (phi + Math.Asin(c[i] * Math.Sin(phi) / a[i])) / 2;
        }

        double sinPhi = Math.Sin(phi);
        double cosPhi = Math.Cos(phi);
        
        return (sinPhi, cosPhi, Math.Sqrt(1 - k * k * sinPhi * sinPhi));
    }
}
