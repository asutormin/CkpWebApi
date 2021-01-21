using CkpWebApi.DAL.Model;
using CkpWebApi.Helpers;
using CkpWebApi.Helpers.FileNameProviders;
using CkpWebApi.Helpers.Providers;
using CkpWebApi.Infrastructure;
using CkpWebApi.Infrastructure.Providers;
using CkpWebApi.InputEntities;
using CkpWebApi.InputEntities.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

namespace CkpWebApi.Services
{
    public partial class AdvertisementService
    {
        private void ProccessFilesInVerstkaFolder(int orderPositionId, byte[] module)
        {
            var orderPosition = _context.OrderPositions
                .Include(op => op.PricePosition)
                .Include(op => op.RubricPositions).ThenInclude(rp => rp.Rubric)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Single(op => op.Id == orderPositionId);

            var graphics = orderPosition.GraphicPositions
                .Where(gp => gp.Graphic.ClosingDate > DateTime.Now)
                .Select(gp => gp.Graphic);

            var fileNameBuilder = new VerstkaFileNameBuilder
            {
                ClientNameProvider = new VerstkaFileClientNameProvider(new StringWithoutBadSymbolsProvider(), maxLength: 4),
                PricePositionNameProvider = new VerstkaFilePricePositionNameProvider(),
                VerstkaIdProvider = new VerstkaFileIdProvider(symbolCount: 6)
            };

            var verstkafileName = fileNameBuilder.Build(orderPosition);

            foreach (var graphic in graphics)
            {
                var verstkaFolderPath = string.Format(_verstkaFolderPathTemplate,
                    graphic.OutDate.Year,
                    graphic.Number,
                    graphic.Supplier.City.Name);

                var verstkaFilePath = string.Format("{0}\\{1}.tiff", verstkaFolderPath, verstkafileName);

                // Сохраняем файл
                if (module != null)
                    File.WriteAllBytes(verstkaFilePath, module);
                else
                    File.Delete(verstkaFilePath);
            }
        }

        #region Create

        private void CreateModulePositionIm(int orderId, int orderPositionId, byte[] bytes, DbTransaction dbTran)
        {
            var taskFileDate = CreateModulePositionImTaskVersion(orderId, orderPositionId, dbTran);
            if (taskFileDate != null)
                CreateSampleImage(orderPositionId, (DateTime)taskFileDate, bytes, "ImgTask");

            var maketFileDate = CreateModulePositionImMaketVersion(orderId, orderPositionId, taskFileDate, dbTran);
            if (maketFileDate != null)
                CreateSampleImage(orderPositionId, (DateTime)maketFileDate, bytes, "ImgMaket");
        }

        private void CreateSampleImage(int orderPositionId, DateTime version, byte[] bytes, string name)
        {
            var filePath = new OrderImSampleNameProvider(_positionImSampleTemplate)
                .GetByValue(new Tuple<int, DateTime, string>(orderPositionId, version, name));

            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory.Create();

            using (var imageFile = new FileStream(filePath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }
        }

        private void CreateModule(int orderPositionId, byte[] bytes)
        {
            var filePath = new OrderImFileNameProvider(_positionImFileTemplate)
                .GetByValue(orderPositionId);

            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory.Create();

            File.WriteAllBytes(filePath, bytes);
        }

        private string ProccessBase64String(string base64String)
        {
            return base64String.Substring(base64String.LastIndexOf(',') + 1);
        }

        private byte[] GetModuleBytes(string base64String)
        {
            var proccessedBase64String = ProccessBase64String(base64String);
            var bytes = Convert.FromBase64String(proccessedBase64String);

            return bytes;
        }

        private void CreateFileInVerstkaFolder(int orderPositionId, byte[] module)
        {
            ProccessFilesInVerstkaFolder(orderPositionId, module);
        }

        #endregion

        #region Update

        private bool CanUpdateModule(PositionIm positionIm)
        {
            return positionIm.TaskFileDate != null || positionIm.MaketFileDate != null;
        }

        private void UpdateModule(int orderPositionId, DateTime updateDate, AdvModule advModule)
        {
            var existsFilePath = new OrderImFileNameProvider(_positionImFileTemplate)
                .GetByValue(orderPositionId);

            var tmpFilePath = new OrderImFileNameProvider(_positionImTmpFileTemplate)
                .GetByValue(orderPositionId);
            
            var bytes = GetModuleBytes(advModule.Base64String);
            File.WriteAllBytes(tmpFilePath, bytes);

            var existsFileInfo = new FileInfo(existsFilePath);
            var tmpFileInfo = new FileInfo(tmpFilePath);
            if (FileComparer.FilesAreEqual_OneByte(existsFileInfo, tmpFileInfo))
            {
                File.Delete(tmpFilePath);
                return;
            }

            var existsFilePathToDelete = new OrderImDeletedFileNameProvider(_positionImFileTemplate)
                .GetByValue(updateDate);
            File.Move(existsFilePath, existsFilePathToDelete);

            File.Move(tmpFilePath, existsFilePath);

            //CreateFileInVerstkaFolder(orderPositionId, advModule.Bytes);
        }

        #endregion

        #region Delete

        private void DeleteModule(int orderPositionId, DateTime deleteDate)
        {
            var existsFilePath = new OrderImFileNameProvider(_positionImFileTemplate)
                .GetByValue(orderPositionId);

            if (!File.Exists(existsFilePath))
                return;

            var deletedFilePath = new OrderImDeletedFileNameProvider(existsFilePath)
                .GetByValue(deleteDate);

            File.Move(existsFilePath, deletedFilePath);

            //DeleteFileInVerstkaFolder(orderPositionId);
        }

        private void DeleteFileInVerstkaFolder(int orderPositionId)
        {
            ProccessFilesInVerstkaFolder(orderPositionId, null);
        }

        #endregion
    }
}
