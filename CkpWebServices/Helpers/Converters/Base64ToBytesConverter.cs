namespace CkpServices.Helpers.Converters
{
    static class Base64ToBytesConverter
    {
        public static byte[] Convert(string base64String)
        {
            if (base64String == null)
                return null;

            var proccessedBase64String = base64String.Substring(base64String.LastIndexOf(',') + 1);
            var bytes = System.Convert.FromBase64String(proccessedBase64String);

            return bytes;
        }
    }
}
