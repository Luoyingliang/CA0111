
using System.Collections.ObjectModel;

namespace Acorisoft.Miga.UI.Core
{
    public interface IPageInfoCollection : IList<PageInfo>{}

    public class PageInfoCollection : Collection<PageInfo>, IPageInfoCollection
    {
        protected override void InsertItem(int index, PageInfo item)
        {
            if (item is null ||
                string.IsNullOrEmpty(item.Name) ||
                string.IsNullOrEmpty(item.Id))
            {
                return;
            }
            
            base.InsertItem(index, item);
        }
    }
}