namespace PKWat.AgentSimulation.Drawing;

using System;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;

public static class BitmapExtensions
{
    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    public static BitmapSource ConvertToBitmapSource(this System.Drawing.Bitmap bitmap)
    {
        IntPtr hBitmap = bitmap.GetHbitmap();

        try
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        finally
        {
            DeleteObject(hBitmap);
        }

        //var hbitmap = bitmap.GetHbitmap();

        //return (
        //    hbitmap, 
        //    Imaging.CreateBitmapSourceFromHBitmap(
        //        hbitmap,
        //        IntPtr.Zero,
        //        System.Windows.Int32Rect.Empty,
        //        BitmapSizeOptions.FromEmptyOptions()));
    }
}
