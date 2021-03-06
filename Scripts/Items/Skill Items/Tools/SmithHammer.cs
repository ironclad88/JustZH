using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [FlipableAttribute(0x13E3, 0x13E4)]
    public class SmithHammer : BaseBashing, ICraftable, IUsesRemaining
    {
        private SmithHammerTool m_Tool;

        // This class is needed to be able to craft with the hammer, 
        // unfortunately it also needs knowledge of its parent for uses remaining...
        private class SmithHammerTool : BaseTool
        {
            private int m_UsesRemaining;
            SmithHammer m_Parent;
            public SmithHammerTool(SmithHammer parent)
            : base(0x13E3)
            {
                m_Parent = parent;
            }
            public SmithHammerTool( Serial serial)
                : base(serial)
            {
            }
            public override CraftSystem CraftSystem
            {
                get
                {
                    return DefBlacksmithy.CraftSystem;
                }
            }
            [CommandProperty(AccessLevel.GameMaster)]
            public override int UsesRemaining
            {
                get
                {
                    return this.m_UsesRemaining;
                }
                set
                {
                    if (this.m_UsesRemaining == value + 1)
                    {
                        m_Parent.FindOwner().SendLocalizedMessage(1044118); // Your tool suffers some damage.
                    }
                    this.m_UsesRemaining = value;
                    this.InvalidateProperties();
                    m_Parent.InvalidateProperties();
                }
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

        [Constructable]
        public SmithHammer()
            : base(0x13E3)
        {
            this.Weight = 8.0;
            this.Layer = Layer.OneHanded;
            this.m_Tool = new SmithHammerTool(this);
            this.m_Tool.UsesRemaining = 100;
        }

        public SmithHammer(Serial serial)
            : base(serial)
        {
            this.Weight = 8.0;
            this.Layer = Layer.OneHanded;
            this.m_Tool = new SmithHammerTool(this);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_Tool.UsesRemaining;
            }
            set
            {
                this.m_Tool.UsesRemaining = value;
                this.InvalidateProperties();
            }
        }
        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack) || this.Parent == from)
            {
                CraftSystem system = m_Tool.CraftSystem;
                m_Tool.Parent = this.Parent;

                int num = system.CanCraft(from, this.m_Tool, null);

                if (num > 0 && (num != 1044267 || !Core.SE)) // Blacksmithing shows the gump regardless of proximity of an anvil and forge after SE
                {
                    from.SendLocalizedMessage(num);
                }
                else
                {
                    CraftContext context = system.GetContext(from);

                    from.SendGump(new CraftGump(from, system, m_Tool, null));
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.CrushingBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.MortalStrike;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 80;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 16;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 20;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 26;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 4.00f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 30;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 10;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 30;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 32;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 110;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            writer.Write(UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            if(version >= 1)
            {
                UsesRemaining = reader.ReadInt();
            }
        }
    }
}