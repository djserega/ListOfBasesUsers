using System;
using System.ComponentModel;

namespace ListOfBasesUsers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertiesColumnsAttribute: Attribute
    {

        public string HeaderName { get; }
        public bool VisibleColumn { get; }
        public string SortMemberPath { get; }
        public ListSortDirection? SortDirection { get; }

        public PropertiesColumnsAttribute(string headerName, bool visibleColumn = true, string sortMemberPath = "", SortDirection sortDirection = ListOfBasesUsers.SortDirection.none)
        {
            HeaderName = headerName;
            VisibleColumn = visibleColumn;
            SortMemberPath = sortMemberPath;
            switch (sortDirection)
            {
                case ListOfBasesUsers.SortDirection.asc:
                    SortDirection = ListSortDirection.Ascending;
                    break;
                case ListOfBasesUsers.SortDirection.dsc:
                    SortDirection = ListSortDirection.Descending;
                    break;
                default:
                    SortDirection = null;
                    break;
            }
        }

    }
}
