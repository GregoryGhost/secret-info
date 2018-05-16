using System.Windows;

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
            //TODO: поставить курсор загрузки пока применяется фильтр
            var blurFilter = this.DataContext
                as BlurViewModel;
            blurFilter.ApllyBlur();
        }
    }
}
