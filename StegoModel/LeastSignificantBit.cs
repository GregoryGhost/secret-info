using System;
using System.Collections;
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

        public Bitmap Pack(Bitmap sourceImage, string text)
        {
            throw new NotImplementedException();
        }

        public string Unpack(Bitmap stegoImage)
        {
            throw new NotImplementedException();
        }
    }
}
