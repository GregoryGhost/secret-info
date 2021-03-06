﻿using StegoModel;
using StegoModel.ImageFilter;
using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace StegoApp
{
    /// <summary>
    /// Представление фильтра размытия изображения.
    /// </summary>
    public class BlurViewModel : Notify
    {
        /// <summary>
        /// Фильтр размытия изображения.
        /// </summary>
        private IFilterImage _blurFilter = new Blur();

        /// <summary>
        /// Помощник по работе с вводом-выводом.
        /// </summary>
        private HelperIO _helperIO = new HelperIO();

        /// <summary>
        /// Расположение исходного изображения.
        /// </summary>
        private string _pathSourceImage = String.Empty;

        /// <summary>
        /// Исходное изображение, загруженное по пути.
        /// </summary>
        private Bitmap _srcImage;

        /// <summary>
        /// Расположение размытого изображения.
        /// </summary>
        private string _pathBluredImage = String.Empty;

        /// <summary>
        /// Количество пикселей для фильтра размытия.
        /// </summary>
        private int _countPixels = 5;

        /// <summary>
        /// Размытое изображение после применения фильтра.
        /// </summary>
        private BitmapImage _bluredImage;

        /// <summary>
        /// Исходное изображение до применения фильтра.
        /// </summary>
        private BitmapImage _sourceImage;

        /// <summary>
        /// Путь до исходного изображения.
        /// </summary>
        public string PathSourceImage
        {
            get
            {
                return _pathSourceImage;
            }
            set
            {
                if (value == String.Empty ||
                    value == null)
                {
                    var msg = "Исходное изображение не задано.";
                    throw new ArgumentException(msg);
                }
                _pathSourceImage = value;
                _srcImage = _helperIO.ReadImage(_pathSourceImage);
                SourceImage = _srcImage.ToImageSource();
                OnPropertyChanged(nameof(PathSourceImage));
            }
        }

        /// <summary>
        /// Путь до размытого изображения.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     Возникает, если передан null, пустая строка
        ///         или другой неверный путь.</exception>
        public string PathBluredImage
        {
            get
            {
                return _pathBluredImage;
            }
            set
            {
                if (value == String.Empty ||
                    value == null)
                {
                    var msg = "Путь для размытого изображения не задан.";
                    throw new ArgumentException(msg);
                }
                _pathBluredImage = value;
                OnPropertyChanged(nameof(PathBluredImage));
            }
        }

        /// <summary>
        /// Исходное изображение.
        /// </summary>
        public BitmapImage SourceImage
        {
            get
            {
                return _sourceImage;
            }
            private set
            {
                _sourceImage = value;
                OnPropertyChanged(nameof(SourceImage));
            }
        }

        /// <summary>
        /// Размытое изображение.
        /// </summary>
        public BitmapImage BluredImage
        {
            get
            {
                return _bluredImage;
            }
            private set
            {
                _bluredImage = value;
                OnPropertyChanged(nameof(BluredImage));
            }
        }

        /// <summary>
        /// Количество соседних пикселей для размытия.
        /// </summary>
        public int CountPixels
        {
            get
            {
                return _countPixels;
            }
            set
            {
                _countPixels = value;
                OnPropertyChanged(nameof(CountPixels));
            }
        }

        /// <summary>
        /// Применить фильтр размытия к исходному изображению.
        /// </summary>
        public void ApllyBlur()
        {
            var bluredImg = _blurFilter.Apply(
                _srcImage, CountPixels);
            BluredImage = bluredImg.ToImageSource();

            _helperIO.WriteImage(
                PathBluredImage, bluredImg);
        }
    }
}
