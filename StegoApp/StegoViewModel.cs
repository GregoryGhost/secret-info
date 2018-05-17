using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    /// Представление модели стеганографической системы.
    /// </summary>
    public class StegoSystemViewModel : Notify
    {
        /// <summary>
        /// Текущая пара стеганографический упаковщик и распаковщик.
        /// </summary>
        private Tuple<PackerViewModel, UnpackerViewModel> _currentStegoAlgo;

        /// <summary>
        /// Стеганографические алгоритмы распаковки и упаковки.
        /// </summary>
        private StegoAlgorithms _stegoAlgo;

        /// <summary>
        /// Инициализация представлений моделей стаканов.
        /// </summary>
        public StegoSystemViewModel()
        {
            _stegoAlgo = new StegoAlgorithms();
            _currentStegoAlgo = _stegoAlgo[0];
        }

        /// <summary>
        /// Названия доступных стеганографических алгоритмов.
        /// </summary>
        public List<string> Names
        {
            get
            {
                //У упаковщика и распаковщика одно и тоже название
                //  стеганографического алгоритма в паре,
                //  берём любой из них
                return _stegoAlgo.ToList()
                               .Select(a => a.Item1.Name)
                               .ToList();
            }
        }

        /// <summary>
        /// Получить название выбранного 
        ///     стеганографического упаковщика.
        /// </summary>
        public string SelectedPackerName
        {
            get
            {
                return _currentStegoAlgo.Item1.Name;
            }
            set
            {
                SelectedStegoAlgo = _stegoAlgo.ToList()
                    .First(g => g.Item1.Name == value);
                OnPropertyChanged(nameof(SelectedPackerName));
            }
        }

        /// <summary>
        /// Получить название выбранного 
        ///     стеганографического распаковщика.
        /// </summary>
        public string SelectedUnpackerName
        {
            get
            {
                return _currentStegoAlgo.Item2.Name;
            }
            set
            {
                SelectedStegoAlgo = _stegoAlgo.ToList()
                    .First(g => g.Item2.Name == value);
                OnPropertyChanged(nameof(SelectedUnpackerName));
            }
        }

        /// <summary>
        /// Выбранный стеганографический алгоритм распаковки и упаковки.
        /// </summary>
        private Tuple<PackerViewModel, UnpackerViewModel> SelectedStegoAlgo
        {
            get
            {
                return _currentStegoAlgo;
            }
            set
            {
                _currentStegoAlgo = value;
                OnPropertyChanged(nameof(SelectedPacker));
                OnPropertyChanged(nameof(SelectedUnpacker));
            }
        }

        /// <summary>
        /// Выбранный стеганографический алгоритм упаковки.
        /// </summary>
        private PackerViewModel SelectedPacker
        {
            get
            {
                return _currentStegoAlgo.Item1;
            }
        }

        /// <summary>
        /// Выбранный стеганографический алгоритм распаковки.
        /// </summary>
        private UnpackerViewModel SelectedUnpacker
        {
            get
            {
                return _currentStegoAlgo.Item2;
            }
        }
    }


    /// <summary>
    /// Коллекция пар упаковщиков и распаковщиков.
    /// </summary>
    public class StegoAlgorithms :
        ObservableCollection<
            Tuple<PackerViewModel, UnpackerViewModel>>
    {
        /// <summary>
        /// Инициализация представлений стегонографических алгоритмов.
        /// </summary>
        public StegoAlgorithms()
        {
            var algoLSB = new LeastSignificantBit();
            var algoKDB = new CrossBrightnessBlueChannel();

            var lsb = "Наименьшего значащего бита";
            var kdb = "Алгоритм Куттера-Джордана-Боссена";

            var vmpLSB = new PackerViewModel(algoLSB, lsb);
            var vmuLSB = new UnpackerViewModel(algoLSB, lsb);

            var vmpKDB = new PackerViewModel(algoKDB, kdb);
            var vmuKDB = new UnpackerViewModel(algoKDB, kdb);

            Add(new Tuple<PackerViewModel, UnpackerViewModel>(
                vmpLSB, vmuLSB));
            Add(new Tuple<PackerViewModel, UnpackerViewModel>(
                vmpKDB, vmuKDB));
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
        private readonly IPacker _packer;

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
        /// <param name="name">Название стеганографического 
        ///     алгоритма упаковки.</param>
        /// <param name="packer">Стеганографический упаковщик.</param>
        public PackerViewModel(IPacker packer, string name)
        {
            _packer = packer;
            _helperIO = new HelperIO();
            Name = name;
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
        /// Название стеганографического алгоритма упаковки.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Упаковать скрываемый текст в стегоконтейнер.
        /// </summary>
        public void Pack()
        {
            var srcImage = _helperIO.ReadImage(
                PathSourceImage);
            var hidingText = _helperIO.ReadText(
                PathHidingText);

            var stegocontainer = _packer.Pack(
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
        private readonly IUnpacker _unpacker;

        /// <summary>
        /// Помощник по работе с вводом-выводом.
        /// </summary>
        private readonly HelperIO _helperIO;

        /// <summary>
        /// Название стеганографического алгоритма распаковки.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Инициализация необходимых объектов.
        /// </summary>
        public UnpackerViewModel(IUnpacker unpacker, string name)
        {
            _unpacker = unpacker;
            _helperIO = new HelperIO();
            Name = name;
        }

        /// <summary>
        /// Распаковка скрываемого текста из стегоконтейнера.
        /// </summary>
        public void Unpack()
        {
            var stegocontainer = _helperIO.ReadImage(
                PathStegoContainer);
            var unhidingText = _unpacker.Unpack(
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