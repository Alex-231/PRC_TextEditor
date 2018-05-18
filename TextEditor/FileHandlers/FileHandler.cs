using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TextEditor.FileHandlers
{
    public class FileHandler
    {
        /// <summary>
        /// Stores the path of the file being handled,
        /// </summary>
        private string filePath;
        /// <summary>
        /// Stores the contents of the file being handled.
        /// </summary>
        public string[] FileLines { get; internal set; }

        /// <summary>
        /// This constructor is used for handling new files which have not been saved yet.
        /// </summary>
        public FileHandler()
        {
        }

        /// <summary>
        /// This constructor is used for handling existing files.
        /// </summary>
        /// <param name="path"></param>
        public FileHandler(string path)
        {
            filePath = path;
            ReadFileAndStoreLinesLocally();
        }

        /// <summary>
        /// Reads the file at the file handler's filePath, and writes the lines to a local fileLines String array.
        /// </summary>
        private void ReadFileAndStoreLinesLocally()
        {
            FileLines = File.ReadAllLines(filePath);
        }

        /// <summary>
        /// Writes the supplied string array to the file handler's filePath.
        /// </summary>
        /// <param name="lines"></param>
        public void SaveFileToCurrentPath(string[] lines)
        {
            File.WriteAllLines(filePath, lines);
        }
    }
}
