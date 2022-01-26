using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
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
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    public sealed partial class UserControl2 : UserControl
    {
        CanvasControl _canvasControl;
        int _value;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                _canvasControl.Invalidate();
            }
        }

        public UserControl2()
        {
            this.InitializeComponent();

            _canvasControl = new CanvasControl();
            _canvasControl.ClearColor = Colors.Transparent;
            _canvasControl.Draw += CanvasControl_Draw;
            _canvasControl.CreateResources += _canvasControl_CreateResources;

            this.Content = _canvasControl;
            this.Unloaded += CustomControl1_Unloaded;
        }

        private void _canvasControl_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }

        Color color = Colors.Red;
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            color = Colors.Blue;
            _canvasControl.Invalidate();
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            color = Colors.Red;
            _canvasControl.Invalidate();
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            using (Microsoft.Graphics.Canvas.CanvasDrawingSession drawingSession = args.DrawingSession)
            {
                double v = this.Width / 2;
                double v1 = this.Height / 2;

                drawingSession.DrawCircle((float)v, (float)v1, 60, Colors.Red, 3);
                drawingSession.DrawText(_value.ToString(),
                    (float)v - 20,
                    (float)v1 - 25,
                    color,
                    new Microsoft.Graphics.Canvas.Text.CanvasTextFormat { FontSize = 40 });
            }
        }

        private void CustomControl1_Unloaded(object sender, RoutedEventArgs e)
        {
            _canvasControl.RemoveFromVisualTree();
            _canvasControl = null;
        }
    }
}
