﻿using Server.Mobiles;
using Server.Spells.Seventh;
using Server.Targeting;
using System;

namespace Server.Spells.Zulu.NecroSpells
{
    public class VengefulSpiritSpell : NecroSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Vengeful Spirit", "Voca amicus tenebris",
            203,
            9051,
            Reagent.ExecutionersCap,
            Reagent.BloodSpawn,
            Reagent.WyrmsHeart,
            Reagent.BlackMoor,
            Reagent.Bone);



        public VengefulSpiritSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }



        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (this.Caster == m)
            {
                this.Caster.SendLocalizedMessage(1061832); // You cannot exact vengeance on yourself.
            }
            else if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                /* Summons a Revenant which haunts the target until either the target or the Revenant is dead.
                * Revenants have the ability to track down their targets wherever they may travel.
                * A Revenant's strength is determined by the Necromancy and Spirit Speak skills of the Caster.
                * The effect lasts for ((Spirit Speak skill level * 80) / 120) + 10 seconds.
                */

                TimeSpan duration = TimeSpan.FromSeconds(((this.GetDamageSkill(this.Caster) * 80) / 120) + 10);

                Revenant rev = new Revenant(this.Caster, m, duration);

                if (BaseCreature.Summon(rev, false, this.Caster, m.Location, 0x81, TimeSpan.FromSeconds(duration.TotalSeconds + 2.0)))
                    rev.FixedParticles(0x373A, 1, 15, 9909, EffectLayer.Waist);

                Caster.PlaySound(0x22B);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly VengefulSpiritSpell m_Owner;
            public InternalTarget(VengefulSpiritSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    this.m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
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
                return 80;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 41;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return false;
            }
        }

    }
}
