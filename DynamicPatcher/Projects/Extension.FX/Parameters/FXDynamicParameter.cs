using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Parameters
{
    public class FXDynamicParameter<T> : FXParameter<T>
    {
        public FXDynamicParameter(Func<T> output) : this("", output)
        {
        }
        public FXDynamicParameter(string name, Func<T> output) : base(name)
        {
            Output = output;
        }

        public Func<T> Output { get; }

        public override T Value
        {
            get => Output();
            set => throw new InvalidOperationException("FXDynamicParameter can not set value.");
        }

        public override FXParameter<T> Clone()
        {
            return new FXDynamicParameter<T>(Name, Output);
        }
    }
}
