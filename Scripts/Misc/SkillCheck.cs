using System;
using Server.Factions;
using Server.Mobiles;

namespace Server.Misc
{
    public class SkillCheck
    {
        private static readonly bool AntiMacroCode = false;		//Change this to false to disable anti-macro code

        public static TimeSpan AntiMacroExpire = TimeSpan.FromMinutes(5.0); //How long do we remember targets/locations?
        public const int Allowance = 3;	//How many times may we use the same location/target for gain
        private const int LocationSize = 1; //The size of eeach location, make this smaller so players dont have to move as far
        private static readonly bool[] UseAntiMacro = new bool[]
        {
            // true if this skill uses the anti-macro code, false if it does not
            false, // Alchemy = 0,
            false, // Anatomy = 1,
            false, // AnimalLore = 2,
            false, // ItemID = 3,
            false, // ArmsLore = 4,
            false, // Parry = 5,
            false, // Begging = 6,
            false, // Blacksmith = 7,
            false, // Fletching = 8,
            false, // Peacemaking = 9,
            false, // Camping = 10,
            false, // Carpentry = 11,
            false, // Cartography = 12,
            false, // Cooking = 13,
            false, // DetectHidden = 14,
            false, // Discordance = 15,
            false, // EvalInt = 16,
            false, // Healing = 17,
            false, // Fishing = 18,
            false, // Forensics = 19,
            false, // Herding = 20,
            false, // Hiding = 21,
            false, // Provocation = 22,
            false, // Inscribe = 23,
            false, // Lockpicking = 24,
            false, // Magery = 25,
            false, // MagicResist = 26,
            false, // Tactics = 27,
            false, // Snooping = 28,
            false, // Musicianship = 29,
            false, // Poisoning = 30,
            false, // Archery = 31,
            false, // SpiritSpeak = 32,
            false, // Stealing = 33,
            false, // Tailoring = 34,
            false, // AnimalTaming = 35,
            false, // TasteID = 36,
            false, // Tinkering = 37,
            false, // Tracking = 38,
            false, // Veterinary = 39,
            false, // Swords = 40,
            false, // Macing = 41,
            false, // Fencing = 42,
            false, // Wrestling = 43,
            false, // Lumberjacking = 44,
            false, // Mining = 45,
            false, // Meditation = 46,
            false, // Stealth = 47,
            false, // RemoveTrap = 48,
            false, // Necromancy = 49,
            false, // Focus = 50,
            false, // Chivalry = 51
            false, // Bushido = 52
            false, //Ninjitsu = 53
            false, // Spellweaving = 54
            #region Stygian Abyss
            false, // Mysticism = 55
            false, // Imbuing = 56
            false// Throwing = 57
            #endregion
        };

        public static void Initialize()
        {
            Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckLocation);
            Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectLocation);

            Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckTarget);
            Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectTarget);
        }

        public static double SuccessChance(Mobile from, SkillName skillName, double difficulty, bool doPrint = false)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return 0;

            double value = skill.Value;

            if (true == doPrint)
            {
                // JustZH: Print difficulty without decimal.
                from.SendMessage("Difficulty: " + (int)difficulty);
            }

            double chance = 0.85; // JustZH: This is the base chance, if skill.Value == difficulty this applies.

            // JustZH: if skill is 15 above or below difficulty, no fail chance
            if (value >= (difficulty + 15))
                return 1; // No challenge

            if (value > difficulty)
            {
                // Skill is above difficulty, increase chance. Linear increase up to 100% at 15 skill over difficulty.
                chance += (value - difficulty) / 100;
            }
            else if ( value < difficulty)
            {
                // Skill is below difficulty, decrease chance. This should go very close to 0 but never reach it.
                double missing_skill = difficulty - value;
                chance *= Math.Pow(0.95, missing_skill);
            }

            // Only 0-100%
            if (chance < 0) chance = 0;
            if (chance > 1) chance = 1;

            return chance;
        }

        public static bool Mobile_SkillCheckLocation(Mobile from, SkillName skillName, double difficulty, bool doPrint )//double minSkill, double maxSkill)
        {
            Skill skill = from.Skills[skillName];
            double chance = SuccessChance(from, skillName, difficulty, doPrint);

            if(skill == null)
                return false;
            if (chance < 0.0)
                return false; // Too difficult
            else if (chance >= 1.0)
                return true; // No challenge

            Point2D loc = new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize);
            return CheckSkill(from, skill, loc, chance, true);
        }

        public static bool Mobile_SkillCheckDirectLocation(Mobile from, SkillName skillName, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false; // Too difficult
            else if (chance >= 1.0)
                return true; // No challenge

            Point2D loc = new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize);
            return CheckSkill(from, skill, loc, chance);
        }

        public static bool CheckSkill(Mobile from, Skill skill, object amObj, double chance, bool isActive = false)
        {
            if (from.Skills.Cap == 0)
                return false;

            bool success = (chance >= Utility.RandomDouble());
            // JustZH: redo gains chance. Some kind of distribution that is high at low skill and decreases towards the end.
            double gc = -Math.Log(skill.Base / skill.Cap) * 0.2;
            #region old
#if false
            double gc = 1.0; // JustZH: was (double)(from.Skills.Cap - from.Skills.Total) / from.Skills.Cap;
            gc += (skill.Cap - skill.Base) / skill.Cap;
            gc /= 3; // was 2

            gc += (1.0 - chance) * (success ? 0.5 : (Core.AOS ? 0.0 : 0.2));
            gc /= 3; // was 2
#endif
#endregion

            gc *= skill.Info.GainFactor;

            if(true == isActive)
            {
                // This is an active skill, and the success chance should affect the gains chance
                gc /= chance; // Evens out success chance and gains chance so that everything yeils about the same net gains.
                // <40% success chance:  halven gains chance
                // >95%  success chance: halven gains chance
                if (chance < 0.4 || chance > 0.95)
                {
                    gc /= 2;
                }
            }

            if (gc < 0.01 || (skill.Value > 50 && !success)) // JustZH: almost no chance to gain on fail past 50 skill
                gc = 0.01;

            if (from is BaseCreature && ((BaseCreature)from).Controlled)
                gc *= 2;

            if (from.Alive && ((gc >= Utility.RandomDouble() && AllowGain(from, skill, amObj)) || skill.Base < 10.0))
                Gain(from, skill);

            return success;
        }

        private static bool IsLoreSkill(SkillName skillName)
        {
            if (SkillName.Anatomy == skillName ||
                SkillName.EvalInt == skillName ||
                SkillName.ItemID == skillName ||
                SkillName.ArmsLore == skillName ||
                SkillName.AnimalLore == skillName ||
                SkillName.Stealing == skillName ||  // TEMPORARY REMOVE LATER WHEN REMAKE STEALING
                SkillName.TasteID == skillName ||
                SkillName.Forensics == skillName )
                return true;
            return false;
        }

        public static bool Mobile_SkillCheckTarget(Mobile from, SkillName skillName, object target, double difficulty, bool doPrint)// double minSkill, double maxSkill)
        {
            Skill skill = from.Skills[skillName];
            if (skill == null)
                return false;
            double chance;
            bool is_active = true;
            if (true == IsLoreSkill(skillName))
            {
                // Special handling of lore skills, since their difficulty is not adjustable by choosing a different target..
                double value;
                if (skill == null)
                    value = 0;
                value = skill.Value;

                chance = 0.2 + (value / 100);
                is_active = false; // don't count these as active
            }
            else
            {
                chance = SuccessChance(from, skillName, difficulty, doPrint);
                if (chance < 0.0)
                    return false; // Too difficult
                else if (chance >= 1.0)
                    return true; // No challenge
            }

            return CheckSkill(from, skill, target, chance, is_active);
        }

        public static bool Mobile_SkillCheckDirectTarget(Mobile from, SkillName skillName, object target, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false; // Too difficult
            else if (chance >= 1.0)
                return true; // No challenge

            return CheckSkill(from, skill, target, chance);
        }

        private static bool IsForge(object obj)
        {
            if (Core.ML && obj is Mobile && ((Mobile)obj).IsDeadBondedPet)
                return false;

            if (obj.GetType().IsDefined(typeof(Server.Engines.Craft.ForgeAttribute), false))
                return true;

            int itemID = 0;

            if (obj is Item)
                itemID = ((Item)obj).ItemID;
            else if (obj is Server.Targeting.StaticTarget)
                itemID = ((Server.Targeting.StaticTarget)obj).ItemID;

            return (itemID == 4017 || (itemID >= 6522 && itemID <= 6569));
        }

        private static bool AllowGain(Mobile from, Skill skill, object obj)
        {
            if (Core.AOS && Faction.InSkillLoss(from))	//Changed some time between the introduction of AoS and SE.
                return false;

#region SA
            if (from is PlayerMobile && from.Race == Race.Gargoyle && skill.Info.SkillID == (int)SkillName.Archery)
                return false;
            else if (from is PlayerMobile && from.Race != Race.Gargoyle && skill.Info.SkillID == (int)SkillName.Throwing)
                return false;
#endregion

#region JustZH special no-gains
             if(skill.SkillName == SkillName.Mining && IsForge(obj))
            {
                // No gaining from smelting...
                return false;
            }
#endregion

            if (AntiMacroCode && from is PlayerMobile && UseAntiMacro[skill.Info.SkillID])
                return ((PlayerMobile)from).AntiMacroCheck(skill, obj);
            else
                return true;
        }

        public enum Stat
        {
            Str,
            Dex,
            Int
        }

        public static void Gain(Mobile from, Skill skill)
        {
            if (from.Region.IsPartOf(typeof(Regions.Jail)))
                return;

            if (from is BaseCreature && ((BaseCreature)from).IsDeadPet)
                return;

            if (skill.SkillName == SkillName.Focus && from is BaseCreature)
                return;

            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                int toGain = 1;

                if (skill.Base <= 10.0)
                    toGain = Utility.Random(4) + 1;

                Skills skills = from.Skills;

                if (from.Player && (skills.Total / skills.Cap) >= Utility.RandomDouble())//( skills.Total >= skills.Cap )
                {
                    for (int i = 0; i < skills.Length; ++i)
                    {
                        Skill toLower = skills[i];

                        if (toLower != skill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= toGain)
                        {
                            toLower.BaseFixedPoint -= toGain;
                            break;
                        }
                    }
                }

#region Mondain's Legacy
                if (from is PlayerMobile)
                    if (Server.Engines.Quests.QuestHelper.EnhancedSkill((PlayerMobile)from, skill))
                        toGain *= Utility.RandomMinMax(2, 4);
#endregion

#region Scroll of Alacrity
                PlayerMobile pm = from as PlayerMobile;

                if (from is PlayerMobile)
                {
                    if (pm != null && skill.SkillName == pm.AcceleratedSkill && pm.AcceleratedStart > DateTime.UtcNow)
                    {
                        pm.SendLocalizedMessage(1077956); // You are infused with intense energy. You are under the effects of an accelerated skillgain scroll.
                        toGain = Utility.RandomMinMax(2, 5);
                    }
                }
#endregion

                if (!from.Player || (skills.Total + toGain) <= skills.Cap)
                {
                    skill.BaseFixedPoint += toGain;
                }
            }

#region Mondain's Legacy
            if (from is PlayerMobile)
                Server.Engines.Quests.QuestHelper.CheckSkill((PlayerMobile)from, skill);
#endregion

            if (skill.Lock == SkillLock.Up)
            {
                SkillInfo info = skill.Info;
                /*   Some classes gain faster/slower   */
                // JustZH Stat gain for Classes
                if ((from.SpecClasse == SpecClasse.Ranger || from.SpecClasse == SpecClasse.Bard) && from.DexLock == StatLockType.Up && (info.DexGain / 30.0) > Utility.RandomDouble()) // rangers and bard have it easier to gain dex
                {
                    GainStat(from, Stat.Dex);
                }
                if ((from.SpecClasse == SpecClasse.Bard) && from.IntLock == StatLockType.Up && (info.IntGain / 30.0) > Utility.RandomDouble()) // bards have it easier to gain int
                {
                    GainStat(from, Stat.Int);
                }
                if ((from.SpecClasse == SpecClasse.Warrior) && from.IntLock == StatLockType.Up && (info.IntGain / 36.6) > Utility.RandomDouble()) // Warriors gain int slower
                {
                    GainStat(from, Stat.Int);
                }
                if ((from.SpecClasse == SpecClasse.Mage) && from.StrLock == StatLockType.Up && (info.StrGain / 36.6) > Utility.RandomDouble()) // mages have harder to gain str
                {
                    GainStat(from, Stat.Str);
                }
                /*   End of class gain   */

                if (from.StrLock == StatLockType.Up && (info.StrGain / 33.3) > Utility.RandomDouble()){
                    GainStat(from, Stat.Str);
                }
                else if (from.DexLock == StatLockType.Up && (info.DexGain / 33.3) > Utility.RandomDouble()){
                    GainStat(from, Stat.Dex);
                }
                else if (from.IntLock == StatLockType.Up && (info.IntGain / 33.3) > Utility.RandomDouble()) { 
                    GainStat(from, Stat.Int);
                }
            }
        }

        public static bool CanLower(Mobile from, Stat stat)
        {
            switch (stat)
            {
                case Stat.Str:
                    return (from.StrLock == StatLockType.Down && from.RawStr > 10);
                case Stat.Dex:
                    return (from.DexLock == StatLockType.Down && from.RawDex > 10);
                case Stat.Int:
                    return (from.IntLock == StatLockType.Down && from.RawInt > 10);
            }

            return false;
        }

        public static bool CanRaise(Mobile from, Stat stat)
        {
            if (!(from is BaseCreature && ((BaseCreature)from).Controlled))
            {
                if (from.RawStatTotal >= from.StatCap)
                    return false;
            }

            switch (stat)
            {
                case Stat.Str:
                    return (from.StrLock == StatLockType.Up && from.RawStr < 125);
                case Stat.Dex:
                    return (from.DexLock == StatLockType.Up && from.RawDex < 125);
                case Stat.Int:
                    return (from.IntLock == StatLockType.Up && from.RawInt < 125);
            }

            return false;
        }

        public static void IncreaseStat(Mobile from, Stat stat, bool atrophy)
        {
            atrophy = atrophy || (from.RawStatTotal >= from.StatCap);

            switch (stat)
            {
                case Stat.Str:
                    {
                        if (atrophy)
                        {
                            if (CanLower(from, Stat.Dex) && (from.RawDex < from.RawInt || !CanLower(from, Stat.Int)))
                                --from.RawDex;
                            else if (CanLower(from, Stat.Int))
                                --from.RawInt;
                        }

                        if (CanRaise(from, Stat.Str))
                            ++from.RawStr;

                        break;
                    }
                case Stat.Dex:
                    {
                        if (atrophy)
                        {
                            if (CanLower(from, Stat.Str) && (from.RawStr < from.RawInt || !CanLower(from, Stat.Int)))
                                --from.RawStr;
                            else if (CanLower(from, Stat.Int))
                                --from.RawInt;
                        }

                        if (CanRaise(from, Stat.Dex))
                            ++from.RawDex;

                        break;
                    }
                case Stat.Int:
                    {
                        if (atrophy)
                        {
                            if (CanLower(from, Stat.Str) && (from.RawStr < from.RawDex || !CanLower(from, Stat.Dex)))
                                --from.RawStr;
                            else if (CanLower(from, Stat.Dex))
                                --from.RawDex;
                        }

                        if (CanRaise(from, Stat.Int))
                            ++from.RawInt;

                        break;
                    }
            }
        }

        private static readonly TimeSpan m_StatGainDelay = TimeSpan.FromMinutes(20.0); // changed from 15
        private static readonly TimeSpan m_PetStatGainDelay = TimeSpan.FromMinutes(10.0); // changed from ...... something, i think 5

        public static void GainStat(Mobile from, Stat stat)
        {
            switch (stat)
            {
                case Stat.Str:
                    {
                        if (from is BaseCreature && ((BaseCreature)from).Controlled)
                        {
                            if ((from.LastStrGain + m_PetStatGainDelay) >= DateTime.UtcNow)
                                return;
                        }
                        else if ((from.LastStrGain + m_StatGainDelay) >= DateTime.UtcNow)
                            return;

                        from.LastStrGain = DateTime.UtcNow;
                        break;
                    }
                case Stat.Dex:
                    {
                        if (from is BaseCreature && ((BaseCreature)from).Controlled)
                        {
                            if ((from.LastDexGain + m_PetStatGainDelay) >= DateTime.UtcNow)
                                return;
                        }
                        else if ((from.LastDexGain + m_StatGainDelay) >= DateTime.UtcNow)
                            return;

                        from.LastDexGain = DateTime.UtcNow;
                        break;
                    }
                case Stat.Int:
                    {
                        if (from is BaseCreature && ((BaseCreature)from).Controlled)
                        {
                            if ((from.LastIntGain + m_PetStatGainDelay) >= DateTime.UtcNow)
                                return;
                        }
                        else if ((from.LastIntGain + m_StatGainDelay) >= DateTime.UtcNow)
                            return;

                        from.LastIntGain = DateTime.UtcNow;
                        break;
                    }
            }

            bool atrophy = ((from.RawStatTotal / (double)from.StatCap) >= Utility.RandomDouble());

            IncreaseStat(from, stat, atrophy);
        }
    }
}