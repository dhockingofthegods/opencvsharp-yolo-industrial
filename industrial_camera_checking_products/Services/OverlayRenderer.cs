using OpenCvSharp;

namespace industrial_camera_checking_products.Services;

public static class OverlayRenderer
{
    public static void DrawDetections(Mat image, IEnumerable<Detection> detections)
    {
        foreach (var d in detections)
        {
            var box = d.Rect;
            box.X = Math.Max(0, box.X);
            box.Y = Math.Max(0, box.Y);
            box.Width = Math.Min(image.Width - box.X, box.Width);
            box.Height = Math.Min(image.Height - box.Y, box.Height);
            var color = Scalar.FromRgb(0, 255, 0);
            Cv2.Rectangle(image, box, color, 2);
            var text = $"{d.Label} {d.Confidence:0.00}";
            int baseLine;
            var size = Cv2.GetTextSize(text, HersheyFonts.HersheySimplex, 0.5, 1, out baseLine);
            var topLeft = new OpenCvSharp.Point(box.X, Math.Max(0, box.Y - size.Height - baseLine - 3));
            Cv2.Rectangle(image, new Rect(topLeft, new OpenCvSharp.Size(size.Width + 6, size.Height + baseLine + 6)), color, -1);
            Cv2.PutText(image, text, new OpenCvSharp.Point(topLeft.X + 3, topLeft.Y + size.Height + 1), HersheyFonts.HersheySimplex, 0.5, Scalar.Black, 1);
        }
    }
}
