using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;

namespace tvims
{
    public partial class Form1 : Form
    {
        List<double> etta = new List<double>();
        List<double> F = new List<double>();
        List<double> ni = new List<double>();

        List<double> Left = new List<double>();
        List<double> Right = new List<double>();
        List<double> Mid = new List<double>();
        List<double> f_mid = new List<double>();
        List<double> h = new List<double>();


        public Form1()
        {
            InitializeComponent();
        }

        double fn_hi(double x, int r)
        {
            if (x <= 0)
                return 0.0;
            else
                return Math.Pow(2, (double)(-r) / 2) * Math.Pow(SpecialFunctions.Gamma((double)(r) / 2), (-1)) * Math.Pow(x, (((double)(r) / 2)) - 1.0) * Math.Pow(Math.E, (-x / 2));
        }

        double trapezoid_formula(double a, double b, int n, int r)
        {
            double h = (b - a) / n;
            double integral = 0.0;
            for(int i = 1; i <= n; i++)
            {
                double g1 = a + h * (i - 1);
                double g2 = a + h * i;
                integral += 0.5 * h * (fn_hi(g1, r) + fn_hi(g2, r));
            }
            return integral;
        }

        double fn(double y, double sigma)
        {
            return y / Math.Pow(sigma, 2) * Math.Exp(-(y * y) / (2 * Math.Pow(sigma, 2)));
        }

        double Fn(double y, double sigma)
        {
            return 1 - Math.Exp(-(y * y) / (2 * Math.Pow(sigma, 2)));
        }
        public double random_number(double min, double max)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            return rand.NextDouble() * (max - min) + min;
        }

        public void clear_all()
        {
            listBox1.Items.Clear();
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            foreach (var series in chart1.Series)
                series.Points.Clear();
            foreach (var series in chart2.Series)
                series.Points.Clear();
            Left.Clear();
            Right.Clear();
            Mid.Clear();
            f_mid.Clear();
            F.Clear();
            ni.Clear();
            ni.Clear();
            h.Clear();
            etta.Clear();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            clear_all();
            double sigma = Double.Parse(textBox1.Text);
            int n = Int32.Parse(textBox2.Text);
            for (int i = 0; i < n; i++)
            {
                double y;
                double t = random_number(0.0, 1.0);
                y = sigma * Math.Sqrt(-2 * Math.Log(1 - t));
                etta.Add(y);
            }
            etta.Sort();
            for (int i = 0; i < n; i++)
            {
                listBox1.Items.Add(etta[i]);
            }
            label17.Text = ("[0; " + Double.PositiveInfinity + "]");

            double M = Math.Sqrt(Math.PI / 2) * sigma;
            double D = (2 - Math.PI / 2) * Math.Pow(sigma, 2);
            double _x = 0, S = 0, R = 0, Me = 0;

            //выборочное среднее и выборочная дисперсия
            for (int i = 0; i < n; i++)
            {
                _x = _x + etta[i];
                S = S + etta[i] * etta[i];
            }
            _x = _x / n;
            S = S / n - _x * _x;
            //размах выборки
            R = etta[n - 1] - etta[0];
            //выборочная медиана
            if (n % 2 == 0)
                Me = (etta[n / 2] + etta[n / 2 - 1]) / 2;
            else Me = etta[n / 2];
            dataGridView1.Rows.Add(M, _x, Math.Abs(M - _x), D, S, Math.Abs(D - S), Me, R);

            double step = 0.01, x = 0.0;
            while (x < etta[n-1]+2)
            {
                chart2.Series[0].Points.AddXY(x, Fn(x, sigma));
                x += step;
            }

            int k = 0, col = 1;
            chart2.Series[1].Points.AddXY(0, 0);
            chart2.Series[1].Points.AddXY(etta[0], 0);
            while (k < etta.Count)
            {
                int j = 0;
                chart2.Series[1].Points.AddXY(etta[k], (double)col / (double)n);
                col = 1;
                while (j < k)
                {
                    if (etta[j] <= etta[k])
                        col++;
                    j++;
                }

                chart2.Series[1].Points.AddXY(etta[k], (double)col / (double)n);
                k++;
            }
            chart2.Series[1].Points.AddXY(etta[k - 1], 1);
            chart2.Series[1].Points.AddXY(etta[k - 1] + 2, 1);

            double div = 0, loc_max = 0;
            for (int i = 0; i < n; i++)
            {
                double F = Fn(etta[i], sigma);
                double expr1 = (double)(i + 1) / (double)n - F;
                double expr2 = F - (double)i / (double)n;
                if (expr1 < expr2)
                    loc_max = expr2;
                else if (expr2 < expr1)
                    loc_max = expr1;
                if (div < loc_max)
                    div = loc_max;
                else if (loc_max < div)
                    div = div;
            }
            label8.Text = div.ToString();
        }

        int j = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            double sigma = Double.Parse(textBox1.Text);
            int n = Int32.Parse(textBox2.Text);
            double left = Double.Parse(textBox3.Text);
            double right = Double.Parse(textBox4.Text);
            double zj = (left + right) / 2.0;
            double fj = fn(zj, sigma);
            double delta = Math.Abs(right - left); ;
            double nj = 0.0;
            int r = 0;

            while (r < etta.Count)
            {
                if ((etta[r] <= right) && (etta[r] >= left)) nj++;
                r++;
            }
            double hj = nj / ((double)n * delta);

            dataGridView2.Rows.Add(zj, fj, hj);
            Left.Add(left);
            Right.Add(right);
            Mid.Add(zj);
            f_mid.Add(fj);
            h.Add(hj);
            ni.Add(nj);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int n = Int32.Parse(textBox2.Text);
            for (int i=0; i<h.Count; i++)
            {
                chart1.Series[0].Points.AddXY(i, h[i]);
                chart1.Series[1].Points.AddXY(i, f_mid[i]);
            }
            List<double> delta = new List<double>();
            for (int i = 0; i < h.Count; i++)
            {
                double d = Math.Abs(h[i] - f_mid[i]);
                delta.Add(d);
            }
            double max_f_h = delta.Max();
            label6.Text = max_f_h.ToString();
        }

        
        List<double> Left_n = new List<double>();
        List<double> Right_n = new List<double>();
        List<double> etta_n = new List<double>();
        List<double> Q = new List<double>();
        private void button6_Click(object sender, EventArgs e)
        {
            int n = Int32.Parse(textBox2.Text);
            //int k = 2;//количество интервалов
            //int t = 0;
            //while (t != 0)
            //{
            //    Left_n.Add(t / k);
            //    if (t > etta[n - 1])
            //        Right_n.Add(Double.PositiveInfinity);
            //    Right_n.Add((t + 1) / k);
            //    t++;
            //}
            double left = Double.Parse(textBox8.Text);
            double right = Double.Parse(textBox7.Text);
            if (right > etta[n - 1])
                right = Double.PositiveInfinity;
            dataGridView4.Rows.Add(left, right);
            Left_n.Add(left);
            Right_n.Add(right);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double sigma = Double.Parse(textBox1.Text);
            int n = Int32.Parse(textBox2.Text);
            double alfa = Double.Parse(textBox9.Text);
            int number_of_samples = Int32.Parse(textBox5.Text);
            int sum = 0;

            dataGridView3.Rows.Clear();
            dataGridView5.Rows.Clear();
            etta_n.Clear();
            int i = 0;
            while(i<number_of_samples)
            {
                etta_n.Clear();
                for (int j = 0; j < n; j++)
                {
                    double y;
                    double p = random_number(0.0, 1.0);
                    y = sigma * Math.Sqrt(-2 * Math.Log(1 - p));
                    etta_n.Add(y);
                }
                etta_n.Sort();
                int t = 0;
                double R0 = 0.0;
                double q = 0.0;
                Q.Clear();
                while (t < Left_n.Count())
                {
                    int r = 0;
                    double nj = 0;
                    if (t == Left_n.Count())
                        q = 1 - Fn(Left_n[t], sigma);
                    else
                        q = Fn(Right_n[t], sigma) - Fn(Left_n[t], sigma);
                    Q.Add(q);
                    while (r < etta_n.Count)
                    {
                        if ((etta_n[r] <= Right_n[t]) && (etta_n[r] >= Left_n[t]))
                            nj++;
                        r++;
                    }
                    R0 += Math.Pow((nj - n * q), 2) / (n * q);
                    t++;
                }
                double FR0;
                FR0 = 1-trapezoid_formula(0, R0, 10000, Left_n.Count() - 1);
                if (FR0 < 1 - alfa)
                {
                    dataGridView5.Rows.Add(R0, FR0, 1);
                    sum++;
                }
                else
                    dataGridView5.Rows.Add(R0, FR0, 0);
                i++;
            }
            int k = 0;
            while (k < Left_n.Count())
            {
                dataGridView3.Rows.Add(k, Q.First());
                Q.RemoveAt(0);
                k++;
            }
            label14.Text = sum.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView3.Rows.Clear();
            dataGridView4.Rows.Clear();
            dataGridView5.Rows.Clear();
            Left_n.Clear();
            Right_n.Clear();
        }
    }
}
