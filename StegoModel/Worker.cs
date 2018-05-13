using System;
using System.Collections.Generic;
using System.Drawing;

namespace StegoModel
{
    public interface IWorker
    {
        Bitmap Pack(Bitmap sourceImage, List<byte> text);
        List<byte> Unpack(Bitmap stegoImage);
        bool IsCombined(Bitmap sourceImage);
    }

    public interface IHelperIO
    {
        Bitmap ReadImage(string path);
        void WriteImage(string path, Bitmap image);
        List<byte> ReadText(string path);
        void WriteImage(string path, List<byte> text);
    }
}
