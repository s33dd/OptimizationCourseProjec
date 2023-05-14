using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OptimizationCourseProject {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private int initResultLength;
        private int initArgsLength;
        public MainWindow() {
            InitializeComponent();
            initResultLength = Result.Content.ToString().Length;
            initArgsLength = Arguments.Content.ToString().Length;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e) {
            Result.Content = Result.Content.ToString().Substring(0, initResultLength);
            Arguments.Content = Arguments.Content.ToString().Substring(0, initArgsLength);
            List<double> limits = new List<double>();
            limits.Add(double.Parse(FirstMin.Text));
            limits.Add(double.Parse(FirstMax.Text));
            limits.Add(double.Parse(SecondMin.Text));
            limits.Add(double.Parse(SecondMax.Text));
            limits.Add(double.Parse(ThirdLimit.Text));
            Optimization opt = new Optimization(double.Parse(X1Start.Text), double.Parse(X2Start.Text), limits, 2,
                                                int.Parse(MaxIter.Text), (bool)Min.IsChecked, double.Parse(Epsilon.Text), 
                                                double.Parse(DeltaPressure1.Text), double.Parse(DeltaPressure2.Text));
            try {
                List<Point> points = opt.RandomSearch();
                ResultTable.ItemsSource = points;
                Point result = points.Last();
                Result.Content += result.Value.ToString() + " у. е.";
                Arguments.Content += $"T1 = {result.X1} °C T2 = {result.X2} °C";
            }
            catch (PointException exc) {
                MessageBox.Show(exc.Message, "Ошибка");
            }
        }
    }
}
