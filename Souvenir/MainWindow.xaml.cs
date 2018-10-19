using System;
using System.Collections.Generic;
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

namespace Souvenir {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Analyzer analyzer;

        public MainWindow() {
            InitializeComponent();
            MouseDown += (s, e) => {
                try {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        DragMove();
                    }
                } catch (Exception) {
                }
            };
            MouseWheel += (s, e) => {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                    // change SizeRate
                    var val = _ringSpectrum.SizeRate + (float)(e.Delta * 0.001);
                    var min = 0.1f;
                    var max = 0.9f;
                    val = Math.Min(Math.Max(val, min), max);
                    _ringSpectrum.SizeRate = val;
                } else {
                    // change Size
                    var val = _ringSpectrum.Size + (float)(e.Delta * 0.1);
                    var min = 10;
                    var max = 600;
                    val = Math.Min(Math.Max(val, min), max);
                    _ringSpectrum.Size = val;
                }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            _ringSpectrum.Opacity = 0.65;
            analyzer = new Analyzer {
                Visualizer = _ringSpectrum
            };
            analyzer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            analyzer.Stop();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}
