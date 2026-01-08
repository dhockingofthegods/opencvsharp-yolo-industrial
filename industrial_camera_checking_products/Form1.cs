using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using industrial_camera_checking_products.Configuration;
using industrial_camera_checking_products.Services;

namespace industrial_camera_checking_products
{
    public partial class Form1 : Form
    {
        private readonly ICameraService _camera;
        private readonly IYoloDetector _detector;
        private readonly ILogger<Form1> _logger;
        private CancellationTokenSource? _cts;
        private UserSettings _settings = UserSettings.Load();
        private readonly Stopwatch _fpsSW = new();
        private double _fps;

        public Form1(ICameraService camera, IYoloDetector detector, ILogger<Form1> logger)
        {
            InitializeComponent();
            _camera = camera;
            _detector = detector;
            _logger = logger;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Cameras
            cboCameras.Items.Clear();
            foreach (var cam in _camera.EnumerateCameras())
                cboCameras.Items.Add(cam);
            if (cboCameras.Items.Count > 0)
                cboCameras.SelectedIndex = Math.Clamp(_settings.CameraIndex, 0, cboCameras.Items.Count - 1);

            // Try load model from settings
            if (!string.IsNullOrEmpty(_settings.ModelPath) && File.Exists(_settings.ModelPath))
            {
                try
                {
                    _detector.Load(_settings.ModelPath!, _settings.InputW, _settings.InputH);
                    SetStatus($"Loaded model: {Path.GetFileName(_settings.ModelPath)}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load saved model");
                }
            }
        }

        private async void btnStart_Click(object? sender, EventArgs e)
        {
            if (_cts != null) return;
            if (cboCameras.SelectedItem is not CameraInfo cam)
            {
                SetStatus("Select a camera");
                return;
            }

            _settings.CameraIndex = cam.Index;
            _settings.Save();

            _cts = new CancellationTokenSource();
            _fpsSW.Restart();
            int frames = 0;

            try
            {
                await Task.Run(() => _camera.StartAsync(cam.Index, _cts.Token, frame =>
                {
                    try
                    {
                        using var display = frame.Clone();
                        var dets = _detector.Detect(display, (float)_settings.ConfThreshold, (float)_settings.NmsThreshold);
                        OverlayRenderer.DrawDetections(display, dets);

                        // FPS
                        frames++;
                        if (_fpsSW.ElapsedMilliseconds >= 1000)
                        {
                            _fps = frames * 1000.0 / _fpsSW.ElapsedMilliseconds;
                            frames = 0;
                            _fpsSW.Restart();
                        }
                        DrawFps(display, _fps);

                        var bmp = BitmapConverter.ToBitmap(display);
                        UpdatePicture(bmp);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Processing frame failed");
                    }
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Camera loop error");
                SetStatus($"Camera error: {ex.Message}");
                StopCaptureInternal();
            }
        }

        private void DrawFps(Mat image, double fps)
        {
            var text = $"FPS: {fps:0.0}";
            Cv2.PutText(image, text, new OpenCvSharp.Point(10, 25), HersheyFonts.HersheySimplex, 0.7, Scalar.Yellow, 2);
        }

        private void btnStop_Click(object? sender, EventArgs e)
        {
            StopCaptureInternal();
        }

        private void StopCaptureInternal()
        {
            try
            {
                _cts?.Cancel();
            }
            catch { }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
            SetStatus("Stopped");
        }

        private void btnLoadModel_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select YOLO ONNX model",
                Filter = "ONNX model (*.onnx)|*.onnx",
                CheckFileExists = true
            };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _detector.Load(ofd.FileName, _settings.InputW, _settings.InputH);
                    _settings.ModelPath = ofd.FileName;
                    _settings.Save();
                    SetStatus($"Loaded model: {Path.GetFileName(ofd.FileName)}");
                }
                catch (Exception ex)
                {
                    SetStatus($"Load model failed: {ex.Message}");
                }
            }
        }

        private void UpdatePicture(Bitmap bmp)
        {
            if (pictureBox.InvokeRequired)
            {
                pictureBox.BeginInvoke(new Action<Bitmap>(UpdatePicture), bmp);
                return;
            }
            var old = pictureBox.Image;
            pictureBox.Image = bmp;
            old?.Dispose();
        }

        private void SetStatus(string text)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.BeginInvoke(new Action<string>(SetStatus), text);
                return;
            }
            lblStatus.Text = text;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopCaptureInternal();
            _detector.Dispose();
            (_camera as IDisposable)?.Dispose();
        }
    }
}
