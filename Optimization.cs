using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationCourseProject {
    public class Optimization {
        private double _x1;
        private double _x2;
        private double _p1;
        private double _p2;
        private double _firstArgMin;
        private double _firstArgMax;
        private double _secondArgMin;
        private double _secondArgMax;
        private double _thirdLimit;

        private const double _gamma = 3.14;

        private int _n;
        private int _maxIterations;
        private bool _isMin;

        private double _minEpsilon;
        public Optimization(double x1, double x2, List<double> limits, int n, int maxIter, bool isMin, double epsilon, double p1, double p2) { 
            _x1 = x1;
            _x2 = x2;

            _firstArgMin = limits[0];
            _firstArgMax = limits[1];
            _secondArgMin = limits[2];
            _secondArgMax = limits[3];
            _thirdLimit = limits[4];

            _isMin = isMin;

            _n = n; 
            _maxIterations = maxIter;

            _minEpsilon = epsilon;

            _p1 = p1;
            _p2 = p2;
        }
        public List<Point> RandomSearch() {
            if (!PointCheck(_x1, _x2)) {
                throw new PointException("Стартовая точка за пределами ограничений!");
            }
            List<Point> result = new List<Point>();
            result.Add(new Point(_x1, _x2, this));
            int count = 0;
            double epsilon = 1;
            while (count < _maxIterations) {
                count++;
                double[] alpha;
                double step = 1.0;
                double nextX1;
                double nextX2;

                for (step /= 1; step > 0.1; step /= 2) {
                    for (int i = 0; i < _n; i++) {
                        alpha = NewRandomVector();
                        nextX1 = _x1 + step * alpha[0];
                        nextX2 = _x2 + step * alpha[1];
                        if (!PointCheck(nextX1, nextX2)) {
                            continue;
                        }
                        switch (_isMin) {
                            case true: {
                                    double prevValue = FunctionValue(_x1, _x2);
                                    double nextValue = FunctionValue(nextX1, nextX2);
                                    if (nextValue < prevValue ) {
                                        _x1 = nextX1;
                                        _x2 = nextX2;
                                        result.Add(new Point(Math.Round(nextX1, 3), Math.Round(nextX2, 3), this));
                                        epsilon = Math.Abs(FunctionValue(nextX1, nextX2) - FunctionValue(_x1, _x2));
                                        i = 0;
                                        step = 1.0;
                                    }
                                    break;
                                }
                            case false: {
                                    if (FunctionValue(nextX1, nextX2) > FunctionValue(_x1, _x2)) {
                                        _x1 = nextX1;
                                        _x2 = nextX2;
                                        result.Add(new Point(Math.Round(nextX1, 3), Math.Round(nextX2, 3), this));
                                        epsilon = Math.Abs(FunctionValue(nextX1, nextX2) - FunctionValue(_x1, _x2));
                                        i = 0;
                                        step = 1.0;
                                    }
                                    break;
                                }

                        }
                    }
                }
                if (epsilon < _minEpsilon) {
                    break;
                }
            }
            return result;
        }
        private bool PointCheck(double x1, double x2) {
            if (x1 < _firstArgMin || x1 > _firstArgMax || x2 < _secondArgMin || x2 > _secondArgMax || x2 - x1 > _thirdLimit) {
                return false;
            } else {
                return true;
            }
        }
        private double[] NewRandomVector() {
            const double upperBound = 1;
            const double lowerBound = -1;
            double[] beta = new double[_n];
            double[] alpha = new double[_n];
            double betaSquareSum = 0;
            for (int i = 0; i < _n; i++) {
                Random rand = new Random();
                beta[i] = rand.NextDouble() * (upperBound - lowerBound) + lowerBound;
                betaSquareSum += Math.Pow(beta[i], 2);
            }
            for (int i = 0; i < _n; i++) {
                alpha[i] = beta[i] / Math.Sqrt(betaSquareSum);
            }
            return alpha;
        }
        public double FunctionValue(double x1, double x2) {
            double rads = _gamma * _p2 * Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(x2, 2)) * Math.PI / 180;
            double value = Math.Abs(800 * (x1 - _p1) * Math.Cos(rads));
            return Math.Round(value, 3);
        }
    }
}
