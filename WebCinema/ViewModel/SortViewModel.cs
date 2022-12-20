﻿namespace WebCinema.ViewModel
{
    public enum SortState
    {
        No,
        Ascending,
        Descending
    }

    public class SortViewModel
    {
        public static SortState ChangeState(SortState sortState)
        {
            return sortState == SortState.No ? SortState.Ascending :
                   sortState == SortState.Ascending ? SortState.Descending : SortState.No;
        }

        public string FieldName { get; set; } = "";

        public SortState CurrentState { get; set; }

        public SortViewModel(SortState sortOrder)
        {
            CurrentState = ChangeState(sortOrder);
        }

        public SortViewModel(SortState sortOrder, string fieldName)
        {
            CurrentState = ChangeState(sortOrder);
            FieldName = fieldName;
        }

        public SortViewModel(SortState sortOrder, string fieldName, string oldFieldName)
        {
            if (oldFieldName == fieldName)
            {
                CurrentState = ChangeState(sortOrder);
                FieldName = fieldName;
            }
            else if (fieldName == null)
            {
                CurrentState = sortOrder;
                FieldName = oldFieldName;
            }
            else
            {
                CurrentState = SortState.No;
                FieldName = fieldName;
            }
        }
    }
}
