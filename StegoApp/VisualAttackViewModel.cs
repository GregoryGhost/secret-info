using StegoModel;
using System.Drawing;

namespace StegoApp
{
    /// <summary>
    /// Представление модели алгоритма визуальной атаки
    ///     на стегоконтейнер.
    /// </summary>
    public class VisualAttackViewModel : Notify
    {
        /// <summary>
        /// Визуальная атака на стегоконтейнер.
        /// </summary>
        private VisualAttack _visualAttack = new VisualAttack();

        /// <summary>
        /// Помощник по работе с вводом-выводом.
        /// </summary>
        private HelperIO _helperIO = new HelperIO();

        /// <summary>
        /// Расположение пустого контейнера.
        /// </summary>
        private string _pathEmptyContainer = string.Empty;

        /// <summary>
        /// Расположение стегоконтейнера.
        /// </summary>
        private string _pathStegoContainer = string.Empty;

        /// <summary>
        /// Путь до пустого контейнера.
        /// </summary>
        public string PathEmptyContainer
        {
            get
            {
                return _pathEmptyContainer;
            }
            set
            {
                _pathEmptyContainer = value;
                OnPropertyChanged(nameof(PathEmptyContainer));
            }
        }

        /// <summary>
        /// Путь до стегоконтенера.
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
        /// Высчитать разницу между пустым и стего- контейнерами.
        /// </summary>
        /// <returns>Высчитанная изображение-разница.</returns>
        public Bitmap DifferenceEmptyAndStegoContainers()
        {
            var emptyContainer = _helperIO.ReadImage(
                _pathEmptyContainer);
            var stegoContainer = _helperIO.ReadImage(
                _pathStegoContainer);

            var diff =_visualAttack.Difference(
                emptyContainer, stegoContainer, false);

            return diff;
        }
    }
}
