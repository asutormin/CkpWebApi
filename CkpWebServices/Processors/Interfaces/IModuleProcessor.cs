using CkpDAL.Entities;
using System;

namespace CkpServices.Processors.Interfaces
{
    public interface IModuleProcessor
    {
        byte[] GetImageTaskBytesById(int orderPositionId);
        void CreateSampleImage(int orderPositionId, byte[] bytes, string name, DateTime version);
        void CreateModuleGraphics(int orderPositionId, byte[] bytes, string fileName);
        bool CanUpdateModule(PositionIm positionIm);
        void UpdateModuleGraphics(int orderPositionId, byte[] bytes, string fileName);
        void DeleteModuleGraphics(int orderPositionId);
    }
}
