using OpenCvSharp;

namespace industrial_camera_checking_products.Services;

public sealed class CameraService : ICameraService
{
    private VideoCapture? _cap;

    public IEnumerable<CameraInfo> EnumerateCameras(int probeCount = 10)
    {
        for (int i = 0; i < probeCount; i++)
        {
            using var cap = new VideoCapture(i, VideoCaptureAPIs.ANY);
            if (cap.IsOpened())
                yield return new CameraInfo(i, $"Camera {i}");
        }
    }

    public async Task StartAsync(int cameraIndex, CancellationToken token, Action<Mat> onFrame)
    {
        _cap = new VideoCapture(cameraIndex);
        if (!_cap.IsOpened())
        {
            _cap.Dispose();
            _cap = null;
            throw new InvalidOperationException("Failed to open camera");
        }

        using var frame = new Mat();
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (_cap == null) break;
                if (!_cap.Read(frame) || frame.Empty())
                {
                    await Task.Delay(10, token);
                    continue;
                }
                onFrame(frame);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                await Task.Delay(50, token);
            }
        }
    }

    public void Dispose()
    {
        _cap?.Release();
        _cap?.Dispose();
        _cap = null;
    }
}
