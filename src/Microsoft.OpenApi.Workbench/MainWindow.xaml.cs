﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;

namespace Microsoft.OpenApi.Workbench
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainModel _mainModel = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _mainModel;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _mainModel.ParseDocument();
            } catch (Exception ex)
            {
                _mainModel.Errors = ex.Message;
            }
        }
    }
}
