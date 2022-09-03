using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVModels;

namespace CVCollection.Models
{
    public class ImageProcessingViewModel : BaseViewModel
    {
        ModelInfo modelInfo;
        IImageProcessingModel _model;
        IImageProcessingModel Model => _model ??= Activator.CreateInstance(modelInfo.Type) as IImageProcessingModel;
        public Command ChooseSampleImageCommand { get; }
        public Command PickImageCommand { get; }
        public Command TakePhotoCommand { get; }
        public Command ProcessImageCommand { get; }
        public Command SaveImageCommand { get; }
        bool _isInputReady = false;
        public bool IsInputReady
        {
            get => _isInputReady;
            set => SetProperty(ref _isInputReady, value);
        }
        bool _isOutputReady = false;
        public bool IsOutputReady
        {
            get => _isOutputReady;
            set => SetProperty(ref _isOutputReady, value);
        }
        byte[] _inputImage = null;
        public byte[] InputImage
        {
            get => _inputImage;
            set {
                SetProperty(ref _inputImage, value);
                IsInputReady = (value != null);
                OutputImage = null;
            }
        }
        byte[] _outputImage = null;
        public byte[] OutputImage
        {
            get => _outputImage;
            set {
                SetProperty(ref _outputImage, value);
                IsOutputReady = (value != null);
            }
        }

        string _progressText = string.Empty;
        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }
        public ImageProcessingViewModel(string modelName)
        {
            modelInfo = ModelList.GetModelInfo(modelName);
            Title = modelInfo.DisplayName;
            CVModels.Settings.RemoteProcessHost = Settings.Instance.RemoteProcessingHost;
            CVModels.Settings.DownloadedModelFolder = Settings.Instance.DownloadedModelFolder;

            Command getInputCommand(Func<Task<byte[]>> getOriginalImage)
            {
                return new Command(async () =>
                {
                    try
                    {
                        ProgressText = "Loading Image";
                        IsBusy = true;
                        var original = await getOriginalImage();
                        if (original != null)
                        {
                            InputImage = await Model.PreprocessImageAsync(original);
                        }
                    }
                    catch (Exception ex)
                    {
                        App.AlertSvc.ShowAlert("Oops!", ex.Message);
                    }
                    finally
                    {
                        IsBusy = false;
                        ProgressText = string.Empty;
                    }
                }, () => !IsBusy);
            }
            ChooseSampleImageCommand = getInputCommand(
                () => Task.Run(() => ResourceLoader.GetEmbeddedResource(modelInfo.SampleImage)));
            PickImageCommand = getInputCommand(PickPhotoAsync);
            TakePhotoCommand = getInputCommand(TakePhotoAsync);

            ProcessImageCommand = new Command(async () =>
            {
                try
                {
                    ProgressText = "Initiating";
                    IsBusy = true;
                    OutputImage = await Model.ProcessImageAsync(InputImage, new Progress<string>(str => ProgressText = str));
                }
                catch (Exception ex)
                {
                    App.AlertSvc.ShowAlert("Oops!", ex.Message);
                }
                finally { 
                    IsBusy = false;
                    ProgressText = string.Empty;
                }
            }, () => !IsBusy && IsInputReady);

            SaveImageCommand = new Command(async () =>
            {
                try
                {
                    ProgressText = "Saving Image";
                    IsBusy = true;
                    await Service.MediaService.SaveImageBytesToGallery(OutputImage, $"{modelName}-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.png");
                }
                catch (Exception ex)
                {
                    App.AlertSvc.ShowAlert("Oops!", ex.Message);
                }
                finally { 
                    IsBusy = false;
                    ProgressText = string.Empty;
                }
            }, () => !IsBusy && IsOutputReady);

            PropertyChanged += (_, _) =>
            {
                ChooseSampleImageCommand.ChangeCanExecute();
                PickImageCommand.ChangeCanExecute();
                TakePhotoCommand.ChangeCanExecute();
                ProcessImageCommand.ChangeCanExecute();
                SaveImageCommand.ChangeCanExecute();
            };
        }

        async Task<byte[]> PickPhotoAsync()
        {
            FileResult photo;

            try
            {
                photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Choose photo" });
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                throw new Exception("Feature is not supported on the device", fnsEx);
            }
            catch (PermissionException pEx)
            {
                throw new Exception("Permissions not granted", pEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"The {nameof(PickPhotoAsync)} method threw an exception", ex);
            }

            if (photo == null)
                return null;

            var bytes = await GetBytesFromPhotoFile(photo);

            return SkiaSharpUtils.HandleOrientation(bytes);
        }

        async Task<byte[]> TakePhotoAsync()
        {
            FileResult photo = null;

            if (MediaPicker.Default.IsCaptureSupported)
            {
                photo = await MediaPicker.Default.CapturePhotoAsync();
            }
            else
            {
                throw new PlatformNotSupportedException("Not supported to take photo.");
            }

            if (photo == null)
                return null;

            var bytes = await GetBytesFromPhotoFile(photo);

            return SkiaSharpUtils.HandleOrientation(bytes);
        }

        async Task<byte[]> GetBytesFromPhotoFile(FileResult fileResult)
        {
            byte[] bytes;

            using Stream stream = await fileResult.OpenReadAsync();
            using MemoryStream ms = new MemoryStream();

            stream.CopyTo(ms);
            bytes = ms.ToArray();

            return bytes;
        }
    }
}
