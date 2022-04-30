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

    [Serializable]
    public class AttackBeacon
    {
        public bool Enable;
        public AttackBeaconData Data;
        public Dictionary<string, int> Types;
        public List<Mission> RecruitMissions;

        public int Count;
        public int Timer;

        public AttackBeacon(AttackBeaconData data)
        {
            this.Enable = data.Enable;
            this.Data = data;
            this.Count = data.Count;
            this.Timer = data.Delay;
            Types = new Dictionary<string, int>();
            for (int i = 0; i < data.Types.Count; i++)
            {
                string type = data.Types[i];
                int num = i < data.Num.Count ? data.Num[i] : 99999;
                Types.Add(type, num);
            }
            RecruitMissions = new List<Mission>();
            RecruitMissions.Add(Mission.None);
            RecruitMissions.Add(Mission.Sleep);
            RecruitMissions.Add(Mission.Guard);
            RecruitMissions.Add(Mission.Area_Guard);
            RecruitMissions.Add(Mission.Stop);
        }

        public bool IsReady()
        {
            return --Timer <= 0 && (Data.Count > 0 ? Count-- > 0 : true);
        }

        public void Reload()
        {
            this.Timer = Data.Rate;
        }
    }

    [Serializable]
    public class AttackBeaconData
    {
        public bool Enable;
        public List<String> Types;
        public List<int> Num;
        public int Rate;
        public int Delay;
        public int RangeMin;
        public int RangeMax;
        public bool Close;
        public bool Force;
        public int Count;
        public bool TargetToCell;
        public bool AffectsOwner;
        public bool AffectsAllies;
        public bool AffectsEnemies;
        public bool AffectsCivilian;


        public AttackBeaconData(bool enable)
        {
            this.Enable = enable;
            this.Types = new List<string>();
            this.Num = new List<int>();
            this.Rate = 0;
            this.Delay = 0;
            this.RangeMin = 0;
            this.RangeMax = -1;
            this.Close = true;
            this.Force = false;
            this.Count = 1;
            this.TargetToCell = false;
            this.AffectsOwner = true;
            this.AffectsAllies = false;
            this.AffectsEnemies = false;
            this.AffectsCivilian = false;
        }

        public void AddType(string type)
        {
            if (null == this.Types)
            {
                this.Types = new List<string>();
            }
            this.Types.Add(type);
        }

        public void AddNum(int num)
        {
            if (null == this.Num)
            {
                this.Num = new List<int>();
            }
            this.Num.Add(num);
        }
    }

    public partial class TechnoExt
    {

        public AttackBeacon attackBeacon;
        public bool attackBeaconRecruited = false;

        public unsafe void TechnoClass_Init_AttackBeacon()
        {
            if (null != Type.AttackBeaconData && Type.AttackBeaconData.Enable && null == attackBeacon)
            {
                attackBeacon = new AttackBeacon(Type.AttackBeaconData);
                OnUpdateAction += TechnoClass_Update_AttackBeacon;
            }
            OnFireAction += TechnoClass_OnFire_AttackBeacon_Recruit;
        }

        public unsafe void TechnoClass_Update_AttackBeacon()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
                if (attackBeacon.IsReady())
                {
                    attackBeacon.Reload();

                    // Find self Unit and set ther Target
                    Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                    CoordStruct location = pTechno.Ref.Base.Base.GetCoords();

                    // find candidate
                    Dictionary<string, SortedList<double, List<Pointer<TechnoClass>>>> candidates = new Dictionary<string, SortedList<double, List<Pointer<TechnoClass>>>>();
                    ExHelper.FindTechno(pHouse, (pTarget) =>
                    {
                        string type = pTarget.Ref.Type.Ref.Base.Base.ID;

                        if ((attackBeacon.Data.Types.Count <= 0 || attackBeacon.Data.Types.Contains(type))
                            && (attackBeacon.Data.Force ? true : attackBeacon.RecruitMissions.Contains(pTarget.Ref.Base.GetCurrentMission())))
                        {
                            double distance = location.DistanceFrom(pTarget.Ref.Base.Base.GetCoords());
                            if (distance > attackBeacon.Data.RangeMin && (attackBeacon.Data.RangeMax < 0 ? true : distance < attackBeacon.Data.RangeMax))
                            {
                                if (attackBeacon.Data.Force || pTarget.Ref.Target.IsNull || pTarget.Ref.Target != pTechno.Convert<AbstractClass>())
                                {
                                    // find one
                                    SortedList<double, List<Pointer<TechnoClass>>> technoDistanceSorted = null;
                                    if (candidates.ContainsKey(type))
                                    {
                                        technoDistanceSorted = candidates[type];
                                    }
                                    else
                                    {
                                        technoDistanceSorted = new SortedList<double, List<Pointer<TechnoClass>>>();
                                        candidates.Add(type, technoDistanceSorted);
                                    }
                                    if (technoDistanceSorted.ContainsKey(distance))
                                    {
                                        technoDistanceSorted[distance].Add(pTarget);
                                    }
                                    else
                                    {
                                        List<Pointer<TechnoClass>> recruits = new List<Pointer<TechnoClass>>();
                                        recruits.Add(pTarget);
                                        technoDistanceSorted.Add(distance, recruits);
                                    }
                                    candidates[type] = technoDistanceSorted;
                                }
                            }
                        }
                        return false;
                    }, attackBeacon.Data.AffectsOwner, attackBeacon.Data.AffectsAllies, attackBeacon.Data.AffectsEnemies, attackBeacon.Data.AffectsCivilian);

                    Pointer<AbstractClass> pBeacon = pTechno.Convert<AbstractClass>();
                    if (attackBeacon.Data.TargetToCell)
                    {
                        Pointer<CellClass> pCell = MapClass.Instance.GetCellAt(pBeacon.Ref.GetCoords());
                        pBeacon = pCell.Convert<AbstractClass>();
                    }

                    bool noLimit = null == attackBeacon.Data.Types || attackBeacon.Data.Types.Count <= 0;
                    foreach (var candidate in candidates)
                    {
                        string type = candidate.Key;
                        var technos = candidate.Value;
                        // check this type is full.
                        int count = 0;
                        bool isFull = false;
                        foreach(var targets in technos)
                        {
                            if (isFull)
                            {
                                break;
                            }
                            foreach(var pTarget in targets.Value)
                            {
                                if (!noLimit && ++count > attackBeacon.Types[type])
                                {
                                    isFull = true;
                                    break;
                                }
                                // recruit one
                                pTarget.Ref.SetTarget(pBeacon);
                                TechnoExt ext = TechnoExt.ExtMap.Find(pTarget);
                                if (null != ext)
                                {
                                    ext.attackBeaconRecruited = true;
                                }
                            }
                        }
                    }

                }
        }

        public unsafe void TechnoClass_OnFire_AttackBeacon_Recruit(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            // 被征召去攻击信标
            if (attackBeaconRecruited)
            {
                attackBeaconRecruited = false;
                // clean recruited target
                OwnerObject.Ref.SetTarget(Pointer<AbstractClass>.Zero);
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public AttackBeaconData AttackBeaconData;

        /// <summary>
        /// [TechnoType]
        /// AttackBeacon.Enable=yes
        /// AttackBeacon.Types=V3,DRED
        /// AttackBeacon.Num=-1,-1
        /// AttackBeacon.Rate=30
        /// AttackBeacon.Delay=0
        /// AttackBeacon.RangeMin=0
        /// AttackBeacon.RangeMax=-1
        /// AttackBeacon.Close=yes
        /// AttackBeacon.Force=no
        /// AttackBeacon.Count=1
        /// AttackBeacon.TargetToCell=no
        /// AttackBeacon.AffectsOwner=yes
        /// AttackBeacon.AffectsAllies=no
        /// AttackBeacon.AffectsEnemies=no
        /// AttackBeacon.AffectsCivilian=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadAttackBeacon(INIReader reader, string section)
        {
            // AttackBeacon
            bool attackBeacon = false;
            if (reader.ReadNormal(section, "AttackBeacon.Enable", ref attackBeacon))
            {
                AttackBeaconData = new AttackBeaconData(attackBeacon);

                string typeStr = null;
                if (reader.ReadNormal(section, "AttackBeacon.Types", ref typeStr))
                {
                    string[] types = typeStr.Split(',');
                    foreach (string type in types)
                    {
                        string typeName = null;
                        if (!String.IsNullOrEmpty(typeName = type.Trim()))
                        {
                            AttackBeaconData.AddType(typeName);
                        }
                    }
                }

                string numStr = null;
                if (reader.ReadNormal(section, "AttackBeacon.Num", ref numStr))
                {
                    string[] num = numStr.Split(',');
                    foreach (string n in num)
                    {
                        string m = null;
                        if (!String.IsNullOrEmpty(m = n.Trim()))
                        {
                            int x = Convert.ToInt32(m);
                            AttackBeaconData.AddNum(x == 0 ? -1 : x);
                        }
                    }
                }

                int rate = 0;
                if (reader.ReadNormal(section, "AttackBeacon.Rate", ref rate))
                {
                    AttackBeaconData.Rate = rate;
                }

                int delay = 0;
                if (reader.ReadNormal(section, "AttackBeacon.Delay", ref delay))
                {
                    AttackBeaconData.Delay = delay;
                }

                int rangeMin = 0;
                if (reader.ReadNormal(section, "AttackBeacon.RangeMin", ref rangeMin))
                {
                    AttackBeaconData.RangeMin = rangeMin * 256;
                }

                int rangeMax = 0;
                if (reader.ReadNormal(section, "AttackBeacon.RangeMax", ref rangeMax))
                {
                    AttackBeaconData.RangeMax = rangeMax * 256;
                }

                bool close = false;
                if (reader.ReadNormal(section, "AttackBeacon.Close", ref close))
                {
                    AttackBeaconData.Close = close;
                }

                bool force = false;
                if (reader.ReadNormal(section, "AttackBeacon.Force", ref force))
                {
                    AttackBeaconData.Force = force;
                }

                int count = 0;
                if (reader.ReadNormal(section, "AttackBeacon.Count", ref count))
                {
                    AttackBeaconData.Count = count;
                }

                bool targetToCell = false;
                if (reader.ReadNormal(section, "AttackBeacon.TargetToCell", ref targetToCell))
                {
                    AttackBeaconData.TargetToCell = targetToCell;
                }

                bool affectsOwner = false;
                if (reader.ReadNormal(section, "AttackBeacon.AffectsOwner", ref affectsOwner))
                {
                    AttackBeaconData.AffectsOwner = affectsOwner;
                }

                bool affectsAllies = false;
                if (reader.ReadNormal(section, "AttackBeacon.AffectsAllies", ref affectsAllies))
                {
                    AttackBeaconData.AffectsAllies = affectsAllies;
                }

                bool affectsEnemies = false;
                if (reader.ReadNormal(section, "AttackBeacon.AffectsEnemies", ref affectsEnemies))
                {
                    AttackBeaconData.AffectsEnemies = affectsEnemies;
                }

                bool affectsCivilian = false;
                if (reader.ReadNormal(section, "AttackBeacon.AffectsCivilian", ref affectsCivilian))
                {
                    AttackBeaconData.AffectsCivilian = affectsCivilian;
                }

            }

        }
    }

}