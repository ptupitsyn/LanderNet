using System.Windows;

namespace LanderNet.UI.MainView
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly GameViewModel _viewModel;

        #region public constructors

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel = new GameViewModel();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetRenderSize();
            _viewModel.RunGameLoop();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            SetRenderSize();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void SetRenderSize()
        {
            _viewModel.RenderWidth = (uint) RootGrid.ActualWidth;
            _viewModel.RenderHeight = (uint) RootGrid.ActualHeight;
        }

        #endregion
    }
}