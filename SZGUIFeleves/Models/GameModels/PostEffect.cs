using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SZGUIFeleves.Models
{
    public static class PostEffect
    {
        public static void GetScreenArray(FrameworkElement element, ref byte[] screenArray, Vec2d screenSize)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)screenSize.x, (int)screenSize.y, 96, 96, PixelFormats.Pbgra32);
            screenArray = new byte[bmp.PixelWidth * bmp.PixelHeight * 4];

            bmp.Render(element);

            int stride = bmp.PixelHeight * (bmp.Format.BitsPerPixel / 8);
            stride = 3136;
            bmp.CopyPixels(screenArray, stride, 0);
        }

        public static void ApplyBloom(ref byte[] screenArray, Vec2d windowSize, int intensity)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            byte[] original = screenArray.ToArray();

            // Bright-pass filter and to matrix conversion
            byte[,] matrix = new byte[(int)windowSize.y, (int)windowSize.x * 4];

            int row = 0;
            int column = 0;
            for (int i = 0; i < screenArray.Count(); i+=4)
            {
                int b = screenArray[i];
                int g = screenArray[i + 1];
                int r = screenArray[i + 2];
                int a = screenArray[i + 3];

                double lum = (0.2126 * r + 0.7152 * g + 0.0722 * b)/255;
                if (lum < 0.75)
                {
                    matrix[row, column] = 0;
                    matrix[row, column + 1] = 0;
                    matrix[row, column + 2] = 0;
                    matrix[row, column + 3] = 255;
                }
                else
                {
                    matrix[row, column] = (byte)b;
                    matrix[row, column + 1] = (byte)g;
                    matrix[row, column + 2] = (byte)r;
                    matrix[row, column + 3] = 255;
                }

                if (column == (int)windowSize.x * 4 - 4)
                {
                    column = 0;
                    row++;
                }
                else
                    column += 4;
            }

            // Downscale
            int downscale = 4;
            for (int y = 0; y < matrix.GetLength(0); y+=downscale)
            {
                for (int x = 0; x < matrix.GetLength(1); x += (4*downscale))
                {
                    for (int y2 = 0; y2 < downscale; y2++)
                    {
                        for (int x2 = 0; x2 < downscale*4; x2+=4)
                        {
                            if (y + y2 < matrix.GetLength(0) && x + x2 < matrix.GetLength(1))
                            {
                                matrix[y + y2, x + x2] = matrix[y, x];
                                matrix[y + y2, x + x2 + 1] = matrix[y, x + 1];
                                matrix[y + y2, x + x2 + 2] = matrix[y, x + 2];
                                matrix[y + y2, x + x2 + 3] = matrix[y, x + 3];
                            }
                        }
                    }
                }
            }

            // Gaussian Blurs
            List<int> kernelSizes = new List<int>() { 51 };
            List<byte[,]> blurs = new List<byte[,]>();
            foreach(int kernelSize in kernelSizes)
            {
                byte[,] blurredMatrix = matrix.Clone() as byte[,];

                //double[,] kernel = GetGaussianKernel(kernelSize, 1);
                double[,] kernel = GetMeanKernel(kernelSize);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x += 4)
                    {
                        byte[] c = ApplyKernel(ref blurredMatrix, x, y, kernel);
                        matrix[y, x] = c[0];
                        matrix[y, x + 1] = c[1];
                        matrix[y, x + 2] = c[2];
                        matrix[y, x + 3] = c[3];
                    }
                }
                //WriteMatrixToFile("blurred.txt", ref blurredMatrix);
                //WriteMatrixToFile("original.txt", ref matrix);
                blurs.Add(blurredMatrix);
            }
            ;


            //List<int> kernelSizes = new List<int>() { 9,27,71,101,201 };
            //List<byte[,]> blurs = new List<byte[,]>();
            //foreach(int kernelSize in kernelSizes)
            //{
            //    byte[,] blurredMatrix = matrix.Clone() as byte[,];

            //    for (int y = kernelSize / 2; y < matrix.GetLength(0); y += kernelSize)
            //    {
            //        for (int x = kernelSize / 2 * 4; x < matrix.GetLength(1); x += kernelSize*4)
            //        {
            //            ApplyKernel(ref blurredMatrix, x, y, GetSampleKernel(kernelSize, 1));
            //        }
            //    }
            //    //WriteMatrixToFile("blurred.txt", ref blurredMatrix);
            //    //WriteMatrixToFile("original.txt", ref matrix);
            //    blurs.Add(blurredMatrix);
            //}

            // Flatten matrix
            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    int value = matrix[y, x];
                    //int value = 0;
                    for (int m = 0; m < blurs.Count(); m++)
                    {
                        value += blurs[m][y, x];
                    }
                    value /= (blurs.Count() + 1);
                    //if (value > 255)
                    //    value = 255;

                    screenArray[y * matrix.GetLength(1) + x] = (byte)value;
                }
            }

            sw.Stop();
            ;
        }

        public static byte[] ApplyKernel(ref byte[,] matrix, int xMiddle, int yMiddle, double[,] kernel)
        {
            Vec2d matrixSizes = new Vec2d(matrix.GetLength(1), matrix.GetLength(0));

            int kernelMiddleX = kernel.GetLength(1) / 2;
            int kernelMiddleY = kernel.GetLength(0) / 2;

            double b = 0;
            double g = 0;
            double r = 0;
            int count = 0;

            for (int y = -kernelMiddleY; y <= kernelMiddleY; y++)
            {
                for (int x = -kernelMiddleX*4; x <= kernelMiddleX*4; x += 4)
                {
                    if(yMiddle + y < matrixSizes.y && yMiddle + y >= 0 && xMiddle + x < matrixSizes.x && xMiddle + x >= 0)
                    { 
                        b += matrix[yMiddle + y, xMiddle + x] * kernel[kernelMiddleY + y, kernelMiddleX + (x / 4)];
                        g += matrix[yMiddle + y, xMiddle + x + 1] * kernel[kernelMiddleY + y, kernelMiddleX + (x / 4)];
                        r += matrix[yMiddle + y, xMiddle + x + 2] * kernel[kernelMiddleY + y, kernelMiddleX + (x / 4)];
                        count++;
                    }
                }
            }

            b /= count;
            g /= count;
            r /= count;
            return new byte[4] { (byte)b, (byte)g, (byte)r, matrix[yMiddle, xMiddle + 3] };
        }

        private static double[,] GetGaussianKernel(int lenght, double weight)
        {
            double[,] kernel = new double[lenght, lenght];
            double kernelSum = 0;
            int foff = (lenght - 1) / 2;
            double distance = 0;
            double constant = 1d / (2 * Math.PI * weight * weight);
            for (int y = -foff; y <= foff; y++)
            {
                for (int x = -foff; x <= foff; x++)
                {
                    distance = ((y * y) + (x * x)) / (2 * weight * weight);
                    kernel[y + foff, x + foff] = constant * Math.Exp(-distance);
                    kernelSum += kernel[y + foff, x + foff];
                }
            }
            for (int y = 0; y < lenght; y++)
            {
                for (int x = 0; x < lenght; x++)
                {
                    kernel[y, x] = kernel[y, x] * 1d / kernelSum;
                }
            }
            return kernel;
        }

        private static double[,] GetMeanKernel(int length)
        {
            double[,] kernel = new double[length, length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    kernel[i, j] = 1;
                }
            }
            return kernel;
        }

        private static void WriteMatrixToFile(string filename, ref byte[,] matrix)
        {
            List<string> lines = new List<string>();
            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                string line = "";
                for (int x = 0; x < matrix.GetLength(1); x+=4)
                {
                    string c = "(" + matrix[y, x + 2] + "," + matrix[y, x + 1] + "," + matrix[y, x] + "," + matrix[y, x+3] + ")";
                    line += c;
                }
                lines.Add(line);
            }
            File.WriteAllLines(filename, lines);
        }
    }
}
