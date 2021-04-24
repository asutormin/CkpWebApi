﻿using CkpDAL.Model;
using CkpInfrastructure;
using CkpServices.Helpers.Providers;
using CkpServices.Processors.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace CkpServices.Processors
{
    class ModuleProcessor : IModuleProcessor
    {
        private readonly string _positionImSampleTemplate;
        private readonly string _positionImGraphicsFolderPathTemplate;

        private readonly OrderImSampleNameProvider _orderImSampleNameProvider;
        private readonly OrderImGraphicsFolderPathProvider _orderImGraphicsFolderPathProvider;

        public ModuleProcessor(string orderImFolderTemplate, string dbName)
        {
            var orderImFolder = string.Format(orderImFolderTemplate, dbName);

            _positionImSampleTemplate = orderImFolder + "{0}\\{1}\\{2}_{3}\\{4}.jpg";
            _positionImGraphicsFolderPathTemplate = orderImFolder + "{0}\\{1}\\Graphics";

            _orderImSampleNameProvider = new OrderImSampleNameProvider(_positionImSampleTemplate);
            _orderImGraphicsFolderPathProvider = new OrderImGraphicsFolderPathProvider(_positionImGraphicsFolderPathTemplate);
        }

        #region Create
        public void CreateSampleImage(int orderPositionId, byte[] bytes, string fileName, DateTime version)
        {
            var filePath = _orderImSampleNameProvider
                .GetByValue(new Tuple<int, DateTime, string>(orderPositionId, version, fileName));

            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory.Create();

            using (var imageFile = new FileStream(filePath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }
        }

        public void CreateModuleGraphics(int orderPositionId, byte[] bytes, string fileName)
        {
            var folderPath = _orderImGraphicsFolderPathProvider.GetByValue(orderPositionId);

            var filePath = new OrderImGraphicsFilePathProvider(folderPath)
                .GetByValue(fileName);

            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory.Create();

            File.WriteAllBytes(filePath, bytes);
        }

        #endregion

        #region Update

        public bool CanUpdateModule(PositionIm positionIm)
        {
            return positionIm != null && (positionIm.TaskFileDate != null || positionIm.MaketFileDate != null);
        }

        public void UpdateModuleGraphics(int orderPositionId, byte[] bytes, string fileName)
        {
            var updateDate = DateTime.Now;

            var folderPath = _orderImGraphicsFolderPathProvider.GetByValue(orderPositionId);

            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            if (directoryInfo.Exists)
            {
                FileInfo[] existsFiles = directoryInfo.GetFiles();

                // Проверяем, есть ли в папке файлы с таким же названием
                var updateFileInfo = existsFiles.Where(f => f.Name == fileName).SingleOrDefault();

                // Если файл с таким же названием не найден
                if (updateFileInfo == null)
                {
                    // Помечаем все файлы как удалённые.
                    foreach (var fileInfo in existsFiles)
                    {
                        var existsFilePathToDelete = new OrderImDeletedFileNameProvider(fileInfo.FullName)
                            .GetByValue(DateTime.Now);
                        File.Move(fileInfo.FullName, existsFilePathToDelete);
                    }

                    // Создаём новый файл
                    CreateModuleGraphics(orderPositionId, bytes, fileName);
                }
                else
                {
                    // Получаем временное название файла
                    var tmpFilePath = folderPath + "\\~" + updateFileInfo.Name;

                    File.WriteAllBytes(tmpFilePath, bytes);

                    var tmpFileInfo = new FileInfo(tmpFilePath);

                    if (FileComparer.FilesAreEqual_OneByte(updateFileInfo, tmpFileInfo))
                    {
                        File.Delete(tmpFilePath);
                        return;
                    }

                    var existsFilePathToDelete = new OrderImDeletedFileNameProvider(updateFileInfo.FullName)
                        .GetByValue(updateDate);
                    File.Move(updateFileInfo.FullName, existsFilePathToDelete);

                    File.Move(tmpFilePath, updateFileInfo.FullName);

                    // Помечаем все остальные файлы как удалённые.
                    var filesToDelete = existsFiles.Where(f => f.Name != fileName);

                    foreach (var fileInfo in filesToDelete)
                    {
                        var filePathToDelete = new OrderImDeletedFileNameProvider(fileInfo.FullName)
                            .GetByValue(updateDate);
                        File.Move(fileInfo.FullName, filePathToDelete);
                    }
                }
            }
            else
                CreateModuleGraphics(orderPositionId, bytes, fileName);
        }

        #endregion

        #region Delete

        public void DeleteModuleGraphics(int orderPositionId)
        {
            var folderPath = new OrderImGraphicsFolderPathProvider(_positionImGraphicsFolderPathTemplate)
                .GetByValue(orderPositionId);

            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            if (directoryInfo.Exists)
            {
                FileInfo[] existsFiles = directoryInfo.GetFiles();

                // Помечаем все файлы как удалённые.
                foreach (var fileInfo in existsFiles)
                {
                    var existsFilePathToDelete = new OrderImDeletedFileNameProvider(fileInfo.FullName)
                        .GetByValue(DateTime.Now);
                    File.Move(fileInfo.FullName, existsFilePathToDelete);
                }
            }
        }

        #endregion
    }
}