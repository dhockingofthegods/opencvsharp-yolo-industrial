namespace industrial_camera_checking_products.Services;

public interface ICameraService : IDisposable
{
    IEnumerable<CameraInfo> EnumerateCameras(int probeCount = 10);
    Task StartAsync(int cameraIndex, CancellationToken token, Action<OpenCvSharp.Mat> onFrame);
}

public readonly record struct CameraInfo(int Index, string Name);
