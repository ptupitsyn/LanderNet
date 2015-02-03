using System.Windows;
using LanderNet.Game;

namespace LanderNet.UI.Hud
{
    /// <summary>
    /// Interaction logic for HeadUpDisplay.xaml
    /// </summary>
    public partial class HeadUpDisplay
    {
        #region public static fields

        public static readonly DependencyProperty GameProperty =
            DependencyProperty.Register("Game", typeof (LanderGame), typeof (HeadUpDisplay),
                new PropertyMetadata(OnLanderGameChanged));

        #endregion

        #region private static methods

        private static void OnLanderGameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var game = e.NewValue as LanderGame;
            if (game != null)
            {
                ((HeadUpDisplay) d).DataContext = new HeadUpDisplayViewModel(game);
            }
        }

        #endregion

        #region public constructors

        public HeadUpDisplay()
        {
            InitializeComponent();
        }

        #endregion

        #region public properties and indexers

        public LanderGame Game
        {
            get { return (LanderGame) GetValue(GameProperty); }
            set { SetValue(GameProperty, value); }
        }

        #endregion
    }
}