﻿using System;

namespace Server.Mobiles
{
    [CorpseName("a ghost llama corpse")]
    public class GhostLlama : BaseMount
    {

        [Constructable]
        public GhostLlama()
            : this("a ghost Llama")
        {
        }

        [Constructable]
        public GhostLlama(string name)
            : base(name, 0xDC, 0x3EA6, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a ghost llama";
            this.stableName = "a ghost llama";
            this.BaseSoundID = 0x3F3;

            this.Hue = 16385;

            this.SetStr(21, 49);
            this.SetDex(36, 55);
            this.SetInt(16, 30);

            this.SetHits(15, 27);
            this.SetMana(0);

            this.SetDamage(3, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);

            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 19.2, 29.0);
            this.SetSkill(SkillName.Wrestling, 19.2, 29.0);

            this.Fame = 300;
            this.Karma = 0;

            this.VirtualArmor = 16;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 50;
        }

        public GhostLlama(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 12;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
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