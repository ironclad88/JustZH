﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Spells.Zulu.NecroSpells
{
    public abstract class NecroSpell : Spell
    {
        public NecroSpell(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
        {
        }

        public abstract double RequiredSkill { get; }
        public abstract int RequiredMana { get; }

        public override SkillName CastSkill
        {
            get
            {
                return SkillName.Magery;
            }
        }
        public override SkillName DamageSkill
        {
            get
            {
                return SkillName.Magery;
            }
        }

        public override bool ClearHandsOnCast
        {
            get
            {
                return false;
            }
        }
        public override double CastDelayFastScalar
        {
            get
            {
                return (Core.SE ? base.CastDelayFastScalar : 0);
            }
        }// Necromancer spells are not affected by fast cast items, though they are by fast cast recovery
        public override int ComputeKarmaAward()
        {
            return 0;
        }

        public override void GetCastSkills(out double min, out double max)
        {
            min = this.RequiredSkill;
            max = this.Scroll != null ? min : this.RequiredSkill + 40.0;
        }

        public virtual double GetResistPercent(Mobile target) // JustZH TODO: remake this
        {
            return this.GetResistPercentForCircle(target);
        }

        public virtual double GetResistPercentForCircle(Mobile target) // JustZH TODO: remake this
        {
            double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
            double secondPercent = target.Skills[SkillName.MagicResist].Value - (((this.Caster.Skills[this.CastSkill].Value - 20.0) / 5.0) + (1 + 10 * 5.0));

            return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0;
        }

        public virtual bool CheckResisted(Mobile target) // JustZH TODO: remake this
        {
            double n = this.GetResistPercent(target);

            n /= 100.0;

            if (n <= 0.0)
                return false;

            if (n >= 1.0)
                return true;

            int maxSkill = (1 + 10) * 10;
            maxSkill += (1 + (10 / 6)) * 25;

            if (target.Skills[SkillName.MagicResist].Value < maxSkill)
                target.CheckSkill(SkillName.MagicResist, 0.0, target.Skills[SkillName.MagicResist].Cap);

            return (n >= Utility.RandomDouble());
        }


        public override int GetMana()
        {
            return this.RequiredMana;
        }
    }
}