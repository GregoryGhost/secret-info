using System.ComponentModel;
using System.Runtime.CompilerServices;
using StegoModel;

namespace StegoApp
{
    /// <summary>
    /// Помощник для работы с INotifyPropertyChanged
    /// </summary>
    public class Notify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Оповестить об изменение свойства
        /// </summary>
        /// <param name="prop">Название свойства</param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    /// <summary>
    /// Представление модели стеганографического упаковщика.
    /// </summary>
    public class PackerViewModel : Notify
    {
        /// <summary>
        /// Стеганографический упаковщик.
        /// </summary>
        private readonly IPacker _workerSLB;

        /// <summary>
        /// Помощник по работе с вводом-выводом.
        /// </summary>
        private readonly HelperIO _helperIO;

        /// <summary>
        /// Расположение пустого контейнера.
        /// </summary>
        private string _pathImage = string.Empty;

        /// <summary>
        /// Расположение скрываемого текста.
        /// </summary>
        private string _pathHidingText = string.Empty;
        
        /// <summary>
        /// Путь до стегоконтейнера.
        /// </summary>
        private string _pathStegoContainer = string.Empty;

        /// <summary>
        /// Инициализация необходимых объектов для работы.
        /// </summary>
        public PackerViewModel()
        {
            _workerSLB = new LeastSignificantBit();
            _helperIO = new HelperIO();
        }

        /// <summary>
        /// Путь до пустого контейнера.
        /// </summary>
        public string PathSourceImage
        {
            get
            {
                return _pathImage;
            }
            set
            {
                _pathImage = value;
                OnPropertyChanged(nameof(PathSourceImage));
            }
        }

        /// <summary>
        /// Путь до скрываемого текста.
        /// </summary>
        public string PathHidingText
        {
            get
            {
                return _pathHidingText;
            }
            set
            {
                _pathHidingText = value;
                OnPropertyChanged(nameof(PathHidingText));
            }
        }

        /// <summary>
        /// Путь до стегоконтейнера.
        /// </summary>
        public string PathStegoContainer
        {
            get
            {
                return _pathStegoContainer;
            }
            set
            {
                _pathStegoContainer = value;
                OnPropertyChanged(nameof(PathStegoContainer));
            }
        }

        /// <summary>
        /// Упаковать скрываемый текст в стегоконтейнер.
        /// </summary>
        public void Pack()
        {
            var srcImage = _helperIO.ReadImage(
                PathSourceImage);
            var hidingText = _helperIO.ReadText(
                PathHidingText);

            var stegocontainer = _workerSLB.Pack(
                srcImage, hidingText);
            _helperIO.WriteImage(PathStegoContainer,
                stegocontainer);
        }
    }


    /// <summary>
    /// Представление модели стеганографического распаковщика.
    /// </summary>
    public class UnpackerViewModel : Notify
    {
        /// <summary>
        /// Путь до скрываемого текста.
        /// </summary>
        private string _pathUnhidingText = string.Empty;

        /// <summary>
        /// Путь до стегоконтейнера.
        /// </summary>
        private string _pathStegoContainer = string.Empty;

        /// <summary>
        /// Стеганографический распаковщик.
        /// </summary>
        private readonly IUnpacker _workerSLB;

        /// <summary>
        /// Помощник по работе с вводом-выводом.
        /// </summary>
        private readonly HelperIO _helperIO;

        /// <summary>
        /// Инициализация необходимых объектов.
        /// </summary>
        public UnpackerViewModel()
        {
            _workerSLB = new LeastSignificantBit();
            _helperIO = new HelperIO();
        }

        /// <summary>
        /// Распаковка скрываемого текста из стегоконтейнера.
        /// </summary>
        public void Unpack()
        {
            var stegocontainer = _helperIO.ReadImage(
                PathStegoContainer);
            var unhidingText = _workerSLB.Unpack(
                stegocontainer);

            _helperIO.WriteText(PathUnhidingText,
                unhidingText);
        }

        /// <summary>
        /// Путь до скрываемого текста.
        /// </summary>
        public string PathUnhidingText
        {
            get
            {
                return _pathUnhidingText;
            }
            set
            {
                _pathUnhidingText = value;
                OnPropertyChanged(nameof(PathUnhidingText));
            }
        }

        /// <summary>
        /// Путь до стегоконтейнера.
        /// </summary>
        public string PathStegoContainer
        {
            get
            {
                return _pathStegoContainer;
            }
            set
            {
                _pathStegoContainer = value;
                OnPropertyChanged(nameof(PathStegoContainer));
            }
        }
    }
}