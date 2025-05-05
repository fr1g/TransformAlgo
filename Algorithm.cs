using MathNet.Numerics.LinearAlgebra;

namespace TransformAlgo;

public static class Algorithm
{
    public static Matrix<double> ConvertToMatrix(double[][] jaggedArray)
    {
        // 1. 验证输入合法性
        if (jaggedArray == null)
            throw new ArgumentNullException(nameof(jaggedArray), "输入数组不能为 null");
        if (jaggedArray.Length == 0)
            throw new ArgumentException("输入数组不能为空");

        int rows = jaggedArray.Length;
        int cols = jaggedArray[0].Length;

        // 检查是否为矩形数组
        for (int i = 1; i < rows; i++)
        {
            if (jaggedArray[i].Length != cols)
                throw new ArgumentException("输入数组必须是矩形（所有行长度相同）");
        }

        // 2. 使用 MathNet 内置方法直接转换
        return Matrix<double>.Build.DenseOfRows(jaggedArray);
    }
    public static List<double[]> Convert2DArrayToList(double[,] array2D)
    {
        // 检查输入合法性
        if (array2D == null)
        {
            throw new ArgumentNullException(nameof(array2D), "输入数组不能为 null");
        }

        int rows = array2D.GetLength(0);    // 获取行数
        int cols = array2D.GetLength(1);    // 获取列数

        List<double[]> resultList = new List<double[]>(rows);

        // 逐行提取数据
        for (int i = 0; i < rows; i++)
        {
            double[] rowData = new double[cols];
            for (int j = 0; j < cols; j++)
            {
                rowData[j] = array2D[i, j];
            }
            resultList.Add(rowData);
        }

        return resultList;
    }

    public static Matrix<double> CreateMatrix(List<double[]> original)
    {
        Matrix<double> matrix = Matrix<double>.Build.Dense(original.Count, original[0].Length);
        (int x, int y) = (0, 0);
        foreach (var submodule in original)
        { // row x
            y = 0;
            foreach (var i in submodule)
            { // line y
                matrix[x, y] = i;
                y++;
            }
            x++;
        }

        return matrix;
    }
    
    public static List<double[]> Invert(List<double[]> original)
    {
        var matrix = CreateMatrix(original).Inverse();
        return Convert2DArrayToList(matrix.ToArray());
    }

    public static string Decrypt(List<double[]> tokens, double[,] key)
    {
        var convertedKey = Matrix<double>.Build.DenseOfArray(key);
        var prepare = "";
        tokens.Reverse();
        var revealed = Convert2DArrayToList(((convertedKey.Inverse() * CreateMatrix(tokens).Transpose()).Transpose().ToArray()));
        foreach (var token in revealed)
        {
            var reversed = token;
            foreach (var chars in reversed)
            {
                var precised = (int)Math.Ceiling(chars);
                // Console.WriteLine(precised);
                prepare += (char)precised;
                
            } 
            // Console.WriteLine("END");
        }

        prepare = new string(prepare.Reverse().ToArray());
        return prepare;
    }
    public static List<double[]> Encrypt(string raw, double[,] key)
    {
        if (raw.Length % 3 != 0)
        {
            for (var n = 0; n <= raw.Length % 3; n++)
            {
                raw += " ";
            }
        }
        var vertical = new List<double[]>();
        double[] buffer = [0, 0, 0];
        string debugReveal = "";
        int current = 0;
        foreach (var chars in raw)
        {
            buffer[current] = (double)chars;
            current++;
            debugReveal += chars;
            if (current == 3)
            {
                // Console.WriteLine($"DR: {debugReveal} ");
                debugReveal = "";
                current = 0;
                vertical.Add(buffer);
                buffer = [0, 0, 0];
            }
        }

        // foreach (var token in vertical)
        // {
        //     Console.WriteLine(": ");
        //     foreach (var item in token)
        //     {
        //         Console.Write($"{item} ");
        //     }
        //     Console.WriteLine("; ");
        // }
        // steps:
        // divide list of submodules into independent lists to parse as matrix (sub-vertical)
        // calculate vertical per sub-vertical
        // collect calculated sub-verticals, merge into one, parse to string

        var convertedKey = Matrix<double>.Build.DenseOfArray(key);
        var realVertical = new List<double[]>();
        foreach (var subVer in vertical)
        {
           
            // var singleMatrix = CreateMatrix(SingleVerticalize(subVer));
            var aMatrix = new List<double[]>();
            // subVer.Reverse();
            aMatrix.Add(subVer.Reverse().ToArray());
            var singleMatrix = CreateMatrix(aMatrix).Transpose();
            // here's a List with only [double, double, double]
            var n = Convert2DArrayToList((convertedKey * singleMatrix).ToArray());
            List<double> verticalValues = new();
            foreach (var values in n)
            {
                verticalValues.Add(values[0]);
            }

            var verValArray = verticalValues.ToArray(); // now <[], [], []> became [, , , ], next step is put them into realvert
            realVertical.Add(verValArray);
        }

        return realVertical;
        /*
         * <[, , ,], [, , , ], ...>: each [, , , ] need to be converted to <[], [], [],> to convert to Matrix
         * and in the end will belike: <<[], [], []>, <[], [], []>, ...>
         *
         *
         * 
         */
    }

    public static List<double[]> SingleVerticalize(double[] was)
    {
        
        List<double[]> result = new();
        foreach (var wases in was)
        {
            result.Add(new double[1]{wases});
        }

        return result;
    }

    /// <summary>
    /// By default, this function is only implemented 3-char size parsing.
    /// </summary>
    /// <param name="text">formulae [1, 2, 3]; [4, 5, 6]; [7, 8, 9]</param>
    /// <returns>Real Vertical Matrix-Array</returns>
    public static List<double[]> ParseCypher(string text)
    {
        // formulae [1, 2, 3]; [4, 5, 6]; [7, 8, 9]
        var vertical = new List<double[]>();
        var modules = text.Trim().Replace(" ", "").Split(";");
        foreach (var subModule in modules)
        {
            double[] empty = [0, 0, 0];
            var index = 0;
            foreach (var token in subModule.Replace("[", "").Replace("]", "").Split(","))
            {
                empty[index] = double.Parse(token);
                index++;
            }
            
            vertical.Add(empty);
        }
        return vertical;
    }

    public static string Stringify(List<double[]> vertical) // zipped vertical
    {
        var result = "";
        foreach (var submodule in vertical)
        {
            var module = "[";
            var index = 0;
            foreach (var token in submodule)
            {
                if (index == 2) module += $"{token}";
                else module += $"{token}, ";
                index++;
            }

            module += "]; ";
            result += module;
        }

        return result[..^2];
    } 
}