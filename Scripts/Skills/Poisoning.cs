using System;
using Server.Items;
using Server.Targeting;

namespace Server.SkillHandlers
{
    public class Poisoning
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Poisoning].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.Target = new InternalTargetPoison();

            m.SendLocalizedMessage(502137); // Select the poison you wish to use

            return TimeSpan.FromSeconds(10.0); // 10 second delay before beign able to re-use a skill
        }

        private class InternalTargetPoison : Target
        {
            public InternalTargetPoison()
                : base(2, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BasePoisonPotion)
                {
                    from.SendLocalizedMessage(502142); // To what do you wish to apply the poison?
                    from.Target = new InternalTarget((BasePoisonPotion)targeted);
                }
                else // Not a Poison Potion
                {
                    from.SendLocalizedMessage(502139); // That is not a poison potion.
                }
            }

            private class InternalTarget : Target
            {
                private readonly BasePoisonPotion m_Potion;
                public InternalTarget(BasePoisonPotion potion)
                    : base(2, false, TargetFlags.None)
                {
                    this.m_Potion = potion;
                }

                protected override void OnTarget(Mobile from, object targeted)
                {
                    if (this.m_Potion.Deleted)
                        return;

                    bool startTimer = false;

                    if (targeted is BaseWeapon)
                    {
                        BaseWeapon weapon = (BaseWeapon)targeted;

                        startTimer = (weapon.Type == WeaponType.Slashing || weapon.Type == WeaponType.Piercing || weapon.Type == WeaponType.Axe || weapon.Type == WeaponType.Bashing || weapon.Type == WeaponType.Polearm || weapon.Type == WeaponType.Staff || weapon.Type == WeaponType.Ranged);
                    }

                    if (startTimer)
                    {
                        new InternalTimer(from, (Item)targeted, this.m_Potion).Start();

                        from.PlaySound(0x4F);

                        if (!Engines.ConPVP.DuelContext.IsFreeConsume(from))
                        {
                            this.m_Potion.Consume();
                            from.AddToBackpack(new Bottle());
                        }
                    }
                    else // Target can't be poisoned
                    {
                        from.SendMessage("Only weapons can be poisoned");
                    }

                }

                private class InternalTimer : Timer
                {
                    private readonly Mobile m_From;
                    private readonly Item m_Target;
                    private readonly Poison m_Poison;
                    private readonly double m_MinSkill;
                    private readonly double m_MaxSkill;
                    public InternalTimer(Mobile from, Item target, BasePoisonPotion potion)
                        : base(TimeSpan.FromSeconds(2.0))
                    {
                        this.m_From = from;
                        this.m_Target = target;
                        this.m_Poison = potion.Poison;
                        this.m_MinSkill = potion.MinPoisoningSkill;
                        this.m_MaxSkill = potion.MaxPoisoningSkill;
                        this.Priority = TimerPriority.TwoFiftyMS;
                    }

                    protected override void OnTick()
                    {
                        if (this.m_From.CheckTargetSkill(SkillName.Poisoning, this.m_Target, this.m_MinSkill, this.m_MaxSkill))
                        {
                            if (this.m_Target is BaseWeapon)
                            {
                                var charges = 25;
                                
                                if (m_From.SpecClasse == SpecClasse.Thief)
                                {
                                    switch (m_From.SpecLevel)
                                    {
                                        case 1:
                                            charges += 10;
                                            break;
                                        case 2:
                                            charges += 20;
                                            break;
                                        case 3:
                                            charges += 30;
                                            break;
                                        case 4:
                                            charges += 40;
                                            break;
                                        case 5:
                                            charges += 50;
                                            break;
                                        case 6:
                                            charges += 60;
                                            break;
                                    }
                                }

                                ((BaseWeapon)this.m_Target).Poison = this.m_Poison;

                                ((BaseWeapon)this.m_Target).PoisonCharges = charges;
                            }

                            this.m_From.SendLocalizedMessage(1010517); // You apply the poison

                        }
                        else // Failed
                        {
                            // 5% of chance of getting poisoned if failed
                            if (this.m_From.Skills[SkillName.Poisoning].Base < 80.0 && Utility.Random(20) == 0) // this is very evil for macroing....
                            {
                                this.m_From.SendLocalizedMessage(502148); // You make a grave mistake while applying the poison.
                                this.m_From.ApplyPoison(this.m_From, this.m_Poison);
                            }
                            else
                            {
                                if (this.m_Target is BaseWeapon)
                                {
                                    BaseWeapon weapon = (BaseWeapon)this.m_Target;

                                    this.m_From.SendLocalizedMessage(1010518); // You fail to apply a sufficient dose of poison
                                }
                                else
                                {
                                    this.m_From.SendLocalizedMessage(1010518); // You fail to apply a sufficient dose of poison
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}