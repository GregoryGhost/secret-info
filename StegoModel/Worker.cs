using System;
using System.Drawing;

namespace StegoModel
{
    public interface IWorker
    {
        Bitmap Pack(Bitmap sourceImage, string text);
        string Unpack(Bitmap stegoImage);
    }

    public interface IHelperIO
    {
        Bitmap ReadImage(string path);
        void WriteImage(string path, Bitmap image);
    }
}
