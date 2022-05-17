using System.Drawing;
using System.Threading;
using System.IO;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{


    public partial class AnimExt
    {
        public bool InitInvisible = false;

        public void AnimClass_Update_Visibility()
        {
            // 断言第一次执行update时，所属已被设置
            if (!InitInvisible)
            {
                // 初始化
                UpdateVisibility(Type.Visibility);
            }
        }

        public void UpdateVisibility(Relation visibility)
        {
            OwnerObject.Ref.Invisible = GetInvisible(visibility);
            InitInvisible = true;
        }

        public bool GetInvisible(Relation visibility)
        {
            // Logger.Log($"{Game.CurrentFrame} - {OwnerObject}[{OwnerObject.Ref.Type.Ref.Base.Base.ID}] get invisible visibility = {visibility}");
            if (!OwnerObject.Ref.Owner.IsNull)
            {
                Relation relation = OwnerObject.Ref.Owner.GetRelationWithPlayer();
                return !visibility.HasFlag(relation);
            }
            return false;
        }

    }

    public partial class AnimTypeExt
    {

        public Relation Visibility = Relation.All;

        private void ReadVisibility(INIReader reader, string section)
        {
            string v = null;
            if (reader.ReadNormal(section, "Visibility", ref v))
            {
                Relation relation = Relation.All;
                string t = v.Substring(0, 1).ToUpper();
                switch (t)
                {
                    case "O":
                        relation = Relation.OWNER;
                        break;
                    case "A":
                        relation = Relation.Team;
                        break;
                    case "E":
                        relation = Relation.ENEMIES;
                        break;
                }
                this.Visibility = relation;
            }
        }
    }

}