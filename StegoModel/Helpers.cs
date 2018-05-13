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

    public class HelperIO : IHelperIO
    {
        public Bitmap ReadImage(string path)
        {
            var file = new FileStream(path, FileMode.Open);
            var bmp = new Bitmap(file);
            file.Close();

            return bmp;
        }

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

        public void WriteImage(string path, Bitmap image)
        {
            image.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public void WriteText(string path, List<byte> text)
        {
            using (var file = new FileStream(path, FileMode.Create))
            {
                using (var writer = new StreamWriter(
                    file, Encoding.Default))
                {
                    var t = Encoding.GetEncoding(1251)
                        .GetString(text.ToArray());
                    writer.Write(t);
                }
            }      
        }
    }
}
