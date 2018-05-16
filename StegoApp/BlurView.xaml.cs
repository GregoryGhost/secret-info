using System.Windows;
using System.Windows.Input;

namespace StegoApp
{
    /// <summary>
    /// Логика взаимодействия для BlurView.xaml
    /// </summary>
    public partial class BlurView : Window
    {
        public BlurView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var blurFilter = this.DataContext
                as BlurViewModel;
            try
            {
                blurFilter.ApllyBlur();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Произошла ошибка при размытии.",
                    Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
