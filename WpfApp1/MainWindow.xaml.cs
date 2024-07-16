using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace WpfApp1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        Adams adams = new Adams();
        private Visibility tableVisibility = Visibility.Visible;
        private Visibility graphVisibility = Visibility.Collapsed;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Visibility TableVisibility
        {
            get => tableVisibility;
            set
            {
                tableVisibility = value;
                OnPropertyChanged(nameof(TableVisibility));
            }
        }

        public Visibility GraphVisibility
        {
            get => graphVisibility;
            set
            {
                graphVisibility = value;
                OnPropertyChanged(nameof(GraphVisibility));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtX0.Text, out double x0) &&
                double.TryParse(txtY0.Text, out double y0) &&
                double.TryParse(txta.Text, out double a) &&
                double.TryParse(txtb.Text, out double b) &&
                double.TryParse(txth.Text, out double h))
            {                
                adams.x0 = x0;
                adams.y0 = y0;
                adams.a = a;
                adams.b = b;
                adams.StepSize = h;
                adams.SolveDifferentialEquation();
                adams.PlotGraph();
                txtResult.Text = adams.maxDeviationRes;                
            }
            else
            {
                txtResult.Text = "Пожалуйста, введите корректные числовые значения.";
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowResult();
        }

        private void ShowResult()
        {
            if (RadioButtonTable.IsChecked == true)
            {
                TableVisibility = Visibility.Visible;
                GraphVisibility = Visibility.Collapsed;                
            }
            else if (RadioButtonGraph.IsChecked == true)
            {
                TableVisibility = Visibility.Collapsed;
                GraphVisibility = Visibility.Visible;
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
