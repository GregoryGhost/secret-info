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
        private PackerViewModel _packer;
        private UnpackerViewModel _unpacker;
        private VisualAttackViewModel _visualAttack;
        private BlurViewModel _blurFilter;

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
            _visualAttack = this.TryFindResource("VisualAttack")
                as VisualAttackViewModel;
            _blurFilter = this.TryFindResource("Blur")
                as BlurViewModel;
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
            var msg = "Стегоконтейнер успешно получен и записан в файл.";
            var msgBoxImage = MessageBoxImage.Information;

            try
            {
                _packer.Pack();
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
            var msg = "Текст успешно получен и записан в файл.";
            var msgBoxImage = MessageBoxImage.Information;

            try
            {
                _unpacker.Unpack();
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
            string pathEmptyContainer = String.Empty;

            var dSavePic = new OpenFileDialog();
            dSavePic.Filter = "Файлы изображений " +
                "(*.bmp)|*.bmp|Все файлы (*.*)|*.*";

            if (dSavePic.ShowDialog() == true)
            {
                pathEmptyContainer = dSavePic.FileName;
            }

            _visualAttack.PathEmptyContainer = pathEmptyContainer;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            string pathStegoContainer = String.Empty;

            var dSavePic = new OpenFileDialog();
            dSavePic.Filter = "Файлы изображений " +
                "(*.bmp)|*.bmp|Все файлы (*.*)|*.*";

            if (dSavePic.ShowDialog() == true)
            {
                pathStegoContainer = dSavePic.FileName;
            }

            _visualAttack.PathStegoContainer = pathStegoContainer;
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
            string pathSrcImage = String.Empty;

            var dOpenPic = new OpenFileDialog();
            dOpenPic.Filter = "Файлы изображений " +
                "(*.bmp)|*.bmp|Все файлы (*.*)|*.*";

            if (dOpenPic.ShowDialog() == true)
            {
                pathSrcImage = dOpenPic.FileName;
            }

            _blurFilter.PathSourceImage = pathSrcImage;
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            var path = String.Empty;

            var dSavePic = new SaveFileDialog();
            dSavePic.Filter = "Файлы изображений " +
                "(*.bmp)|*.bmp|Все файлы (*.*)|*.*";

            if (dSavePic.ShowDialog() == true)
            {
                path = dSavePic.FileName;
            }

            _blurFilter.PathBluredImage = path;
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
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
