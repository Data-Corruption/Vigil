using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Vigil.Views
{
  public class ScrollingBitmap
  {
    public WriteableBitmap Bitmap { get; private set; }
    public System.Windows.Media.Color PixelColor { get; set; } = Colors.Black;
    public System.Windows.Media.Color BackgroundColor { get; set; } = Colors.Transparent;
    public int Width { get; private set; }
    public int Height { get; private set; }

    public ScrollingBitmap(int width, int height, int dpiX, int dpiY)
    {
      Bitmap = new WriteableBitmap(width, height, dpiX, dpiY, PixelFormats.Bgra32, null);
      Width = width;
      Height = height;
      Clear();
    }

    public void Push(double lineHeight)
    {
      ScrollLeftOnePixel(Bitmap, (int)(Height * lineHeight));
    }

    // shift the bitmap to the left by one pixel, add a new column on the right. newLineHeight is the height of the new column in pixels, everything above it is background color
    private void ScrollLeftOnePixel(WriteableBitmap bitmap, int newLineHeight)
    {
      bitmap.Lock();
      try
      {
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;
        int stride = bitmap.BackBufferStride;
        int size = height * stride;

        // Copy out current bitmap pixels
        byte[] pixels = new byte[size];
        System.Runtime.InteropServices.Marshal.Copy(bitmap.BackBuffer, pixels, 0, size);

        for (int row = 0; row < height; row++)
        {
          int rowStart = row * stride;
          // Shift row's bytes left by one pixel (4 bytes per pixel)
          Buffer.BlockCopy(pixels, rowStart + 4, pixels, rowStart, (width - 1) * 4);

          // Fill the rightmost pixel
          int last = rowStart + (width - 1) * 4;
          bool isLinePixel = (row >= height - newLineHeight);
          var color = isLinePixel ? PixelColor : BackgroundColor;
          pixels[last] = color.B;
          pixels[last + 1] = color.G;
          pixels[last + 2] = color.R;
          pixels[last + 3] = color.A;
        }

        // Copy back and mark dirty
        System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bitmap.BackBuffer, size);
        bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
      }
      finally
      {
        bitmap.Unlock();
      }
    }

    // Sets all pixels to the background color
    public void Clear()
    {
      // Clear the bitmap
      Bitmap.Lock();
      int stride = Bitmap.BackBufferStride;
      int size = Bitmap.PixelHeight * stride;
      byte[] pixels = new byte[size];
      for (int i = 0; i < size; i += 4)
      {
        pixels[i] = BackgroundColor.B;
        pixels[i + 1] = BackgroundColor.G;
        pixels[i + 2] = BackgroundColor.R;
        pixels[i + 3] = BackgroundColor.A;
      }
      System.Runtime.InteropServices.Marshal.Copy(pixels, 0, Bitmap.BackBuffer, size);
      Bitmap.AddDirtyRect(new Int32Rect(0, 0, Bitmap.PixelWidth, Bitmap.PixelHeight));
      Bitmap.Unlock();
    }
  }
}