# Industrial Camera + YOLO (WinForms, .NET 8, OpenCvSharp)

[![Build](https://github.com/dhockingofthegods/opencvsharp-yolo-industrial/actions/workflows/build.yml/badge.svg)](https://github.com/dhockingofthegods/opencvsharp-yolo-industrial/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/dhockingofthegods/opencvsharp-yolo-industrial/blob/main/LICENSE)

A Windows Forms application that connects to a USB camera and performs real-time object detection using YOLO (ONNX) via OpenCV DNN.
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
---

