namespace PKWat.AgentSimulation.Drawing;

using System;
using System.Windows.Media.Imaging;

public static class BitmapExtensions
{
    public static BitmapSource ConvertToBitmapSource(this System.Drawing.Bitmap bitmap) 
        => System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               bitmap.GetHbitmap(),
               IntPtr.Zero,
               System.Windows.Int32Rect.Empty,
               BitmapSizeOptions.FromEmptyOptions());
}
