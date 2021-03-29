using CkpDAL.Model.String;

namespace CkpServices.Helpers.Factories.Interfaces.String
{
    interface IStringOccurrenceFactory
    {
        StringOccurrence Create(int stringId, int occurrenceId, int typeId, int orderBy);
    }
}
