using StegoModel;
using System.Drawing;

namespace StegoApp
{
    public class VisualAttackViewModel : Notify
    {
        private VisualAttack _visualAttack = new VisualAttack();
        private HelperIO _helperIO = new HelperIO();
        private string _pathEmptyContainer = string.Empty;
        private string _pathStegoContainer = string.Empty;

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
