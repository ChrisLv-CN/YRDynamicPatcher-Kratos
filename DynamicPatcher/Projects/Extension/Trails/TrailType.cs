using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public enum TrailMode
    {
        NONE = 0, LASER = 1, ELECTIRIC = 2, BEAM = 3, PARTICLE = 4, ANIM = 5
    }

    [Serializable]
    public partial class TrailType : Enumerable<TrailType>, INewType<Trail>
    {

        public TrailMode Mode;
        public int Distance;
        public bool IgnoreVertical;
        public int InitialDelay;

        public TrailType(string name) : base(name)
        {
            MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods.Where(m => m.Name.StartsWith("Init")))
            {
                method.Invoke(this, null);
            }
        }

        private void InitTrailType()
        {
            this.Mode = TrailMode.LASER;
            this.Distance = 64;
            this.IgnoreVertical = false;
            this.InitialDelay = 0;
        }

        public override void LoadFromINI(Pointer<CCINIClass> pINI)
        {
            // Logger.Log("TrailType {0} 读取INI配置", Name);
            INIReader reader = new INIReader(pINI);
            string section = Name;

            MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            object[] param = new object[] { reader, section };

            foreach (MethodInfo method in methods.Where(m => m.Name.StartsWith("Read")))
            {
                method.Invoke(this, param);
            }

            base.LoadFromINI(pINI);
        }

        public Trail CreateObject()
        {
            return new Trail(this);
        }

        private void ReadTrailType(INIReader reader, string section)
        {
             string mode = null;
            if (reader.ReadNormal(section, "Mode", ref mode))
            {
                TrailMode trailMode = default;
                string t = mode.Substring(0, 1).ToUpper();
                switch (t)
                {
                    case "L":
                        trailMode = TrailMode.LASER;
                        break;
                    case "E":
                        trailMode = TrailMode.ELECTIRIC;
                        break;
                    case "B":
                        trailMode = TrailMode.BEAM;
                        break;
                    case "P":
                        trailMode = TrailMode.PARTICLE;
                        break;
                    case "A":
                        trailMode = TrailMode.ANIM;
                        break;
                }
                this.Mode = trailMode;
            }
            
            int distance = 0;
            if (reader.ReadNormal(section, "Distance", ref distance))
            {
                this.Distance = distance;
            }

            bool ignoreVertical = false;
            if (reader.ReadNormal(section, "IgnoreVertical", ref ignoreVertical))
            {
                this.IgnoreVertical = ignoreVertical;
            }

            int initialDelay = 0;
            if (reader.ReadNormal(section, "InitialDelay", ref initialDelay))
            {
                this.InitialDelay = initialDelay;
            }
        }

    }

}