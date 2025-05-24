using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Adams _adams = new Adams();

        public MainWindow()
        {
            InitializeComponent(); 
            DataContext = _adams;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtX0.Text, out double x0) &&
                double.TryParse(txtY0.Text, out double y0) &&
                double.TryParse(txtA.Text, out double a) &&
                double.TryParse(txtB.Text, out double b) &&
                double.TryParse(txtH.Text, out double h))
            {
                _adams.x0 = x0;
                _adams.y0 = y0;
                _adams.a = a;
                _adams.b = b;
                _adams.StepSize = h;

                _adams.SolveDifferentialEquation();
                _adams.PlotGraph();
                txtResult.Text = _adams.maxDeviationRes;
            }
            else
            {
                _adams.maxDeviationRes = "Ошибка: введите корректные числовые значения.";
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioButtonGraph == null || RadioButtonTable == null) return;

            _adams.GraphVisibility = RadioButtonGraph.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            _adams.TableVisibility = RadioButtonTable.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}