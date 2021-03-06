﻿using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Spells.Zulu.EarthSpells
{
    class Earthportal : EarthSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
           "Earth Portal", "Destraves Limites Da Natureza",
           263,
           9032,
           Reagent.BrimStone,
           Reagent.ExecutionersCap,
           Reagent.EyeofNewt);
        private readonly RunebookEntry m_Entry;
        public Earthportal(Mobile caster, Item scroll)
            : this(caster, scroll, null)
        {
        }

        public Earthportal(Mobile caster, Item scroll, RunebookEntry entry)
            : base(caster, scroll, m_Info)
        {
            this.m_Entry = entry;
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 90; // dunno about this, gotta check
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }

        public override void OnCast()
        {
            if (this.m_Entry == null)
                this.Caster.Target = new InternalTarget(this);
            else
                this.Effect(this.m_Entry.Location, this.m_Entry.Map, true);
        }

        public override bool CheckCast()
        {

            return SpellHelper.CheckTravel(this.Caster, TravelCheckType.GateFrom);
        }

        public void Effect(Point3D loc, Map map, bool checkMulti) // JustZH removed some stupid conditions
        {
            if (!SpellHelper.CheckTravel(this.Caster, TravelCheckType.GateFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(this.Caster, map, loc, TravelCheckType.GateTo))
            {
            }
            else if (!map.CanSpawnMobile(loc.X, loc.Y, loc.Z))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if ((checkMulti && SpellHelper.CheckMulti(loc, map)))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (Core.SE && (this.GateExistsAt(map, loc) || this.GateExistsAt(this.Caster.Map, this.Caster.Location))) // SE restricted stacking gates
            {
                this.Caster.SendLocalizedMessage(1071242); // There is already a gate there.
            }
            else if (this.CheckSequence())
            {
                this.Caster.SendLocalizedMessage(501024); // You open a magical gate to another location

                Effects.PlaySound(this.Caster.Location, this.Caster.Map, 0x20F);

                InternalItem firstGate = new InternalItem(loc, map);
                firstGate.MoveToWorld(this.Caster.Location, this.Caster.Map);

               // firstGate.Hue = 1160;



                Effects.PlaySound(loc, map, 0x20F);

                InternalItem secondGate = new InternalItem(this.Caster.Location, this.Caster.Map);
                secondGate.MoveToWorld(loc, map);
              //  secondGate.Hue = 1160;
            }

            this.FinishSequence();
        }

        private bool GateExistsAt(Map map, Point3D loc)
        {
            bool _gateFound = false;

            IPooledEnumerable eable = map.GetItemsInRange(loc, 0);
            foreach (Item item in eable)
            {
                if (item is Moongate || item is PublicMoongate)
                {
                    _gateFound = true;
                    break;
                }
            }
            eable.Free();

            return _gateFound;
        }

        [DispellableField]
        private class InternalItem : Moongate
        {
            public InternalItem(Point3D target, Map map)
                : base(target, map)
            {
                this.Map = map;
                this.ItemID = 0xF6C;
                this.Hue = 1160;

                this.Dispellable = true;

                InternalTimer t = new InternalTimer(this);
                t.Start();
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override bool ShowFeluccaWarning
            {
                get
                {
                    return Core.AOS;
                }
            }
            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                this.Delete();
            }

            private class InternalTimer : Timer
            {
                private readonly Item m_Item;
                public InternalTimer(Item item)
                    : base(TimeSpan.FromSeconds(50.0)) // longer duration, but still a worhtless spell ^^
                {
                    this.Priority = TimerPriority.OneSecond;
                    this.m_Item = item;
                }

                protected override void OnTick()
                {
                    this.m_Item.Delete();
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly Earthportal m_Owner;
            public InternalTarget(Earthportal owner)
                : base(12, false, TargetFlags.None)
            {
                this.m_Owner = owner;

                owner.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501029); // Select Marked item.
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is RecallRune)
                {
                    RecallRune rune = (RecallRune)o;

                    if (rune.Marked)
                        this.m_Owner.Effect(rune.Target, rune.TargetMap, true);
                    else
                        from.SendLocalizedMessage(501803); // That rune is not yet marked.
                }
                else if (o is Runebook)
                {
                    RunebookEntry e = ((Runebook)o).Default;

                    if (e != null)
                        this.m_Owner.Effect(e.Location, e.Map, true);
                    else
                        from.SendLocalizedMessage(502354); // Target is not marked.
                }
                else if (o is HouseRaffleDeed && ((HouseRaffleDeed)o).ValidLocation())
                {
                    HouseRaffleDeed deed = (HouseRaffleDeed)o;

                    this.m_Owner.Effect(deed.PlotLocation, deed.PlotFacet, true);
                }
                else
                {
                    from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501030, from.Name, "")); // I can not gate travel from that object.
                }
            }

            protected override void OnNonlocalTarget(Mobile from, object o)
            {
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}
