using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ACW_Neuron
{
    class Program
    {
        static Random random = new Random();
        static void Main(string[] args)
        {  
            using (var package = new ExcelPackage(new FileInfo("track.xlsx")))
            {
                //DOWNLOAD XLSX FILE
                int reader_pos = 1;
                ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
                var totalRows = workSheet.Dimension.End.Row;
                var totalColumns = workSheet.Dimension.End.Column;
                string[,] data = new string[totalRows / 3,4];
                for (int i = 0; i < totalRows / 3; ++i)
                {
                    for (int j = 0; j < 4; ++j) //read 4 values in, if its the end of input dont read more 
                    {
                        if (j == 3) //sets bias
                        {
                            data[i, 3] = "1";
                            break;
                        }
                        data[i, j] = workSheet.Cells[reader_pos, 1].Value.ToString(); //read from 1 column,
                        ++reader_pos;
                    }
                }
                PerceptronTrainer(data, totalRows);
            }
            Console.ReadLine();
        }

        static void PerceptronTrainer(string[,] data, int totalRows)
        {
            int target = 1;
            double output = 0;
            double error_squared = 1;
            double learning_rate = 1;
            double delta = 0;
            double[,] weights = new double[totalRows / 3,4];
            for(int i = 0; i < totalRows / 3; ++i) //initialize weights randomly 
            {
                for(int j = 0; j < 4; ++j)
                {
                    weights[i, j] = GetRandomNumber();
                }
            }
            while(error_squared != 0)
            {
                for(int i = 0; i < data.Length / 3; ++i) //for each sample, 3 + bias
                {
                    if (i * 4 < data.Length)
                    {
                        output = Perceptron(i, data, weights);
                        for (int j = 0; j < 4; ++j) //for every input in data, so 4
                        {
                            delta = target - output;
                            weights[i, j] += learning_rate * delta * double.Parse(data[i, j]);
                            error_squared = ErrorSquaredFunction(delta);
                            Console.WriteLine("error: " + error_squared);
                        }
                    }
                } 
            }
           
            //GET WEIGHTS
            //Perceptron Training Algorithm
            //initialize weights randomly
            //While error_squared function != 0
            //  For each sample
            //      Simulate perceptron
            //          For all inputs i
            //              Delta=Target-Output
            //              Weights[i] += learning_rate*Delta*input[i]
            //              Error = Delta
            //          End for
            //  End for
            //end while    
           
        }

        static double ErrorSquaredFunction(double error)
        {
            error = Math.Pow(error, 2);
            return error;
        }

        static double Perceptron(int position, string[,] data, double[,] weights) //ADD BIAS THRESHOLD STUFF
        {
            double net_sum = 0;
            double output = 0;
            for (int j = 0; j < 4; ++j)
            {
                net_sum = net_sum + (double.Parse(data[position, j]) * weights[position, j]);
            }
            output = Activation(net_sum);
            return output;
            //PERCEPTRON
            //Computing output of perceptron
            //net_sum=0
            //for all i
            //  net_sum=net_sum+input*weight[1]
            //end for
            //output = activation(net_sum)
        }

        static double Activation(double net_sum) //REPLACE WITH SIGMOID FUNCTION
        {
            //          1
            // 1 = -----------
            //       1 + e^x
            // e is Euler's Number
            // x is input?
            return 1 / (1 + Math.Exp(-net_sum));
        }

        // from https://gist.github.com/MachineCharmer/941949 accessed 12/03/18
        static double GetRandomNumber()
        {
            double maximum = 0.5f;
            double minimum = -0.5f;
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
