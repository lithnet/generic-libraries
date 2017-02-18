using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Lithnet.Common.Presentation
{
    public interface IIconProvider
    {
        BitmapSource GetImageForItem(object item);
    }
}
