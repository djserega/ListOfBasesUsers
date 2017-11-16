using System;

namespace ListOfBasesUsers
{
    public class RowBase : IComparable
    {

        #region public properties

        [PropertiesColumns("Имя")]
        public string Name { get; set; }
        [PropertiesColumns("Каталог кеша (Local)", false)]
        public string PathCacheLocal { get; set; }
        [PropertiesColumns("Каталог кеша (AppData)", false)]
        public string PathCacheAppData{ get; set; }
        [PropertiesColumns("Размер (B)", sortDirection: SortDirection.dsc)]
        public ulong SizeByte { get; set; }
        [PropertiesColumns("Размер (Local)", sortMemberPath: "SizeByte")]
        public string SizeLocal { get; set; }
        [PropertiesColumns("Размер (AppData)", sortMemberPath: "SizeByte")]
        public string SizeAppData{ get; set; }
        [PropertiesColumns("ID")]
        public string ID { get; set; }
        [PropertiesColumns("Каталог")]
        public string Folder { get; set; }
        [PropertiesColumns("Строка подключения")]
        public string Connect { get; set; }
        [PropertiesColumns("Тип запуска")]
        public string App { get; set; }
        [PropertiesColumns("Версия")]
        public string Version { get; set; }
        [PropertiesColumns("Дата создания")]
        public DateTime DateCreate { get; set; }
        [PropertiesColumns("Дата изменения")]
        public DateTime DateEdit { get; set; }

        #endregion

        public int CompareTo(object obj) => -SizeByte.CompareTo(((RowBase)obj).SizeByte);

    }
}
