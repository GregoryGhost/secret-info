using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace StegoModel
{
    /// <summary>
    /// Упаковщик/распаковщик стегоконтейнеров, использующий 
    ///     алгоритм LSB(Замена наименее значащего бита).
    /// </summary>
    public class LeastSignificantBit : IPacker, IUnpacker
    {
        /// <summary>
        /// Метка (или маркер) начала записи скрываемого текста.
        /// </summary>
        private const string _marker = "/";

        /// <summary>
        /// Проверяет является ли контейнер стегоконтейнером.
        /// </summary>
        /// <param name="sourceImage">Проверяемый стегоконтейнер.</param>
        /// <returns>Возвращает результат проверки,
        ///     true - является стегоконтейнером,
        ///     false - пустой контейнер.</returns>
        public bool IsCombined(Bitmap sourceImage)
        {
            bool result = false;

            byte[] rez = new byte[1];
            Color color = sourceImage.GetPixel(0, 0);
            BitArray colorArray = color.R.ToBits();
            BitArray messageArray = color.R.ToBits();
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = color.G.ToBits();
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = color.B.ToBits();
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];

            rez[0] = messageArray.ToByte();
            string m = Encoding.GetEncoding(1251).GetString(rez);

            result = (m == _marker);

            return result;
        }

        /// <summary>
        /// Упаковывать скрываемый текст в пустой стегоконтейнер.
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
            // в sizeText - размер скрываемого текста в байтах
            int sizeText = text.Count;

            //проверяем, поместиться ли исходный текст
            //  в пустом контейнере
            if (sizeText > ((sourceImage.Width * sourceImage.Height) - 4))
            {
                var msg = $"Размер скрываемого текста" +
                    $" больше размера пустого контейнера.";
                throw new ArgumentException(msg, nameof(text));
            }

            if (this.IsCombined(sourceImage))
            {
                var msg = $"Пустой контейнер является стегоконтейнером.";
                throw new ArgumentException(msg, nameof(sourceImage));
            }

            var symbols = Encoding.GetEncoding(1251).GetBytes(_marker);
            var beginSymbols = symbols[0].ToBits();
            var curColor = sourceImage.GetPixel(0, 0);
            var temp = curColor.R.ToBits();
            temp[0] = beginSymbols[0];
            temp[1] = beginSymbols[1];
            byte nR = temp.ToByte();

            temp = curColor.G.ToBits();
            temp[0] = beginSymbols[2];
            temp[1] = beginSymbols[3];
            temp[2] = beginSymbols[4];
            byte nG = temp.ToByte();

            temp = curColor.B.ToBits();
            temp[0] = beginSymbols[5];
            temp[1] = beginSymbols[6];
            temp[2] = beginSymbols[7];
            var nB = temp.ToByte();

            var nColor = Color.FromArgb(nR, nG, nB);
            var stegoImage = sourceImage.Clone() as Bitmap;
            //установка маркера в первый пиксель стегоконтейнера
            stegoImage.SetPixel(0, 0, nColor);

            //записываем количество символов для упаковки
            WriteCountText(sizeText, stegoImage);

            int index = 0;
            bool st = false;
            for (int i = 4; i < stegoImage.Width; i++)
            {
                for (int j = 0; j < stegoImage.Height; j++)
                {
                    var pixelColor = stegoImage.GetPixel(i, j);
                    if (index == text.Count)
                    {
                        st = true;
                        break;
                    }
                    BitArray colorArray = pixelColor.R.ToBits();
                    BitArray messageArray = (text[index]).ToBits();
                    //запись в наименее значимые биты стегоконтейнера
                    //  биты текста
                    colorArray[0] = messageArray[0];
                    colorArray[1] = messageArray[1];
                    byte newR = colorArray.ToByte();

                    colorArray = pixelColor.G.ToBits();
                    colorArray[0] = messageArray[2];
                    colorArray[1] = messageArray[3];
                    colorArray[2] = messageArray[4];
                    byte newG = colorArray.ToByte();

                    colorArray = pixelColor.B.ToBits();
                    colorArray[0] = messageArray[5];
                    colorArray[1] = messageArray[6];
                    colorArray[2] = messageArray[7];
                    byte newB = colorArray.ToByte();

                    Color newColor = Color.FromArgb(newR, newG, newB);
                    sourceImage.SetPixel(i, j, newColor);
                    index++;
                }
                if (st)
                {
                    break;
                }
            }

            return stegoImage;
        }

        /// <summary>
        /// Распаковать скрытый текст из стегоконтейнера.
        /// </summary>
        /// <param name="stegoImage">Стегоконтейнер 
        ///     (изображение со скрытым текстом).</param>
        /// <returns>Возвращает скрытый текст 
        ///     из стегоконтейнера.</returns>
        /// <exception cref="ArgumentException">
        /// Возникает, когда стегоконтейнер 
        ///     является пустым контейнером.</exception>
        public List<byte> Unpack(Bitmap stegoImage)
        {
            if (IsCombined(stegoImage) == false)
            {
                var msg = $"Стегоконтейнер является пустым контейнером.";
                throw new ArgumentException(msg, nameof(stegoImage));
            }

            //считывание размера скрытого текста
            int nSymbols = ReadCountText(stegoImage);
            var message = new byte[nSymbols];
            int index = 0;
            bool st = false;

            //извлечение скрытого текста из менее значимых битов RGB 
            //  каналов стегоконтейнера
            for (int i = 4; i < stegoImage.Width; i++)
            {
                for (int j = 0; j < stegoImage.Height; j++)
                {
                    Color pixelColor = stegoImage.GetPixel(i, j);
                    if (index == message.Length)
                    {
                        st = true;
                        break;
                    }
                    BitArray colorArray = pixelColor.R.ToBits();
                    BitArray messageArray = pixelColor.R.ToBits(); ;
                    messageArray[0] = colorArray[0];
                    messageArray[1] = colorArray[1];

                    colorArray = pixelColor.G.ToBits();
                    messageArray[2] = colorArray[0];
                    messageArray[3] = colorArray[1];
                    messageArray[4] = colorArray[2];

                    colorArray = pixelColor.B.ToBits();
                    messageArray[5] = colorArray[0];
                    messageArray[6] = colorArray[1];
                    messageArray[7] = colorArray[2];
                    message[index] = messageArray.ToByte();
                    index++;
                }
                if (st)
                {
                    break;
                }
            }
            //медленно работает преобразование byte[] в
            //  List<byte>
            return message.OfType<byte>().ToList();
        }

        /// <summary>
        /// Прочитать размер текста, скрытого в стегоконтейнере.
        /// </summary>
        /// <param name="src">Стегоконтейнер 
        ///     (изображение со скрытым текстом).</param>
        /// <returns>Возвращает размер 
        ///     скрытого текста в байтах.</returns>
        private int ReadCountText(Bitmap src)
        {
            const int size = 3;
            //массив на 3 элемента, т.е. максимум 999 символов шифруется
            byte[] rez = new byte[size];

            //подсчет размера текста из RGB каналов 
            //  текста, скрытого в стегоконтейнере
            for (int i = 0; i < size; i++)
            {
                Color color = src.GetPixel(0, i + 1);
                BitArray colorArray = color.R.ToBits();

                BitArray bitCount = color.R.ToBits();
                bitCount[0] = colorArray[0];
                bitCount[1] = colorArray[1];

                colorArray = color.G.ToBits();
                bitCount[2] = colorArray[0];
                bitCount[3] = colorArray[1];
                bitCount[4] = colorArray[2];

                colorArray = color.B.ToBits();
                bitCount[5] = colorArray[0];
                bitCount[6] = colorArray[1];
                bitCount[7] = colorArray[2];
                rez[i] = bitCount.ToByte();
            }
            string m = Encoding.GetEncoding(1251).GetString(rez);

            return Convert.ToInt32(m, 10);
        }

        /// <summary>
        /// Записывает размер скрываемого текста в стегоконтейнер.
        /// </summary>
        /// <param name="count">Размер скрываемого текста.</param>
        /// <param name="src">Пустой стегоконтейнер.</param>
        private void WriteCountText(int count, Bitmap src)
        {
            const int size = 3;
            var countSymbols = Encoding.GetEncoding(1251).GetBytes(
                count.ToString());

            //запись размера скрываемого текста в стегоконтейнер
            for (int i = 0; i < size; i++)
            {
                //биты количества символов
                BitArray bitCount = countSymbols[i].ToBits();
                //1, 2, 3 пикселы
                Color pColor = src.GetPixel(0, i + 1);
                //бит цветов текущего пикселя
                BitArray bitsCurColor = pColor.R.ToBits();
                bitsCurColor[0] = bitCount[0];
                bitsCurColor[1] = bitCount[1];
                //новый бит цвета пиксея
                byte nR = bitsCurColor.ToByte();

                //биты цветов текущего пикселя
                bitsCurColor = pColor.G.ToBits();
                bitsCurColor[0] = bitCount[2];
                bitsCurColor[1] = bitCount[3];
                bitsCurColor[2] = bitCount[4];
                //новый цвет пикселя
                byte nG = bitsCurColor.ToByte();

                //биты цветов текущего пикселя
                bitsCurColor = pColor.B.ToBits();
                bitsCurColor[0] = bitCount[5];
                bitsCurColor[1] = bitCount[6];
                bitsCurColor[2] = bitCount[7];
                //новый цвет пикселя
                byte nB = bitsCurColor.ToByte();

                //новый цвет из полученных битов
                Color nColor = Color.FromArgb(nR, nG, nB);
                //запись полученного цвет в картинку
                src.SetPixel(0, i + 1, nColor);
            }
        }
    }
}
