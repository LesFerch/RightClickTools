using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;

namespace DT2DC
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dt2dc <file specifier> [/s] [/m]");
                return;
            }

            string fileSpecifier = args[0];
            bool processSubfolders = args.Contains("/s");
            bool alsoSetModified = args.Contains("/m");

            // If no directory path is specified, assume the current directory
            string directory = string.IsNullOrEmpty(Path.GetDirectoryName(fileSpecifier)) ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(fileSpecifier);
            string searchPattern = Path.GetFileName(fileSpecifier);

            var files = Directory.EnumerateFiles(directory, searchPattern, processSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    DateTime? dateTaken = GetDateTaken(file);

                    if (dateTaken.HasValue)
                    {
                        SetCreationTime(file, dateTaken.Value);
                        if (alsoSetModified)
                        {
                            File.SetLastWriteTime(file, dateTaken.Value);
                            Console.WriteLine($"Updated 'Date created' and 'Date modified' for {file} to {dateTaken.Value}");
                        }
                        else
                        {
                            Console.WriteLine($"Updated 'Date created' for {file} to {dateTaken.Value}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: 'Date taken' property missing for {file}. Skipping.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {file}: {ex.Message}");
                }
            }
        }

        static DateTime? GetDateTaken(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                var property = shell.Properties.System.Photo.DateTaken;
                return property?.Value;
            }
        }

        static void SetCreationTime(string filePath, DateTime dateTime)
        {
            File.SetCreationTime(filePath, dateTime);
        }
    }
}
