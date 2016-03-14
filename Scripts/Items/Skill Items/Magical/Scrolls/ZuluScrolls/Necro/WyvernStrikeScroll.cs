﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items.Skill_Items.Magical.Scrolls.ZuluScrolls.Necro
{
    public class WyvernStrikeScroll : SpellScroll
    {
        [Constructable]
        public WyvernStrikeScroll()
            : this(1)
        {
        }

        [Constructable]
        public WyvernStrikeScroll(int amount)
            : base(80, 0x1f3c, amount)
        {
            this.Hue = 0x66D;
            this.Name = "Wyvern strike scroll";
        }

        public WyvernStrikeScroll(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}