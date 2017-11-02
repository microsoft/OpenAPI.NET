using System.Windows;

namespace Microsoft.OpenApi.Workbench
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainModel mainModel = new MainModel();
        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mainModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                this.mainModel.Validate();
        }
    }
}
