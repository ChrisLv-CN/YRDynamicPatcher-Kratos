using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension.Ext;
using DynamicPatcher;

namespace Extension.Utilities
{

    public static class BulletEffectHelper
    {

        public static void RedCrosshair(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1)
        {
            Crosshair(sourcePos, length, new ColorStruct(255, 0, 0), default, thickness, duration);
        }

        public static void RedCell(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1, bool crosshair = false)
        {
            if (crosshair)
            {
                GreenCrosshair(sourcePos, length, thickness, duration);
            }
            Cell(sourcePos, length, new ColorStruct(255, 0, 0), default, thickness, duration);
        }

        public static void GreenCrosshair(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1)
        {
            Crosshair(sourcePos, length, new ColorStruct(0, 255, 0), default, thickness, duration);
        }

        public static void GreenCell(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1, bool crosshair = false)
        {
            if (crosshair)
            {
                GreenCrosshair(sourcePos, length, thickness, duration);
            }
            Cell(sourcePos, length, new ColorStruct(0, 255, 0), default, thickness, duration);
        }

        public static void BlueCrosshair(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1)
        {
            Crosshair(sourcePos, length, new ColorStruct(0, 0, 255), default, thickness, duration);
        }

        public static void BlueCell(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1, bool crosshair = false)
        {
            if (crosshair)
            {
                GreenCrosshair(sourcePos, length, thickness, duration);
            }
            Cell(sourcePos, length, new ColorStruct(0, 0, 255), default, thickness, duration);
        }

        public static void Crosshair(CoordStruct sourcePos, int length, ColorStruct lineColor, ColorStruct outerColor = default, int thickness = 1, int duration = 1)
        {
            DrawLine(sourcePos, sourcePos + new CoordStruct(length, 0, 0), lineColor, outerColor, thickness, duration);
            DrawLine(sourcePos, sourcePos + new CoordStruct(-length, 0, 0), lineColor, outerColor, thickness, duration);
            DrawLine(sourcePos, sourcePos + new CoordStruct(0, -length, 0), lineColor, outerColor, thickness, duration);
            DrawLine(sourcePos, sourcePos + new CoordStruct(0, length, 0), lineColor, outerColor, thickness, duration);
        }

        public static void Cell(CoordStruct sourcePos, int length, ColorStruct lineColor, ColorStruct outerColor = default, int thickness = 1, int duration = 1)
        {
            CoordStruct p1 = sourcePos + new CoordStruct(length, length, 0);
            CoordStruct p2 = sourcePos + new CoordStruct(-length, length, 0);
            CoordStruct p3 = sourcePos + new CoordStruct(-length, -length, 0);
            CoordStruct p4 = sourcePos + new CoordStruct(length, -length, 0);
            DrawLine(p1, p2, lineColor, outerColor, thickness, duration);
            DrawLine(p2, p3, lineColor, outerColor, thickness, duration);
            DrawLine(p3, p4, lineColor, outerColor, thickness, duration);
            DrawLine(p4, p1, lineColor, outerColor, thickness, duration);
        }

        public static void RedLineZ(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1)
        {
            RedLine(sourcePos, sourcePos + new CoordStruct(0, 0, length), thickness, duration);
        }

        public static void RedLine(CoordStruct sourcePos, CoordStruct targetPos, int thickness = 1, int duration = 1)
        {
            DrawLine(sourcePos, targetPos, new ColorStruct(255, 0, 0), default, thickness, duration);
        }

        public static void GreenLineZ(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1)
        {
            GreenLine(sourcePos, sourcePos + new CoordStruct(0, 0, length), thickness, duration);
        }

        public static void GreenLine(CoordStruct sourcePos, CoordStruct targetPos, int thickness = 1, int duration = 1)
        {
            DrawLine(sourcePos, targetPos, new ColorStruct(0, 255, 0), default, thickness, duration);
        }

        public static void BlueLineZ(CoordStruct sourcePos, int length, int thickness = 1, int duration = 1)
        {
            BlueLine(sourcePos, sourcePos + new CoordStruct(0, 0, length), thickness, duration);
        }

        public static void BlueLine(CoordStruct sourcePos, CoordStruct targetPos, int thickness = 1, int duration = 1)
        {
            DrawLine(sourcePos, targetPos, new ColorStruct(0, 0, 255), default, thickness, duration);
        }

        public static void DrawLine(CoordStruct sourcePos, CoordStruct targetPos, ColorStruct innerColor, ColorStruct outerColor = default, int thickness = 2, int duration = 15)
        {
            LaserType type = new LaserType(true);
            type.InnerColor = innerColor;
            type.OuterColor = outerColor;
            type.Thickness = thickness;
            type.Duration = duration;
            DrawLine(sourcePos, targetPos, type);
        }

        public static void DrawLine(CoordStruct sourcePos, CoordStruct targetPos, LaserType type, ColorStruct houseColor = default)
        {
            ColorStruct innerColor = type.InnerColor;
            ColorStruct outerColor = type.OuterColor;
            ColorStruct outerSpread = type.OuterSpread;
            if (default != houseColor)
            {
                innerColor = houseColor;
                outerColor = default;
            }
            Pointer<LaserDrawClass> pLaser = YRMemory.Create<LaserDrawClass>(sourcePos, targetPos, innerColor, outerColor, outerSpread, type.Duration);
            pLaser.Ref.Thickness = type.Thickness;
            pLaser.Ref.IsHouseColor = type.Fade ? type.Fade : type.IsHouseColor;
            pLaser.Ref.IsSupported = type.IsSupported ? type.IsSupported : (type.Thickness > 5 && !type.Fade);
        }

        public static void DrawBeam(CoordStruct sourcePos, CoordStruct targetPos, BeamType type, ColorStruct customColor = default)
        {
            ColorStruct beamColor = type.BeamColor;

            if (default != customColor)
            {
                beamColor = customColor;
            }
            Pointer<RadBeam> pRadBeam = RadBeam.Allocate(type.RadBeamType);
            if (!pRadBeam.IsNull)
            {
                pRadBeam.Ref.SetCoordsSource(sourcePos);
                pRadBeam.Ref.SetCoordsTarget(targetPos);
                pRadBeam.Ref.Color = beamColor;
                pRadBeam.Ref.Period = type.Period;
                pRadBeam.Ref.Amplitude = type.Amplitude;
            }
        }

        public static void DrawBolt(CoordStruct sourcePos, CoordStruct targetPos, bool alternate = false)
        {
            BoltType type = new BoltType(alternate);
            DrawBolt(sourcePos, targetPos, type);
        }

        public static void DrawBolt(CoordStruct sourcePos, CoordStruct targetPos, BoltType type)
        {
            Pointer<EBolt> pBolt = YRMemory.Create<EBolt>();
            if (!pBolt.IsNull)
            {
                EBoltExt ext = EBoltExt.ExtMap.Find(pBolt);
                if (null != ext)
                {
                    ext.Color1 = type.Color1;
                    ext.Color2 = type.Color2;
                    ext.Color3 = type.Color3;
                    ext.Disable1 = type.Disable1;
                    ext.Disable2 = type.Disable2;
                    ext.Disable3 = type.Disable3;
                }
                pBolt.Ref.AlternateColor = type.IsAlternateColor;
                pBolt.Ref.Fire(sourcePos, targetPos, 0);
            }
        }

        public static void DrawBolt(Pointer<TechnoClass> pShooter, Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon, CoordStruct sourcePos)
        {
            Pointer<EBolt> pEBolt = pShooter.Ref.Electric_Zap(pTarget, pWeapon, sourcePos);
        }

        public static void DrawParticele(CoordStruct sourcePos, CoordStruct targetPos, string systemName)
        {
            Pointer<ParticleSystemTypeClass> psType = ParticleSystemTypeClass.ABSTRACTTYPE_ARRAY.Find(systemName);
            if (!psType.IsNull)
            {
                BulletEffectHelper.DrawParticele(psType, sourcePos, targetPos);
            }
        }

        public static void DrawParticele(Pointer<ParticleSystemTypeClass> psType, CoordStruct sourcePos, CoordStruct targetPos)
        {
            BulletEffectHelper.DrawParticele(psType, sourcePos, Pointer<TechnoClass>.Zero, targetPos);
        }

        public static void DrawParticele(Pointer<ParticleSystemTypeClass> psType, CoordStruct sourcePos, Pointer<TechnoClass> pOwner, CoordStruct targetPos)
        {
            BulletEffectHelper.DrawParticele(psType, sourcePos, Pointer<AbstractClass>.Zero, pOwner, targetPos);
        }

        public static void DrawParticele(Pointer<ParticleSystemTypeClass> psType, CoordStruct sourcePos, Pointer<AbstractClass> pTarget, Pointer<TechnoClass> pOwner, CoordStruct targetPos)
        {
            YRMemory.Create<ParticleSystemClass>(psType, sourcePos, pTarget, pOwner, targetPos);
        }

    }

    [Serializable]
    public class LaserType
    {
        public ColorStruct InnerColor;
        public ColorStruct OuterColor;
        public ColorStruct OuterSpread;
        public int Duration;
        public int Thickness;
        public bool IsHouseColor;
        public bool IsSupported;
        public bool Fade;

        public LaserType(bool def)
        {
            if (def)
            {
                this.InnerColor = new ColorStruct(204, 64, 6);
                this.OuterColor = new ColorStruct(102, 32, 3);
            }
            else
            {
                this.InnerColor = default;
                this.OuterColor = default;
            }
            this.OuterSpread = default;
            this.Duration = 15;
            this.Thickness = 2;
            this.IsHouseColor = false;
            this.IsSupported = false;
            this.Fade = true;
        }

        public void SetInnerColor(int R, int G, int B)
        {
            this.InnerColor.R = (byte)R;
            this.InnerColor.G = (byte)G;
            this.InnerColor.B = (byte)B;
        }

        public void SetOuterColor(int R, int G, int B)
        {
            this.OuterColor.R = (byte)R;
            this.OuterColor.G = (byte)G;
            this.OuterColor.B = (byte)B;
        }

        public void SetOuterSpread(int R, int G, int B)
        {
            this.OuterSpread.R = (byte)R;
            this.OuterSpread.G = (byte)G;
            this.OuterSpread.B = (byte)B;
        }

    }

    [Serializable]
    public class BeamType
    {
        public RadBeamType RadBeamType;
        public ColorStruct BeamColor;
        public int Period;
        public double Amplitude;

        public BeamType(RadBeamType radBeamType)
        {
            SetBeamType(radBeamType);
            this.Period = 15;
            this.Amplitude = 40.0;
        }

        public void SetBeamType(RadBeamType radBeamType)
        {
            this.RadBeamType = radBeamType;
            switch (radBeamType)
            {
                case RadBeamType.Temporal:
                    this.BeamColor = new ColorStruct(128, 200, 255);
                    break;
                default:
                    this.BeamColor = new ColorStruct(0, 255, 0);
                    break;
            }
        }

    }

    [Serializable]
    public class BoltType
    {
        public bool IsAlternateColor;
        public ColorStruct Color1;
        public ColorStruct Color2;
        public ColorStruct Color3;
        public bool Disable1;
        public bool Disable2;
        public bool Disable3;

        public BoltType(bool alternate)
        {
            IsAlternateColor = alternate;
            Color1 = default;
            Color2 = default;
            Color3 = default;
            Disable1 = false;
            Disable2 = false;
            Disable3 = false;
        }
    }
}

