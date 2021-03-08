﻿using CkpWebApi.InputEntities.Module;
using CkpWebApi.OutputEntities;
using CkpWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace CkpWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModulesService _modulesService;

        public ModulesController(IModulesService maketsService)
        {
            _modulesService = maketsService;
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
                var sample = _modulesService.CreateImageSample(stream.ToArray(), ImageFormat.Jpeg);

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
        public ActionResult<ImageInfo> BuildModuleSampleStandard([FromBody] ModuleParamsStandartInfo moduleParams)
        {
            if (moduleParams == null)
                return BadRequest(new { message = "Параметры модуля не переданы." });

            var sample = _modulesService.BuildModuleSampleStandard(moduleParams);

            return sample;
        }

        [HttpPost("build/module")]
        public ActionResult<ImageInfo> BuildModuleStandart([FromBody] ModuleParamsStandartInfo moduleParams)
        {
            return Ok();
        }
    }
}