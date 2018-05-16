using StegoModel;
using StegoModel.ImageFilter;
using System.Drawing;

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
        private string _pathSourceImage = string.Empty;

        /// <summary>
        /// Расположение размытого изображения.
        /// </summary>
        private string _pathBluredImage = string.Empty;

        /// <summary>
        /// Количество пикселей для фильтра размытия.
        /// </summary>
        private int _countPixels;

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
                _pathSourceImage = value;
                OnPropertyChanged(nameof(PathSourceImage));
            }
        }

        /// <summary>
        /// Путь до размытого изображения.
        /// </summary>
        public string PathBluredImage
        {
            get
            {
                return _pathBluredImage;
            }
            set
            {
                _pathBluredImage = value;
                OnPropertyChanged(nameof(PathBluredImage));
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
        /// <returns>Возращает размытое изображение.</returns>
        public Bitmap ApllyBlur()
        {

            var srcImage = _helperIO.ReadImage(
                _pathSourceImage);
            var bluredImage = _blurFilter.Apply(
                srcImage, CountPixels);

            _helperIO.WriteImage(
                _pathBluredImage, bluredImage);

            return bluredImage;
        }
    }
}
