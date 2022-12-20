
using WebCinema.ViewModel;

namespace WebCinema.CacheService
{
    public class ViewModelComparsion
    {
        public static bool Compare(SortViewModel sortViewModel, SortState sortOrder, string fieldName)
        {
            return SortViewModel.ChangeState(sortOrder) == sortViewModel.CurrentState &&
                   sortViewModel.FieldName == fieldName;
        }

        public static bool Compare(PageViewModel pageViewModel, int page)
        {
            return pageViewModel.PageNumber == page;
        }
    }
}
