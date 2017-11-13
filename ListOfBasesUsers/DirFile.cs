using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace ListOfBasesUsers
{
    internal class DirFile
    {

        internal string Path { get; }

        #region constructors

        public DirFile()
        {
        }

        public DirFile(string path)
        {
            Path = path;
        }

        #endregion

        #region internal methods

        internal List<string> GetListUsers()
        {

            List<string> list = new List<string>();

            DirectoryInfo dirInfo = new DirectoryInfo(DefaultValues.pathUsers);

            foreach (DirectoryInfo item in dirInfo.GetDirectories())
            {

                if (
                    (item.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden
                    || 
                    (item.Attributes & FileAttributes.System) == FileAttributes.System
                    )
                    continue;

                list.Add(item.Name);

            }

            list.Sort(delegate(string x, string y)
            {
                return x.CompareTo(y);
            }
            );

            return list;

        }

        internal ulong GetDirSize() => CalculateSize(Path);

        internal string GetSizeFormat(ulong DirSize)
        {

            if (DirSize < 1024)
                return (DirSize).ToString("F0") + " bytes";

            else if ((DirSize >> 10) < 1024)
                return (DirSize / (float)1024).ToString("F1") + " KB";

            else if ((DirSize >> 20) < 1024)
                return ((DirSize >> 10) / (float)1024).ToString("F1") + " MB";

            else if ((DirSize >> 30) < 1024)
                return ((DirSize >> 20) / (float)1024).ToString("F1") + " GB";

            else if ((DirSize >> 40) < 1024)
                return ((DirSize >> 30) / (float)1024).ToString("F1") + " TB";

            else if ((DirSize >> 50) < 1024)
                return ((DirSize >> 40) / (float)1024).ToString("F1") + " PB";

            else
                return ((DirSize >> 50) / (float)1024).ToString("F0") + " EB";

        }

        internal void OpenDirectory()
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"\"{Path}\""));
        }

        internal Process OpenDirectory(bool returnProcess)
        {

            Process process = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("explorer.exe", $"\"{Path}\"")
            };

            return process;

        }

        internal void DeleteCatalogCache()
        {
            DeleteDir(Path);
        }

        internal void DeleteCatalogCache(List<RowBase> list)
        {
            foreach (RowBase item in list)
            {
                DeleteDir(item.PathCacheLocal);
                DeleteDir(item.PathCacheAppData);
            }
        }

        #endregion

        #region private methods

        private void DeleteDir(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                DeleteSubDir(path);
                try
                {
                    Directory.Delete(path);
                }
                catch (Exception)
                {
                }
            }
        }

        private void DeleteSubDir(string path)
        {

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                }
            }
            foreach (DirectoryInfo currentSubDir in dirInfo.GetDirectories())
            {
                try
                {
                    DeleteSubDir(currentSubDir.FullName);
                    currentSubDir.Delete(true);
                }
                catch (Exception)
                {
                }
            }

        }

        private ulong CalculateSize(string path)
        {
            ulong size = 0;

            foreach (string files in Directory.GetFiles(path))
                size += (ulong)new FileInfo(files).Length;

            foreach (string dir in Directory.GetDirectories(path))
                size += CalculateSize(dir);

            return size;
        }

        #endregion

    }
}
