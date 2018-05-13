using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StegoModel
{
    public class LeastSignificantBit : IWorker
    {
        public bool IsCombined(Bitmap sourceImage)
        {
            const string marker = "/";
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

            if (m == marker)
            {
                result = true;
            }

            return result;
        }

        public Bitmap Pack(Bitmap sourceImage, List<byte> text)
        {
            // в CountText - размер прятываемого текста в байтах
            int sizeText = text.Count;

            //проверяем, поместиться ли исходный текст в картинке
            if (sizeText > ((sourceImage.Width * sourceImage.Height)) - 4)
            {
                //TODO: выкидывать исключение
                return null;
            }

            //проверяем, может быть картинка уже зашифрована
            if (this.IsCombined(sourceImage))
            {
                //TODO: выкидывать исключение
                return null;
            }

            var symbols = Encoding.GetEncoding(1251).GetBytes("/");
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
            Bitmap stegoImage = sourceImage;
            //в первом пикселе будет маркер, 
            //  который говорит о том, что картика зашифрована
            stegoImage.SetPixel(0, 0, nColor);

            //записываем количество символов для шифрования
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
                    //меняем в нашем цвете биты
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

        public List<byte> Unpack(Bitmap stegoImage)
        {
            if (IsCombined(stegoImage) == false)
            {
                //TODO: выкидывать исключение
                return null;
            }

            //считали количество скрытых символов
            int nSymbols = ReadCountText(stegoImage);
            var message = new List<byte>(nSymbols);
            int index = 0;
            bool st = false;

            for (int i = 4; i < stegoImage.Width; i++)
            {
                for (int j = 0; j < stegoImage.Height; j++)
                {
                    Color pixelColor = stegoImage.GetPixel(i, j);
                    if (index == message.Count)
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

            return message;
        }

        private int ReadCountText(Bitmap src)
        {
            //массив на 3 элемента, т.е. максимум 999 символов шифруется
            byte[] rez = new byte[3];

            for (int i = 0; i < 3; i++)
            {
                //цвет 1, 2, 3 пикселей 
                Color color = src.GetPixel(0, i + 1);
                //биты цвета
                BitArray colorArray = color.R.ToBits();
                //инициализация результирующего массива бит
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

        private void WriteCountText(int count, Bitmap src)
        {
            byte[] countSymbols = Encoding.GetEncoding(1251).GetBytes(count.ToString());

            for (int i = 0; i < 3; i++)
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
                //записали полученный цвет в картинку
                src.SetPixel(0, i + 1, nColor);
            }
        }
    }
}
