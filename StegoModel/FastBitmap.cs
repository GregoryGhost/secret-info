using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace StegoModel
{
    namespace ImageFilter
    {
        /// <summary>
        /// Быстрая реализация работы с изображениями.
        /// </summary>
        unsafe public class FastBitmap
        {
            /// <summary>
            /// Внутреннее представление пикселя.
            /// </summary>
            private struct PixelData
            {
                public byte blue;
                public byte green;
                public byte red;
                public byte alpha;

                /// <summary>
                /// Преобразование значений каналов RGB пикселя в строку.
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    return String.Format("({0}, {1}, {2}, {3})",
                        alpha, red, green, blue);
                }
            }

            /// <summary>
            /// Текущее рабочее изображение.
            /// </summary>
            private Bitmap workingBitmap = null;

            /// <summary>
            /// Ширина рабочего изображения.
            /// </summary>
            private int width = 0;

            /// <summary>
            /// Пиксели изображения.
            /// </summary>
            private BitmapData bitmapData = null;

            /// <summary>
            /// Начальный пиксель изображения.
            /// </summary>
            private Byte* pBase = null;

            /// <summary>
            /// Инициализация экземпляра.
            /// </summary>
            /// <param name="inputBitmap">Исходное изображение.</param>
            public FastBitmap(Bitmap inputBitmap)
            {
                workingBitmap = inputBitmap;
            }

            /// <summary>
            /// Заблокировать доступ к изображению.
            /// </summary>
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

            /// <summary>
            /// Указатель на текущий пиксель изображения.
            /// </summary>
            private PixelData* pixelData = null;

            /// <summary>
            /// Получить цвет пикселя в точке (x, y).
            /// </summary>
            /// <param name="x">Координата пикселя по оси OX.</param>
            /// <param name="y">Координата пикселя по оси OY.</param>
            /// <returns>Возвращает цвет пикселя в точке (x, y).</returns>
            public Color GetPixel(int x, int y)
            {
                pixelData = (PixelData*)(pBase + y * width 
                    + x * sizeof(PixelData));
                return Color.FromArgb(pixelData->alpha, pixelData->red,
                    pixelData->green, pixelData->blue);
            }

            /// <summary>
            /// Получить следующий пиксель изображения.
            /// </summary>
            /// <returns>Возвращает следующий пиксель изображения.</returns>
            public Color GetPixelNext()
            {
                pixelData++;
                return Color.FromArgb(pixelData->alpha, pixelData->red,
                    pixelData->green, pixelData->blue);
            }

            /// <summary>
            /// Устанавливает цвет для пикселя в точке (x, y).
            /// </summary>
            /// <param name="x">Координата по OX пикселя.</param>
            /// <param name="y">Координата по OY пикселя.</param>
            /// <param name="color">Новый цвет пикселя.</param>
            public void SetPixel(int x, int y, Color color)
            {
                PixelData* data = (PixelData*)(pBase + y * width
                    + x * sizeof(PixelData));
                data->alpha = color.A;
                data->red = color.R;
                data->green = color.G;
                data->blue = color.B;
            }

            /// <summary>
            /// Разблокировать доступ к изображению.
            /// </summary>
            public void UnlockImage()
            {
                workingBitmap.UnlockBits(bitmapData);
                bitmapData = null;
                pBase = null;
            }
        }
    }
}
