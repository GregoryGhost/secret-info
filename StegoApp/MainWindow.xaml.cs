using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace StegoApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StegoSystemViewModel _stegoSystem;
        private VisualAttackViewModel _visualAttack;
        private BlurViewModel _blurFilter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _stegoSystem = this.TryFindResource("StegoSystem")
                as StegoSystemViewModel;

            _visualAttack = this.TryFindResource("VisualAttack")
                as VisualAttackViewModel;

            _blurFilter = this.TryFindResource("Blur")
                as BlurViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _stegoSystem.SelectedPacker.PathSourceImage = OpenImage();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _stegoSystem.SelectedPacker.PathHidingText = OpenTextFile();
        }

        private string OpenTextFile()
        {
            string pathHidingText = String.Empty;

            var dText = new OpenFileDialog
            {
                Filter = "Текстовые файлы " +
                "(*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (dText.ShowDialog() == true)
            {
                pathHidingText = dText.FileName;
            }

            return pathHidingText;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _stegoSystem.SelectedPacker.PathStegoContainer = SaveImage();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var msg = "Стегоконтейнер успешно получен и записан в файл.";
            var msgBoxImage = MessageBoxImage.Information;

            try
            {
                _stegoSystem.SelectedPacker.Pack();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                msgBoxImage = MessageBoxImage.Error;
            }

            MessageBox.Show($"{msg}", Title,
                MessageBoxButton.OK, msgBoxImage);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            _stegoSystem.SelectedUnpacker.PathStegoContainer = OpenImage();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            _stegoSystem.SelectedUnpacker.PathUnhidingText = SaveTextFile();
        }

        private string SaveTextFile()
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

            return pathUnhidingText;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var msg = "Текст успешно получен и записан в файл.";
            var msgBoxImage = MessageBoxImage.Information;

            try
            {
                _stegoSystem.SelectedUnpacker.Unpack();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                msgBoxImage = MessageBoxImage.Error;
            }

            MessageBox.Show($"{msg}", Title,
                MessageBoxButton.OK, msgBoxImage);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            _visualAttack.PathEmptyContainer = OpenImage();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            _visualAttack.PathStegoContainer = OpenImage();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            var msg = "Сравнение успешно произведено.";
            var msgBoxImage = MessageBoxImage.Information;
            Bitmap diff = null;

            try
            {
                diff = _visualAttack.DifferenceEmptyAndStegoContainers();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                msgBoxImage = MessageBoxImage.Error;
            }

            MessageBox.Show($"{msg}", Title,
                MessageBoxButton.OK, msgBoxImage);

            var resultWindow = new ShowingDiffContainers();
            resultWindow.DiffImage.Source = diff.ToImageSource();
            resultWindow.ShowDialog();
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            try
            {
                _blurFilter.PathSourceImage = OpenImage();
            }
            catch (ArgumentException ex)
            {
                var msg = ex.Message;
                MessageBox.Show(msg, Title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            try
            {
                _blurFilter.PathBluredImage = SaveImage();
            }
            catch (ArgumentException ex)
            {
                var msg = ex.Message;
                MessageBox.Show(msg, Title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private string OpenImage()
        {
            var path = String.Empty;

            var dOpenPic = new OpenFileDialog
            {
                Filter = "Файлы изображений " +
                "(*.bmp)|*.bmp|Все файлы (*.*)|*.*"
            };

            if (dOpenPic.ShowDialog() == true)
            {
                path = dOpenPic.FileName;
            }

            return path;
        }

        private string SaveImage()
        {
            var path = String.Empty;
            var dSavePic = new SaveFileDialog
            {
                Filter = "Файлы изображений " +
                    "(*.bmp)|*.bmp|Все файлы (*.*)|*.*"
            };

            if (dSavePic.ShowDialog() == true)
            {
                path = dSavePic.FileName;
            }

            return path;
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            try
            {
                _blurFilter.PathSourceImage = _blurFilter.PathSourceImage;
                _blurFilter.PathBluredImage = _blurFilter.PathBluredImage;
            }
            catch (ArgumentException ex)
            {
                var msg = ex.Message;
                MessageBox.Show(msg, Title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            //Открыть окно размытия
            var blurWindow = new BlurView
            {
                DataContext = _blurFilter
            };
            blurWindow.ShowDialog();
        }
    }


    public static class BitmapExtension
    {
        /// <summary>
        /// Конвертировать Bitmap в Image Source WPF контрола.
        /// </summary>
        /// <param name="bitmap">Исходное изображение.</param>
        /// <returns>Возвращает изображение для Image Source WPF.</returns>
        public static BitmapImage ToImageSource(this Bitmap bitmap)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapImg = new BitmapImage();
                bitmapImg.BeginInit();
                bitmapImg.StreamSource = memory;
                bitmapImg.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImg.EndInit();

                return bitmapImg;
            }
        }
    }
}
