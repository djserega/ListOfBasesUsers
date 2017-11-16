using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace ListOfBasesUsers
{
    internal class ListIbases
    {

        private bool _tryGetAccess;
        private string _nameUser;
        private string _nameRowIsNotList = "нет в списке";

        public ListIbases(string nameUser)
        {
            _nameUser = nameUser;
        }

        public List<RowBase> GetListBases(MainWindow main)
        {

            List<RowBase> list = new List<RowBase>();

            string pathIbases = DefaultValues.GetNameFileIbases(_nameUser);
            string pathUserDir = DefaultValues.GetPathUserDir(_nameUser);

            try
            {

                new DirectoryInfo(pathUserDir).GetAccessControl();

                if (File.Exists(pathIbases))
                {
                    ReadFileIbases(list, pathIbases);
                    RemoveIncorrectRows(list);
                }
                else
                    throw new DirectoryNotFoundException(pathIbases);

            }
            catch (DirectoryNotFoundException)
            {
                Dialog.ShowMessage($"Файл списка баз не существует.");
            }
            catch (UnauthorizedAccessException)
            {

                if (_tryGetAccess)
                    Dialog.ShowMessage($"Нет доступа к файлу списка баз: " +
                        $"\n{pathIbases}");

                else if (Dialog.DialogQuestion($"Нет доступа к файлу списка баз: " +
                    $"\n{pathIbases}" +
                    $"\nПопробовать получить доступ?"))
                {

                    _tryGetAccess = true;

                    new Access(pathUserDir).TryGetAccess();

                    Thread.Sleep(1 * 1000);

                    main.Activate();

                    Dialog.ShowMessage("Вы уже должны были получить запрос на получение доступа в каталог выбранного пользователя." +
                            "\nПосле закрытия этого окна будет выполнена попытка чтения данных.");

                    GetListBases(main);

                    //Process process = new Access(pathUserDir).TryGetAccess();

                    //if (process != null)
                    //{
                    //    process.EnableRaisingEvents = true;
                    //    process.Exited += Process_Exited;
                    //    process.Start();
                    //    process.WaitForExit();
                    //    Dialog.ShowMessage("Вы уже должны были получить запрос на получение доступа." +
                    //        "\nПосле закрытия каталога будет выполнен повторный запрос на получение данных.");
                    //}
                    //else
                    //    GetListBases();

                }

            }
            catch (Exception ex)
            {
                Dialog.ShowMessage("Произошла непредвиденная ошибка.");
                Dialog.ShowMessage(ex.Message);
            }

            return list;

        }

        //private void Process_Exited(object sender, EventArgs e)
        //{
        //    GetListBases();
        //}

        private static void ReadFileIbases(List<RowBase> list, string pathIbases)
        {
            using (StreamReader stream = new StreamReader(pathIbases))
            {

                RowBase rowBase = null;

                string rowFile = null;
                do
                {

                    rowFile = stream.ReadLine();

                    if (rowFile != null)
                    {

                        if (rowFile.StartsWith("["))
                        {

                            string name = rowFile.Replace("[", "").Replace("]", "");

                            rowBase = new RowBase()
                            {
                                Name = name,
                                DateCreate = DateTime.MaxValue
                            };

                            list.Add(rowBase);

                        }
                        else if (rowBase != null)
                        {

                            string[] arrDataRow = rowFile.Split('=');

                            switch (arrDataRow[0])
                            {
                                case "ID":
                                    rowBase.ID = arrDataRow[1];
                                    break;
                                case "Folder":
                                    rowBase.Folder = arrDataRow[1];
                                    break;
                                case "Connect":
                                    rowBase.Connect = rowFile.Replace("Connect=", "");
                                    break;
                                case "App":
                                    rowBase.App = arrDataRow[1];
                                    break;
                                case "Version":
                                    rowBase.Version = arrDataRow[1];
                                    break;
                            }

                        }

                    }

                }
                while (rowFile != null);

            }
        }

        #region ReadCache

        public void ReadCache(ref IEnumerable<RowBase> list, ref ulong totalByte, ref string total)
        {

            List<RowBase> listCache = list.ToList();

            totalByte = 0;
            total = "";

            for (int i = 0; i < listCache.Count; i++)
            {
                listCache[i].SizeByte = totalByte;
                listCache[i].SizeLocal = total;
                listCache[i].SizeAppData = total;
            }

            ReadCacheLocal(ref list, ref totalByte, ref total, listCache);
            ReadCacheAppData(ref list, ref totalByte, ref total, listCache);

        }

        private void ReadCacheLocal(ref IEnumerable<RowBase> list, ref ulong totalByte, ref string total, List<RowBase> listCache)
        {
            string pathCache = DefaultValues.GetNameDirCacheLocal(_nameUser);

            ReadCachePath(ref list, ref totalByte, ref total, listCache, pathCache, TypeCache.Local);
        }

        private void ReadCacheAppData(ref IEnumerable<RowBase> list, ref ulong totalByte, ref string total, List<RowBase> listCache)
        {
            string pathCache = DefaultValues.GetNameDirCacheAppData(_nameUser);

            ReadCachePath(ref list, ref totalByte, ref total, listCache, pathCache, TypeCache.AppData);
        }

        private void ReadCachePath(ref IEnumerable<RowBase> list, ref ulong totalByte, ref string total, List<RowBase> listCache, string path, TypeCache typeCache)
        {
            try
            {

                DirectoryInfo[] dirCache = new DirectoryInfo(path).GetDirectories();

                foreach (DirectoryInfo dir in dirCache)
                {

                    string nameDir = dir.Name;

                    if (nameDir.Count(f => f == '-') == 4)
                    {

                        RowBase rowBase = listCache.FirstOrDefault(f => f.ID == nameDir);

                        if (rowBase == null)
                        {
                            rowBase = new RowBase()
                            {
                                Name = _nameRowIsNotList,
                                ID = nameDir,
                                DateCreate = DateTime.MaxValue
                            };
                            listCache.Add(rowBase);
                        }

                        DirFile dirFile = new DirFile(dir.FullName);

                        ulong sizeByte = dirFile.GetDirSize();
                        Tuple<DateTime, DateTime> dateCreateEdit = dirFile.GetDateCreateEdited();

<<<<<<< HEAD
                        rowBase.DateCreate = dirFile.CompareDateCreate(rowBase.DateCreate, dateCreateEdit.Item1);
                        rowBase.DateEdit = dirFile.CompareDateEdit(rowBase.DateEdit, dateCreateEdit.Item2);
=======
                        rowBase.DateCreate = dirFile.CompareDatePlus(rowBase.DateCreate, dateCreateEdit.Item1);
                        rowBase.DateEdit = dirFile.CompareDateMinus(rowBase.DateEdit, dateCreateEdit.Item2);
>>>>>>> b56eda3fd570568cac4f49b3107f9c9947dcaf38
                        //if (rowBase.DateCreate.CompareTo(dateCreateEdit.Item1) == 1)
                        //    rowBase.DateCreate = dateCreateEdit.Item1;
                        //if (rowBase.DateEdit.CompareTo(dateCreateEdit.Item2) == -1)
                        //    rowBase.DateEdit = dateCreateEdit.Item2;

                        string size = new DirFile().GetSizeFormat(sizeByte);

                        if (typeCache == TypeCache.Local)
                        {
                            rowBase.PathCacheLocal = dir.FullName;
                            rowBase.SizeLocal = size;
                        }
                        else if (typeCache == TypeCache.AppData)
                        {
                            rowBase.PathCacheAppData = dir.FullName;
                            rowBase.SizeAppData = size;
                        }

                        rowBase.SizeByte += sizeByte;

                        totalByte += sizeByte;

                    }

                }

                RemoveIncorrectRows(listCache);

                total = new DirFile().GetSizeFormat(totalByte);

                listCache.Sort();

                list = listCache;

            }
            catch (DirectoryNotFoundException)
            {
                Dialog.ShowMessage($"Не удалось получить данные с каталога: \n{path}\nКаталог кеша не существует.");
            }
            catch (UnauthorizedAccessException)
            {
                Dialog.ShowMessage($"Нет доступа к каталогу кеша: \n{path}");
            }
            catch (Exception ex)
            {
                Dialog.ShowMessage("Произошла непредвиденная ошибка.");
                Dialog.ShowMessage(ex.Message);
            }
        }

        #endregion

        private void RemoveIncorrectRows(List<RowBase> list)
        {
            for (int i = 0; i < list.Count(); i++)
            {

                if (list[i].Name == _nameRowIsNotList
                    || !string.IsNullOrWhiteSpace(list[i].Connect))
                    continue;

                list.RemoveAt(i);

            }
        }

    }

}
