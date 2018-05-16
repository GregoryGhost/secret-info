using StegoModel;
using StegoModel.ImageFilter;
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
        private string _pathSourceImage = string.Empty;

        /// <summary>
        /// Исходное изображение, загруженное по пути.
        /// </summary>
        private Bitmap _srcImage;

        /// <summary>
        /// Расположение размытого изображения.
        /// </summary>
        private string _pathBluredImage = string.Empty;

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
                _pathSourceImage = value;
                _srcImage = _helperIO.ReadImage(
                    _pathSourceImage);
                SourceImage = _srcImage.ToImageSource();
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
