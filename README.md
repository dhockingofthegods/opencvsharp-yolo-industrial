# Industrial Camera + YOLO (WinForms, .NET 8, OpenCvSharp)

[![Build](https://github.com/dhockingofthegods/opencvsharp-yolo-industrial/actions/workflows/build.yml/badge.svg)](https://github.com/dhockingofthegods/opencvsharp-yolo-industrial/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/dhockingofthegods/opencvsharp-yolo-industrial/blob/main/LICENSE)

A Windows Forms application that connects to a USB camera and performs real-time object detection using YOLO (ONNX) via OpenCV DNN.

Note: Replace OWNER/REPO in the badge URLs above after pushing to GitHub.

## Features
- Live USB camera preview using OpenCvSharp `VideoCapture`.
- Object detection with YOLO ONNX (YOLOv5/YOLOv8 style outputs).
- Correct letterbox preprocessing and coordinate mapping back to original frame.
- FPS overlay and responsive UI updates.
- Dependency Injection (`Microsoft.Extensions.DependencyInjection`) and logging (`Microsoft.Extensions.Logging`).
- User settings persisted to `%AppData%/industrial_camera_checking_products/settings.json`.
- x64 build configuration for OpenCvSharp native binaries.
- Modular services: `CameraService`, `YoloDetector`, `OverlayRenderer`.

## Requirements
- Windows 10/11 x64
- .NET 8 SDK
- Visual Studio (with .NET desktop development workload) or `dotnet` CLI
- USB camera
- YOLO ONNX model file (e.g., yolov5s.onnx or a YOLOv8 ONNX export)

## Getting Started
1. Clone the repository.
2. Restore and build:
   - Using CLI:
     - `dotnet restore`
     - `dotnet build -c Debug`
   - Using Visual Studio: open the solution and build (ensure x64 platform).
3. Run the app:
   - CLI: `dotnet run --project industrial_camera_checking_products`
   - Visual Studio: Start Debugging.

## Usage
1. Click "Load YOLO" and select a `.onnx` model.
2. Select your USB camera from the dropdown.
3. Click "Start" to begin capture and detection.
4. Click "Stop" to end the session.

Notes:
- The included class list assumes the COCO 80 classes. If your model uses different labels, update the list in `Services/YoloDetector.cs`.
- The app uses CPU by default. For GPU acceleration, consider ONNX Runtime GPU or a CUDA-enabled OpenCV build.

## Architecture
- `Services/ICameraService` and `CameraService`: enumerates cameras and provides an async capture loop (`StartAsync`).
- `Services/IYoloDetector` and `YoloDetector`: loads YOLO ONNX, performs letterbox, forward pass, parses outputs, and applies NMS.
- `Services/OverlayRenderer`: draws bounding boxes and labels on frames.
- `Configuration/UserSettings`: loads/saves user preferences (camera index, model path, thresholds, input size).
- `Form1`: UI layer using DI services for camera and detection; handles FPS overlay and thread-safe image updates.
- `Program`: DI and logging setup; resolves and runs `Form1`.

## Configuration
User settings are stored at:
- `%AppData%/industrial_camera_checking_products/settings.json`

Example fields:
```json
{
  "CameraIndex": 0,
  "ModelPath": "C:\\models\\yolov5s.onnx",
  "ConfThreshold": 0.25,
  "NmsThreshold": 0.45,
  "InputW": 640,
  "InputH": 640
}
```
Adjust thresholds and input size to match your model for best performance and accuracy.

## Screenshots / Demo
Place your screenshots or GIFs in `docs/images/` and reference them here:

- App UI: `![screenshot](docs/images/screenshot.png)`
- Live detection GIF: `![demo](docs/images/demo.gif)`

## Troubleshooting
- No camera found: ensure the device is connected; increase probe range in `CameraService.EnumerateCameras()` if needed.
- Black screen or crash: verify the app runs as x64 (the project sets `PlatformTarget` to x64).
- Low FPS: reduce input size (e.g., 416x416), close other apps using the camera, or use GPU acceleration.
- Wrong/empty detections: ensure the ONNX export matches expected layout (YOLOv5/YOLOv8). Update class labels if using a custom dataset.

## Roadmap
- UI controls for thresholds and input size.
- Snapshot/video recording.
- GPU acceleration (ONNX Runtime GPU / OpenCV DNN CUDA).
- Support segmentation/pose models.

## License
Choose a license (e.g., MIT) and add a `LICENSE` file to the repository.

---

# Phiên b?n Ti?ng Vi?t

?ng d?ng Windows Forms k?t n?i USB camera và ch?y nh?n di?n ??i t??ng th?i gian th?c dùng YOLO (ONNX) qua OpenCV DNN.

## Huy hi?u (Badges)
- C?p nh?t OWNER/REPO sau khi ??y lên GitHub:
  - Build: `https://github.com/dhockingofthegods/opencvsharp-yolo-industrial/actions/workflows/build.yml/badge.svg`
  - License: `https://img.shields.io/badge/License-MIT-green.svg`

## Tính n?ng
- Xem tr?c ti?p USB camera b?ng OpenCvSharp `VideoCapture`.
- Nh?n di?n ??i t??ng v?i YOLO ONNX (??u ra ki?u YOLOv5/YOLOv8).
- Ti?n x? lý letterbox chu?n và ánh x? to? ?? bbox v? ?nh g?c.
- Hi?n th? FPS và c?p nh?t UI m??t mà.
- Dùng DI và logging chu?n .NET.
- L?u cài ??t ng??i dùng (AppData).
- Build x64.
- Mô-?un hoá services.

## ?nh minh ho? / Demo
??t ?nh/GIF vào `docs/images/` và tham chi?u:
- Giao di?n: `![screenshot](docs/images/screenshot.png)`
- GIF demo: `![demo](docs/images/demo.gif)`
