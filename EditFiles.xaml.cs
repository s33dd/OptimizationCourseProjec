using System;
using System.Collections.Generic;
using System.IO;
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
	/// Interaction logic for EditFiles.xaml
	/// </summary>
	public partial class EditFiles : Window {
		private string path;
		public EditFiles(string path) {
			InitializeComponent();
			this.path = path;
		}

		private void Add_Click(object sender, RoutedEventArgs e) {
			FileInfo file = new FileInfo(path);
			if (!file.Exists) {
				MessageBox.Show("Произошла ошибка. Файл не найден.");
				this.Close();
			} else {
				using (StreamWriter sw = file.AppendText()) {
					if (NewBox.Text.Trim() != string.Empty) {
						sw.WriteLine(NewBox.Text);
					}
				}
			}
		}
    }
}
