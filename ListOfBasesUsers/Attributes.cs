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

        public PropertiesColumnsAttribute(string headerName, bool visibleColumn = true, string sortMemberPath = "", string sortDirection = "")
        {
            HeaderName = headerName;
            VisibleColumn = visibleColumn;
            SortMemberPath = sortMemberPath;
            switch (sortDirection)
            {
                case "asc":
                    SortDirection = ListSortDirection.Ascending;
                    break;
                case "dsc":
                    SortDirection = ListSortDirection.Descending;
                    break;
                default:
                    SortDirection = null;
                    break;
            }
        }

    }
}
