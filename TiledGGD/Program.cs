using System;
using System.Collections.Generic;
using System.Windows.Forms;

//https://www.codeproject.com/Articles/19869/Powerful-and-simple-command-line-parsing-in-C
using Plossum.CommandLine;

namespace TiledGGD
{
    [CommandLineManager(ApplicationName = "TiledGGD")]
    class Options
    {
        [CommandLineOption(Description = "Displays this help text")]
        public bool help = false;

        [CommandLineOption(Description = "Runs this application in batch mode")]
        public bool batch = false;

        [CommandLineOption(Description = "The path to graphic file")]
        public string graphicFile
        {
            get { return _graphicFile; }
            set
            {
                if (System.IO.File.Exists(value))
                {
                    _graphicFile = value;
                }
                else
                {
                    throw new InvalidOptionValueException("Input graphic file does not exist.", false);
                }
            }
        }
        private string _graphicFile;

        [CommandLineOption(Description = "The path to palette file")]
        public string paletteFile
        {
            get { return _paletteFile; }
            set
            {
                if (System.IO.File.Exists(value))
                {
                    _paletteFile = value;
                }
                else
                {
                    throw new InvalidOptionValueException("Input palette file does not exist.", false);
                }
            }
        }
        private string _paletteFile;

        [CommandLineOption(Description = "The path to output file")]
        public string outputFile = "output.png";

        [CommandLineOption(Description = "The output width")]
        public uint width = 0;

        [CommandLineOption(Description = "The output height")]
        public uint height = 0;

        [CommandLineOption(Description = "The graphics format")]
        public string graphicsFormat {
            get { return _graphicsFormat.ToString(); }
            set
            {
                _graphicsFormat = Enum.IsDefined(typeof(GraphicsFormat), value) ? (GraphicsFormat)Enum.Parse(typeof(GraphicsFormat), value) : GraphicsFormat.FORMAT_8BPP;
            }
        }
        public GraphicsFormat _graphicsFormat;

    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Options options = new Options();
            CommandLineParser parser = new CommandLineParser(options);
            parser.Parse();

            if (options.help)
            {
                Console.WriteLine(parser.UsageInfo.GetOptionsAsString(78));
                return;
            }

            var paletteData = new PaletteData(PaletteFormat.FORMAT_2BPP, PaletteOrder.BGR);
            var graphicsData = new GraphicsData(paletteData);

            if (options.batch)
            {
                generateOutput(paletteData, graphicsData, options.graphicFile, options.paletteFile, options.outputFile, options.width, options.height, options._graphicsFormat);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow(paletteData, graphicsData));
        }

        static void generateOutput(PaletteData paletteData, GraphicsData graphicsData, string graphicFile, string paletteFile, string outputFile, uint width, uint height, GraphicsFormat graphicsFormat)
        {
            GraphicsData.GraphFormat = graphicsFormat;
            if (width > 0)
            {
                GraphicsData.Width = width;
            }
            if (height > 0)
            {
                GraphicsData.Height = height;
            }
            graphicsData.load(graphicFile);
            paletteData.load(paletteFile);
            graphicsData.toBitmap().Save(outputFile, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}