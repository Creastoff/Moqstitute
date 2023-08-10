using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data.Common;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.Console;
using Microsoft.CodeAnalysis.Operations;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;

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
            var configFile = @".\config.txt";
            var fileContents = File.ReadAllLines(configFile).ToList();

            var findReplaceItems = new List<FindReplace>();

            for (int i = 0; i < fileContents.Count; i += 2)
            {
                var replace = (fileContents[i + 1] == "blank") ? "" : fileContents[i + 1];
                findReplaceItems.Add(new FindReplace(fileContents[i], replace));
            }
            
            var baseDir = args[0];
            var files = Directory.EnumerateFiles(baseDir, "*.cs", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                var csFile = File.ReadAllText(file);

                foreach (var findReplace in findReplaceItems)
                {
                    csFile = Regex.Replace(csFile, findReplace.Find, findReplace.Replace);
                }

                File.WriteAllText(file, csFile);
            }
        }
    }
}