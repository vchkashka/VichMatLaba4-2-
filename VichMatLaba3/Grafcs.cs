using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VichMatLaba4
{
    public partial class Grafcs : Form
    {

        Functions functionApproximation;
        float step = (float)Math.Pow(10, -2);
        float xi_min, xi_max;
        public Grafcs(List<float> xi, List<float> fxi)
        {
            xi_min = xi.Min(); xi_max = xi.Max();

            functionApproximation = new Functions(xi, fxi);

            InitializeComponent();

            this.chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            for (int i = 0; i < xi.Count; i++)
            {
                this.chart1.Series[0].Points.AddXY(xi[i], fxi[i]);
            }
            chart1.MouseWheel += chart1_MouseWheel;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0";
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0";
        }

        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Колесико вниз, уменьшаем масштаб
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Колесико вверх, увеличиваем масштаб
                {
                    double xMin = xAxis.ScaleView.ViewMinimum;
                    double xMax = xAxis.ScaleView.ViewMaximum;
                    double yMin = yAxis.ScaleView.ViewMinimum;
                    double yMax = yAxis.ScaleView.ViewMaximum;

                    double posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    double posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    double posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    double posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 1; i < chart1.Series.Count; i++)
                {
                    this.chart1.Series[i].Points.Clear();
                }
                (float[] a, float[] b, float[] c, float[] d) tuple = functionApproximation.SplineCoef();

                if (checkedListBox1.GetItemChecked(0))
                {
                    for (float i = xi_min; i <= xi_max; i += step)
                    {
                        float y = functionApproximation.Spline(i, tuple);
                        this.chart1.Series[1].Points.AddXY(i, y);
                    }
                }
                if (checkedListBox1.GetItemChecked(1))
                {
                    for (float i = xi_min + step; i <= xi_max - step; i += step) 
                    {
                        float y = functionApproximation.FirstDerivative(i, tuple, step);
                        this.chart1.Series[2].Points.AddXY(i, y);
                    }
                }
                if (checkedListBox1.GetItemChecked(2))
                {
                    for (float i = xi_min + 2 * step; i <= xi_max - 2 * step; i += step) 
                    {
                        float y = functionApproximation.SecondDerivative(i, tuple, step);
                        this.chart1.Series[3].Points.AddXY(i, y);
                    }
                }
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < chart1.Series.Count; i++)
            {
                this.chart1.Series[i].Points.Clear();
            }
            
        }
    }
}
