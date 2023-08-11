using System.Text.RegularExpressions;

namespace Moqstitute
{
    internal class Program
    {
        class FindReplace
        {
            public FindReplace(string find, string replace)
            {
                Find = find;
                Replace = replace;
            }

            public string Find { get; set; }
            public string Replace { get; set; }
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"arg{i}: {args[i]}");
            }

            Console.WriteLine("Reading configuration file ...");

            var findReplaceItems = new List<FindReplace>();
            var configFile = "config.txt";
            var fileContents = File.ReadAllLines(configFile).ToList();

            Console.WriteLine("Making replace list ...");

            for (int i = 0; i < fileContents.Count; i += 2)
            {
                var replace = (fileContents[i + 1] == "blank") ? "" : fileContents[i + 1];
                findReplaceItems.Add(new FindReplace(fileContents[i], replace));
            }
            
            Console.WriteLine("Getting *.cs files ...");

            var baseDir = args[0];
            var fileNames = Directory.EnumerateFiles(baseDir, "*.cs", SearchOption.AllDirectories);
            
            Console.WriteLine($"Discovered: {fileNames.Count()} files");

            foreach (var fileName in fileNames)
            {
                // Skip generated files
                if (fileName.EndsWith(".g.cs") || fileName.EndsWith("AssemblyAttributes.cs"))
                {
                    continue;
                }

                var csFile = File.ReadAllText(fileName);

                if (!csFile.Contains("Mock"))
                {
                    continue;
                }

                var csFileOriginalSize = csFile.Length;

                foreach (var findReplace in findReplaceItems)
                {
                    csFile = Regex.Replace(csFile, findReplace.Find, findReplace.Replace);
                }

                if (csFile.Length != csFileOriginalSize)
                {
                    Console.WriteLine($"Replacing Moq in: {fileName}");
                    File.WriteAllText(fileName, csFile);
                }
            }
        }
    }
}
