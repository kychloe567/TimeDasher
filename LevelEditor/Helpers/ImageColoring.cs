using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using SZGUIFeleves.Models;

namespace LevelEditor.Helpers
{
    public static class ImageColoring
    {
        public enum ColorFilters
        {
            Red,Green,Blue
        }

        public static BitmapImage SetColor(BitmapImage bi, ColorFilters cf)
        {
            Bitmap b = BitmapImage2Bitmap(bi);
            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    var oldColor = b.GetPixel(x, y);

                    int R = oldColor.R;
                    int G = oldColor.G;
                    int B = oldColor.B;

                    if(cf == ColorFilters.Red)
                    {
                        G -= 255;
                        G = Math.Max(G, 0);
                        G = Math.Min(255, G);
                        B -= 255;
                        B = Math.Max(B, 0);
                        B = Math.Min(255, B);
                    }
                    else if(cf == ColorFilters.Green)
                    {
                        R -= 255;
                        R = Math.Max(R, 0);
                        R = Math.Min(255, R);
                        B -= 255;
                        B = Math.Max(B, 0);
                        B = Math.Min(255, B);
                    }
                    else if(cf == ColorFilters.Blue)
                    {
                        G -= 255;
                        G = Math.Max(G, 0);
                        G = Math.Min(255, G);
                        R -= 255;
                        R = Math.Max(R, 0);
                        R = Math.Min(255, R);
                    }

                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(R, G, B);
                    b.SetPixel(x, y, newColor);
                }
            }

            //return Bitmap2BitmapImage(b);
            return ToBitmapImage(b);
        }

        private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        //[System.Runtime.InteropServices.DllImport("gdi32.dll")]
        //private static extern bool DeleteObject(IntPtr hObject);

        //private static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        //{
        //    IntPtr hBitmap = bitmap.GetHbitmap();
        //    BitmapImage retval;

        //    try
        //    {
        //        retval = (BitmapImage)Imaging.CreateBitmapSourceFromHBitmap(
        //                     hBitmap,
        //                     IntPtr.Zero,
        //                     Int32Rect.Empty,
        //                     BitmapSizeOptions.FromEmptyOptions());
        //    }
        //    finally
        //    {
        //        DeleteObject(hBitmap);
        //    }

        //    return retval;
        //}

        private static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}
