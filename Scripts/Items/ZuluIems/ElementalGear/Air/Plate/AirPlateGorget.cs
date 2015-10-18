﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items.ZuluIems.ElementalGear.Water.Plate
{
    public class AirPlateGorget : BaseArmor
    {
        [Constructable]
        public AirPlateGorget()
            : base(0x1413)
        {
            this.Weight = 2.0;
            this.Hue = 1161;
            this.EnergyBonus = 20;
            this.Attributes.BonusDex = 1;
            this.Name = "Plate gorget of the Air Element";
        }

        public AirPlateGorget(Serial serial)
            : base(serial)
        {
        }

        public override int Dexpenalty
        {
            get
            {
                return 0;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 2;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 2;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 50;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 65;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 45;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 30;
            }
        }
        public override int OldDexBonus
        {
            get
            {
                return -1;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 50;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Plate;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}