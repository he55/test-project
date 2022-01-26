using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            this.InitializeComponent();

            dispatcherTimer.Interval = TimeSpan.FromSeconds(0.02);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();
            else
                dispatcherTimer.Start();

            myButton.Content = "Clicked";
        }

        int i;
        private void DispatcherTimer_Tick(object sender, object e)
        {
            if (i >999)
                i = 0;

            userControl2.Value = i;
            ++i;
        }

        private void Slider_ValueChanged(object s, RangeBaseValueChangedEventArgs e)
        {
            userControl2.Value = (int)slider.Value;
        }
    }
}
