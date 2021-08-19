using CkpModel.Input.Module;
using CkpModel.Output;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;

namespace CkpServices.Interfaces
{
    public interface IModuleService
    {
        ActionResult<ImageInfo> CreateImageSample(byte[] imageBytes, ImageFormat format);
        ActionResult<ImageInfo> BuildModuleSampleStandard(ModuleParamsStandartData moduleParams);
    }
}
