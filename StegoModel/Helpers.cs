using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegoModel
{
    public static class BitsExtension
    {
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
}
