using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Parameters
{
    public class FXRandomRangeFloat : FXDynamicParameter<float>
    {
        public FXRandomRangeFloat(float min, float max)
            : this("", min, max)
        {

        }
        public FXRandomRangeFloat(string name, float min, float max)
            : base(name, () => FXEngine.CalculateRandomRange(min, max))
        {
            Min = min;
            Max = max;
        }

        public float Min { get; }
        public float Max { get; }

        public override FXParameter<float> Clone()
        {
            return new FXRandomRangeFloat(Name, Min, Max);
        }
    }
}
