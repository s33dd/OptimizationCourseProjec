using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationCourseProject {
    public class Optimization {
        private const double _gamma = 3.14;
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double P1 { get; set; }
        public double P2 { get; set; }
        public double FirstArgMin { get; set; }
        public double FirstArgMax { get; set; }
        public double SecondArgMin { get; set; }
        public double SecondArgMax { get; set; }
        public double ThirdLimit { get; set; }
        public int N { get; set; }
        public int MaxItertions { get; set; }
        public double MinEpsilon { get; set; }
        public bool IsMin { get; set; }
        public Optimization() {
            IsMin = true;
            P1 = 1;
            P2 = 1;
            FirstArgMin = -3;
            FirstArgMax = 0;
            SecondArgMin = -0.5;
            SecondArgMax = 3;
            ThirdLimit = 3;
            N = 2;
            MaxItertions = 20;
            MinEpsilon = 0.01;

        }
        public List<Point> RandomSearch() {
            if (!PointCheck(X1, X2)) {
                throw new PointException("Стартовая точка за пределами ограничений!");
            }
            List<Point> result = new List<Point>();
            result.Add(new Point(Math.Round(X1, 3), Math.Round(X2, 3), this));
            int count = 0;
            double epsilon = 1;
            while (count < MaxItertions) {
                count++;
                double[] alpha;
                double step = 1.0;
                double nextX1;
                double nextX2;

                for (step /= 1; step > 0.1; step /= 2) {
                    for (int i = 0; i < N; i++) {
                        alpha = NewRandomVector();
                        nextX1 = X1 + step * alpha[0];
                        nextX2 = X2 + step * alpha[1];
                        if (!PointCheck(nextX1, nextX2)) {
                            continue;
                        }
                        switch (IsMin) {
                            case true: {
                                    double prevValue = FunctionValue(X1, X2);
                                    double nextValue = FunctionValue(nextX1, nextX2);
                                    if (nextValue < prevValue ) {
                                        X1 = nextX1;
                                        X2 = nextX2;
                                        result.Add(new Point(Math.Round(nextX1, 3), Math.Round(nextX2, 3), this));
                                        epsilon = Math.Abs(FunctionValue(nextX1, nextX2) - FunctionValue(X1, X2));
                                        i = 0;
                                        step = 1.0;
                                    }
                                    break;
                                }
                            case false: {
                                    if (FunctionValue(nextX1, nextX2) > FunctionValue(X1, X2)) {
                                        X1 = nextX1;
                                        X2 = nextX2;
                                        result.Add(new Point(Math.Round(nextX1, 3), Math.Round(nextX2, 3), this));
                                        epsilon = Math.Abs(FunctionValue(nextX1, nextX2) - FunctionValue(X1, X2));
                                        i = 0;
                                        step = 1.0;
                                    }
                                    break;
                                }

                        }
                    }
                }
                if (epsilon < MinEpsilon) {
                    break;
                }
            }
            return result;
        }
        private bool PointCheck(double x1, double x2) {
            if (x1 < FirstArgMin || x1 > FirstArgMax || x2 < SecondArgMin || x2 > SecondArgMax || x2 - x1 > ThirdLimit) {
                return false;
            } else {
                return true;
            }
        }
        private double[] NewRandomVector() {
            const double upperBound = 1;
            const double lowerBound = -1;
            double[] beta = new double[N];
            double[] alpha = new double[N];
            double betaSquareSum = 0;
            for (int i = 0; i < N; i++) {
                Random rand = new Random();
                beta[i] = rand.NextDouble() * (upperBound - lowerBound) + lowerBound;
                betaSquareSum += Math.Pow(beta[i], 2);
            }
            for (int i = 0; i < N; i++) {
                alpha[i] = beta[i] / Math.Sqrt(betaSquareSum);
            }
            return alpha;
        }
        public double FunctionValue(double x1, double x2) {
            double rads = _gamma * P2 * Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(x2, 2)) * Math.PI / 180;
            double value = Math.Abs(800 * (x1 - P1) * Math.Cos(rads));
            return Math.Round(value, 3);
        }
    }
}
