using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yahoo.Yui.Compressor;

namespace HTML5Compiler.Helpers
{
    public static class FileManagment
    {
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool minify)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                return;
                //throw new DirectoryNotFoundException("El directorio de origen no existe: "+ sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = System.IO.Path.Combine(destDirName, file.Name);

                // Copy the file.
                if (minify)
                {
                    // minify
                    string fileExtension = System.IO.Path.GetExtension(temppath);
                    string sFile = string.Empty;

                    switch (fileExtension)
                    {
                        case ".js":
                            if (file.Name.IndexOf("min") > 0)
                            {
                                file.CopyTo(temppath, true);
                            }
                            else
                            {
                                JavaScriptCompressor oJsCompressor = new JavaScriptCompressor();
                                oJsCompressor.Encoding = Encoding.UTF8;
                                //oJsCompressor.IgnoreEval = true;
                                oJsCompressor.ThreadCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                                oJsCompressor.DisableOptimizations = true;
                                using (var inputFile = new StreamReader(file.FullName))
                                {
                                    sFile = inputFile.ReadToEnd();
                                }
                                try
                                {
                                    sFile = oJsCompressor.Compress(sFile);
                                    CreateFileFromString(sFile, temppath);
                                }
                                catch
                                {
                                    file.CopyTo(temppath, true);
                                }
                            }
                            #region other minimifier methods
                            // ajax minifier
                            /*var minifier = new Minifier();
                            var settings  =  new CodeSettings();
                            try
                            {
                                sFile = minifier.MinifyJavaScript(sFile, settings);
                                CreateFileFromString(sFile, temppath);
                            }
                            catch (Exception ex) {
                                file.CopyTo(temppath, true);
                            }*/
                            // packer
                            /*ECMAScriptPacker p = new ECMAScriptPacker(ECMAScriptPacker.PackerEncoding.Normal, true, true);
                            try {
                                sFile = p.Pack(sFile).Replace("\n", "\r\n");
                                CreateFileFromString(sFile, temppath);
                            } catch (Exception ex) {
                                file.CopyTo(temppath, true);
                            }*/
                            #endregion
                            break;
                        case ".css":
                            CssCompressor oCssCompressor = new CssCompressor();
                            oCssCompressor.CompressionType = CompressionType.Standard;
                            sFile = GetStringFromFileInfo(file);
                            sFile = oCssCompressor.Compress(sFile);
                            CreateFileFromString(sFile, temppath);
                            break;
                        default:
                            file.CopyTo(temppath, true);
                            break;
                    }
                }
                else
                {
                    file.CopyTo(temppath, true);
                }
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, minify);
                }
            }
        }

        public static void CreateFileFromString(string sContent, string destinationPath)
        {
            // Borro el archivo si existe
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            // Creo el archivo
            using (FileStream fs = File.Create(destinationPath))
            {
                Byte[] content = new UTF8Encoding(true).GetBytes(sContent);
                // Escribo en el archivo
                fs.Write(content, 0, content.Length);
            }
        }

        public static string GetStringFromFileInfo(FileInfo file)
        {
            string sFile = string.Empty;
            //Open the file to read from.
            using (StreamReader sr = file.OpenText())
            {
                string s = string.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    sFile = string.Concat(sFile, s);
                }
            }
            return sFile;
        }
    }
}
