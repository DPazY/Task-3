using OxyPlot;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;

public class Adams : INotifyPropertyChanged
{
    public double StepSize { get; set; }
    public double x0 { get; set; }
    public double y0 { get; set; }
    public double a { get; set; }
    public double b { get; set; }
    public string maxDeviationRes { get; set; }

    public ObservableCollection<double> YValues
    {
        get { return _yValues; }
        set
        {
            _yValues = value;
            NotifyPropertyChanged(nameof(YValues));
        }
    }
    private ObservableCollection<double> _yValues;
    private PlotModel _gPlotModel;
    public PlotModel GPlotModel
    {
        get { return _gPlotModel; }
        set
        {
            _gPlotModel = value;
            NotifyPropertyChanged(nameof(GPlotModel));
        }
    }

    public Adams()
    {
        YValues = new ObservableCollection<double>();
        GPlotModel = new PlotModel { Title = "График решения методом Адамса" };
    }

    public void PlotGraph()
    {
        GPlotModel.Series.Clear();
        var series1 = new LineSeries
        {
            Title = "Приближенное решение",
            MarkerType = MarkerType.Circle
        };
        var series2 = new LineSeries
        {
            Title = "Точное решение",
            MarkerType = MarkerType.Circle
        };
        int iter = (int)Math.Round((b - a) / StepSize);
        double[] YTrue = new double[iter];
        double y = y0;
        double x = x0;
        double C = y0 / x0;
        double maxDeviation = 0;
        for (int i = 0; i < iter; i++)
        {
            y = C * (x + i * StepSize);
            YTrue[i] = y;
            double deviation = Math.Abs(YValues[i] - YTrue[i]);
            if (deviation > maxDeviation)
            {
                maxDeviation = deviation;
            }
        }

        for (int i = 0; i < YValues.Count; i++)
        {
            series1.Points.Add(new DataPoint(x0 + i * StepSize, YValues[i]));
            series2.Points.Add(new DataPoint(x0 + i * StepSize, YTrue[i]));
        }

        GPlotModel.Series.Add(series1);
        GPlotModel.Series.Add(series2);
        GPlotModel.InvalidatePlot(true);
        NotifyPropertyChanged(nameof(GPlotModel));
        maxDeviationRes = $"Максимум отклонений в узловых точках приближенного решения от точного: {maxDeviation}";
    }

    public double FirstDerivative(double x, double y)
    {
        return y / x;
    }

    public double SecondDerivative(double x, double y)
    {
        return -y / (x * x) + 2 * y / (x * x * x);
    }

    public double ThirdDerivative(double x, double y)
    {
        return 2 * y / (x * x * x) - 6 * y / (x * x * x * x);
    }

    public double[] SolveDifferentialEquation()
    {
        int iter = (int)Math.Round((b - a) / StepSize);
        double x = x0;
        double y = y0;
        double dy = y0 / x0;
        double[] Y = new double[iter];
        Y[0] = y0;

        double[] dY = new double[iter];
        dY[0] = dy;
        double[] deltay1 = new double[iter - 1];
        double[] deltay2 = new double[iter - 2];

        for (int i = 1; i < iter; i++) // 2 итераций для шага 0.2
        {
            if (i < 3)
            {
                double yNext = y + x * FirstDerivative(x, y) + (x * x) * (SecondDerivative(x, y) / 2 + (x * x * x) * (ThirdDerivative(x, y))) / 6;
                double dyNext = x / yNext;
                Y[i] = yNext;
                dY[i] = dyNext;
                deltay1[i - 1] = dY[i] - dY[i - 1];
                x += StepSize;
                y = yNext;
            }
            else
            {
                deltay2[0] = deltay1[1] - deltay1[0];
                double yNext = Y[i - 1] + StepSize * dY[i - 1] + StepSize * deltay1[i - 2] / 2 + StepSize * 5 / 12 * deltay2[i - 3] / 6;
                double dyNext = x / yNext;
                Y[i] = yNext;
                dY[i] = dyNext;
                deltay1[i - 1] = dY[i] - dY[i - 1];
                deltay2[i - 2] = deltay1[i - 1] - deltay1[i - 2];
                x += StepSize;
                y = yNext;
            }
        }
        YValues = new ObservableCollection<double>(Y);
        return Y;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
