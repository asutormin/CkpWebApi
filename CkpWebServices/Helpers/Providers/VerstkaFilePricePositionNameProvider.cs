using CkpInfrastructure.Providers.Interfaces;

namespace CkpServices.Helpers.Providers
{
    public class VerstkaFilePricePositionNameProvider : IKeyedProvider<string, string>
    {
        public string GetByValue(string formatName)
        {
            var formatNameForVerstkaFile = formatName.Replace("нед.", "").Replace("\"", "").Trim();
            var pos = formatNameForVerstkaFile.IndexOfAny(new[] { '.', ' ' });
            if (pos >= 0) formatNameForVerstkaFile = formatNameForVerstkaFile.Substring(0, pos);

            return formatNameForVerstkaFile;
        }
    }
}
