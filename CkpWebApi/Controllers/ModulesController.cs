using CkpModel.Input.Module;
using CkpModel.Output;
using CkpServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.IO;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet("sample/{orderPositionId}")]
        public ActionResult<ImageInfo> GetSampleById(int orderPositionId)
        {
            var sample = _moduleService.GetSampleImageById(orderPositionId);

            return sample;
        }

        [HttpPost("create/sample")]
        [DisableRequestSizeLimit]
        public ActionResult<ImageInfo> CreateSampleFromFile(IFormFile module)
        {
            if (module == null)
                return BadRequest("module is null");

            if (module.Length == 0)
                return BadRequest("module.Length == 0");
            
            using (var stream = new MemoryStream())
            {
                module.CopyTo(stream);
                var sample = _moduleService.CreateImageSample(stream.ToArray(), ImageFormat.Jpeg);

                return sample;
            }
        }

        /*
        [HttpPost("save")]
        public ActionResult<string> SaveModuleToDisk(IFormFile module)
        {
            if (module == null)
                return BadRequest("module is null");

            if (module.Length == 0)
                return BadRequest("module.Length == 0");

            using (var stream = module.OpenReadStream())
            {
                var length = (int)module.Length;
                byte[] bytes = new byte[length];
                stream.Read(bytes, 0, length);

                var fileId = _modulesService.SaveModuleToDisk(bytes);

                return fileId;
            }

        }

        [HttpPost("delete/{fileId}")]
        public ActionResult DeleteModuleFromDisk(string fileId)
        {
            if (fileId == null)
                return BadRequest("fileId is null");

            _modulesService.DeleteModuleFromDisk(fileId);

            return Ok();
        }
        */
        [HttpPost("build/sample/standard")]
        public ActionResult<ImageInfo> BuildModuleSampleStandard([FromBody] ModuleParamsStandartData moduleParams)
        {
            if (moduleParams == null)
                return BadRequest(new { message = "Параметры модуля не переданы." });

            var sample = _moduleService.BuildModuleSampleStandard(moduleParams);

            return sample;
        }

        [HttpPost("build/module")]
        public ActionResult<ImageInfo> BuildModuleStandart([FromBody] ModuleParamsStandartData moduleParams)
        {
            return Ok();
        }
    }
}
