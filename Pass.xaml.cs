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
using System.Windows.Shapes;

namespace OptimizationCourseProject {
	/// <summary>
	/// Interaction logic for Pass.xaml
	/// </summary>
	public partial class Pass : Window {
		private string pass;
		public Pass(string pass) {
			InitializeComponent();
			this.pass = pass;
		}

		private void Check_Click(object sender, RoutedEventArgs e) {
			if (PassBox.Password == pass) {
				this.DialogResult = true;
			} else {
				MessageBox.Show("Пароль неверный.");
				this.DialogResult = false;
			}
        }
    }
}
