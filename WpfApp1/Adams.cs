using OxyPlot;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

public class Adams : INotifyPropertyChanged
{
    private ObservableCollection<KeyValuePair<double, double>> _yValues;
    private PlotModel _gPlotModel;
    private Visibility _graphVisibility = Visibility.Collapsed;
    private Visibility _tableVisibility = Visibility.Visible;
    private string _maxDeviationRes;

    public double StepSize { get; set; }
    public double x0 { get; set; }
    public double y0 { get; set; }
    public double a { get; set; }
    public double b { get; set; }

    public string maxDeviationRes
    {
        get => _maxDeviationRes;
        set
        {
            _maxDeviationRes = value;
            NotifyPropertyChanged(nameof(maxDeviationRes));
        }
    }

    public ObservableCollection<KeyValuePair<double, double>> YValues
    {
        get => _yValues;
        set
        {
            _yValues = value;
            NotifyPropertyChanged(nameof(YValues));
        }
    }

    public PlotModel GPlotModel
    {
        get => _gPlotModel;
        set
        {
            _gPlotModel = value;
            NotifyPropertyChanged(nameof(GPlotModel));
        }
    }

    public Visibility GraphVisibility
    {
        get => _graphVisibility;
        set
        {
            _graphVisibility = value;
            NotifyPropertyChanged(nameof(GraphVisibility));
        }
    }

    public Visibility TableVisibility
    {
        get => _tableVisibility;
        set
        {
            _tableVisibility = value;
            NotifyPropertyChanged(nameof(TableVisibility));
        }
    }

    public Adams()
    {
        YValues = new ObservableCollection<KeyValuePair<double, double>>();
        GPlotModel = new PlotModel { Title = "График решения" };
    }

    public void PlotGraph()
    {
        GPlotModel.Series.Clear();
        var approxSeries = new LineSeries { Title = "Приближенное решение", MarkerType = MarkerType.Circle };
        var exactSeries = new LineSeries { Title = "Точное решение", MarkerType = MarkerType.Circle };

        int iter = (int)Math.Ceiling((b - a) / StepSize);
        double C = (y0 - Math.Sin(x0) + 1) * Math.Exp(Math.Sin(x0));
        double maxDeviation = 0;

        for (int i = 0; i < iter; i++)
        {
            double currentX = a + i * StepSize;
            double exactY = Math.Sin(currentX) - 1 + C * Math.Exp(-Math.Sin(currentX));
            exactSeries.Points.Add(new DataPoint(currentX, exactY));

            if (i < YValues.Count)
            {
                approxSeries.Points.Add(new DataPoint(currentX, YValues[i].Value));
                double deviation = Math.Abs(YValues[i].Value - exactY);
                maxDeviation = Math.Max(maxDeviation, deviation);
            }
        }

        GPlotModel.Series.Add(approxSeries);
        GPlotModel.Series.Add(exactSeries);
        GPlotModel.InvalidatePlot(true);
        maxDeviationRes = $"Максимальное отклонение: {maxDeviation:F6}";
    }

    public double FirstDerivative(double x, double y)
    {
        return -y * Math.Cos(x) + Math.Cos(x) * Math.Sin(x);
    }

    public double SecondDerivative(double x, double y)
    {
        return y * Math.Pow(Math.Cos(x), 2)
               - 2 * Math.Pow(Math.Cos(x), 2) * Math.Sin(x)
               + y * Math.Sin(x)
               + Math.Pow(Math.Cos(x), 3)
               - 3 * Math.Sin(x) * Math.Pow(Math.Cos(x), 2);
    }

    public double ThirdDerivative(double x, double y)
    {
        return -y * Math.Pow(Math.Cos(x), 3)
               + 3 * y * Math.Cos(x) * Math.Pow(Math.Sin(x), 2)
               - 6 * Math.Pow(Math.Cos(x), 3) * Math.Sin(x)
               + 4 * Math.Cos(x) * Math.Pow(Math.Sin(x), 3)
               - 3 * Math.Pow(Math.Cos(x), 2) * Math.Sin(x);
    }

    public double[] SolveDifferentialEquation()
    {
        int iter = (int)Math.Ceiling((b - a) / StepSize);
        double[] Y = new double[iter];
        Y[0] = y0;
        double x = a;

        // Метод Тейлора для первых 3 точек
        for (int i = 1; i < Math.Min(3, iter); i++)
        {
            double h = StepSize;
            double f1 = FirstDerivative(x, Y[i - 1]);
            double f2 = SecondDerivative(x, Y[i - 1]);
            double f3 = ThirdDerivative(x, Y[i - 1]);

            Y[i] = Y[i - 1] + h * f1 + h * h / 2 * f2 + h * h * h / 6 * f3;
            x += h;
        }

        // Метод Адамса-Башфорта 3-го порядка
        for (int i = 3; i < iter; i++)
        {
            double h = StepSize;
            double f0 = FirstDerivative(x - 3 * h, Y[i - 3]);
            double f1 = FirstDerivative(x - 2 * h, Y[i - 2]);
            double f2 = FirstDerivative(x - h, Y[i - 1]);

            Y[i] = Y[i - 1] + h / 12 * (23 * f2 - 16 * f1 + 5 * f0);
            x += h;
        }

        YValues = new ObservableCollection<KeyValuePair<double, double>>(
            Enumerable.Range(0, iter)
                      .Select(i => new KeyValuePair<double, double>(a + i * StepSize, Y[i]))
        );
        return Y;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}