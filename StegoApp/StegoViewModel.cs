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

    public class PackerViewModel : Notify
    {
        private readonly IPacker _workerSLB;
        private readonly HelperIO _helperIO;
        private string _pathImage = string.Empty;
        private string _pathHidingText = string.Empty;
        private string _pathStegoContainer = string.Empty;

        public PackerViewModel()
        {
            _workerSLB = new LeastSignificantBit();
            _helperIO = new HelperIO();
        }

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


    public class UnpackerViewModel : Notify
    {
        private string _pathUnhidingText = string.Empty;
        private string _pathStegoContainer = string.Empty;

        private readonly IUnpacker _workerSLB;
        private readonly HelperIO _helperIO;

        public UnpackerViewModel()
        {
            _workerSLB = new LeastSignificantBit();
            _helperIO = new HelperIO();
        }

        public void Unpack()
        {
            var stegocontainer = _helperIO.ReadImage(
                PathStegoContainer);
            var unhidingText = _workerSLB.Unpack(
                stegocontainer);

            _helperIO.WriteText(PathUnhidingText,
                unhidingText);
        }

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