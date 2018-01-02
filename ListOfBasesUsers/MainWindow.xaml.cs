using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ListOfBasesUsers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Properties
        private const string _nameRowIsNotList = "нет в списке";

        private static string CultureDateTime = new CultureApp().GetFormatData();

        private IEnumerable<RowBase> _listBases = new List<RowBase>();
        private IEnumerable<RowBase> _listBasesFiltering = new List<RowBase>();
        private List<RowBase> _selectedBases = new List<RowBase>();

        private bool _filterIsNotList;
        private bool _visibleAdditionalPanel;
        private bool _visibleColumnID;
        private bool _tableLoad;

        private string[] _arrayFilter = { "Сегодня", "Последняя неделя", "Последний месяц", "Текущий месяц", "Произвольный" };
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                GetListUser();
                SetCurrentUser();
                ChangeItemSoursedgTable();
                SetVisibleElements();
                InitializeFilteringPeriodSetByDefault();
            }
        }

        #region main menu

        private void CbUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillTable();
            UpdateSize();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateTable();
        }

        private void BtnUpdateSize_Click(object sender, RoutedEventArgs e)
        {
            UpdateSize();
        }

        private void BtnDelCacheSelected_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBases.Count == 0)
            {
                Dialog.ShowMessage("Не выбраны строки для удаления.");
                return;
            }
            else if (!Dialog.DialogQuestion($"Удалить кеши для баз в количестве - {_selectedBases.Count} шт."))
                return;

            new DirFile().DeleteCatalogCache(_selectedBases);

            if (Dialog.DialogQuestion("Каталоги удалены.\nПеречитать таблицу?"))
                UpdateTable();
        }

        private void BtnFilterIsNotList_Click(object sender, RoutedEventArgs e)
        {
            _filterIsNotList = !_filterIsNotList;
            ChangeItemSoursedgTable();
        }

        #endregion

        #region additional panel

        private void BtnOpenAdditionalPanel_Click(object sender, RoutedEventArgs e)
        {
            _visibleAdditionalPanel = !_visibleAdditionalPanel;
            SetVisibleElements();
        }

        private void DatePBegin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tableLoad)
                ChangeItemSoursedgTable();
        }

        private void DatePEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tableLoad)
                ChangeItemSoursedgTable();
        }

        private void CbFilterPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedPeriod = cbFilterPeriod.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedPeriod))
                return;

            string elementPeriod = _arrayFilter.FirstOrDefault(f => f == selectedPeriod);

            if (string.IsNullOrWhiteSpace(elementPeriod))
                return;

            DateTime dateTimeNow = DateTime.Now;

            _tableLoad = false;
            switch (elementPeriod)
            {
                case "Сегодня":
                    SetValuePeriod(dateTimeNow.Date, dateTimeNow.Date.AddDays(1).AddTicks(-1));
                    break;
                case "Последняя неделя":
                    SetValuePeriod(dateTimeNow.Date.AddDays(-6), dateTimeNow.Date.AddDays(1).AddTicks(-1));
                    break;
                case "Последний месяц":
                    SetValuePeriod(dateTimeNow.Date.AddMonths(-1), dateTimeNow.Date.AddDays(1).AddTicks(-1));
                    break;
                case "Текущий месяц":
                    DateTime beginMonth = dateTimeNow.Date.AddDays(-dateTimeNow.Date.Day + 1);
                    SetValuePeriod(
                        beginMonth,
                        beginMonth.AddMonths(1).AddTicks(-1));
                    break;
            }
            _tableLoad = true;
            ChangeItemSoursedgTable();
        }

        private void SetValuePeriod(DateTime dateBegin, DateTime dateEnd)
        {
            datePBegin.SelectedDate = dateBegin;
            datePEnd.SelectedDate = dateEnd;
        }

        private void BtnVisibleID_Click(object sender, RoutedEventArgs e)
        {
            _visibleColumnID = !_visibleColumnID;
            SetVisibleColumnID();
        }

        #endregion

        #region datagrid Table

        private void DgTable_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGridColumn columnDateCreate = dgTable.Columns.FirstOrDefault(f => (string)f.Header == "DateCreate");
            if (columnDateCreate != null)
                (columnDateCreate as DataGridTextColumn).Binding.StringFormat = CultureDateTime;

            DataGridColumn columnDateEdit = dgTable.Columns.FirstOrDefault(f => (string)f.Header == "DateEdit");
            if (columnDateEdit != null)
                (columnDateEdit as DataGridTextColumn).Binding.StringFormat = CultureDateTime;

            foreach (PropertyInfo propInfo in new RowBase().GetType().GetProperties())
            {
                DataGridColumn column = dgTable.Columns.FirstOrDefault(f => (string)f.Header == propInfo.Name);
                if (column != null)
                {
                    var propAttribute = propInfo.GetCustomAttributes<PropertiesColumnsAttribute>();

                    if (propAttribute.Count() > 0)
                    {
                        var attribute = propAttribute.First();

                        column.Header = attribute.HeaderName;
                        column.Visibility = attribute.VisibleColumn ? Visibility.Visible : Visibility.Hidden;

                        if (!string.IsNullOrWhiteSpace(attribute.SortMemberPath))
                            column.SortMemberPath = attribute.SortMemberPath;

                        if (attribute.SortDirection != null)
                            column.SortDirection = attribute.SortDirection;
                    }
                }
            }

            SetVisibleColumnID();
        }

        private void DgTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SetVisibleElements();
        }

        #endregion

        #region datagrid Table ContextMenu

        private void DgTableOpenDirectoryLocal_Click(object sender, RoutedEventArgs e)
        {

            string pathCache = GetPathCacheCurrentRow(TypeCache.Local);
            if (string.IsNullOrWhiteSpace(pathCache))
            {
                Dialog.ShowMessage("Каталог кеша (Local) не найден");
                return;
            }

            new DirFile(pathCache).OpenDirectory();

        }

        private void DgTableOpenDirectoryAppData_Click(object sender, RoutedEventArgs e)
        {

            string pathCache = GetPathCacheCurrentRow(TypeCache.AppData);
            if (string.IsNullOrWhiteSpace(pathCache))
            {
                Dialog.ShowMessage("Каталог кеша (AppData) не найден");
                return;
            }

            new DirFile(pathCache).OpenDirectory();

        }

        private void DgTableClearCache_Click(object sender, RoutedEventArgs e)
        {
            ClearCacheRow(TypeCache.Local);
            ClearCacheRow(TypeCache.AppData);

            if (Dialog.DialogQuestion("Каталог удален.\nПересчитать размеры кеша?"))
                UpdateSize();
        }

        private void ClearCacheRow(TypeCache typeCache)
        {
            string pathCache = GetPathCacheCurrentRow(typeCache);
            if (string.IsNullOrWhiteSpace(pathCache))
                return;

            new DirFile(pathCache).DeleteCatalogCache();
        }

        #endregion

        #region private methods

        private void UpdateTable()
        {
            FillTable();
            UpdateSize();
        }
        private void InitializeFilteringPeriodSetByDefault()
        {
            cbFilterPeriod.ItemsSource = _arrayFilter;
            cbFilterPeriod.SelectedItem = "Произвольный";
        }

        private void GetListUser()
        {
            SetValueStatusBar("Загрузка списка пользователей...");

            cbUser.Items.Clear();
            foreach (string item in new DirFile().GetListUsers())
                cbUser.Items.Add(item);

            SetValueStatusBar();
        }

        private void SetCurrentUser()
        {
            cbUser.SelectedValue = DefaultValues.nameCurrentUser;
        }

        private void FillTable()
        {
            _tableLoad = false;

            SetValueStatusBar("Получение списка баз...");

            string selectedName = cbUser.SelectedItem?.ToString();

            if (!string.IsNullOrWhiteSpace(selectedName))
                _listBases = new ListIbases(selectedName).GetListBases(this);

            ChangeItemSoursedgTable();

            SetValueStatusBar();

            _tableLoad = true;
        }

        private void UpdateSize()
        {
            _tableLoad = false;

            if (_listBases.Count() == 0)
                return;

            SetValueStatusBar("Получение размеров кеша...");

            string selectedName = cbUser.SelectedItem?.ToString();
            ulong totalByte = 0;
            string total = "";

            if (!string.IsNullOrWhiteSpace(selectedName))
                new ListIbases(selectedName).ReadCache(ref _listBases, ref totalByte, ref total);

            FixedDateListBases();
            SetValuePeriod();

            txtTotalByte.Text = totalByte.ToString();
            txtTotal.Text = total;

            ChangeItemSoursedgTable();

            SetValueStatusBar();

            InitializeFilteringPeriodSetByDefault();

            _tableLoad = true;
        }

        private void FixedDateListBases()
        {
            DateTime maxValueDate = DateTime.MaxValue;
            DateTime minValueDate = DateTime.MinValue;

            for (int i = 0; i < _listBases.Count(); i++)
            {
                if (_listBases.ElementAt(i).DateCreate == maxValueDate)
                    _listBases.ElementAt(i).DateCreate = minValueDate;
                if (_listBases.ElementAt(i).DateEdit== maxValueDate)
                    _listBases.ElementAt(i).DateEdit = minValueDate;
            }
        }

        private void SetValuePeriod()
        {
            DateTime maxValueDate = DateTime.MinValue;
            DateTime minValueDate = DateTime.MaxValue;

            DirFile dirFile = new DirFile();
            for (int i = 0; i < _listBases.Count(); i++)
            {
                RowBase currentElement = _listBases.ElementAt(i);
                maxValueDate = dirFile.CompareDateMinus(maxValueDate, currentElement.DateCreate);
                maxValueDate = dirFile.CompareDateMinus(maxValueDate, currentElement.DateEdit);
                minValueDate = dirFile.CompareDatePlus(minValueDate, currentElement.DateCreate);
                minValueDate = dirFile.CompareDatePlus(minValueDate, currentElement.DateEdit);
            }

            datePBegin.SelectedDate = minValueDate;
            datePEnd.SelectedDate = maxValueDate;

        }

        private void SetValueStatusBar(string text = "")
        {
            //sbItem.Text = text;
        }

        private string GetPathCacheCurrentRow(TypeCache typeCache)
        {
            string pathCache = "";

            switch (typeCache)
            {
                case TypeCache.Local:
                    pathCache = ((RowBase)dgTable.CurrentItem).PathCacheLocal;
                    break;
                case TypeCache.AppData:
                    pathCache = ((RowBase)dgTable.CurrentItem).PathCacheAppData;
                    break;
                default:
                    pathCache = null;
                    break;
            }

            if (pathCache == null)
            {
                //Dialog.ShowMessage("Каталог кеша не найден.");
                return null;
            }

            return pathCache;
        }


        #region Visibility

        private void SetVisibleElements()
        {
            SetVisibleBtnDelCacheSelected();
            SetVisibleButtonMainMenu();
            SetVisibleTotalInSelected();
            SetVisibleAdditionalPanel();
            SetVisibleColumnID();
        }

        private void SetVisibleButtonMainMenu()
        {
            bool lastElementHide = false;
            double lastPosition = 0;
            for (int i = 0; i < grMainMenu.Children.Count; i++)
            {

                UIElement currentItem = grMainMenu.Children[i];
                FrameworkElement currentElement = (FrameworkElement)currentItem;

                if (currentItem.Visibility == Visibility.Visible)
                {

                    if (lastElementHide
                        || lastPosition >= (currentElement.Width + currentElement.Margin.Left))
                        currentElement.Margin = new Thickness(lastPosition + 10, 0, 0, 0);

                    lastPosition = currentElement.Margin.Left + currentElement.Width;
                    lastElementHide = false;

                }
                else
                {
                    lastElementHide = true;
                }

            }
        }

        private void SetVisibleTotalInSelected()
        {
            ulong totalByte = 0;

            foreach (RowBase item in _selectedBases)
                totalByte += item.SizeByte;

            string total = new DirFile().GetSizeFormat(totalByte);

            txtTotalByteSelected.Text = totalByte.ToString();
            txtTotalSelected.Text = total;

            grSelectedTotal.Visibility = _selectedBases.Count > 1 ? Visibility.Visible : Visibility.Hidden;
        }

        private void SetVisibleAdditionalPanel()
        {
            double height = 0;
            double fixedHeight = (grAdditionalPanel.Height + 3);

            if (_visibleAdditionalPanel)
            {
                if (grAdditionalPanel.Visibility != Visibility.Visible)
                {
                    grAdditionalPanel.Visibility = Visibility.Visible;
                    height = -fixedHeight;
                }
            }
            else
            {
                if (grAdditionalPanel.Visibility == Visibility.Visible)
                {
                    grAdditionalPanel.Visibility = Visibility.Hidden;
                    height = fixedHeight;
                }
            };
            Thickness thickness = dgTable.Margin;
            dgTable.Margin = new Thickness(thickness.Left, thickness.Top -= height, thickness.Right, thickness.Bottom);
        }

        private void SetVisibleBtnDelCacheSelected()
        {
            _selectedBases.Clear();

            foreach (RowBase item in dgTable.SelectedItems)
            {
                if (item.SizeByte > 0)
                    _selectedBases.Add(item);
            }

            Visibility newVisibility;
            if (_selectedBases.Count > 1)
                newVisibility = Visibility.Visible;
            else
                newVisibility = Visibility.Hidden;


            btnDelCacheSelected.Visibility = newVisibility;
        }

        private void SetVisibleColumnID()
        {
            DataGridColumn columnID = dgTable.Columns.FirstOrDefault(f => (string)f.Header == "ID");
            if (columnID != null)
                columnID.Visibility = _visibleColumnID ? Visibility.Visible : Visibility.Hidden;

            btnVisibleID.FontWeight = FontWeight.FromOpenTypeWeight(_visibleColumnID ? 500 : 1);
        }

        #endregion

        #region Filter

        private void ChangeItemSoursedgTable()
        {
            List<RowBase> list = new List<RowBase>();

            DateTime datePEndEndDay = DateTime.MinValue;
            if (datePEnd.SelectedDate != null)
                datePEndEndDay = (DateTime)datePEnd.SelectedDate;

            foreach (RowBase item in _listBases)
            {
                if (!(_filterIsNotList && item.Name != _nameRowIsNotList))
                {
                    if (item.DateCreate >= datePBegin.SelectedDate && item.DateCreate <= datePEndEndDay
                        || item.DateEdit >= datePBegin.SelectedDate && item.DateEdit <= datePEndEndDay)
                    {
                        list.Add(item);
                    }
                }
            }

            _listBasesFiltering = list;

            dgTable.ItemsSource = _listBasesFiltering;

            btnFilterIsNotList.FontWeight = FontWeight.FromOpenTypeWeight(_filterIsNotList ? 500 : 1);
        }

        #endregion

        #endregion

    }
}
