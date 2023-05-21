using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Plot3D.Graph3D;
using static Plot3D.ColorSchema;
using System.Drawing;

namespace OptimizationCourseProject {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private int initResultLength;
        private int initArgsLength;
        private Optimization opt;
        private bool isAdmin = false;
        private string pass = "admin";
        public MainWindow() {
            InitializeComponent();
            initResultLength = Result.Content.ToString().Length;
            initArgsLength = Arguments.Content.ToString().Length;
            opt = new Optimization();
            this.DataContext = opt;
			FileInfo passFile = new FileInfo("pass.txt");
			if (!passFile.Exists) {
				passFile.Create().Close();
				using (StreamWriter sw = passFile.AppendText()) {
					sw.WriteLine(pass);
				}
			}
			using (StreamReader sr = passFile.OpenText()) {
				string s;
				while ((s = sr.ReadLine()) != null) {
					pass = s;
				}
			}
            InitComboBoxes();
			Method.SelectedIndex = 0;
            Variant.SelectedIndex = 0;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject {
            if (depObj == null)
                yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null)
                    continue;
                if (ithChild is T t)
                    yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild))
                    yield return childOfChild;
            }
        }
        private void Calculate_Click(object sender, RoutedEventArgs e) {
            foreach (TextBox tb in FindVisualChildren<TextBox>(this)) {
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
            List<Point> points = new List<Point>();
            Result.Content = Result.Content.ToString().Substring(0, initResultLength);
            Arguments.Content = Arguments.Content.ToString().Substring(0, initArgsLength);
            switch (Method.SelectedIndex) {
                case 0: {
                        try {
                            points = opt.RandomSearch();
                            ResultTable.ItemsSource = points;
                            Point result = points.Last();
                            Result.Content += result.Value.ToString() + " у. е.";
                            Arguments.Content += $"T1 = {Math.Round(result.X1, 2)} °C T2 = {Math.Round(result.X2, 2)} °C";
                        }
                        catch (PointException exc) {
                            MessageBox.Show(exc.Message, "Ошибка");
                        }
                        break;
                    }
                case 1: {
                        try {
                            points = opt.Box();
                            ResultTable.ItemsSource = points;
                            Point result = points.Last();
                            Result.Content += result.Value.ToString() + " у. е.";
                            Arguments.Content += $"T1 = {Math.Round(result.X1, 2)} °C T2 = {Math.Round(result.X2, 2)} °C";
                        }
                        catch (PointException exc) {
                            MessageBox.Show(exc.Message, "Ошибка");
                        }
                        break;
                    }
                default: {
                        return;
                    }
            }
            Draw3dChart();
            Draw2dChart(points);
        }

        private void TextBoxValidation_Error(object sender, ValidationErrorEventArgs e) {
            foreach (TextBox tb in FindVisualChildren<TextBox>(this)) {
                if (Validation.GetHasError(tb)) {
                    Calculate.IsEnabled = false;
                    return;
                }
            }
            Calculate.IsEnabled = true;
        }

        private void Draw3dChart() {
            Graph3d.Raster = eRaster.Labels;
            System.Drawing.Color[] c_Colors = GetSchema(eSchema.Hot);
            Graph3d.SetColorScheme(c_Colors, 3);
            int stepQuantity = 30;
            cPoint3D[,] points3d = new cPoint3D[stepQuantity, stepQuantity];
            int row = 0;
            int col = 0;
            double stepX1 = (opt.FirstArgMax - opt.FirstArgMin) / stepQuantity;
            double stepX2 = (opt.SecondArgMax - opt.SecondArgMin) / stepQuantity;
            for (double i = opt.FirstArgMin; Math.Round(i, 1) < opt.FirstArgMax; i += stepX1)
            {
                for (double j = opt.SecondArgMin; Math.Round(j, 1) < opt.SecondArgMax; j += stepX2)
                {
                    double value = Math.Round(opt.FunctionValue(i, j), 3);
                    points3d[row, col] = new cPoint3D(i, j, value);
                    col++;
                }
                row++;
                col = 0;
            }
            Graph3d.AxisX_Legend = "T1, °C";
            Graph3d.AxisY_Legend = "T2, °C";
            Graph3d.AxisZ_Legend = "F, у.е.";

            PointF start = new PointF(-50, -50);
            PointF end = new PointF(50, 50);
            delRendererFunction function = opt.FunctionValue;

			Graph3d.SetFunction(function, start, end, 2.0, eNormalize.MaintainXY);
        }

        private void Draw2dChart(List<Point> points) {
            HeatMap chart = new HeatMap(opt, points);
            Plot.DataContext = chart;
        }

        private void Method_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (Method.SelectedIndex == 1) {
                X1Start.IsEnabled = false;
                X2Start.IsEnabled = false;
            } else {
                X1Start.IsEnabled = true;
                X2Start.IsEnabled = true;
            }
        }

		private void ChangeUsrBtn_Click(object sender, RoutedEventArgs e) {
            switch (isAdmin) {
                case true: {
                        this.Title = "Исследователь";
                        isAdmin = false;
                        NewMethod.Visibility = Visibility.Collapsed;
                        NewVar.Visibility = Visibility.Collapsed;
                        break;
                    }
                case false: {
                        Pass dialog = new Pass(pass);
                        if (dialog.ShowDialog() == true) {
                            this.Title = "Администратор";
                            isAdmin = true;
                            NewMethod.Visibility = Visibility.Visible;
                            NewVar.Visibility = Visibility.Visible;
                        }
                        break;
                    }
            }
		}

		private void NewMethod_Click(object sender, RoutedEventArgs e) {
            EditFiles window = new EditFiles("methods.txt");
            window.ShowDialog();
            InitComboBoxes();
		}

		private void NewVar_Click(object sender, RoutedEventArgs e) {
            EditFiles window = new EditFiles("variants.txt");
            window.ShowDialog();
            InitComboBoxes();
		}

        private void InitComboBoxes() {
			FileInfo methodsFile = new FileInfo("methods.txt");
			if (!methodsFile.Exists) {
				methodsFile.Create().Close();
				using (StreamWriter sw = methodsFile.AppendText()) {
					sw.WriteLine("Метод случайных направлений (мод.)");
					sw.WriteLine("Метод Бокса");
				}
			}
			FileInfo varsFile = new FileInfo("variants.txt");
			if (!varsFile.Exists) {
				varsFile.Create().Close();
				using (StreamWriter sw = varsFile.AppendText()) {
					sw.WriteLine("Вариант 11");
				}
			}
			List<string> methods = new List<string>();
			List<string> vars = new List<string>();
			using (StreamReader sr = methodsFile.OpenText()) {
				string s;
				while ((s = sr.ReadLine()) != null) {
					methods.Add(s);
				}
			}
			using (StreamReader sr = varsFile.OpenText()) {
				string s;
				while ((s = sr.ReadLine()) != null) {
					vars.Add(s);
				}
			}
			Method.ItemsSource = methods;
			Variant.ItemsSource = vars;
		}
	}
}
