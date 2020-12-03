using System;
using System.Collections.Generic;

namespace Lab2SaiIO
{
    public class Matrix:ICloneable
    {
        public int[][] Values;
        public int[][] Potential;
        public int[][] StartState { get; private set; }
        private Matrix() { }
        public Matrix(int[][] values)
        {
            Values = values;
            StartState = new int[Values.Length][];
            for (int indexFirst = 0; indexFirst < Values.Length; indexFirst++)
            {
                StartState[indexFirst] = new int[Values.Length];
                for (int indexSecond = 0; indexSecond < Values.Length; indexSecond++)
                {
                    StartState[indexFirst][indexSecond] = values[indexFirst][indexSecond];
                }
            }
            Potential = new int[Values.Length][];
            for (int index = 0; index < Values.Length; index++)
            {
                Potential[index] = new int[Values.Length];
            }
        }
        public object Clone()
        {
            Matrix newMatrix = new Matrix
            {
                Values = new int[this.Values.Length][],
                Potential = new int[this.Values.Length][],
                StartState = new int[this.Values.Length][]
            };
            for (int indexFirst = 0; indexFirst < this.Values.Length; indexFirst++)
            {
                newMatrix.Values[indexFirst] = new int[this.Values.Length];
                newMatrix.Potential[indexFirst] = new int[this.Values.Length];
                newMatrix.StartState[indexFirst] = new int[this.Values.Length];
                for (int indexSecond = 0; indexSecond < this.Values.Length; indexSecond++)
                {
                    newMatrix.Values[indexFirst][indexSecond] = this.Values[indexFirst][indexSecond];
                    newMatrix.Potential[indexFirst][indexSecond] = this.Potential[indexFirst][indexSecond];
                    newMatrix.StartState[indexFirst][indexSecond] = this.StartState[indexFirst][indexSecond];
                    
                }
            }
            return newMatrix;
        }
        public void FindPotential(List<int> from, List<int> to)
        {
            for (int indexFirst = 0; indexFirst < Values.Length; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < Values.Length; indexSecond++)
                {
                    Potential[indexFirst][indexSecond] = 0;
                    int lineMin = Int32.MaxValue;
                    int columnMin = Int32.MaxValue;
                    if (Values[indexFirst][indexSecond] == 0 && !from.Contains(indexFirst) && !to.Contains(indexSecond))
                    {
                        for (int indexThird = 0; indexThird < Values.Length; indexThird++)
                        {
                            if (indexFirst != indexThird && Values[indexThird][indexSecond] < lineMin && !from.Contains(indexThird))
                            {
                                lineMin = Values[indexThird][indexSecond];
                            }

                            if (indexSecond != indexThird && Values[indexFirst][indexThird] < columnMin && !to.Contains(indexThird))
                            {
                                columnMin = Values[indexFirst][indexThird];
                            }
                        }
                        if (lineMin == Int32.MaxValue)
                        {
                            lineMin = 0;
                        }
                        if (columnMin == Int32.MaxValue)
                        {
                            columnMin = 0;
                        }
                        Potential[indexFirst][indexSecond] += columnMin + lineMin;
                    }
                }
            }
        }
    }

    static class Program
    {
        static int _min;
        static readonly List<int> OptimalFrom = new List<int>();
        static readonly List<int> OptimalTo = new List<int>();
        static int _count;
        static void Solve(Matrix matrix, int record, List<int> currentFrom, List<int> currentTo)
        {
            Console.WriteLine("Current path contains next roads:");
            for (int index = 0; index < currentFrom.Count; index++)
            {
                Console.WriteLine((currentFrom[index] + 1) + " -> " + (currentTo[index] + 1));
            }
            if (currentFrom.Count == _count)
            {
                Console.WriteLine("Optimal cycle contains next roads:");
                for (int index = 0; index < _count; index++)
                {
                    Console.WriteLine((OptimalFrom[index] + 1) + " -> " + (OptimalTo[index] + 1));
                }
                Console.WriteLine("Cost of optimal cycle is " + _min);
                int length = 0;
                for (int index = 0; index < _count; index++)
                {
                    length += matrix.StartState[currentFrom[index]][currentTo[index]];
                }
                if (length < _min)
                {
                    _min = length;
                    OptimalFrom.Clear();
                    foreach (int item in currentFrom) OptimalFrom.Add(item);
                    OptimalTo.Clear();
                    foreach (int item in currentTo) OptimalTo.Add(item);
                }
                Console.WriteLine("Cost of current path is " + length);
                Console.WriteLine("Current optimal cycle contains next roads:");
                for (int index = 0; index < _count; index++)
                {
                    Console.WriteLine((OptimalFrom[index] + 1) + " -> " + (OptimalTo[index] + 1));
                }
                Console.WriteLine("Cost of current optimal cycle is " + _min);
                Console.WriteLine("Return to previous state cause find new cycle...");
                return;
            }
            Console.WriteLine("Matrix before reducing:");
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    if (matrix.Values[indexFirst][indexSecond] != Int32.MaxValue)
                    {
                        Console.Write("{0,3} ", matrix.Values[indexFirst][indexSecond]);
                    }
                    else Console.Write("NAN");
                }
                Console.WriteLine();
            }
            int localRecord = record;
            int[] lineMin = new int[_count];
            int[] columnMin = new int[_count];
            for (int index = 0; index < _count; index++)
            {
                if (currentFrom.Contains(index))
                {
                    lineMin[index] = 0;
                }
                else
                {
                    lineMin[index] = Int32.MaxValue;
                }
                if (currentTo.Contains(index))
                {
                    columnMin[index] = 0;
                }
                else
                {
                    columnMin[index] = Int32.MaxValue;
                }
            }
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    if (matrix.Values[indexFirst][indexSecond] < lineMin[indexFirst]
                        && !currentFrom.Contains(indexFirst) && !currentTo.Contains(indexSecond))
                    {
                        lineMin[indexFirst] = matrix.Values[indexFirst][indexSecond];
                    }
                }
            }
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    if (!currentFrom.Contains(indexFirst) && !currentTo.Contains(indexSecond)
                                                          && matrix.Values[indexFirst][indexSecond] != Int32.MaxValue)
                    {
                        matrix.Values[indexFirst][indexSecond] -= lineMin[indexFirst];
                    }
                }
            }
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    if (matrix.Values[indexFirst][indexSecond] < columnMin[indexSecond] && !currentFrom.Contains(indexFirst)
                        && !currentTo.Contains(indexSecond))
                    {
                        columnMin[indexSecond] = matrix.Values[indexFirst][indexSecond];
                    }
                }
            }
            Console.WriteLine("Matrix after line reducing:");
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    if (matrix.Values[indexFirst][indexSecond] != Int32.MaxValue)
                    {
                        Console.Write("{0,3}", matrix.Values[indexFirst][indexSecond]);
                    }
                    else Console.Write("NAN");
                }
                Console.WriteLine();
            }
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    if (!currentFrom.Contains(indexFirst) && !currentTo.Contains(indexSecond) && matrix.Values[indexFirst][indexSecond] != Int32.MaxValue)
                    {
                        matrix.Values[indexFirst][indexSecond] -= columnMin[indexSecond];
                    }
                }
            }
            for (int index = 0; index < _count; index++)
            {
                if (!currentFrom.Contains(index) && lineMin[index] != Int32.MaxValue)
                {
                    localRecord += lineMin[index];
                }
                if (!currentTo.Contains(index) && columnMin[index] != Int32.MaxValue)
                {
                    localRecord += columnMin[index];
                }
            }
            Console.WriteLine("Matrix after column reducing:");
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    if (matrix.Values[indexFirst][indexSecond] != Int32.MaxValue)
                    {
                        Console.Write("{0,3}", matrix.Values[indexFirst][indexSecond]);
                    }
                    else Console.Write("NAN");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Current record is " + localRecord);
            if (localRecord >= _min)
            {
                Console.WriteLine("Return to previous state cause local record greater, than length of current optimal cycle...");
                return;
            }
            matrix.FindPotential(currentFrom, currentTo);
            Console.WriteLine("Potential matrix:");
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    Console.Write(matrix.Potential[indexFirst][indexSecond] + " ");
                }
                Console.WriteLine();
            }
            int indexFirstMax = -1;
            int indexSecondMax = -1;
            int minPotential = Int32.MinValue;
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    if (matrix.Values[indexFirst][indexSecond] != Int32.MaxValue)
                    {
                        if (matrix.Potential[indexFirst][indexSecond] > minPotential
                            && !currentFrom.Contains(indexFirst) && !currentTo.Contains(indexSecond))
                        {
                            minPotential = matrix.Potential[indexFirst][indexSecond];
                            indexFirstMax = indexFirst;
                            indexSecondMax = indexSecond;
                        }
                    }
                }
            }
            if (indexFirstMax != -1)
            {
                currentFrom.Add(indexFirstMax);
                currentTo.Add(indexSecondMax);
                Matrix matrixToTransfer = (Matrix) matrix.Clone();
                matrixToTransfer.Values[indexFirstMax][indexSecondMax] = Int32.MaxValue;
                matrixToTransfer.Values[indexSecondMax][indexFirstMax] = Int32.MaxValue;
                Console.WriteLine("Including road from city " + (indexFirstMax + 1) + " to city " +
                                  (indexSecondMax + 1) + "...");
                Solve(matrixToTransfer, localRecord, currentFrom, currentTo);
                currentFrom.Remove(indexFirstMax);
                currentTo.Remove(indexSecondMax);
                localRecord += matrix.Potential[indexFirstMax][indexSecondMax];
                matrixToTransfer = (Matrix) matrix.Clone();
                matrixToTransfer.Values[indexFirstMax][indexSecondMax] = Int32.MaxValue;
                Console.WriteLine("Ban road from city " + (indexFirstMax + 1) + " to city " + (indexSecondMax + 1) +
                                  "...");
                Solve(matrixToTransfer, localRecord, currentFrom, currentTo);
            }
            Console.WriteLine("Return to previous state cause watched all ways...");
        }
        static void Main()
        {
            _count = 5;
            int[][] graph = new int[][]
            {
                new[] {Int32.MaxValue, 32, 37, 37, 29},
                new[] {40, Int32.MaxValue, 40, 28, 31},
                new[] {41, 34, Int32.MaxValue, 34, 37},
                new[] {36, 37, 33, Int32.MaxValue, 27},
                new[] {31, 39, 33, 38, Int32.MaxValue}
            };
            for (int indexFirst = 0; indexFirst < _count; indexFirst++)
            {
                for (int indexSecond = 0; indexSecond < _count; indexSecond++)
                {
                    Console.Write(graph[indexFirst][indexSecond] + " ");
                }
                Console.WriteLine();
            }
            int record = 0;
            for (int index = 0; index < _count; index++)
            {
                OptimalFrom.Add(index);
            }
            for (int index = 0; index < _count; index++)
            {
                OptimalTo.Add((index + 1) % _count);
            }
            for (int index = 0; index < _count; index++)
            {
                _min += graph[OptimalFrom[index]][OptimalTo[index]];
            }
            Matrix matrix = new Matrix(graph);
            Console.WriteLine("First cycle contains next roads:");
            for (int index = 0; index < _count; index++)
            {
                Console.WriteLine((OptimalFrom[index] + 1) + " -> " + (OptimalTo[index] + 1));
            }
            Console.WriteLine("First cycle cost is " + _min);
            Solve(matrix, record, new List<int>(), new List<int>());
            Console.WriteLine("Optimal cycle contains next roads:");
            for (int index = 0; index < _count; index++)
            {
                Console.WriteLine((OptimalFrom[index] + 1) + " -> " + (OptimalTo[index] + 1));
            }
            Console.WriteLine("Optimal cycle cost is " + _min);
        }
    }
}