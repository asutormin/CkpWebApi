using CkpWebApi.InputEntities.Module;
using CkpWebApi.OutputEntities;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;

namespace CkpWebApi.Services.Interfaces
{
    public interface IModulesService
    {
        //ActionResult<string> SaveModuleToDisk(byte[] moduleBytes);
        //void DeleteModuleFromDisk(string path);
        //ActionResult<ImageInfo> CreateSample(byte[] maketBytes, ImageFormat format);
        ActionResult<ImageInfo> CreateImageSample(byte[] imageBytes, ImageFormat format);
        ActionResult<ImageInfo> BuildModuleSampleStandard(ModuleParamsStandartInfo moduleParams);
    }
}
