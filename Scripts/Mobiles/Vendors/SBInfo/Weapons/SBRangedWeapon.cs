using System;
using System.Collections.Generic;
using Server.Items;
using Server.Custom;
using Server.Items.ZuluIems.Weapons.Ranged;

namespace Server.Mobiles
{
    public class SBRangedWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBRangedWeapon()
        {
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return base.SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return this.m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                this.Add(new GenericBuyInfo(typeof(Crossbow), 55, 20, 0xF50, 0));
                this.Add(new GenericBuyInfo(typeof(HeavyCrossbow), 55, 20, 0x13FD, 0));
               /* if (Core.AOS)
                {
                    this.Add(new GenericBuyInfo(typeof(RepeatingCrossbow), 46, 20, 0x26C3, 0));
                    this.Add(new GenericBuyInfo(typeof(CompositeBow), 45, 20, 0x26C2, 0));
                }*/
                this.Add(new GenericBuyInfo(typeof(Bolt), 2, GlobalSettings.ArrowAmount, 0x1BFB, 0));
                this.Add(new GenericBuyInfo(typeof(Bow), 40, 20, 0x13B2, 0));
                this.Add(new GenericBuyInfo(typeof(Arrow), 2, GlobalSettings.ArrowAmount, 0xF3F, 0));

                this.Add(new GenericBuyInfo(typeof(Icebow), 500, 20, 0x13B2, 0x0492));
                this.Add(new GenericBuyInfo(typeof(IceArrow), 50, GlobalSettings.ArrowAmount, 0xF3F, 0x0492));

                this.Add(new GenericBuyInfo(typeof(Firebow), 500, 20, 0x13B2, 0x0494));
                this.Add(new GenericBuyInfo(typeof(FireArrow), 50, GlobalSettings.ArrowAmount, 0xF3F, 0x0494));

                this.Add(new GenericBuyInfo(typeof(Feather), 2, GlobalSettings.ArrowAmount, 0x1BD1, 0));
                this.Add(new GenericBuyInfo(typeof(Shaft), 3, GlobalSettings.ArrowAmount, 0x1BD4, 0));

                this.Add(new GenericBuyInfo(typeof(ThunderHeavyCrossbow), 500, 20, 0x13FD, 0x502));
                this.Add(new GenericBuyInfo(typeof(ThunderBolt), 50, GlobalSettings.ArrowAmount, 0x1BFB, 0x502));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Bolt), 1);
                this.Add(typeof(Arrow), 1);
                this.Add(typeof(Shaft), 1);
                this.Add(typeof(Feather), 1);			

                this.Add(typeof(HeavyCrossbow), 27);
                this.Add(typeof(Bow), 17);
                this.Add(typeof(Crossbow), 25); 

                if (Core.AOS)
                {
                    this.Add(typeof(CompositeBow), 23);
                    this.Add(typeof(RepeatingCrossbow), 22);
                }
            }
        }
    }
}