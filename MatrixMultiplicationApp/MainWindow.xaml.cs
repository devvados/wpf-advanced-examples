using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MatrixMultiplicationApp
{
    public delegate int[,] MatrixMult(int[,] mat1, int[,] mat2);

    public partial class MainWindow
    {
        public static int N;

        public static int[][] A;
        public static int[][] B;
        public static int[][] C;
        public static object[][] ObjectMatrix;

        public void Multiplicate()
        {
            try
            {
                if (A.Length == B.Length)
                {
                    C = new int[N][];
                    for (var i = 0; i < N; i++)
                    {
                        C[i] = new int[N];
                    }

                    Parallel.For(0, N, (i) =>
                    {
                        for (var j = 0; j < N; j++)
                        {
                            for (var k = 0; k < N; k++)
                            {
                                C[i][j] += A[i][k]*B[k][j];
                            }
                        }
                    });
                    PbLoadC.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                    {
                        PbLoadC.IsIndeterminate = false;
                    }));
                    LabStatus.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                    {
                        LabStatus.Content = "Готово";
                    }));
                }
                else
                {
                    PbLoadC.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                    {
                        PbLoadC.IsIndeterminate = false;
                    }));
                    LabStatus.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                    {
                        LabStatus.Content = "Ошибка!";
                    }));
                    throw new Exception("Матрицы разных размерностей!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        static void FillMatrix(int[][] mat)
        {
            if (mat == null)
                throw new ArgumentNullException(nameof(mat));
          
            var r = new Random();
            for (var i = 0; i < N; i++)
                for (var j = 0; j < N; j++)
                    mat[i][j] = r.Next(-3, 3);
        }
        static bool CheckMatrix()
        {
            bool check = false;
            try
            {
                if (N == 0)
                    throw new Exception("Задайте размерность матрицы");
                else
                {
                    check = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return check;
        }

        public static Style TextAlignmentInCells()
        {
            //And here is the C# code to achieve the above
            var style = new Style(typeof(DataGridCell));
            style.Setters.Add(new Setter
            {
                Property = HorizontalAlignmentProperty,
                Value = HorizontalAlignment.Right
            });
            return style;
        }

        public MainWindow()
        {
            InitializeComponent();
        }
   
        private void bCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (CheckMatrix())
            {
                Task.Factory.StartNew(new Action(() =>
                {
                    PbLoadC.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                    {
                        PbLoadC.IsIndeterminate = true;
                    }));
                }));
                Task.Factory.StartNew(Multiplicate);
            }
        }

        private void FillMatrixA_Click(object sender, RoutedEventArgs e)
        {
            if (CheckMatrix())
            {
                Task.Factory.StartNew(new Action(() =>
                {
                    PbLoadA.Dispatcher.Invoke(new Action(() =>
                    {
                        PbLoadA.IsIndeterminate = true;
                    }));
                }));
                Task.Factory.StartNew(FillA);
                Task.Factory.StartNew(new Action(() =>
                {
                    PbLoadA.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                    {
                        PbLoadA.IsIndeterminate = false;
                    }));
                    LabStatus.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action((() =>
                    {
                        LabStatus.Content = "Матрица A заполнена";
                    })));
                }));
            }
        }

        private void FillMatrixB_Click(object sender, RoutedEventArgs e)
        {
            if (CheckMatrix())
            {
                Task.Factory.StartNew((() =>
                {
                    PbLoadB.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action((() =>
                    {
                        PbLoadB.IsIndeterminate = true;
                    })));
                }));
                Task.Factory.StartNew(FillB);
                Task.Factory.StartNew(new Action(() =>
                {
                    PbLoadB.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                    {
                        PbLoadB.IsIndeterminate = false;
                    }));
                    LabStatus.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action((() =>
                    {
                        LabStatus.Content = "Матрица В заполнена";
                    })));
                }));
            }
        }

        private void SaveMatA_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(new Action(() =>
            {
                using (var bW = new BinaryWriter(File.Open("MatrixA.bin", FileMode.Open)))
                {
                    bW.Flush();
                    bW.Write(N);
                    for (var i = 0; i < N; i++)
                    {
                        for (var j = 0; j < N; j++)
                        {
                            bW.Write(A[i][j]);
                        }
                    }
                }
            }));        
        }
        private void SaveMatB_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(new Action(()=>
            {
                using (var bW = new BinaryWriter(File.Open("MatrixB.bin", FileMode.Open)))
                {
                    bW.Flush();
                    bW.Write(N);
                    for (var i = 0; i < N; i++)
                    {
                        for (var j = 0; j < N; j++)
                        {
                            bW.Write(B[i][j]);
                        }
                    }
                }
            }));           
        }

        private void LoadMatA_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(new Action(() =>
            {
                using (var bR = new BinaryReader(File.Open("MatrixA.bin", FileMode.Open)))
                {
                    N = bR.ReadInt32();
                    A = new int[N][];
                    for (var i = 0; i < N; i++)
                    {
                        A[i] = new int[N];
                    }
                    for (var i = 0; i < N; i++)
                    {
                        for (var j = 0; j < N; j++)
                        {
                            A[i][j] = bR.ReadInt32();
                        }
                    }
                }
                LabStatus.Dispatcher.Invoke(new Action(() =>
                {
                    LabStatus.Content = "Матрица А заполнена";
                }));
            }));
        }
        private void LoadMatB_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(new Action(() =>
            {
                using (var bR = new BinaryReader(File.Open("MatrixB.bin", FileMode.Open)))
                {
                    N = bR.ReadInt32();
                    B = new int[N][];
                    for (var i = 0; i < N; i++)
                    {
                        B[i] = new int[N];
                    }
                    for (var i = 0; i < N; i++)
                    {
                        for (var j = 0; j < N; j++)
                        {
                            B[i][j] = bR.ReadInt32();
                        }
                    }  
                }
                LabStatus.Dispatcher.Invoke(new Action(() =>
                {
                    LabStatus.Content = "Матрица B заполнена";
                }));
            }));
        }

        private void DimItem_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new MatrixDimension();
            wnd.ShowDialog();
        }

        private void FillA()
        {
            A = new int[N][];
            for (var i = 0; i < N; i++)
            {
                A[i] = new int[N];
            }
            FillMatrix(A);
        }
        private void FillB()
        {
            B = new int[N][];
            for (var i = 0; i < N; i++) 
            {
                B[i] = new int[N];
            }
            FillMatrix(B);          
        }
    
        private void FillDgA()
        {
            var dt = new DataTable();

            for (var i = 0; i < N; i++)
            {
                dt.Columns.Add((i + 1).ToString(), typeof(int));
            }
            ObjectMatrix =  new object[N][];
            for (var i = 0; i < N; i++)
            {
                ObjectMatrix[i] = new object[N];
                for (var j = 0; j < N; j++)
                {
                    ObjectMatrix[i][j] = A[i][j];
                }
            }
            for (var i = 0; i < N; i++)
            {
                dt.LoadDataRow(ObjectMatrix[i], true);
            }
            DgMatA.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                DgMatA.DataContext = dt.DefaultView;
            }));
        }
        private void FillDgB()
        {
            var dt = new DataTable();
           
            for (var i = 0; i < N; i++)
            {
                dt.Columns.Add((i + 1).ToString(), typeof(int));
            }
            ObjectMatrix = new object[N][];
            for (var i = 0; i < N; i++)
            {
                ObjectMatrix[i] = new object[N];
                for (var j = 0; j < N; j++)
                {
                    ObjectMatrix[i][j] = B[i][j];
                }
            }
            for (var i = 0; i < N; i++)
            {
                dt.LoadDataRow(ObjectMatrix[i], true);
            }

            DgMatB.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                DgMatB.DataContext = dt.DefaultView;
            }));
        }
        private void FillDgC()
        {
            var dt = new DataTable();

            for (var i = 0; i < N; i++)
            {
                dt.Columns.Add((i + 1).ToString(), typeof(int));
            }
            ObjectMatrix = new object[N][];
            for (var i = 0; i < N; i++)
            {
                ObjectMatrix[i] = new object[N];
                for (var j = 0; j < N; j++)
                {
                    ObjectMatrix[i][j] = C[i][j];
                }
            }
            for (var i = 0; i < N; i++)
            {
                dt.LoadDataRow(ObjectMatrix[i], true);
            }

            DgMatC.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                DgMatC.DataContext = dt.DefaultView;
            }));
        }

        private void BFillDgA_OnClick(object sender, RoutedEventArgs e)
        {
            if (A != null)
            {
                Task.Factory.StartNew(new Action(() =>
                {
                    PbLoadA.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action((() =>
                    {
                        PbLoadA.IsIndeterminate = true;
                    })));
                }));
                Task.Factory.StartNew(FillDgA);
                Task.Factory.StartNew(new Action(() =>
                {
                    LabStatus.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action((() =>
                    {
                        LabStatus.Content = "Матрица А отображена";
                    })));
                    PbLoadA.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action((() =>
                    {
                        PbLoadA.IsIndeterminate = false;
                    })));
                }));
            }
            else
            {
                MessageBox.Show("Заполните матрицу А");
            }
        }
        private void BFillDgB_OnClick(object sender, RoutedEventArgs e)
        {
            if (B != null)
            {
                Task.Factory.StartNew(new Action(() =>
                {
                    PbLoadB.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action((() =>
                    {
                        PbLoadB.IsIndeterminate = true;
                    })));
                }));
                Task.Factory.StartNew(FillDgB);
                Task.Factory.StartNew(new Action(() =>
                {
                    LabStatus.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action((() =>
                    {
                        LabStatus.Content = "Матрица В отображена";
                    })));
                    PbLoadA.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action((() =>
                    {
                        PbLoadB.IsIndeterminate = false;
                    })));
                }));
            }
            else
            {
                MessageBox.Show("Заполните матрицу B");
            }
        }
        private void BFillDgC_OnClick(object sender, RoutedEventArgs e)
        {
            if (C != null)
            {
                Task.Factory.StartNew(new Action(() =>
                {
                    PbLoadC.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action((() =>
                    {
                        PbLoadC.IsIndeterminate = true;
                    })));
                }));

                Task.Factory.StartNew(FillDgC);

                Task.Factory.StartNew(new Action(() =>
                {
                    LabStatus.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action((() =>
                    {
                        LabStatus.Content = "Матрица С отображена";
                    })));

                    PbLoadC.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action((() =>
                    {
                        PbLoadC.IsIndeterminate = false;
                    })));
                }));
            }
            else
            {
                MessageBox.Show("Сначала посчитайте матрицу С");
            }
        }
    }
}
