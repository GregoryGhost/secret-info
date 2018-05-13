using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StegoApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PackerViewModel _packer;
        private UnpackerViewModel _unpacker;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _packer = this.TryFindResource("Packer")
                as PackerViewModel;
            _unpacker = this.TryFindResource("Unpacker")
                as UnpackerViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string pathSrcImg = String.Empty;

            var dPic = new OpenFileDialog();
            dPic.Filter = "Файлы изображений " +
                "(*.bmp)|*.bmp|Все файлы (*.*)|*.*";

            if (dPic.ShowDialog() == true)
            {
                pathSrcImg = dPic.FileName;
            }

            _packer.PathSourceImage = pathSrcImg;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string pathHidingText = String.Empty;

            var dText = new OpenFileDialog();
            dText.Filter = "Текстовые файлы " +
                "(*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (dText.ShowDialog() == true)
            {
                pathHidingText = dText.FileName;
            }

            _packer.PathHidingText = pathHidingText;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string pathStegoContainer = String.Empty;

            var dSavePic = new SaveFileDialog();
            dSavePic.Filter = "Файлы изображений " +
                "(*.bmp)|*.bmp|Все файлы (*.*)|*.*";

            if (dSavePic.ShowDialog() == true)
            {
                pathStegoContainer = dSavePic.FileName;
            }

            _packer.PathStegoContainer = pathStegoContainer;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                _packer.Pack();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;

                MessageBox.Show($"{ex}", "MainWindow",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string pathStegoContainer = String.Empty;

            var dSavePic = new OpenFileDialog();
            dSavePic.Filter = "Файлы изображений " +
                "(*.bmp)|*.bmp|Все файлы (*.*)|*.*";

            if (dSavePic.ShowDialog() == true)
            {
                pathStegoContainer = dSavePic.FileName;
            }

            _unpacker.PathStegoContainer = pathStegoContainer;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string pathUnhidingText = String.Empty;

            var dText = new SaveFileDialog
            {
                Filter = "Текстовые файлы " +
                "(*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (dText.ShowDialog() == true)
            {
                pathUnhidingText = dText.FileName;
            }

            _unpacker.PathUnhidingText = pathUnhidingText;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                _unpacker.Unpack();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;

                MessageBox.Show($"{ex}", "MainWindow",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
