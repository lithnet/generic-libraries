using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Lithnet.Common.Presentation
{
    public interface ICollectionViewModel<TModel> :
        IEnumerable
        where TModel : class
    {
        void Remove(TModel model);
        void Add(TModel model);
        void MoveUp(TModel model);
        void MoveDown(TModel model);
        int IndexOf(TModel model);
    }
}
