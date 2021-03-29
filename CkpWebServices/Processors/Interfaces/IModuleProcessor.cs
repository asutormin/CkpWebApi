using CkpDAL.Model;
using System;

namespace CkpServices.Processors.Interfaces
{
    interface IModuleProcessor
    {
        void CreateSampleImage(int orderPositionId, byte[] bytes, string name, DateTime version);
        void CreateModuleGraphics(int orderPositionId, byte[] bytes, string fileName);
        bool CanUpdateModule(PositionIm positionIm);
        void UpdateModuleGraphics(int orderPositionId, byte[] bytes, string fileName);
        void DeleteModuleGraphics(int orderPositionId);
    }
}
