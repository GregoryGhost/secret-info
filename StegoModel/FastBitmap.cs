using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace StegoModel
{
    namespace ImageFilter
    {
        unsafe public class FastBitmap
        {
            private struct PixelData
            {
                public byte blue;
                public byte green;
                public byte red;
                public byte alpha;

                public override string ToString()
                {
                    return String.Format("({0}, {1}, {2}, {3})",
                        alpha, red, green, blue);
                }
            }

            private Bitmap workingBitmap = null;
            private int width = 0;
            private BitmapData bitmapData = null;
            private Byte* pBase = null;

            public FastBitmap(Bitmap inputBitmap)
            {
                workingBitmap = inputBitmap;
            }

            public void LockImage()
            {
                var bounds = new Rectangle(Point.Empty, workingBitmap.Size);

                width = (int)(bounds.Width * sizeof(PixelData));
                if (width % 4 != 0) width = 4 * (width / 4 + 1);

                //Lock Image
                bitmapData = workingBitmap.LockBits(bounds,
                    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                pBase = (Byte*)bitmapData.Scan0.ToPointer();
            }

            private PixelData* pixelData = null;

            public Color GetPixel(int x, int y)
            {
                pixelData = (PixelData*)(pBase + y * width 
                    + x * sizeof(PixelData));
                return Color.FromArgb(pixelData->alpha, pixelData->red,
                    pixelData->green, pixelData->blue);
            }

            public Color GetPixelNext()
            {
                pixelData++;
                return Color.FromArgb(pixelData->alpha, pixelData->red,
                    pixelData->green, pixelData->blue);
            }

            public void SetPixel(int x, int y, Color color)
            {
                PixelData* data = (PixelData*)(pBase + y * width
                    + x * sizeof(PixelData));
                data->alpha = color.A;
                data->red = color.R;
                data->green = color.G;
                data->blue = color.B;
            }

            public void UnlockImage()
            {
                workingBitmap.UnlockBits(bitmapData);
                bitmapData = null;
                pBase = null;
            }
        }
    }
}
