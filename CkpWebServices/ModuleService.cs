using CkpInfrastructure.Configuration;
using CkpModel.Input.Module;
using CkpModel.Output;
using CkpServices.Helpers.Builders;
using CkpServices.Interfaces;
using CkpServices.Processors.Interfaces;
using CkpServices.Processors.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CkpServices
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleMaketProcessor _moduleMaketProcessor;

        public ModuleService(IOptions<AppSettings> appSettingsAccessor)
        {
            _moduleMaketProcessor = new ModuleMaketProcessor(
                appSettingsAccessor.Value.OrderImFolderTemplate,
                appSettingsAccessor.Value.DatabaseName);
        }

        /*
        public ActionResult<string> SaveModuleToDisk(byte[] moduleBytes)
        {
            var fileId = Guid.NewGuid().ToString();
            var filePath = Path.Combine(Path.GetTempPath(), fileId);
            File.WriteAllBytes(filePath, moduleBytes);

            return fileId;
        }

        public void DeleteModuleFromDisk(string fileId)
        {
            var filePath = Path.Combine(Path.GetTempPath(), fileId);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public ActionResult<ImageInfo> CreateSample(byte[] moduleBytes, ImageFormat format)
        {
            var filePath = Path.GetTempFileName();
            File.WriteAllBytes(filePath, moduleBytes);

            var sample = new ImageInfo();

            using (var image = Image.FromFile(filePath))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, format);
                    
                    sample.Height = image.PhysicalDimension.Height;
                    sample.Width = image.PhysicalDimension.Width;
                    sample.VResolution = image.VerticalResolution;
                    sample.HResolution = image.HorizontalResolution;
                    sample.Base64String = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            File.Delete(filePath);

            return sample;
        }
        */

        public ActionResult<ImageInfo> GetSampleImageById(int orderPositionId)
        {
            var bytes = _moduleMaketProcessor.GetSampleImageBytesById(orderPositionId, "ImgMaket");

            if (bytes.Length == 0)
                bytes = _moduleMaketProcessor.GetSampleImageBytesById(orderPositionId, "ImgTask");

            var sample = CreateImageSample(bytes, ImageFormat.Jpeg);

            return sample;
        }

        public ActionResult<ImageInfo> CreateImageSample(byte[] imageBytes, ImageFormat format)
        {
            var sample = new ImageInfo();

            if (imageBytes.Length == 0)
                return sample;
            
            using (MemoryStream maketMemoryStream = new MemoryStream(imageBytes))
            {
                using (var image = Image.FromStream(maketMemoryStream))
                {
                    using (var sampleStream = new MemoryStream())
                    {
                        image.Save(sampleStream, format);

                        sample.Height = image.PhysicalDimension.Height;
                        sample.Width = image.PhysicalDimension.Width;
                        sample.VResolution = image.VerticalResolution;
                        sample.HResolution = image.HorizontalResolution;
                        sample.Base64String = Convert.ToBase64String(sampleStream.ToArray());
                    };
                };
            }

            return sample;
        }

        public ActionResult<ImageInfo> BuildModuleSampleStandard(ModuleParamsStandartData moduleParams)
        {
            var builder = new MaketStandartBuilder();
            builder.MaketParams = moduleParams;
            builder.Build();
            var bitmap = builder.GetResult();

            var bytes = ImageToBytes(bitmap, ImageFormat.Jpeg);
            var sample = CreateImageSample(bytes, ImageFormat.Jpeg);

            return sample;
        }

        private static byte[] ImageToBytes(Image image, ImageFormat format)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, format);
                return stream.ToArray();
            }
        }
    }
}
