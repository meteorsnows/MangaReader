﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MangaReader
{
    class Comperssion
    {
        /// <summary>
        /// Упаковка всех глав.
        /// </summary>
        /// <param name="message">Папка манги.</param>
        public static void ComperssChapters(string message)
        {
            message = Page.MakeValidPath(message);
            if (!Directory.Exists(message))
                return;
            var volumes = Directory.GetDirectories(message);
            foreach (var volume in volumes)
            {
                Directory.SetCurrentDirectory(volume);
                var chapters = Directory.GetDirectories(".\\");
                foreach (var chapter in chapters)
                {
                    var acr = string.Concat(GetFolderName(message), "_", GetFolderName(volume), "_", GetFolderName(chapter), ".zip");
                    if (File.Exists(acr))
                        AddToArchive(acr, chapter);
                    else
                        ZipFile.CreateFromDirectory(chapter, acr, CompressionLevel.NoCompression, false, Encoding.UTF8);
                    Directory.Delete(chapter, true);
                }
            }
        }

        /// <summary>
        /// Упаковка всех томов.
        /// </summary>
        /// <param name="message">Папка манги.</param>
        public static void ComperssVolumes(string message)
        {
            message = Page.MakeValidPath(message);
            if (!Directory.Exists(message))
                return;
            Directory.SetCurrentDirectory(message);
            var volumes = Directory.GetDirectories(".\\");
            foreach (var volume in volumes)
            {
                var acr = string.Concat(GetFolderName(message), "_", GetFolderName(volume), ".zip");
                if (File.Exists(acr))
                    AddToArchive(acr, volume);
                else
                    ZipFile.CreateFromDirectory(volume, acr, CompressionLevel.NoCompression, false, Encoding.UTF8);
                Directory.Delete(volume, true);
            }
        }

        /// <summary>
        /// Вернуть название папки.
        /// </summary>
        /// <param name="path">Путь к папке. Абсолютный или относительный.</param>
        /// <returns>Название папки. '.\Folder\' -> 'Folder'</returns>
        private static string GetFolderName(string path)
        {
            return path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                            StringSplitOptions.RemoveEmptyEntries)
                       .Last();
        }

        /// <summary>
        /// Добавить файлы в архив.
        /// </summary>
        /// <param name="archive">Существующий архив.</param>
        /// <param name="folder">Папка, файлы которой необходимо запаковать.</param>
        private static void AddToArchive(string archive, string folder)
        {
            using (var zip = ZipFile.Open(archive, ZipArchiveMode.Update, Encoding.UTF8))
            foreach (var file in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
            {
                var fileName = file.Replace(folder+ "\\", string.Empty);
                var fileInZip = zip.Entries.FirstOrDefault(f => f.FullName == fileName);
                if (fileInZip != null)
                    fileInZip.Delete();
                zip.CreateEntryFromFile(file, fileName, CompressionLevel.NoCompression);
            }
        }
    }
}