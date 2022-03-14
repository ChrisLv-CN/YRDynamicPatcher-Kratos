using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Components
{
    public interface IHaveComponent
    {
        Component AttachedComponent { get; }
    }
}
