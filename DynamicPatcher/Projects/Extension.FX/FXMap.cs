using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX
{
    public class FXMap
    {
        public FXMap(FXScript script)
        {
            _script = script;
        }

        FXScript _script;

        public FXParameterMap System => _script.System.Map;
        public FXParameterMap Emitter => _script.Emitter.Map;
        public FXParameterMap this[FXParticle particle] => particle.Map;
    }
}
