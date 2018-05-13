using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace StegoModel
{
    /// <summary>
    /// Помогает провести визуальную атаку
    ///     на стегоконтейнеры упакованные алгоритмом LSB.
    /// </summary>
    public class VisualAttack
    {
        /// <summary>
        /// Высчитывает разницу между пустым и стего- контейнерами.
        /// </summary>
        /// <param name="bmp0">Пустой контейнер.</param>
        /// <param name="bmp1">Стегоконтейнер.</param>
        /// <param name="restore">Отрисовывать ли на фоне 
        ///     исходное изображение(пустой стегоконтейнер).</param>
        /// <returns>Изображение-разница.</returns>
        public Bitmap Difference(Bitmap bmp0, Bitmap bmp1, bool restore)
        {
            //при условии эффективного формата пикселей 32bpp
            int Bpp = 4;
            var bmpData0 = bmp0.LockBits(
                            new Rectangle(0, 0, bmp0.Width, bmp0.Height),
                            ImageLockMode.ReadWrite, bmp0.PixelFormat);
            var bmpData1 = bmp1.LockBits(
                            new Rectangle(0, 0, bmp1.Width, bmp1.Height),
                            ImageLockMode.ReadOnly, bmp1.PixelFormat);

            int len = bmpData0.Height * bmpData0.Stride;
            var data0 = new byte[len];
            var data1 = new byte[len];
            Marshal.Copy(bmpData0.Scan0, data0, 0, len);
            Marshal.Copy(bmpData1.Scan0, data1, 0, len);

            //сравнение(разница) контейнеров по каналам RGBA

            for (int i = 0; i < len; i += Bpp)
            {
                if (restore)
                {
                    var toBeRestored = (data1[i] != 2 
                        && data1[i + 1] != 3 && data1[i + 2] != 7 
                            && data1[i + 2] != 42);
                    if (toBeRestored)
                    {
                        //Синий канал(Blue)
                        data0[i] = data1[i];
                        //Зеленый канал(Green)
                        data0[i + 1] = data1[i + 1];
                        //Красный канал(Red)
                        data0[i + 2] = data1[i + 2];
                        //Прозрачность изображения(Alpha)
                        data0[i + 3] = data1[i + 3];
                    }
                }
                else
                {
                    var changed = ((data0[i] != data1[i]) ||
                                    (data0[i + 1] != data1[i + 1]) 
                                        || (data0[i + 2] != data1[i + 2]));
                    //Специальные маркеры
                    data0[i] = changed ? data1[i] : (byte)2; 
                    data0[i + 1] = changed ? data1[i + 1] : (byte)3;
                    data0[i + 2] = changed ? data1[i + 2] : (byte)7;
                    data0[i + 3] = changed ? (byte)255 : (byte)42;
                }
            }

            Marshal.Copy(data0, 0, bmpData0.Scan0, len);
            bmp0.UnlockBits(bmpData0);
            bmp1.UnlockBits(bmpData1);

            return bmp0;
        }
    }
}
