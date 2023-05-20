using System;
using OxyPlot;
using OxyPlot.Series;

namespace OptimizationCourseProject
{
    public class HeatMap {
        public PlotModel Model { get; set; }
        public HeatMap(Optimization opt) {
            Model = new PlotModel { Title = "График линий равных значений" };

            double x0 = -35;
            double x1 = 35;
            double y0 = -35;
            double y1 = 35;

            Func<double, double, double> peaks = (x, y) => opt.FunctionValue(x, y);
            var xx = ArrayBuilder.CreateVector(x0, x1, 100);
            var yy = ArrayBuilder.CreateVector(y0, y1, 100);
            var peaksData = ArrayBuilder.Evaluate(peaks, xx, yy);

            var cs = new ContourSeries
            {
                Color = OxyColors.Black,
                LabelBackground = OxyColors.White,
                ContourLevelStep = 2000,
                ColumnCoordinates = yy,
                RowCoordinates = xx,
                Data = peaksData
            };

            Model.Series.Add(cs);
        }
    }
}
