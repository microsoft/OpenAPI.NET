using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Tavis.OpenApi;

namespace OpenApiWorkbench
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
