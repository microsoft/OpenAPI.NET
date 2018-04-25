// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Windows;

namespace Microsoft.OpenApi.Workbench
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainModel _mainModel = new MainModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _mainModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mainModel.ParseDocument();
        }
    }
}