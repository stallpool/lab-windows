// ref: https://gist.github.com/NotFounds/0f3420a0f60597114eb1f30fd74720c1
// modified
using System;

namespace CVxLab
{
    public static class MatrixUitls
    {
        public static Matrix Zero(int n)
        {
            if (n <= 0) throw new ArgumentException();
            return new Matrix(n, n);
        }

        public static Matrix Identity(int n)
        {
            if (n <= 0) throw new ArgumentException();
            var ret = new Matrix(n, n);
            for (int i = 0; i < n; ++i)
                ret[i, i] = 1;
            return ret;
        }

        public static double Trace(Matrix mat)
        {
            if (mat.Row != mat.Col) throw new ArithmeticException();
            var ret = 0.0;
            for (int i = 0; i < mat.Row; ++i)
                ret += mat[i, i];
            return ret;
        }

        public static Matrix Inverse(Matrix mat)
        {
            if (mat.Col != mat.Row) throw new ArithmeticException();
            if (mat.Col == 2) return Inverse2(mat);

            var N = mat.Row;
            var ret = new Matrix(N, N);
            var identify = Identity(N);

            for (int i = 0; i < N; ++i)
            {
                if (Math.Abs(mat[i, i]) < double.Epsilon) return null;
                var inv = 1.0 / mat[i, i];

                for (int j = 0; j < N; j++)
                {
                    mat[i, j] *= inv;
                    ret[i, j] *= inv;
                }

                for (int j = 0; j < N; j++)
                {
                    if (i != j)
                    {
                        inv = mat[j, i];
                        for (int k = 0; k < N; k++)
                        {
                            mat[j, k] -= mat[i, k] * inv;
                            ret[k, j] -= ret[i, k] * inv;
                        }
                    }
                }
            }

            return ret;
        }

        public static Matrix Inverse2(Matrix mat)
        {
            var det = Determinant2(mat);
            if (Math.Abs(det) < double.Epsilon) return null;
            return new Matrix(2, 2, mat[1, 1] / det, -mat[0, 1] / det, -mat[1, 0] / det, mat[0, 0] / det);
        }

        public static double Determinate(Matrix mat)
        {
            if (mat.Col != mat.Row) throw new ArithmeticException();
            if (mat.Col == 2) return Determinant2(mat);
            if (mat.Col == 3) return Determinant3(mat);

            Matrix L, U;
            LUDecomposition(mat, out L, out U);
            double ret = 1;
            for (int i = 0; i < U.Row; ++i)
                ret *= U[i, i];
            return ret;
        }

        public static double Determinant2(Matrix mat)
        {
            if (mat.Col != 2 || mat.Row != 2) throw new ArithmeticException();
            return mat[0, 0] * mat[1, 1] - mat[0, 1] * mat[1, 0];
        }

        public static double Determinant3(Matrix mat)
        {
            if (mat.Col != 3 || mat.Row != 3) throw new ArithmeticException();
            return mat[0, 0] * mat[1, 1] * mat[2, 2] + mat[0, 1] * mat[1, 2] * mat[2, 0] + mat[0, 2] * mat[1, 0] * mat[2, 1]
                - mat[0, 2] * mat[1, 1] * mat[2, 0] - mat[1, 2] * mat[2, 1] * mat[0, 0] - mat[2, 2] * mat[0, 1] * mat[1, 0];
        }

        public static Matrix Pow(Matrix mat, int n)
        {
            if (mat.Col != mat.Row) throw new ArithmeticException();
            if (n < 0) throw new ArithmeticException();
            if (n == 1) return mat;
            if (n % 2 == 0)
            {
                return Pow(mat * mat, n / 2);
            }
            else
            {
                return Pow(mat, n - 1) * mat;
            }
        }

        public static Matrix Transposition(Matrix mat)
        {
            var ret = new Matrix(mat.Col, mat.Row);
            for (int i = 0; i < mat.Row; ++i)
            {
                for (int j = 0; j < mat.Col; ++j)
                {
                    ret[j, i] = mat[i, j];
                }
            }
            return ret;
        }

        public static void LUDecomposition(Matrix mat, out Matrix L, out Matrix U)
        {
            if (mat.Col != mat.Row) throw new ArithmeticException();

            var N = mat.Row;

            L = Identity(N);
            U = Zero(N);

            var sum = 0.0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i > j)
                    {
                        sum = 0.0;
                        for (int k = 0; k < j; k++)
                        {
                            sum += L[i, k] * U[k, j];
                        }
                        L[i, j] = (mat[i, j] - sum) / U[j, j];
                    }
                    else
                    {
                        sum = 0.0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += L[i, k] * U[k, j];
                        }
                        U[i, j] = mat[i, j] - sum;
                    }
                }
            }
        }

        public static Matrix LForwardsubs(Matrix L, Matrix b)
        {
            if (L.Row != b.Row && b.Col != 1) throw new ArithmeticException();
            var N = L.Row;

            var ret = new Matrix(N, 1);
            for (int i = 0; i < N; ++i)
                ret[i, 0] = b[i, 0];

            for (int i = 0; i < N; i++)
            {
                ret[i, 0] /= L[i, i];
                for (int j = i + 1; j < N; j++)
                {
                    ret[j, 0] -= ret[i, 0] * L[j, i];
                }
            }

            return ret;
        }

        public static Matrix UBackwardsubs(Matrix U, Matrix y)
        {
            if (U.Row != y.Row && y.Col != 1) throw new ArithmeticException();
            var N = U.Row;

            var ret = new Matrix(N, 1);
            for (int i = 0; i < N; i++)
            {
                ret[i, 0] = y[i, 0];
            }
            for (int i = N - 1; i >= 0; i--)
            {
                ret[i, 0] /= U[i, i];
                for (int j = i - 1; j >= 0; j--)
                {
                    ret[j, 0] -= ret[i, 0] * U[j, i];
                }
            }

            return ret;
        }

        public static Matrix SolveSimultaneousEquations(Matrix mat, Matrix b)
        {
            var N = mat.Row;
            Matrix L, U;
            L = Identity(N);
            U = Zero(N);

            var y = LForwardsubs(L, b);
            return UBackwardsubs(U, y);
        }
    }

    public class Matrix
    {
        private double[,] _Matrix;
        public int Row
        {
            private set;
            get;
        }
        public int Col
        {
            private set;
            get;
        }

        public Matrix(double[,] mat)
        {
            if (mat == null) throw new ArgumentNullException();

            _Matrix = mat;
            Row = mat.GetLength(0);
            Col = mat.GetLength(1);
        }

        public Matrix(int row, int col)
        {
            if (row <= 0 || col <= 0) throw new ArgumentException();

            _Matrix = new double[row, col];
            Row = row;
            Col = col;
        }

        public Matrix(int row, int col, params double[] args)
        {
            if (row <= 0 || col <= 0) throw new ArgumentException();
            if (args.Length != row * col) throw new ArgumentException();

            _Matrix = new double[row, col];
            Row = row;
            Col = col;

            for (int i = 0; i < row; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    _Matrix[i, j] = args[i * col + j];
                }
            }
        }

        public double this[int i, int j]
        {
            set
            {
                if (i < 0 || i >= Row) throw new IndexOutOfRangeException();
                if (j < 0 || j >= Col) throw new IndexOutOfRangeException();
                _Matrix[i, j] = value;
            }
            get
            {
                if (i < 0 || i >= Row) throw new IndexOutOfRangeException();
                if (j < 0 || j >= Col) throw new IndexOutOfRangeException();
                return _Matrix[i, j];
            }
        }

        public double[] GetRaw()
        {
            int size = Row * Col;
            if (size == 0) return null;
            double[] ret = new double[size];
            int k = 0;
            for (int i = 0; i < Row; ++i)
            {
                for (int j = 0; j < Col; ++j)
                {
                    ret[k++] = this[i, j];
                }
            }
            return ret;
        }

        public Matrix GetRow(int r)
        {
            if (r < 0 || r >= Row) throw new IndexOutOfRangeException();
            var ret = new Matrix(1, Col);

            for (int i = 0; i < Col; ++i)
            {
                ret[0, i] = this[0, i];
            }
            return ret;
        }

        public Matrix GetCol(int c)
        {
            if (c < 0 || c >= Col) throw new IndexOutOfRangeException();
            var ret = new Matrix(Row, 1);

            for (int i = 0; i < Row; ++i)
            {
                ret[i, 0] = this[i, 0];
            }
            return ret;
        }

        public Matrix GetLowerTriangularMatrix()
        {
            if (Row != Col) throw new ArithmeticException();
            var ret = new Matrix(Row, Col);

            for (int i = 0; i < Row; ++i)
            {
                for (int j = 0; j <= i; ++j)
                {
                    ret[i, j] = this[i, j];
                }
            }

            return ret;
        }


        public Matrix GetUpperTriangularMatrix()
        {
            if (Row != Col) throw new ArithmeticException();
            var ret = new Matrix(Row, Col);

            for (int i = 0; i < Row; ++i)
            {
                for (int j = i; j < Col; ++j)
                {
                    ret[i, j] = this[i, j];
                }
            }

            return ret;
        }

        public Matrix SelfAbs()
        {
            for (int i = 0; i < Row; ++i)
            {
                for (int j = i; j < Col; ++j)
                {
                    if (this[i, j] < 0) this[i, j] = -this[i, j];
                }
            }
            return this;
        }

        public double AggregateSquadSum()
        {
            double sum = 0;
            for (int i = 0; i < Row; ++i)
            {
                for (int j = i; j < Col; ++j)
                {
                    sum += this[i, j] * this[i, j];
                }
            }
            return sum;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            //if (a == null || b == null) throw new ArgumentNullException();
            if (a.Row != b.Row || a.Col != b.Col) throw new ArithmeticException();
            var row = a.Row;
            var col = a.Col;
            var ret = new Matrix(row, col);

            for (int i = 0; i < row; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    ret[i, j] = a[i, j] + b[i, j];
                }
            }

            return ret;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.Row != b.Row || a.Col != b.Col) throw new ArithmeticException();
            var row = a.Row;
            var col = a.Col;
            var ret = new Matrix(row, col);

            for (int i = 0; i < row; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    ret[i, j] = a[i, j] - b[i, j];
                }
            }
            return ret;
        }

        public static Matrix operator *(double x, Matrix mat)
        {
            var ret = new Matrix(mat.Row, mat.Col);
            for (int i = 0; i < mat.Row; ++i)
            {
                for (int j = 0; j < mat.Col; ++j)
                {
                    ret[i, j] = x * mat[i, j];
                }
            }

            return ret;
        }

        public static Matrix operator *(Matrix mat, double x)
        {
            return x*mat;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Col != b.Row) throw new ArithmeticException();
            var row = a.Row;
            var col = b.Col;
            var m = a.Col;
            var ret = new Matrix(row, col);

            for (int i = 0; i < row; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    for (int k = 0; k < m; ++k)
                    {
                        ret[i, j] = a[i, k] * b[k, j] + ret[i, j];
                    }
                }
            }

            return ret;
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.Row != b.Row || a.Col != b.Col) return false;

            for (int i = 0; i < a.Row; ++i)
            {
                for (int j = 0; j < a.Col; ++j)
                {
                    if (!a[i, j].Equals(b[i, j])) return false;
                }
            }

            return true;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var mat = obj as Matrix;
            return this == mat;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

