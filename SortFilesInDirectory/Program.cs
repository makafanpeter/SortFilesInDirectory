using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace SortFilesInDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            var option = Parser.Default.ParseArguments<Options>(args).MapResult(SortFilesInDirectory, //in case parser sucess
                HandleParseError);
        }

        private static int HandleParseError(IEnumerable<Error> errs)
        {
            var result = -2;
            Console.WriteLine("errors {0}", errs.Count());
            if (errs.Any(x => x is HelpRequestedError or VersionRequestedError))
                result = -1;
            Console.WriteLine("Exit code {0}", result);
            return result;
        }

        private static int SortFilesInDirectory(Options opts)
        {
            var result = -2;
            if (!Directory.Exists(opts.Path))
            {
                Console.WriteLine($"Directory {opts.Path} does not exist");
                return result;
            }
            var files = Directory.GetFiles(opts.Path);

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLower();
                try
                {
                    if (ext is "")
                    {
                        ext = "others";
                    }
                    ext = ext.TrimStart('.');
                    var location = $"{opts.Path}/{ext}";
                    if (Directory.Exists(location))
                    {
                        var newFile = location  +'/'+ Path.GetFileName(file);
                        if (!File.Exists(newFile))
                        {
                            File.Move(file, newFile);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(location);
                        var newFile = location +'/'+ Path.GetFileName(file);
                        if (!File.Exists(newFile))
                        {
                            File.Move(file, newFile);
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unable to process file={file} , reason:{e.Message}");
                }
            }
            

            return result;
        }
    }
    
    
    public class Options
    {
        [Option('p', "path", Required = true, HelpText = "the path of the directory you want the files sorted.")]
        public string Path { get; set; }
    }
    
    
}