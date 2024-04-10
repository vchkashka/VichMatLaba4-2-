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

namespace VichMatLaba4
{
    public partial class Form1 : Form
    {
        List<float> xi = new List<float>() { -2, 0, 2, 3, 4 };
        List<float> fxi = new List<float>() { 18, 12, 7, -1, 0 };

        public Form1()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        //Сортировка точек
        private static void Sort(List<float> xi, List<float> fxi)
        {
            for (int i = 0; i < xi.Count(); i++)
            {
                for (int j = 0; j < xi.Count() - 1; j++)
                {
                    if (xi[j] > xi[j + 1])
                    {
                        float temp1 = xi[j];
                        xi[j] = xi[j + 1];
                        xi[j + 1] = temp1;

                        float temp2 = fxi[j];
                        fxi[j] = fxi[j + 1];
                        fxi[j + 1] = temp2;
                    }
                }
            }
        }

        private static void Check(List<float> xi, List<float> fxi)
        {
            for (int i = 0; i < xi.Count(); i++)
                for (int j = 1; j < xi.Count(); j++)
                    if ((xi[i] == xi[j]) && (fxi[i] == fxi[j]) && (i != j))
                        throw new Exception("Внимание! Не должно быть одинаковых точек. Попробуйте снова.");
                    else if ((xi[i] == xi[j]) && (i != j))
                        throw new Exception("Внимание! Не должно быть точек с одинаковыми координатами x. Попробуйте снова.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                xi.Clear();
                fxi.Clear();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1[0, i].Value != null && dataGridView1[0, i].Value.ToString() != "")
                    {
                        xi.Add(Convert.ToInt32(dataGridView1[0, i].Value));
                    }
                    if (dataGridView1[1, i].Value != null && dataGridView1[1, i].Value.ToString() != "")
                    {
                        fxi.Add(Convert.ToInt32(dataGridView1[1, i].Value));
                    }
                }
                if (xi.Count() < 4 || fxi.Count() < 4)
                    throw new Exception("Внимание! Должно быть не менее 4 точек. Попробуйте снова.");
                Check(xi, fxi);
                Sort(xi, fxi);
                Grafcs grafcs = new Grafcs(xi, fxi);
                grafcs.Show();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}
