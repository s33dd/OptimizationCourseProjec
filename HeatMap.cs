using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;

namespace OptimizationCourseProject
{
    public class HeatMap {
        public PlotModel Model { get; set; }
        public HeatMap(Optimization opt, List<Point> way) {
            Model = new PlotModel { Title = "График линий равных значений" };

            double x0 = -35;
            double x1 = 35;
            double y0 = -35;
            double y1 = 35;

            Func<double, double, double> peaks = (x, y) => opt.FunctionValue(x, y);
            var xx = ArrayBuilder.CreateVector(x0, x1, 1000);
            var yy = ArrayBuilder.CreateVector(y0, y1, 1000);
            var peaksData = ArrayBuilder.Evaluate(peaks, xx, yy);

            var cs = new ContourSeries {
                Color = OxyColors.Black,
                LabelBackground = OxyColors.White,
                ContourLevelStep = 2000,
                ColumnCoordinates = yy,
                RowCoordinates = xx,
                Data = peaksData
            };
            cs.TrackerFormatString = "X1={2}\nX2={4}\nZ={6}";

			var startPoint = new LineSeries {
                Color = OxyColors.Red,
                MarkerType = MarkerType.Circle,
                MarkerSize = 5
            };
			startPoint.ItemsSource = new Point[1] { way[0] };
			startPoint.TrackerFormatString = "X1={2}\nX2={4}\nZ={Value}";
            var endPoint = new LineSeries {
                Color = OxyColors.Red,
                MarkerType = MarkerType.Star,
                MarkerStroke = OxyColors.Red,
                MarkerStrokeThickness = 2,
				MarkerSize = 5
			};
			endPoint.ItemsSource = new Point[1] { way[way.Count - 1] };
			endPoint.TrackerFormatString = "X1={2}\nX2={4}\nZ={Value}";
            var ls = new LineSeries {
                Color = OxyColors.Red
            };
			ls.ItemsSource = way.ToArray();
			ls.TrackerFormatString = "X1={2}\nX2={4}\nZ={Value}";

			Model.Series.Add(cs);
            Model.Series.Add(ls);
            Model.Series.Add(startPoint);
            Model.Series.Add(endPoint);
        }
    }
}
