using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    public interface IReloadable
    {
        public void SaveToStream(IStream stream);
        public void LoadFromStream(IStream stream);
    }
}
