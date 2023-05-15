using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OptimizationCourseProject {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private int initResultLength;
        private int initArgsLength;
        private Optimization opt;
        public MainWindow() {
            InitializeComponent();
            initResultLength = Result.Content.ToString().Length;
            initArgsLength = Arguments.Content.ToString().Length;
            opt = new Optimization();
            this.DataContext = opt;
            FileInfo methodsFile = new FileInfo("methods.txt ");
            if (!methodsFile.Exists) {
                methodsFile.Create().Close();
                using (StreamWriter sw = methodsFile.AppendText()) {
                    sw.WriteLine("Метод случайных направлений (мод.)");
                    sw.WriteLine("Метод Бокса");
                }
            }
            FileInfo varsFile = new FileInfo("variants.txt ");
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
            Result.Content = Result.Content.ToString().Substring(0, initResultLength);
            Arguments.Content = Arguments.Content.ToString().Substring(0, initArgsLength);
            switch (Method.SelectedIndex) {
                case 0: {
                        try {
                            List<Point> points = opt.RandomSearch();
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
                            List<Point> points = opt.Box();
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
                        break;
                    }
            }
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
    }
}
