using CkpDAL.Entities.String;
using CkpModel.Output.String;
using System.Collections.Generic;

namespace CkpServices.Processors.Interfaces
{
    public interface IHandbookProcessor
    {
        string GetEducation(int educationId);
        List<Handbook> GetEducations();
        string GetExperience(int experienceId);
        List<Handbook> GetExperiences();
        string GetWorkGraphic(int workGraphicId);
        List<Handbook> GetWorkGraphics();
        string GetCurrency(int currencyId);
        List<Handbook> GetCurrencies();
    }
}
