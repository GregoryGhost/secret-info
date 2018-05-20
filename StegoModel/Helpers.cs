using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegoModel
{
    /// <summary>
    /// Методы расширения по конвертации битов и байтов между собой.
    /// </summary>
    public static class BitsExtension
    {
        /// <summary>
        /// Представление байта как восемь бит.
        /// </summary>
        /// <param name="src">Исходный байт.</param>
        /// <returns>Возвращает массив бит.</returns>
        public static BitArray ToBits(this byte src)
        {
            const int sizeByte = 8;
            BitArray bitArray = new BitArray(sizeByte);
            bool st = false;

            for (int i = 0; i < 8; i++)
            {
                st = (src >> i & 1) == 1 ? true : false;
                bitArray[i] = st;
            }

            return bitArray;
        }

        /// <summary>
        /// Представление 8 бит как один байт.
        /// </summary>
        /// <param name="scr">Исходный массив бит.</param>
        /// <returns>Значение байта.</returns>
        public static byte ToByte(this BitArray scr)
        {
            byte num = 0;

            for (int i = 0; i < scr.Count; i++)
            {
                if (scr[i] == true)
                    num += (byte)Math.Pow(2, i);
            }

            return num;
        }
    }


    /// <summary>
    /// Помощник по работе с вводом выводом для текста и изображения.
    /// </summary>
    public class HelperIO : IHelperIO
    {
        /// <summary>
        /// Прочитать изображение по указанному пути.
        /// </summary>
        /// <param name="path">Путь до изображения.</param>
        /// <returns>Прочитанное изображение.</returns>
        public Bitmap ReadImage(string path)
        {
            var bmp = (Bitmap)Image.FromFile(path);

            return bmp;
        }

        /// <summary>
        /// Прочитать текст по указанному пути.
        /// </summary>
        /// <param name="path">Путь до текста.</param>
        /// <returns>Текст в байтах.</returns>
        public List<byte> ReadText(string path)
        {
            var file = new FileStream(path, FileMode.Open);
            var reader = new BinaryReader(file, Encoding.ASCII);
            var text = new List<byte>();

            while (reader.PeekChar() != -1)
            {
                text.Add(reader.ReadByte());
            }

            reader.Close();
            file.Close();

            return text;
        }

        /// <summary>
        /// Записать изображение по указанному пути.
        /// </summary>
        /// <param name="path">Расположение изображения.</param>
        /// <param name="image">Записываемое изображение.</param>
        public void WriteImage(string path, Bitmap image)
        {
            image.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        /// <summary>
        /// Записать текст по указанному пути.
        /// </summary>
        /// <param name="path">Расположение текста.</param>
        /// <param name="text">Записываемый текст в байтах.</param>
        public void WriteText(string path, List<byte> text)
        {
            var t = Encoding.GetEncoding(1251).GetString(text.ToArray());

            using (var file = new FileStream(path, FileMode.Create))
            {
                using (var writer = new StreamWriter(
                    file, Encoding.Default))
                {
                    writer.Write(t);
                }
            }
        }
    }


    public class ExtractorRGB
    {
        private int[,] _redChannel;
        private int[,] _greenChannel;
        private int[,] _blueChannel;

        public void ExtractRGB(Bitmap image)
        {
            var w = image.Width;
            var h = image.Height;
            _redChannel = new int[w, h];
            _greenChannel = new int[w, h];
            _blueChannel = new int[w, h];
            Color pixel;

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    pixel = image.GetPixel(i, j);
                    _redChannel[i, j] = pixel.R;
                    _greenChannel[i, j] = pixel.G;
                    _blueChannel[i, j] = pixel.B;
                }
            }
        }

        public void ChangeBlueChannel(Bitmap image, int[,] blue)
        {
            var w = image.Width;
            var h = image.Height;
            Color pixel;
            for (int i = 2; i < w; i++)
            {
                for (int j = 2; j < h; j++)
                {
                    pixel = image.GetPixel(i, j);
                    var c = Color.FromArgb(pixel.R, pixel.G, blue[i, j]);
                    image.SetPixel(i, j, c);
                }
            }
        }

        public int[,] getR()
        {
            return _redChannel;
        }

        public int[,] getG()
        {
            return _greenChannel;
        }

        public int[,] getB()
        {
            return _blueChannel;
        }
    }
}
