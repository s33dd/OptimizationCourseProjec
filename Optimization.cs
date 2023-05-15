using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OptimizationCourseProject {
    public class Optimization : IDataErrorInfo {
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
        public int MaxIterations { get; set; }
        public double MinEpsilon { get; set; }
        public bool IsMin { get; set; }
        public string this[string columnName] {
            get {
                string error = string.Empty;
                switch (columnName) {
                    case "P1":
                        if (P1 <= 0) {
                            error = "Перепад давлений должен быть больше нуля.";
                        }
                        break;
                    case "P2":
                        if (P2 <= 0) {
                            error = "Перепад давлений должен быть больше нуля.";
                        }
                        break;
                    case "MaxIterations":
                        if (MaxIterations <= 0) {
                            error = "Количество итераций должно быть больше нуля.";
                        }
                        break;
                    case "MinEpsilon":
                        if (MinEpsilon <= 0) {
                            error = "Точность должна быть больше нуля.";
                        }
                        break;
                    default:
                        break;
                }
                return error;
            }

        }
        public string Error {
            get { throw new NotImplementedException(); }
        }
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
            MaxIterations = 20;
            MinEpsilon = 0.01;

        }
        public List<Point> RandomSearch() {
            /*List<int> test = new List<int>();
            for (int i = 0; i < 10; i += 2) {
                test[i] = i;
            }*/
            if (!PointCheckAllLimits(X1, X2)) {
                throw new PointException("Стартовая точка за пределами ограничений!");
            }
            List<Point> result = new List<Point>();
            result.Add(new Point(Math.Round(X1, 3), Math.Round(X2, 3), this));
            int count = 0;
            double epsilon = 1;
            while (count < MaxIterations) {
                count++;
                double[] alpha;
                double step = 1.0;
                double nextX1;
                double nextX2;

                for (step /= 1; step > 0.1; step /= 2) {
                    for (int i = 0; i < N; i++) {
                        alpha = NewRandomVector(N, -1, 1);
                        nextX1 = X1 + step * alpha[0];
                        nextX2 = X2 + step * alpha[1];
                        if (!PointCheckAllLimits(nextX1, nextX2)) {
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
        public List<Point> Box() {
            if (!PointCheckAllLimits(X1, X2)) {
                throw new PointException("Стартовая точка за пределами ограничений!");
            }
            List<Point> result = new List<Point>();

            //Определение количества вершин
            int vertexQuantity;
            switch (N > 5) {
                case true: {
                        vertexQuantity = N + 1;
                        break;
                    }
                case false: {
                        vertexQuantity = N * 2;
                        break;
                    }
            }

            List<Point> fixedVertices = new List<Point>();
            List<Point> tempVertices = new List<Point>();

            //Генерация вершин
            for (int j = 0; j < vertexQuantity; j++) {
                double[] r = NewRandomVector(N, 0, 1);
                X1 = FirstArgMin + r[0] * (FirstArgMax - FirstArgMin);
                X2 = SecondArgMin + r[1] * (SecondArgMax - SecondArgMin);
                Point vertex = new Point(Math.Round(X1, 3), Math.Round(X2, 3), this);
                tempVertices.Add(vertex);
                if (PointCheckSecondLimits(X1, X2)) {
                    fixedVertices.Add(vertex);
                }
                if (j == vertexQuantity - 1 & fixedVertices.Count == 0) {
                    j = 0;
                }
            }
            //Проверка на незафиксированные вершины и смещение
            while (fixedVertices.Count < vertexQuantity) {
                List<Point> wrongVertices = new List<Point>();
                foreach (Point point in tempVertices) {
                    if (!fixedVertices.Contains(point)) {
                        wrongVertices.Add(point);
                    }
                }
                double fixedX1sum = 0;
                double fixedX2sum = 0;
                foreach (Point point in fixedVertices) {
                    fixedX1sum += point.X1;
                    fixedX2sum += point.X2;
                }
                int index = tempVertices.IndexOf(wrongVertices[0]);
                while (!PointCheckAllLimits(wrongVertices[0].X1, wrongVertices[0].X2)) {
                    wrongVertices[0].X1 = 1.0 / 2.0 * (wrongVertices[0].X1 + 1 / fixedVertices.Count * fixedX1sum);
                    wrongVertices[0].X2 = 1.0 / 2.0 * (wrongVertices[0].X2 + 1 / fixedVertices.Count * fixedX2sum);
                }
                tempVertices[index] = wrongVertices[0];
                fixedVertices.Add(wrongVertices[0]);
            }

            //Вычисление значений ц.ф. в фиксированных вершинах
            List<double> verticesValues = new List<double>();
            foreach (Point point in fixedVertices) {
                verticesValues.Add(point.Value);
            }

            int count = 0;
            double epsilon = 1;

            while (count < MaxIterations) {
                count++;

                //Выбор наилучшей и наихудшей вершины
                int bestVertex;
                int worstVertex;
                switch (IsMin) {
                    case true:
                        bestVertex = verticesValues.IndexOf(verticesValues.Min());
                        worstVertex = verticesValues.IndexOf(verticesValues.Max());
                        break;
                    case false:
                        worstVertex = verticesValues.IndexOf(verticesValues.Min());
                        bestVertex = verticesValues.IndexOf(verticesValues.Max());
                        break;
                }

                //Определение координат центра
                double x1Sum = 0;
                double x2Sum = 0;
                foreach (Point point in fixedVertices) {
                    x1Sum += point.X1;
                    x2Sum += point.X2;
                }
                double x1Center = 1 / (double)(vertexQuantity - 1) * (x1Sum - fixedVertices[worstVertex].X1);
                double x2Center = 1 / (double)(vertexQuantity - 1) * (x2Sum - fixedVertices[worstVertex].X2);
                Point center = new Point(Math.Round(x1Center, 3),Math.Round(x2Center, 3), this);

                result.Add(center);
                //Проверка условий выхода
                double term1 = Math.Abs(center.X1 - fixedVertices[worstVertex].X1);
                double term2 = Math.Abs(center.X1 - fixedVertices[bestVertex].X1);
                double term3 = Math.Abs(center.X2 - fixedVertices[worstVertex].X2);
                double term4 = Math.Abs(center.X2 - fixedVertices[bestVertex].X2);
                epsilon = 1 / (2 * (double)N) * (term1 + term2 + term3 + term4);
                if (epsilon < MinEpsilon) {
                    break;
                }

                //Вычисление следующей координаты комплекса
                X1 = 2.3 * center.X1 - 1.3 * fixedVertices[worstVertex].X1;
                X2 = 2.3 * center.X2 - 1.3 * fixedVertices[worstVertex].X2;

                //Проверка условий первого и второго рода
                if (!PointCheckFirstLimits(X1, X2)) {
                    if (X1 < FirstArgMin) {
                        X1 = FirstArgMin + MinEpsilon;
                    } else if (X1 > FirstArgMax) {
                        X1 = FirstArgMax - MinEpsilon;
                    }
                    if (X2 < SecondArgMin) {
                        X2 = SecondArgMin + MinEpsilon;
                    } else if (X2 > SecondArgMax) {
                        X2 = SecondArgMax - MinEpsilon;
                    }
                }
                while (!PointCheckSecondLimits(X1, X2)) {
                    X1 = 1 / 2 * (X1 + center.X1);
                    X2 = 1 / 2 * (X2 + center.X2);
                }

                //Вычисление нового значения ц.ф.
                double value = FunctionValue(X1, X2);
                switch (IsMin) {
                    case true: {
                            while (value > verticesValues[worstVertex]) {
                                X1 = 1 / 2.0 * (X1 + fixedVertices[bestVertex].X1);
                                X2 = 1 / 2.0 * (X2 + fixedVertices[bestVertex].X2);
                                value = FunctionValue(X1, X2);
                            }
                            Point newVertex = new Point(Math.Round(X1, 3), Math.Round(X2, 3), this);
                            fixedVertices[worstVertex] = newVertex;
                            verticesValues[worstVertex] = newVertex.Value;
                            break;
                        }
                    case false: {
                            while (value < verticesValues[worstVertex]) {
                                X1 = 1 / 2.0 * (X1 + fixedVertices[bestVertex].X1);
                                X2 = 1 / 2.0 * (X2 + fixedVertices[bestVertex].X2);
                                value = FunctionValue(X1, X2);
                            }
                            Point newVertex = new Point(Math.Round(X1, 3), Math.Round(X2, 3), this);
                            fixedVertices[worstVertex] = newVertex;
                            verticesValues[worstVertex] = newVertex.Value;
                            break;
                        }
                }
            }
            return result;
        }
        private bool PointCheckAllLimits(double x1, double x2) {
            if (x1 < FirstArgMin || x1 > FirstArgMax || x2 < SecondArgMin || x2 > SecondArgMax || x2 - x1 > ThirdLimit) {
                return false;
            } else {
                return true;
            }
        }
        private bool PointCheckFirstLimits(double x1, double x2) {
            if (x1 < FirstArgMin || x1 > FirstArgMax || x2 < SecondArgMin || x2 > SecondArgMax) {
                return false;
            }
            else {
                return true;
            }
        }
        private bool PointCheckSecondLimits(double x1, double x2) {
            if (x2 - x1 > ThirdLimit) {
                return false;
            }
            else {
                return true;
            }
        }
        private double[] NewRandomVector(int n, double upperBound, double lowerBound) {
            double[] beta = new double[N];
            double[] alpha = new double[N];
            double betaSquareSum = 0;
            for (int i = 0; i < n; i++) {
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
