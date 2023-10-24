// Copyright 2016 - 2023 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HelloButton_Click(null, null);
        }

        int i = -1;

        private void HelloButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Delay(Math.Max(i, 0) * 1000).ContinueWith(t =>
            Application.Current.Dispatcher.BeginInvoke(new Action(() => HelloButton.Content = ++i)));
        }

        private void Reset_Click_1(object sender, RoutedEventArgs e)
        {
            i = -1;
            HelloButton_Click(null, null);
        }
    }
}
