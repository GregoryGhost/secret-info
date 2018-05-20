using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace StegoModel
{
    /// <summary>
    /// Упаковщик/распаковщик стегоконтейнеров, использующий 
    ///     алгоритм Куттера-Джордана-Боссена(КДБ) - метод "креста".
    /// </summary>
    public class CrossBrightnessBlueChannel : IPacker, IUnpacker
    {
        /// <summary>
        /// Коэффициент, задающий энергию встраиваемого бита данных.
        /// </summary>
        private readonly double _lambda = 0.5;

        /// /// <summary>
        /// Размер области(где находиться "крест"), 
        ///     по которой будет прогнозироваться яркость.
        /// </summary>
        private readonly int _sigma = 3;

        /// <summary>
        /// Помощник по работе с RGB-каналами стегоконтейнера.
        /// </summary>
        private ExtractorRGB _rgbExtractor;

        /// <summary>
        /// Инициализация необходимых для работы параметров.
        /// </summary>
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
        ///     больше размера пустого контейнера</exception>
        public Bitmap Pack(Bitmap sourceImage, List<byte> text)
        {
            var msg = Encoding.Default.GetString(text.ToArray());
            var packedImg = sourceImage.Clone()
                as Bitmap;

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
            var container = stegoImage.Clone() as Bitmap;
            var msg = ExtractMessage(container);
            var unpackMsg = Encoding.Default
                .GetBytes(msg).ToList();

            return unpackMsg;
        }

        /// <summary>
        /// Скрыть сообщение в стегоконтейнере.
        /// </summary>
        /// <param name="text"><Тескст сообщения./param>
        /// <param name="image">Изображение-стегоконтейнер.</param>
        /// <returns>Возвращает стегоконтейнер со скрытым текстом.</returns>
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
            var r = _rgbExtractor.R;
            var g = _rgbExtractor.G;
            var b = _rgbExtractor.B;
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
                        //высчитывание изменение яркости синего канала
                        //  при встраивании информации
                        int br = Convert.ToInt32(
                            (0.299 * r[i, j] + 0.587 * g[i, j]
                                + 0.114 * b[i, j]) * _lambda);
                        var ch = chars[t];
                        //определение знака формулы для пикселя
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

        /// <summary>
        /// Извлечь сообщение из стегоконтейнера.
        /// </summary>
        /// <param name="image">Изображение-стегоконтейнер.</param>
        /// <returns>Возвращает текст сообщения, спрятанный
        ///     в стегоконтейнере.</returns>
        public string ExtractMessage(Bitmap image)
        {
            _rgbExtractor.ExtractRGB(image);
            var r = _rgbExtractor.R;
            var g = _rgbExtractor.G;
            var b = _rgbExtractor.B;

            var w = image.Width;
            var h = image.Height;
            var bStar = new int[w, h];
            var text = "";

            for (int j = _sigma; j < h - h % 7; j += 7)
            {
                for (int i = _sigma; i < w - w % 7; i += 7)
                { //распаковка квадранта "креста" при _sigma = 3
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

            var unpackMsg = BitArrayToMessage(text);

            return unpackMsg;
        }

        /// <summary>
        /// Переводит битовую строку в строку символов.
        /// </summary>
        /// <param name="bits">Битовая строка.</param>
        /// <returns>Возвращает строку символов
        ///     исходного текста.</returns>
        private string BitArrayToMessage(string bits)
        {
            var bt = new byte[1];
            var info = "";
            var sByte = 8;
            var size = bits.Length / sByte;

            for (int i = 0; i < size; i++)
            {
                var str = "";
                for (int j = 0; j < sByte; j++)
                {
                    str += Convert.ToString(bits[i * sByte + j]);
                }
                bt[0] = Convert.ToByte(str, 2);
                info += Encoding.Default.GetString(bt);
            }

            return info;
        }

        /// <summary>
        /// Переводит текст сообщения в битовую строку.
        /// </summary>
        /// <param name="msg">Исходный текст сообщения.</param>
        /// <returns>Возвращает битовую строку.</returns>
        private string MessageToBitArray(string msg)
        {
            var bytes = Encoding.Default.GetBytes(msg);
            var length = msg.Length;
            var result = String.Empty;
            var sByte = 8;

            for (var i = 0; i < length; i++)
            {
                var str = Convert.ToString(msg[i], 2);
                while (str.Length < sByte)
                {
                    var str1 = "0" + str;
                    str = str1;
                }
                result += str;
            }

            return result;
        }
    }
}
