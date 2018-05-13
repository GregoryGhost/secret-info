﻿using System.ComponentModel;
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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

    public class PackerViewModel : Notify
    {
        private readonly LeastSignificantBit _workerSLB;
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
}