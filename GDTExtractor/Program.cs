using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhilUtil.PrintUtil;
using System.Reflection;

namespace GDTExtractor
{
    class Program
    {
        static readonly string TA_TOOLS_PATH = Environment.GetEnvironmentVariable("TA_TOOLS_PATH");

        static readonly Dictionary<int, string> AssetLocations = new Dictionary<int, string>()
        {
            { 1, "model_export" },
            { 2, "xanim_export" },
            { 3, "sound_assets" },
        };

        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            Print.PrintLine("╔═══════════════════════════════╗", "INIT");
            Print.PrintLine("║ AssetExtractor by Scobalula   ║", "INIT");
            Print.PrintLine("║                               ║", "INIT");
            Print.PrintLine("║ Usage:                        ║", "INIT");
            Print.PrintLine("║  Drag and drop GDTs and/or    ║", "INIT");
            Print.PrintLine("║  alias CSVs.                  ║", "INIT");
            Print.PrintLine("║                               ║", "INIT");
            Print.PrintLine("║ Version:                      ║", "INIT");
            Print.PrintLine("║  1.0 Doggo Enhanced Alpha     ║", "INIT");
            Print.PrintLine("╚═══════════════════════════════╝", "INIT");

            int processedFiles = 0;

            foreach(string file in args)
            {
                try
                {
                    if (!File.Exists(file))
                    {
                        continue;
                    }

                    if (Path.GetExtension(file) == ".gdt")
                    {
                        processedFiles++;
                        ExtractGDTAssets(file);
                    }
                    else if (Path.GetExtension(file) == ".csv")
                    {
                        processedFiles++;
                        ExtractAliasAssets(file);
                    }
                }
                catch(Exception e)
                {
                    Print.PrintLine(e, "ERROR", 12, ConsoleColor.Red);
                }
            }

            if(processedFiles == 0)
            {
                Print.PrintLine("No valid files given", "ERROR", 12, ConsoleColor.Red);
            }


            Print.PrintLine("Execution complete, press Enter to exit.", "COMPLETE");

            Console.ReadLine();
        }

        static void ExtractAliasAssets(string fileName)
        {
            Print.PrintLine(String.Format("Processing {0}", Path.GetFileName(fileName)), "INFO");

            string outputDir = "output";

            string[] lines = File.ReadAllLines(fileName);

            if(lines.Length > 0)
            {
                string[] headerSplit = lines[0].Split(',');

                int fileSpecIndex           = Array.IndexOf(headerSplit, "FileSpec");
                int fileSpecSustainIndex    = Array.IndexOf(headerSplit, "FileSpecSustain");
                int fileSpecReleaseIndex    = Array.IndexOf(headerSplit, "FileSpecRelease");


                if (fileSpecIndex == -1)
                {
                    Print.PrintLine("Alias file is missing required column \"FileSpec\"", "ERROR", 12, ConsoleColor.Red);
                    return;
                }

                string newFilePath = Path.Combine(outputDir, fileName.Replace(TA_TOOLS_PATH, ""));

                PhilUtil.PathUtil.CreateFilePath(newFilePath);

                try
                {
                    File.Copy(fileName, newFilePath, true);
                }
                catch
                {
                    Print.PrintLine("Failed to copy CSV, file requires manual copying.", "ERROR", 12, ConsoleColor.Red);
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    Print.PrintLine(String.Format("Processing Alias: {0}", lines[i].Split(',')[0]), "SNDUtil");

                    try
                    {
                        if (fileSpecIndex >= 0)
                        {
                            string file = lines[i].Split(',')[fileSpecIndex];

                            if (String.IsNullOrWhiteSpace(file))
                                continue;

                            Print.PrintLine(String.Format("Copying: {0}", file), "SNDUtil");

                            string fullPath = Path.Combine(TA_TOOLS_PATH, AssetLocations[3], file);
                            string newPath = Path.Combine("output", "sound_assets", file);

                            PhilUtil.PathUtil.CreateFilePath(newPath);


                            File.Copy(fullPath, newPath, true);
                        }

                        if (fileSpecSustainIndex >= 0)
                        {
                            string file = lines[i].Split(',')[fileSpecSustainIndex];

                            if (String.IsNullOrWhiteSpace(file))
                                continue;

                            Print.PrintLine(String.Format("Copying: {0}", file), "SNDUtil");

                            string fullPath = Path.Combine(TA_TOOLS_PATH, "sound_assets", file);
                            string newPath = Path.Combine("output", "sound_assets", file);

                            PhilUtil.PathUtil.CreateFilePath(newPath);

                            File.Copy(fullPath, newPath, true);
                        }

                        if (fileSpecReleaseIndex >= 0)
                        {
                            string file = lines[i].Split(',')[fileSpecReleaseIndex];

                            if (String.IsNullOrWhiteSpace(file))
                                continue;

                            Print.PrintLine(String.Format("Copying: {0}", file), "SNDUtil");

                            string fullPath = Path.Combine(TA_TOOLS_PATH, "sound_assets", file);
                            string newPath = Path.Combine("output", "sound_assets", file);

                            PhilUtil.PathUtil.CreateFilePath(newPath);

                            File.Copy(fullPath, newPath, true);
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        Print.PrintLine("Could not find file assigned to sound.", "ERROR", 12, ConsoleColor.Red);
                    }
                    catch (Exception e)
                    {
                        Print.PrintLine(e.Message, "ERROR", 12, ConsoleColor.Red);
                    }
                }
            }
        }

        static void ExtractGDTAssets(string fileName)
        {

            Print.PrintLine(String.Format("Processing {0}", Path.GetFileName(fileName)), "INFO");


            string outputDir = "output";

            string assetName;

            string newFilePath = Path.Combine(outputDir, fileName.Replace(TA_TOOLS_PATH, ""));

            PhilUtil.PathUtil.CreateFilePath(newFilePath);

            try
            {
                File.Copy(fileName, newFilePath, true);
            }
            catch
            {
                Print.PrintLine("Failed to copy GDT, file requires manual copying.", "ERROR", 12, ConsoleColor.Red);
            }

            string[] lines = File.ReadAllLines(fileName);

            int state = 0;

            foreach (string line in lines)
            {
                try
                {
                    string[] lineSplit = line.Split().Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    if (lineSplit.Length < 2)
                        continue;


                    if (line.Contains("xmodel.gdf"))
                    {
                        assetName = line.Trim().Split()[0].Replace("\"", "");
                        Print.PrintLine(String.Format("Copying xModel : {0}", assetName), "GDTUtil");
                        state = 1;
                        continue;
                    }
                    else if (line.Contains("xanim.gdf"))
                    {
                        assetName = line.Trim().Split()[0].Replace("\"", "");
                        Print.PrintLine(String.Format("Copying xAnim  : {0}", assetName), "GDTUtil");
                        state = 2;
                        continue;
                    }
                    else if (line.Contains("image.gdf"))
                    {
                        assetName = line.Trim().Split()[0].Replace("\"", "");
                        Print.PrintLine(String.Format("Copying xImage : {0}", assetName), "GDTUtil");
                        continue;
                    }


                    if (lineSplit[0] == "\"filename\"")
                    {
                        string path = Path.Combine(TA_TOOLS_PATH, AssetLocations[state], lineSplit[1].Trim('"'));
                        string npath = Path.Combine(outputDir, AssetLocations[state], lineSplit[1].Trim('"'));

                        Directory.CreateDirectory(Path.GetDirectoryName(npath));

                        File.Copy(path, npath, true);
                    }
                    else if (lineSplit[0] == "\"model\"")
                    {
                        string path = Path.Combine(TA_TOOLS_PATH, AssetLocations[1], lineSplit[1].Trim('"'));
                        string npath = Path.Combine(outputDir, AssetLocations[1], lineSplit[1].Trim('"'));

                        Directory.CreateDirectory(Path.GetDirectoryName(npath));

                        File.Copy(path, npath, true);
                    }
                    else if (lineSplit[0] == "\"baseImage\"")
                    {
                        string path = Path.Combine(TA_TOOLS_PATH, lineSplit[1].Trim('"'));
                        string npath = Path.Combine(outputDir, lineSplit[1].Trim('"'));
                        Console.WriteLine(path);
                        Directory.CreateDirectory(Path.GetDirectoryName(npath));

                        File.Copy(path, npath, true);
                    }
                }
                catch (FileNotFoundException)
                {
                    Print.PrintLine("Could not find file assigned to xAsset", "ERROR", 12, ConsoleColor.Red);
                }
                catch (Exception e)
                {
                    Print.PrintLine(e.Message, "ERROR", 12, ConsoleColor.Red);
                }
            }
        }
    }
}
