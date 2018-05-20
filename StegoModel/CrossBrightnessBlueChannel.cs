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
        private readonly double _lambda = 0.5;

        private readonly int _sigma = 3;

        private ExtractorRGB _rgbExtractor;

        private const int _codepage = 1251;

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
            var msg = Encoding.Default.GetString(text.ToArray());
            //var packedImg = sourceImage.Clone()
            //    as Bitmap;
            var packedImg = sourceImage;

            packedImg = HideMessage(msg, packedImg);

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
            var container = stegoImage;//stegoImage.Clone() as Bitmap;
            var msg = ExtractMessage(container);
            var unpackMsg = Encoding.Default
                .GetBytes(msg).ToList();

            return unpackMsg;
        }

        private Bitmap HideMessage(string text, Bitmap image)
        {
            var w = image.Width;
            var h = image.Height;
            //проверяем, поместиться ли исходный текст
            //  в пустом контейнере
            if (text.Length * 8 > w * h)
            {
                var msg = $"Размер скрываемого текста" +
                    $" больше размера пустого контейнера.";
                throw new ArgumentException(msg, nameof(text));
            }
            _rgbExtractor.ExtractRGB(image);
            //каналы RGB изображения
            var r = _rgbExtractor.GetR();
            var g = _rgbExtractor.GetG();
            var b = _rgbExtractor.GetB();
            var bStar = new int[w, h];

            //var y = CalculateBrightness(r, g, b, w, h);
            //инициализация нового массива синего канала
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    bStar[i, j] = b[i, j];
                }
            }

            var bitsMsg = MessageToBitArray(text);
            var chars = bitsMsg.ToCharArray();
            var pixels = new int[chars.Length];

            for (int j = _sigma; j < h - h % 7; j += 7)
            {
                for (int i = _sigma; i < w - w % 7; i += 7)
                {
                    for (int t = 0; t < chars.Length; t++)
                    {
                        int br = Convert.ToInt32(
                            (0.299 * r[i, j] + 0.587 * g[i, j]
                                + 0.114 * b[i, j]) * _lambda);
                        var ch = chars[t];
                        int sing = (2 * Convert.ToInt32(
                            Convert.ToString(ch)) - 1);

                        pixels[t] = b[i, j] + sing * br;
                        if (pixels[t] < 0)
                            pixels[t] = 0;
                        if (pixels[t] > 255)
                            pixels[t] = 255;
                    }
                }
            }

            var k = 0;
            for (int j = _sigma; j < h - h % 7; j += 7)
            {
                for (int i = _sigma; i < w - w % 7; i += 7)
                {
                    if (h >= chars.Length)
                        break;
                    bStar[i, j] = pixels[k];
                    k++;
                }
            }

            _rgbExtractor.ChangeBlueChannel(image, bStar);

            return image;
        }

        public string ExtractMessage(Bitmap image)
        {

            _rgbExtractor.ExtractRGB(image);
            var r = _rgbExtractor.GetR();
            var g = _rgbExtractor.GetG();
            var b = _rgbExtractor.GetB();

            var w = image.Width;
            var h = image.Height;
            var bStar = new int[w, h];
            var text = "";

            for (int j = _sigma; j < h - h % 7; j += 7)
            {
                for (int i = _sigma; i < w - w % 7; i += 7)
                { //распаковка при _sigma = 3
                    bStar[i, j] = (b[i - 1, j] + b[i - 2, j]
                        + b[i - 3, j] + b[i, j - 1]
                        + b[i, j - 2] + b[i, j - 3]
                        + b[i + 1, j] + b[i + 2, j]
                        + b[i + 3, j] + b[i, j + 1]
                        + b[i, j + 2] + b[i, j + 3]) / (4 * _sigma);

                    if (b[i, j] > bStar[i, j])
                        text += "1";
                    if (b[i, j] < bStar[i, j])
                        text += "0";
                }
            }

            //перевод в строку из битовой строки
            var bt = new byte[1];
            //var info = new string[text.Length / 8];
            var info = "";
            for (int i = 0; i < text.Length / 8; i++)
            {
                var str = "";
                for (int j = 0; j < 8; j++)
                {
                    str += Convert.ToString(text[i * 8 + j]);
                }
                bt[0] = Convert.ToByte(str, 2);
                info += Encoding.Default.GetString(bt);
            }

            return info;
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
                    var str1 = "0" + str;
                    str = str1;
                }
                result += str;
            }

            return result;
        }

        private double[,] CalculateBrightness(int[,] r,
            int[,] g, int[,] b, int width, int height)
        {
            var y = new double[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    y[i, j] = (int)(0.298 * r[i, j]
                        + 0.586 * g[i, j] + 0.114 * b[i, j]);
                }
            }
            return y;
        }
    }
}
