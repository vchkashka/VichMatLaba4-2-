using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VichMatLaba4
{
    public class Functions
    {
        float[] xi;
        float[] fxi;

        public Functions(List<float> x, List<float> fx)
        {
            if (x.Count != fx.Count)
            {
                throw new Exception("Внимание! Разное количество значений x и f(x). Попробуйте снова.");
            }

            xi = new float[x.Count];
            fxi = new float[fx.Count];

            for (int i = 0; i < x.Count; i++)
            {
                xi[i] = x[i];
                fxi[i] = fx[i];
            }
        }

        public (float [] a, float[] b, float[] c, float[] d) SplineCoef()
        {
            int k = xi.Count();
            //коэффициенты сплайна
            float[] a = new float[k];
            float[] b = new float[k];
            float[] c = new float[k];
            float[] d = new float[k];
            //шаг
            float[] h = new float[k];

            c[0] = 0;
            c[k-1] = 0;

            for (int i = 0; i < k-1; i++)
                h[i] = xi[i+1] - xi[i];

            //по формуле 3.3
            for (int i = 0; i < k; i++)
                a[i] = fxi[i];

            //матрица для нахождения коэф с по формуле 3.11
            float[][] A = new float[k - 2][];

            for (int i = 0; i < A.Length; i++)
            {
                A[i] = new float[k - 2];
            }

            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    if ((i == 0 && j == A.Length - 1) || (i == A.Length - 1 && j == 0))
                        A[i][j] = 0;
                    else
                        if (i == j)
                        A[i][j] = 2 * (h[j] + h[j + 1]);
                    else
                        if (i < j) 
                        A[i][j] = h[j];
                    else
                        if (i > j)
                        A[i][j] = h[i];
                }
            }

            float[] B = new float[k - 2];

            for(int i = 0; i < B.Length; i++)
            {
                B[i] = 3 * ((fxi[i + 2] - fxi[i + 1]) / h[i + 1] - (fxi[i + 1] - fxi[i]) / h[i]);
            }

            for (int i = 1; i < k-1; i++)
            {
                c[i] = RunThroughMethod(A, B)[i - 1];
            }

            //по формуле 3.10
            for (int i = 0; i < k-1; i++)
                b[i] = (fxi[i + 1] - fxi[i]) / h[i] - (c[i + 1] + 2 * c[i]) * h[i] / 3;

            //по формуле 3.9
            for (int i = 0; i < k-1; i++)
                d[i] = (c[i + 1] - c[i]) / (3 * h[i]);            
            return (a, b, c, d);
        }

        //Построение сплайна
        public float Spline(float x, (float[] a, float[] b, float[] c, float[] d) tuple)
        {
            float fx = -1;
            for (int i = 0; i < xi.Length - 1; i++)
                if (x >= xi[i] && x < xi[i + 1])
                {
                    fx = tuple.a[i] + tuple.b[i] * (x - xi[i]) + tuple.c[i] * (float)Math.Pow((x - xi[i]), 2) + tuple.d[i] * (float)Math.Pow((x - xi[i]), 3);
                }
            return fx;
        }

        //Первая производная по формуле 4.3
        public float FirstDerivative(float x, (float[] a, float[] b, float[] c, float[] d) tuple, float step)
        {
            float fx = (Spline(x + step, tuple) - Spline(x - step, tuple)) / (2 * step);

            //по готовой формуле производной сплайна
            //for (int i = 0; i < xi.Length - 1; i++)
            //    if (x >= xi[i] && x <= xi[i + 1])
            //    {
            //        fx = tuple.b[i] + 2 * tuple.c[i] * (x - xi[i]) + 3 * tuple.d[i] * (float)Math.Pow((x - xi[i]), 2);
            //    }
            return fx;
        }

        //Вторая производная по формуле 4.4
        public float SecondDerivative(float x, (float[] a, float[] b, float[] c, float[] d) tuple, float step)
        {
            float fx = (Spline(x + 2 * step, tuple) - 2 * Spline(x, tuple) + Spline(x - 2 * step, tuple)) / (4*(float)Math.Pow(step, 2));

            //по готовой формуле производной сплайна
            //for (int i = 0; i < xi.Length - 1; i++)
            //    if (x >= xi[i] && x <= xi[i + 1])
            //    {
            //        fx = 2 * tuple.c[i] + 6 * tuple.d[i] * (x - xi[i]);
            //    }
            return fx;
        }

        //Метод прогонки из прошлой лабораторной
        static float[] RunThroughMethod(float[][] a, float[] b)
        {
            int n = a.Length;
            for (int i = 0; i < n; i++)
            {
                if (a[i].Length != n)
                {
                    throw new Exception("Размерность не соответствует");
                }
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i][i] == 0)
                {
                    throw new Exception("Нулевые элементы на главной диагонали");
                }
            }

            //Вектор решений
            float[] x = new float[n];
            //Прямой ход
            float[] alpha = new float[n];
            float[] beta = new float[n];
            //для первой строки
            alpha[0] = a[0][1] / -a[0][0];
            beta[0] = -b[0] / -a[0][0];
            //для всех строк, кроме первой и последней
            for (int i = 1; i < n - 1; i++)
            {
                alpha[i] = a[i][i + 1] / (-a[i][i] - a[i][i - 1] * alpha[i - 1]);
                beta[i] = (a[i][i - 1] * beta[i - 1] - b[i]) / (-a[i][i] - a[i][i - 1] * alpha[i - 1]);
            }
            //для последней строки
            alpha[n - 1] = 0;
            beta[n - 1] = (a[n - 1][n - 2] * beta[n - 2] - b[n - 1]) / (-a[n - 1][n - 1] - a[n - 1][n - 2] * alpha[n - 2]);
            //Обратный ход
            x[n - 1] = beta[n - 1];
            for (int i = n - 1; i > 0; i--)
                x[i - 1] = alpha[i - 1] * x[i] + beta[i - 1];
            return x;
        }
    }
}
