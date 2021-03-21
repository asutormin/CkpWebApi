using CkpEntities.Input.Module;
using CkpEntities.Output;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;

namespace CkpServices.Interfaces
{
    public interface IModulesService
    {
        ActionResult<ImageInfo> CreateImageSample(byte[] imageBytes, ImageFormat format);
        ActionResult<ImageInfo> BuildModuleSampleStandard(ModuleParamsStandartInfo moduleParams);
    }
}
