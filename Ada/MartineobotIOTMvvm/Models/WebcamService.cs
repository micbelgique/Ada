using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;

namespace MartineobotIOTMvvm.Models
{
    public class WebcamService
    {
        public MediaCapture MediaCapture { get; private set; }

        public CaptureElement CaptureElement { get; set; }

        public bool IsInitialized { get; private set; }

        public FaceDetectionEffect FaceDetectionEffect { get; private set; }

        /// <summary>
        /// Asynchronously initializes camera
        /// </summary>
        public async Task InitializeCameraAsync()
        {
            if (MediaCapture == null)
            {
                // Gets attached webcam
                var cameraDevice = await FindCameraDevice();

                if (cameraDevice == null)
                {
                    // No camera found
                    Debug.WriteLine("No camera found!");
                    IsInitialized = false;
                    return; 
                }

                var settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    VideoDeviceId = cameraDevice.Id
                };

                MediaCapture = new MediaCapture();
                await MediaCapture.InitializeAsync(settings);
                await SetDefinitionAsync(1280, 720); 
                IsInitialized = true; 
            }
        }

        /// <summary>
        /// Aynchronously looks for and returns first camera device found.
        /// If no device is found, return null
        /// </summary>
        private static async Task<DeviceInformation> FindCameraDevice()
        {
            // Gets available devices for capturing pictures 
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            return allVideoDevices.Any() ? allVideoDevices[0] : null;
        }

        /// <summary>
        /// Asynchronously begins live webcam feed
        /// </summary>
        public async Task StartCameraPreviewAsync()
        {
            try
            {
                CaptureElement.Source = MediaCapture; 
                await MediaCapture.StartPreviewAsync(); 
            }
            catch
            {
                IsInitialized = false;
                Debug.WriteLine("Failed to start camera preview stream");
            }
        }

        /// <summary>
        /// Asynchronously ends live webcam feed
        /// </summary>
        public async Task StopCameraPreviewAsync()
        {
            try
            {
                await MediaCapture.StopPreviewAsync();
            }
            catch
            {
                Debug.WriteLine("Failed to stop camera preview"); 
            }
        }

        /// <summary>
        /// Asynchronously clean camera states
        /// </summary>
        public async Task CleanUpAsync()
        {
            if (MediaCapture != null)
            {
                if (FaceDetectionEffect != null)
                {
                    await StopFaceDetectionAsync(); 
                }

                await StopCameraPreviewAsync();
                MediaCapture.Dispose();
                MediaCapture = null; 
            }
        }

        /// <summary>
        /// Asynchronously start face detection
        /// </summary>
        public async Task<bool> StartFaceDetectionAsync(int detectionInterval)
        {
            if (!FaceDetector.IsSupported){
                Debug.WriteLine("Face detection is not supported on this device");
                return false; 
            }

            if (FaceDetectionEffect == null)
            {
                var definition = new FaceDetectionEffectDefinition
                {
                    DetectionMode = FaceDetectionMode.HighQuality,
                    SynchronousDetectionEnabled = false
                };

                FaceDetectionEffect = (FaceDetectionEffect)await MediaCapture.AddVideoEffectAsync
                    (definition, MediaStreamType.VideoPreview);
            }

            FaceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(detectionInterval);
            FaceDetectionEffect.Enabled = true;

            return true; 
        }

        /// <summary>
        /// Asynchronously stop face detection
        /// </summary>
        public async Task StopFaceDetectionAsync()
        {
            if (FaceDetectionEffect != null)
            {
                FaceDetectionEffect.Enabled = false;
                await MediaCapture.ClearEffectsAsync(MediaStreamType.VideoPreview);
                FaceDetectionEffect = null; 
            }
        }

        /// <summary>
        /// Asynchronously set definition of webcam stream
        /// </summary>
        public async Task SetDefinitionAsync(int width, int height)
        {
            if (MediaCapture != null)
            {
                var resolution = MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties
                    (MediaStreamType.VideoPreview).Select(r => r as VideoEncodingProperties)
                        .FirstOrDefault(r => r.Width == width && r.Height == height);

                if (resolution == null)
                {
                    Debug.WriteLine("The resolution " + width + "x" + height + " is not supported");
                    return; 
                }

                await MediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync
                    (MediaStreamType.VideoPreview, resolution); 
            }
        }
    }
}
