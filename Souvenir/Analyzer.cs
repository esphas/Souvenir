using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Dsp;

namespace Souvenir {
    class Analyzer {
        public IVisualizer Visualizer;
        private WasapiLoopbackCapture _caputre;
        private readonly int _fftLength;
        private readonly Complex[] _fftBuffer;
        private readonly float[] _amplitudes;

        public Analyzer() {
            _caputre = new WasapiLoopbackCapture();
            _caputre.DataAvailable += DataAvailable;
            _fftLength = 2048;
            _fftBuffer = new Complex[_fftLength];
            _amplitudes = new float[_fftLength];
        }

        public void Start() {
            _caputre.StartRecording();
        }

        public void Stop() {
            _caputre.StopRecording();
        }

        private void DataAvailable(object sender, WaveInEventArgs args) {
            var count = args.BytesRecorded / 4; // 32-bit
            var buffer = new WaveBuffer(args.Buffer);
            for (int i = 0; i < count; ++i) {
                _fftBuffer[i].X = (float)(buffer.FloatBuffer[i] * FastFourierTransform.HannWindow(i, _fftLength));
                _fftBuffer[i].Y = 0;
                if (i + 1 >= _fftLength) {
                    FastFourierTransform.FFT(true, (int)Math.Log(this._fftLength, 2.0), _fftBuffer);
                    for (int j = 0; j < _fftLength; ++j) {
                        _amplitudes[j] = (float)Math.Sqrt(Math.Pow(_fftBuffer[j].X, 2) + Math.Pow(_fftBuffer[j].Y, 2));
                    }
                    Visualizer.SetData(_amplitudes);
                    break;
                }
            }
        }
    }
}
