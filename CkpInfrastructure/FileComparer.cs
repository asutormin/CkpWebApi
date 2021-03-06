using System.IO;
using System.Security.Cryptography;

namespace CkpInfrastructure
{
    public static class FileComparer
    {
        public static bool FilesAreEqual_OneByte(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
                return false;

            //if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
            //    return true;

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                for (int i = 0; i < first.Length; i++)
                {
                    if (fs1.ReadByte() != fs2.ReadByte())
                        return false;
                }
            }

            return true;
        }

        public static bool FilesAreEqual_Hash(FileInfo first, FileInfo second)
        {
            byte[] firstHash = MD5.Create().ComputeHash(first.OpenRead());
            byte[] secondHash = MD5.Create().ComputeHash(second.OpenRead());

            for (int i = 0; i < firstHash.Length; i++)
            {
                if (firstHash[i] != secondHash[i])
                    return false;
            }
            return true;
        }
    }
}
