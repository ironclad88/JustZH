﻿using System;
using Server.Mobiles;
using Server.Spells;
using Server.Custom;

namespace Server.Items
{
    public class DragonEgg : Item
    {
        [Constructable]
        public DragonEgg()
            : base(0x1726) //0xFF2
        {
            this.Movable = true;
            this.Hue = 33;
            this.Stackable = true;
        }

        [Constructable]
        public DragonEgg(Serial serial)
            : base(serial)
        {
        }

        private static readonly Type[] m_Types = new Type[] // shouldnt be able to summon a GD ;)
        {
            typeof(Drake),
            typeof(Dragon)
        };

        public override string DefaultName
        {
            get
            {
                return "Dragon Egg";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            BaseCreature creature = (BaseCreature)Activator.CreateInstance(m_Types[Utility.Random(m_Types.Length)]);
            TimeSpan duration;
            duration = TimeSpan.FromDays(1);
            SpellHelper.Summon(creature, from, 0x215, duration, false, false);
            creature.Summoned = false;
            from.SendMessage("The egg begins to move and");
            RandomClass rnd = new RandomClass();
            var diceRoll = rnd.D20Roll(1);
            Console.WriteLine("D20 Dice roll: " + diceRoll);
            if (diceRoll <= 6)
            {
                if (from.Skills.AnimalLore.Value >= 115)
                {
                    from.SendMessage("A baby dragon appears and accepts you as his master!");
                    creature.Controlled = true;
                }
                else { 
                creature.Controlled = false;
                }
            }
            else
            {
                from.SendMessage("A baby dragon appears!");
                creature.Controlled = false;
            }
            this.Consume(1);
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