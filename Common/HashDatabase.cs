using System;
using System.Collections.Generic;
using System.IO;

namespace SlyTools.Common.Hashing
{
    public class HashDatabase
    {
        private static HashSet<uint> missingValues = new HashSet<uint>();
        private static HashSet<uint> foundValues = new HashSet<uint>();
        private static Dictionary<uint, string> stringsByHash = new Dictionary<uint, string>();
        private static Dictionary<string, uint> hashByString = new Dictionary<string, uint>();

        public static void Add(string value)
        {
            uint hash = HashString.Get(value);
            stringsByHash.Add(hash, value);
            hashByString.Add(value, hash);
        }

        public static bool LookUp(uint hash, out string value)
        {
            if (!stringsByHash.ContainsKey(hash) && !missingValues.Contains(hash))
            {
                missingValues.Add(hash);
            }
            else if (stringsByHash.ContainsKey(hash) && !foundValues.Contains(hash))
            {
                foundValues.Add(hash);
            }

            return stringsByHash.TryGetValue(hash, out value);
        }

        public static bool LookUp(string value, out uint hash)
        {
            return hashByString.TryGetValue(value, out hash);
        }

        public static void LoadFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string line = null;

                while ((line = reader.ReadLine()) != null)
                {
                    Add(line);
                }
            }
        }

        public static void WriteDiagnostics(string path)
        {
            using (StreamWriter writer = new StreamWriter(Path.Join(path, "hashes.txt")))
            {
                foreach (uint hash in missingValues)
                {
                    writer.WriteLine(hash.ToString("x8"));
                }
            }

            using (StreamWriter writer = new StreamWriter(Path.Join(path, "strings.txt")))
            {
                foreach (uint hash in foundValues)
                {
                    writer.WriteLine(stringsByHash[hash]);
                }
            }

            Console.WriteLine($"Hash Database Statitics: {missingValues.Count} missing, {foundValues.Count} found");
        }
    }
}
