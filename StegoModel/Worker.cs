using System;
using System.Collections.Generic;
using System.Drawing;

namespace StegoModel
{
    /// <summary>
    /// Стеганографический упаковщик.
    /// </summary>
    public interface IPacker
    {
        /// <summary>
        /// Упаковывает скрываемый текст в пустой стегоконтейнер.
        /// </summary>
        /// <param name="sourceImage">Пустой стегоконтейнер.</param>
        /// <param name="text">Скрываемый текст.</param>
        /// <returns>Возвращает стегоконтейнер.
        ///     (изображение со скрытым текстом).</returns>
        Bitmap Pack(Bitmap sourceImage, List<byte> text);
    }

    /// <summary>
    /// Стеганографический распаковщик.
    /// </summary>
    public interface IUnpacker
    {
        /// <summary>
        /// Распаковать скрытый текст из стегоконтейнера.
        /// </summary>
        /// <param name="stegoImage">Стегоконтейнер.
        ///     (изображение со скрытым текстом).</param>
        /// <returns>Возвращает извлеченный скрытый текст 
        ///     из стегоконтейнера.</returns>
        List<byte> Unpack(Bitmap stegoImage);
    }

    /// <summary>
    /// Помощник для работы с вводом-выводом текста и изображений.
    /// </summary>
    public interface IHelperIO
    {
        /// <summary>
        /// Прочитать изображение по указанному пути.
        /// </summary>
        /// <param name="path">Путь до изображения.</param>
        /// <returns>Прочитанное изображение.</returns>
        Bitmap ReadImage(string path);

        /// <summary>
        /// Записать изображение по указанному пути.
        /// </summary>
        /// <param name="path">Расположение изображения.</param>
        /// <param name="image">Записываемое изображение.</param>
        void WriteImage(string path, Bitmap image);

        /// <summary>
        /// Прочитать текст по указанному пути.
        /// </summary>
        /// <param name="path">Путь до текста.</param>
        /// <returns>Текст в байтах.</returns>
        List<byte> ReadText(string path);

        /// <summary>
        /// Записать текст по указанному пути.
        /// </summary>
        /// <param name="path">Расположение текста.</param>
        /// <param name="text">Записываемый текст в байтах.</param>
        void WriteText(string path, List<byte> text);
    }
}
