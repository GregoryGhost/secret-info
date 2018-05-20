using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegoModel
{
    /// <summary>
    /// Упаковщик/распаковщик стегоконтейнеров, использующий 
    ///     алгоритм Куттера-Джордана-Боссена(КДБ) - метод "креста".
    /// </summary>
    public class CrossBrightnessBlueChannel : IPacker, IUnpacker
    {
        private readonly double _lambda = 0.1;

        private readonly int _sigma = 2;

        private ExtractorRGB _rgbExtractor;

        private static List<Point> _key = null;

        public CrossBrightnessBlueChannel()
        {
            _rgbExtractor = new ExtractorRGB();
        }

        /// <summary>
        /// Упаковывать скрываемый текст в пустой стегоконтейнер
        ///     по алгоритму Куттера-Джордана-Боссена.
        /// </summary>
        /// <param name="sourceImage">Пустой стегоконтейнер
        ///     (исходное изображение).</param>
        /// <param name="text">Текст, который нужно скрыть.</param>
        /// <returns>Возвращает стегоконтейнер,
        ///     который содержит скрытый текст.</returns>
        /// <exception cref="ArgumentException">
        /// Возникает, когда размер скрываемого текста 
        ///     больше размера пустого контейнера,
        ///     а также, когда пустой контейнер
        ///     уже является стегоконтейнером.</exception>
        public Bitmap Pack(Bitmap sourceImage, List<byte> text)
        {
            const int codepage = 1251;
            var msg = Encoding.GetEncoding(
                codepage).GetString(text.ToArray());
            var packedImg = sourceImage.Clone()
                as Bitmap;

            _key = HideMessage(msg, packedImg);

            return packedImg;
        }

        /// <summary>
        /// Распаковать скрытый текст из стегоконтейнера
        ///     по алгоритму Куттера-Джордана-Боссена.
        /// </summary>
        /// <param name="stegoImage">Стегоконтейнер 
        ///     (изображение со скрытым текстом).</param>
        /// <returns>Возвращает скрытый текст 
        ///     из стегоконтейнера.</returns>
        /// <exception cref="ArgumentException">
        /// Возникает, когда размер скрываемого текста 
        ///     больше размера пустого контейнера.</exception>
        public List<byte> Unpack(Bitmap stegoImage)
        {
            var msg = ExtractMessage(stegoImage, _key);
            var unpackMsg = Encoding.Default.GetBytes(msg).ToList();

            return unpackMsg;
        }

        private List<Point> HideMessage(string text, Bitmap image)
        {

            var width = image.Width;
            var height = image.Height;
            //проверяем, поместиться ли исходный текст
            //  в пустом контейнере
            if (text.Length * 8 > width * height)
            {
                var msg = $"Размер скрываемого текста" +
                    $" больше размера пустого контейнера.";
                throw new ArgumentException(msg, nameof(text));
            }
            _rgbExtractor.ExtractRGB(image);
            //каналы RGB изображения
            var r = _rgbExtractor.getB();
            var g = _rgbExtractor.getG();
            var b = _rgbExtractor.getB();
            var bStar = new int[width, height];

            var y = CalculateBrightness(r, g, b, width, height);
            //инициализация нового массива синего канала
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    bStar[i, j] = b[i, j];
                }
            }

            var bitsMsg = MessageToBitArray(text);

            var coords = new List<Point>();

            //выбор новой точки для сообщения
            for (int i = 0; i < bitsMsg.Length; i++)
            {
                var p = RandomPoint(coords, width, height);
                bStar[p.X, p.Y] = ChangeBlueValue(
                    b[p.X, p.Y], y[p.X, p.Y], bitsMsg[i]);
                coords.Add(p);
            }

            //запись нового синего цвета
            _rgbExtractor.ChangeBlueChannel(image, bStar);

            return coords;
        }

        private Point RandomPoint(List<Point> currentPoints,
            int width, int height)
        {
            var f = true;
            var random = new Random();
            int x = 0, y = 0;
            while (f)
            {
                x = Math.Abs(random.Next()) % (width - 4) + _sigma;
                y = Math.Abs(random.Next()) % (height - 4) + _sigma;
                f = CheckExisting(currentPoints, new Point(x, y));
            }
            var p = new Point(x, y);

            return p;
        }

        private bool CheckExisting(List<Point> points, Point point)
        {
            var f = false;
            for (int i = 0; !f && i < points.Count; i++)
            {
                if (points[i].X == point.X
                    && points[i].Y == point.Y)
                {
                    f = true;
                }
            }
            return f;
        }

        public string ExtractMessage(Bitmap image, List<Point> coords)
        {

            _rgbExtractor.ExtractRGB(image);
            var blues = _rgbExtractor.getB();
            var builder = new StringBuilder
            {
                Length = 0
            };
            var s = new StringBuilder();

            int count = 0;
            int bit;

            for (int i = 0; i < coords.Count; i++)
            {
                var p = coords[i];
                bit = RetrieveBit(blues, p.X, p.Y);
                s.Append(bit);
                count++;
                if (count == 8)
                {
                    int character = Convert.ToInt32(s.ToString(), 2);
                    builder.Append((char)character);
                    count = 0;
                    s.Length = 0;
                }
            }
            return builder.ToString();
        }

        private int RetrieveBit(int[,] b, int x, int y)
        {
            double value = 0;
            //при _sigma = 2 
            value = b[x - 2, y] + b[x - 1, y] + b[x + 1, y] + b[x + 2, y]
                    + b[x, y - 2] + b[x, y - 1] + b[x, y + 1] + b[x, y + 2];

            value = value / (4 * _sigma);
            var delta = b[x, y] - value;

            if (delta == 0)
            {
                if (b[x, y] == 0)
                {
                    delta = -0.5;
                }
                if (b[x, y] == 255)
                {
                    delta = 0.5;
                }
            }
            if (delta > 0)
            {
                return 1;
            }
            return 0;
        }

        private string MessageToBitArray(string msg)
        {
            var bytes = Encoding.Default.GetBytes(msg);
            var length = msg.Length;
            var result = String.Empty;

            for(var i = 0; i < length; i++)
            {
                var str = Convert.ToString(msg[i], 2);
                while (str.Length < 8)
                {
                    string str1 = "0" + str;
                    str = str1;
                }
                result += str;
            }

            return result;
        }

        private int ChangeBlueValue(int bxy, double yxy, char bit)
        {
            int result;
            if (bit == '1')
            {
                result = (int)(bxy + _lambda * yxy);
                if (result > 255)
                {
                    result = 255;
                }
            }
            else
            {
                //m =0
                result = (int)(bxy - _lambda * yxy);
                if (result < 0)
                {
                    result = 0;
                }
            }
            return result;
        }

        private double[,] CalculateBrightness(int[,] r,
            int[,] g, int[,] b, int width, int height)
        {
            var y = new double[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    y[i, j] = (int)(0.298 * r[i, j]
                        + 0.586 * g[i, j] + 0.114 * b[i, j]);
                }
            }
            return y;
        }
    }
}
