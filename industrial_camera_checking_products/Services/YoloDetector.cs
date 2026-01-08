using OpenCvSharp;
using OpenCvSharp.Dnn;
using System.Runtime.InteropServices;

namespace industrial_camera_checking_products.Services;

public interface IYoloDetector : IDisposable
{
    void Load(string onnxPath, int inputW = 640, int inputH = 640);
    IReadOnlyList<Detection> Detect(Mat bgrImage, float conf = 0.25f, float nms = 0.45f);
}

public sealed class YoloDetector : IYoloDetector
{
    private Net? _net;
    private int _w = 640, _h = 640;

    private static readonly string[] Classes = new[]
    {
        "person","bicycle","car","motorbike","aeroplane","bus","train","truck","boat","traffic light",
        "fire hydrant","stop sign","parking meter","bench","bird","cat","dog","horse","sheep","cow",
        "elephant","bear","zebra","giraffe","backpack","umbrella","handbag","tie","suitcase","frisbee",
        "skis","snowboard","sports ball","kite","baseball bat","baseball glove","skateboard","surfboard","tennis racket","bottle",
        "wine glass","cup","fork","knife","spoon","bowl","banana","apple","sandwich","orange",
        "broccoli","carrot","hot dog","pizza","donut","cake","chair","sofa","pottedplant","bed",
        "diningtable","toilet","tvmonitor","laptop","mouse","remote","keyboard","cell phone","microwave","oven",
        "toaster","sink","refrigerator","book","clock","vase","scissors","teddy bear","hair drier","toothbrush"
    };

    public void Load(string onnxPath, int inputW = 640, int inputH = 640)
    {
        _net?.Dispose();
        _net = CvDnn.ReadNetFromOnnx(onnxPath);
        _net.SetPreferableBackend(Backend.OPENCV);
        _net.SetPreferableTarget(Target.CPU);
        _w = inputW; _h = inputH;
    }

    public IReadOnlyList<Detection> Detect(Mat bgrImage, float conf = 0.25f, float nms = 0.45f)
    {
        if (_net == null) return Array.Empty<Detection>();

        using var resized = Letterbox(bgrImage, _w, _h, out float scale, out int padX, out int padY);
        using var blob = CvDnn.BlobFromImage(resized, 1 / 255.0, new OpenCvSharp.Size(_w, _h), new Scalar(), swapRB: true, crop: false);
        _net.SetInput(blob);
        var outputNames = _net.GetUnconnectedOutLayersNames();
        using var outMat = _net.Forward(outputNames[0]);

        var results = ParseDetections(outMat, conf);
        for (int i = 0; i < results.Count; i++)
        {
            var det = results[i];
            var x = (det.Rect.X - padX) / scale;
            var y = (det.Rect.Y - padY) / scale;
            var w = det.Rect.Width / scale;
            var h = det.Rect.Height / scale;
            results[i] = det with { Rect = new Rect((int)x, (int)y, (int)w, (int)h) };
        }

        CvDnn.NMSBoxes(results.Select(r => r.Rect).ToList(),
                       results.Select(r => r.Confidence).ToList(),
                       conf, nms, out int[] indices);

        if (indices.Length == 0) return Array.Empty<Detection>();
        return indices.Select(i => results[i]).ToArray();
    }

    private static Mat Letterbox(Mat src, int w, int h, out float scale, out int padX, out int padY)
    {
        var r = Math.Min((float)w / src.Width, (float)h / src.Height);
        int newW = (int)Math.Round(src.Width * r);
        int newH = (int)Math.Round(src.Height * r);
        scale = r;
        padX = (w - newW) / 2;
        padY = (h - newH) / 2;

        var dst = new Mat(new OpenCvSharp.Size(w, h), MatType.CV_8UC3, Scalar.All(114));
        using var resized = src.Resize(new OpenCvSharp.Size(newW, newH));
        resized.CopyTo(new Mat(dst, new Rect(padX, padY, newW, newH)));
        return dst;
    }

    private static List<Detection> ParseDetections(Mat outMat, float confThresh)
    {
        var detections = new List<Detection>();
        if (outMat.Dims != 3) return detections;

        int dim1 = outMat.Size(1);
        int dim2 = outMat.Size(2);
        int total = (int)outMat.Total();
        var data = new float[total];
        Marshal.Copy(outMat.Data, data, 0, total);

        if (dim2 >= 80 && dim2 > dim1)
        {
            int n = dim1; // rows
            int step = dim2; // cols
            for (int i = 0; i < n; i++)
            {
                int baseIdx = i * step;
                float cx = data[baseIdx + 0];
                float cy = data[baseIdx + 1];
                float w = data[baseIdx + 2];
                float h = data[baseIdx + 3];
                float obj = data[baseIdx + 4];

                int best = -1; float score = 0f;
                for (int c = 5; c < step; c++)
                {
                    float s = data[baseIdx + c] * obj;
                    if (s > score) { score = s; best = c - 5; }
                }
                if (score < confThresh) continue;
                var rect = new Rect((int)(cx - w / 2), (int)(cy - h / 2), (int)w, (int)h);
                detections.Add(new Detection(rect, best, score, best >= 0 && best < Classes.Length ? Classes[best] : $"id:{best}"));
            }
        }
        else
        {
            int n = dim2; // columns
            int step = dim1; // rows
            for (int i = 0; i < n; i++)
            {
                float cx = data[0 * n + i];
                float cy = data[1 * n + i];
                float w = data[2 * n + i];
                float h = data[3 * n + i];
                float obj = data[4 * n + i];

                int best = -1; float score = 0f;
                for (int c = 5; c < step; c++)
                {
                    float s = data[c * n + i] * obj;
                    if (s > score) { score = s; best = c - 5; }
                }
                if (score < confThresh) continue;
                var rect = new Rect((int)(cx - w / 2), (int)(cy - h / 2), (int)w, (int)h);
                detections.Add(new Detection(rect, best, score, best >= 0 && best < Classes.Length ? Classes[best] : $"id:{best}"));
            }
        }

        return detections;
    }

    public void Dispose()
    {
        _net?.Dispose();
        _net = null;
    }
}

public readonly record struct Detection(Rect Rect, int ClassId, float Confidence, string Label);
