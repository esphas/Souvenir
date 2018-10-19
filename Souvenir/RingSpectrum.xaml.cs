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
    /// Interaction logic for RingSpectrum.xaml
    /// </summary>
    public partial class RingSpectrum : UserControl, IVisualizer {
        private float _size;
        public float Size {
            set {
                if (_size != value) {
                    _size = value;
                    RefreshLayout();
                }
            }
            get { return _size; }
        }
        private float _sizeRate;
        public float SizeRate {
            set {
                if (_sizeRate != value) {
                    _sizeRate = value;
                    RefreshLayout();
                }
            }
            get { return _sizeRate; }
        }
        private int _binCount;
        public int BinCount {
            set {
                if (_binCount != value) {
                    _binCount = value;
                    RefreshLayout();
                }
            }
            get { return _binCount; }
        }
        private float _baseAngle; // in deg
        public float BaseAngle {
            set {
                if (_baseAngle != value) {
                    _baseAngle = value;
                    RefreshLayout();
                }
            }
            get { return _baseAngle; }
        }
        private Ellipse _circle;
        private Rectangle[] _bins;
        private float[] _binData;
        private float _overallMaximum;
        
        public RingSpectrum() {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            _size = 400;
            _sizeRate = 0.45f;
            _binCount = 16;
            _baseAngle = 135;
            _overallMaximum = 0.001f;
            _circle = new Ellipse {
                Fill = new ImageBrush(new BitmapImage(new Uri(@"test.jpg", UriKind.Relative))), // TEST
                Stroke = new SolidColorBrush(Colors.White)
            };
            RefreshLayout();
        }

        private void RefreshLayout() {
            _canvas.Width = _size;
            _canvas.Height = _size;
            _canvas.Children.Clear();
            var cSize = _size * _sizeRate;
            _circle.Width = cSize;
            _circle.Height = cSize;
            _circle.StrokeThickness = cSize / 20;
            Canvas.SetLeft(_circle, (_size - cSize) / 2);
            Canvas.SetTop(_circle, (_size - cSize) / 2);
            _canvas.Children.Add(_circle);
            _bins = new Rectangle[_binCount];
            _binData = new float[_binCount];
            var binWidth = cSize * Math.PI / _binCount * 0.9;
            var baseAngle = _baseAngle - 180;
            for (int i = 0; i < _binCount; ++i) {
                var angle = baseAngle + 360 * (float)i / _binCount;
                var angleRad = angle * Math.PI / 180; // to rad
                var cos = Math.Cos(angleRad);
                var sin = Math.Sin(angleRad);
                _bins[i] = new Rectangle {
                    Fill = new SolidColorBrush(Colors.White),
                    Width = binWidth,
                    RenderTransform = new RotateTransform {
                        Angle = angle
                    }
                };
                Canvas.SetLeft(_bins[i], (_size - cSize * sin - binWidth * cos) / 2);
                Canvas.SetTop(_bins[i], (_size + cSize * cos - binWidth * sin) / 2);
                _canvas.Children.Add(_bins[i]);
            }
        }

        public void SetData(float[] data) {
            var step = data.Length / _binCount;
            for (int i = 0; i < _binCount - 1; ++i) {
                _binData[i] = data.Skip(step * i).Take(step).Average();
            }
            _binData[_binCount - 1] = data.Skip(step * (_binCount - 1)).Average();
            var max = _binData.Max();
            if (max > _overallMaximum) {
                _overallMaximum = max;
            }
            Dispatcher.Invoke(() => {
                var bSize = _size * (1 - _sizeRate) / 2;
                for (int i = 0; i < _binCount; ++i) {
                    _bins[i].Height = bSize * _binData[i] / _overallMaximum;
                }
            });
        }
    }
}
