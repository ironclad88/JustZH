using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Harvest
{
    public class Mining : HarvestSystem
    {
        private static Mining m_System;

        public static Mining System
        {
            get
            {
                if (m_System == null)
                    m_System = new Mining();

                return m_System;
            }
        }

        private readonly HarvestDefinition m_OreAndStone;

        private readonly HarvestDefinition m_Sand;

        public HarvestDefinition OreAndStone
        {
            get
            {
                return this.m_OreAndStone;
            }
        }

        public HarvestDefinition Sand
        {
            get
            {
                return this.m_Sand;
            }
        }

        private Mining()
        {
            HarvestResource[] res;
            HarvestVein[] veins;

            #region Mining for ore and stone
            HarvestDefinition oreAndStone = this.m_OreAndStone = new HarvestDefinition();

            // Resource banks are every 1x1 tiles
            oreAndStone.BankWidth = 1;
            oreAndStone.BankHeight = 1;

            // Every bank holds from 5 to 15 loops
            oreAndStone.MinTotal = 8;
            oreAndStone.MaxTotal = 12;

            // A resource bank will respawn its content every 10 to 15 minutes
            oreAndStone.MinRespawn = TimeSpan.FromMinutes(3.0);
            oreAndStone.MaxRespawn = TimeSpan.FromMinutes(5.0);

            // Skill checking is done on the Mining skill
            oreAndStone.Skill = SkillName.Mining;

            // Set the list of harvestable tiles
            oreAndStone.Tiles = m_MountainAndCaveTiles;

            // Players must be within 2 tiles to harvest
            oreAndStone.MaxRange = 2;

            // One ore per harvest action
            oreAndStone.ConsumedPerHarvest = 1;
            oreAndStone.ConsumedPerFeluccaHarvest = 1;

            // The digging effect
            oreAndStone.EffectActions = new int[] { 11 };
            oreAndStone.EffectSounds = new int[] { 0x125, 0x126 };
            oreAndStone.EffectCounts = new int[] { 5 };
            oreAndStone.EffectDelay = TimeSpan.FromSeconds(1.1);
            oreAndStone.EffectSoundDelay = TimeSpan.FromSeconds(0.9);

            oreAndStone.NoResourcesMessage = 503040; // There is no metal here to mine.
            oreAndStone.DoubleHarvestMessage = 503042; // Someone has gotten to the metal before you.
            oreAndStone.TimedOutOfRangeMessage = 503041; // You have moved too far away to continue mining.
            oreAndStone.OutOfRangeMessage = 500446; // That is too far away.
            oreAndStone.FailMessage = 503043; // You loosen some rocks but fail to find any useable ore.
            oreAndStone.PackFullMessage = 1010481; // Your backpack is full, so the ore you mined is lost.
            oreAndStone.ToolBrokeMessage = 1044038; // You have worn out your tool!

            int difficulty = 0;
            int max_skill_offset = 50;
            res = new HarvestResource[]
            {
                //new HarvestResource(00.0, 00.0, difficulty+max_skill_offset, 1007072, typeof(IronOre)),
                
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Iron).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Iron ore and put it in your backpack.", typeof(IronOre)),
                //new HarvestResource(70.0, 30.0, 110.0, 1007074, typeof(ShadowIronOre),	typeof(ShadowIronGranite), typeof(ShadowIronElemental)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.DullCopper).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Dull Copper ore and put it in your backpack.", typeof(DullCopperOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Copper).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Copper ore and put it in your backpack.", typeof(CopperOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Bronze).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Bronze ore and put it in your backpack.", typeof(BronzeOre)),
                //new HarvestResource(85.0, 45.0, 125.0, 1007077, typeof(GoldOre), typeof(GoldGranite), typeof(GoldenElemental)),
                //new HarvestResource(90.0, 50.0, 130.0, 1007078, typeof(AgapiteOre), typeof(AgapiteGranite), typeof(AgapiteElemental)),
                //new HarvestResource(95.0, 55.0, 135.0, 1007079, typeof(VeriteOre), typeof(VeriteGranite), typeof(VeriteElemental)),
                //new HarvestResource(99.0, 59.0, 139.0, 1007080, typeof(ValoriteOre), typeof(ValoriteGranite), typeof(ValoriteElemental)),
                //JustZH : remove zulu ore here and put it with the gems..
                //new HarvestResource(99.0, 59.0, difficulty+max_skill_offset, "You found some elusive NEW ZULU ORE!", typeof(ZuluOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Onyx).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Onyx ore and put it in your backpack.", typeof(OnyxOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Pyrite).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Pyrite ore and put it in your backpack.", typeof(PyriteOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Malachite).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Malachite ore and put it in your backpack.", typeof(MalachiteOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Azurite).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Azurite ore and put it in your backpack.", typeof(AzuriteOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Platinum).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Platinum ore and put it in your backpack.", typeof(PlatinumOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Lavarock).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Lavarock ore and put it in your backpack.", typeof(LavarockOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Mystic).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Mystic ore and put it in your backpack.", typeof(MysticOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Spike).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Spike ore and put it in your backpack.", typeof(SpikeOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Fruity).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Fruity ore and put it in your backpack.", typeof(FruityOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.IceRock).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Ice Rock ore and put it in your backpack.", typeof(IceRockOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.SilverRock).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Silver Rock ore and put it in your backpack.", typeof(SilverRockOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Spectral).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Spectral ore and put it in your backpack.", typeof(SpectralOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Undead).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Undead ore and put it in your backpack.", typeof(UndeadOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.DarkPagan).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Dark Pagan ore and put it in your backpack.", typeof(DarkPaganOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.OldBritain).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Old Britain ore and put it in your backpack.", typeof(OldBritainOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Virginity).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Virginity ore and put it in your backpack.", typeof(VirginityOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.BlackDwarf).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Black Dwarf ore and put it in your backpack.", typeof(BlackDwarfOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.RedElven).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Red Elven ore and put it in your backpack.", typeof(RedElvenOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Dripstone).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Dripstone ore and put it in your backpack.", typeof(DripstoneOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Executor).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Executor ore and put it in your backpack.", typeof(ExecutorOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Peachblue).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Peachblue ore and put it in your backpack.", typeof(PeachblueOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Destruction).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Destruction ore and put it in your backpack.", typeof(DestructionOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.Anra).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Anra ore and put it in your backpack.", typeof(AnraOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.GoddessMetal).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Goddess ore and put it in your backpack.", typeof(GoddessOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.CrystalMetal).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Crystal ore and put it in your backpack.", typeof(CrystalOre)),
                new HarvestResource((difficulty = CraftResources.GetInfo(CraftResource.DoomMetal).AttributeInfo.Difficulty), Math.Max(0,(difficulty-15)), difficulty+max_skill_offset, "You dig up some Doom ore and put it in your backpack.", typeof(DoomOre)),
            };

            veins = new HarvestVein[res.Length];
            //{
            //    new HarvestVein(10.0, 0.0, res[0], null), // Iron
            //    new HarvestVein(10.0, 0.0, res[1], res[0]), // Dull Copper
            //    new HarvestVein(10.0, 0.0, res[2], res[0]), // Shadow Iron
            //    new HarvestVein(10.0, 0.0, res[3], res[0]), // Copper
            //    new HarvestVein(10.0, 0.0, res[4], res[0]), // Bronze
            //    new HarvestVein(10.0, 0.0, res[5], res[0]), // Gold
            //    new HarvestVein(10.0, 0.0, res[6], res[0]), // Agapite
            //    new HarvestVein(10.0, 0.0, res[7], res[0]), // Verite
            //    new HarvestVein(10.0, 0.0, res[8], res[0]),// Valorite
            //    new HarvestVein(10.0, 0.0, res[9], res[0])// Zulu
            //};
            double mean_chance = (100.0 / res.Length);
            veins[0] = new HarvestVein(mean_chance, 0.0, res[0],  null);
            for (int i = 1; i < res.Length; i++)
            {
                veins[i] = new HarvestVein(mean_chance, 0.10, res[i], res[0]);
            }

            oreAndStone.Resources = res;
            oreAndStone.Veins = veins;

            if (Core.ML)
            {
                oreAndStone.BonusResources = new BonusHarvestResource[]
                {
                    new BonusHarvestResource(50, 1.0, "You found some elusive NEW ZULU ORE!", typeof(ZuluOre)),
                    new BonusHarvestResource(CraftResources.GetInfo(CraftResource.ETS).AttributeInfo.Difficulty-30, 0.03, "You found a rare Ebon Twiligh Saphire!", typeof(ETSOre)),
                    new BonusHarvestResource(CraftResources.GetInfo(CraftResource.DSR).AttributeInfo.Difficulty-30, 0.03, "You found a rare Dark Sable Ruby!", typeof(DSROre)),
                    new BonusHarvestResource(CraftResources.GetInfo(CraftResource.RND).AttributeInfo.Difficulty-30, 0.01, "You found a rare Radiant Nimbus Diamond!", typeof(RNDOre)),
                    new BonusHarvestResource(50, 5.0, "You found a diamond!", typeof(Diamond)),
                    new BonusHarvestResource(0, 5.0, "You found a piece of amber!", typeof(Amber)),
                    //new BonusHarvestResource(0, 88.7, null, null), //Nothing, THIS MUST BE LAST! JustZH: increased this a lot
                    //new BonusHarvestResource(100, .1, 1072562, typeof(BlueDiamond)),
                    //new BonusHarvestResource(100, .1, 1072567, typeof(DarkSapphire)),
                    //new BonusHarvestResource(100, .1, 1072570, typeof(EcruCitrine)),
                    //new BonusHarvestResource(100, .1, 1072564, typeof(FireRuby)),
                    //new BonusHarvestResource(100, .1, 1072566, typeof(PerfectEmerald)),
                    //new BonusHarvestResource(100, .1, 1072568, typeof(Turquoise))
                };
            }

            oreAndStone.RaceBonus = Core.ML;
            oreAndStone.RandomizeVeins = Core.ML;

            this.Definitions.Add(oreAndStone);
            #endregion

            #region Mining for sand
            HarvestDefinition sand = this.m_Sand = new HarvestDefinition();

            // Resource banks are every 1x1 tile1
            sand.BankWidth = 1;
            sand.BankHeight = 1;

            // Every bank holds from 6 to 12 sand
            sand.MinTotal = 6;
            sand.MaxTotal = 12;

            // A resource bank will respawn its content every 10 to 20 minutes
            sand.MinRespawn = TimeSpan.FromMinutes(10.0);
            sand.MaxRespawn = TimeSpan.FromMinutes(20.0);

            // Skill checking is done on the Mining skill
            sand.Skill = SkillName.Mining;

            // Set the list of harvestable tiles
            sand.Tiles = m_SandTiles;

            // Players must be within 2 tiles to harvest
            sand.MaxRange = 2;

            // One sand per harvest action
            sand.ConsumedPerHarvest = 1;
            sand.ConsumedPerFeluccaHarvest = 1;

            // The digging effect
            sand.EffectActions = new int[] { 11 };
            sand.EffectSounds = new int[] { 0x125, 0x126 };
            sand.EffectCounts = new int[] { 5 };
            sand.EffectDelay = TimeSpan.FromSeconds(1.1);
            sand.EffectSoundDelay = TimeSpan.FromSeconds(0.9);

            sand.NoResourcesMessage = 1044629; // There is no sand here to mine.
            sand.DoubleHarvestMessage = 1044629; // There is no sand here to mine.
            sand.TimedOutOfRangeMessage = 503041; // You have moved too far away to continue mining.
            sand.OutOfRangeMessage = 500446; // That is too far away.
            sand.FailMessage = 1044630; // You dig for a while but fail to find any of sufficient quality for glassblowing.
            sand.PackFullMessage = 1044632; // Your backpack can't hold the sand, and it is lost!
            sand.ToolBrokeMessage = 1044038; // You have worn out your tool!

            res = new HarvestResource[]
            {
                new HarvestResource(100.0, 70.0, 400.0, 1044631, typeof(Sand))
            };

            veins = new HarvestVein[]
            {
                new HarvestVein(100.0, 0.0, res[0], null)
            };

            sand.Resources = res;
            sand.Veins = veins;

            this.Definitions.Add(sand);
            #endregion
        }

        public override Type GetResourceType(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            if (def == this.m_OreAndStone)
            {
                PlayerMobile pm = from as PlayerMobile;
                // JustZH Mining bonus from specced crafter
                double scale = from.SpecBonus(SpecClasse.Crafter); 
                if (pm != null && pm.GemMining && pm.ToggleMiningGem && from.Skills[SkillName.Mining].Base >= 100.0 && (0.1*scale) > Utility.RandomDouble())
                    return Loot.RandomGem().GetType();

                if (pm != null && pm.StoneMining && pm.ToggleMiningStone && from.Skills[SkillName.Mining].Base >= 100.0 && (0.15*scale) > Utility.RandomDouble())
                    return resource.Types[1];

                return resource.Types[0];
            }

            return base.GetResourceType(from, tool, def, map, loc, resource);
        }

        public override bool CheckHarvest(Mobile from, Item tool)
        {
            if (!base.CheckHarvest(from, tool))
                return false;

            //if (from.Mounted)
            //{
            //    from.SendLocalizedMessage(501864); // You can't mine while riding.
            //    return false;
            //}
            //else if (from.IsBodyMod && !from.Body.IsHuman)
            //{
            //    from.SendLocalizedMessage(501865); // You can't mine while polymorphed.
            //    return false;
            //}

            return true;
        }

        public override void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
        {
            if (item is BaseGranite)
                from.SendLocalizedMessage(1044606); // You carefully extract some workable stone from the ore vein!
            else if (item is IGem)
                from.SendLocalizedMessage(1112233); // You carefully extract a glistening gem from the vein!
            else
                base.SendSuccessTo(from, item, resource);
        }

        public override bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            if (!base.CheckHarvest(from, tool, def, toHarvest))
                return false;

            if (def == this.m_Sand && !(from is PlayerMobile && from.Skills[SkillName.Mining].Base >= 100.0 /*&& ((PlayerMobile)from).SandMining*/))
            {
                this.OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }
            //else if (from.Mounted)
            //{
            //    from.SendLocalizedMessage(501864); // You can't mine while riding.
            //    return false;
            //}
            //else if (from.IsBodyMod && !from.Body.IsHuman)
            //{
            //    from.SendLocalizedMessage(501865); // You can't mine while polymorphed.
            //    return false;
            //}

            return true;
        }

        public override HarvestVein MutateVein(Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
        {
            //if (tool is GargoylesPickaxe && def == this.m_OreAndStone)
            //{
            //    int veinIndex = Array.IndexOf(def.Veins, vein);

            //    if (veinIndex >= 0 && veinIndex < (def.Veins.Length - 1))
            //        return def.Veins[veinIndex + 1];
            //}
            return base.MutateVein(from, tool, def, bank, toHarvest, vein);
        }

        private static readonly int[] m_Offsets = new int[]
        {
            -1, -1,
            -1, 0,
            -1, 1,
            0, -1,
            0, 1,
            1, -1,
            1, 0,
            1, 1
        };

        public override void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
            if (tool is GargoylesPickaxe && def == this.m_OreAndStone && 0.1 > Utility.RandomDouble())
            {
                HarvestResource res = vein.PrimaryResource;

                if (res == resource && res.Types.Length >= 3)
                {
                    try
                    {
                        Map map = from.Map;

                        if (map == null)
                            return;

                        BaseCreature spawned = Activator.CreateInstance(res.Types[2], new object[] { 25 }) as BaseCreature;

                        if (spawned != null)
                        {
                            int offset = Utility.Random(8) * 2;

                            for (int i = 0; i < m_Offsets.Length; i += 2)
                            {
                                int x = from.X + m_Offsets[(offset + i) % m_Offsets.Length];
                                int y = from.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                                if (map.CanSpawnMobile(x, y, from.Z))
                                {
                                    spawned.OnBeforeSpawn(new Point3D(x, y, from.Z), map);
                                    spawned.MoveToWorld(new Point3D(x, y, from.Z), map);
                                    spawned.Combatant = from;
                                    return;
                                }
                                else
                                {
                                    int z = map.GetAverageZ(x, y);

                                    if (Math.Abs(z - from.Z) < 10 && map.CanSpawnMobile(x, y, z))
                                    {
                                        spawned.OnBeforeSpawn(new Point3D(x, y, z), map);
                                        spawned.MoveToWorld(new Point3D(x, y, z), map);
                                        spawned.Combatant = from;
                                        return;
                                    }
                                }
                            }

                            spawned.OnBeforeSpawn(from.Location, from.Map);
                            spawned.MoveToWorld(from.Location, from.Map);
                            spawned.Combatant = from;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public override bool BeginHarvesting(Mobile from, Item tool)
        {
            if (!base.BeginHarvesting(from, tool))
                return false;

            from.SendLocalizedMessage(503033); // Where do you wish to dig?
            return true;
        }

        public override void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            base.OnHarvestStarted(from, tool, def, toHarvest);

            if (Core.ML)
                from.RevealingAction();
        }

        public override void OnBadHarvestTarget(Mobile from, Item tool, object toHarvest)
        {
            if (toHarvest is LandTarget)
                from.SendLocalizedMessage(501862); // You can't mine there.
            else
                from.SendLocalizedMessage(501863); // You can't mine that.
        }

        #region Tile lists
        private static readonly int[] m_MountainAndCaveTiles = new int[]
        {
            220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
            230, 231, 236, 237, 238, 239, 240, 241, 242, 243,
            244, 245, 246, 247, 252, 253, 254, 255, 256, 257,
            258, 259, 260, 261, 262, 263, 268, 269, 270, 271,
            272, 273, 274, 275, 276, 277, 278, 279, 286, 287,
            288, 289, 290, 291, 292, 293, 294, 296, 296, 297,
            321, 322, 323, 324, 467, 468, 469, 470, 471, 472,
            473, 474, 476, 477, 478, 479, 480, 481, 482, 483,
            484, 485, 486, 487, 492, 493, 494, 495, 543, 544,
            545, 546, 547, 548, 549, 550, 551, 552, 553, 554,
            555, 556, 557, 558, 559, 560, 561, 562, 563, 564,
            565, 566, 567, 568, 569, 570, 571, 572, 573, 574,
            575, 576, 577, 578, 579, 581, 582, 583, 584, 585,
            586, 587, 588, 589, 590, 591, 592, 593, 594, 595,
            596, 597, 598, 599, 600, 601, 610, 611, 612, 613,
            1010, 1741, 1742, 1743, 1744, 1745, 1746, 1747, 1748, 1749,
            1750, 1751, 1752, 1753, 1754, 1755, 1756, 1757, 1771, 1772,
            1773, 1774, 1775, 1776, 1777, 1778, 1779, 1780, 1781, 1782,
            1783, 1784, 1785, 1786, 1787, 1788, 1789, 1790, 1801, 1802,
            1803, 1804, 1805, 1806, 1807, 1808, 1809, 1811, 1812, 1813,
            1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821, 1822, 1823,
            1824, 1831, 1832, 1833, 1834, 1835, 1836, 1837, 1838, 1839,
            1840, 1841, 1842, 1843, 1844, 1845, 1846, 1847, 1848, 1849,
            1850, 1851, 1852, 1853, 1854, 1861, 1862, 1863, 1864, 1865,
            1866, 1867, 1868, 1869, 1870, 1871, 1872, 1873, 1874, 1875,
            1876, 1877, 1878, 1879, 1880, 1881, 1882, 1883, 1884, 1981,
            1982, 1983, 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991,
            1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001,
            2002, 2003, 2004, 2028, 2029, 2030, 2031, 2032, 2033, 2100,
            2101, 2102, 2103, 2104, 2105,
            0x453B, 0x453C, 0x453D, 0x453E, 0x453F, 0x4540, 0x4541,
            0x4542, 0x4543, 0x4544, 0x4545, 0x4546, 0x4547, 0x4548,
            0x4549, 0x454A, 0x454B, 0x454C, 0x454D, 0x454E, 0x454F
        };

        private static readonly int[] m_SandTiles = new int[]
        {
            22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
            32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
            42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61,
            62, 68, 69, 70, 71, 72, 73, 74, 75,
            286, 287, 288, 289, 290, 291, 292, 293, 294, 295,
            296, 297, 298, 299, 300, 301, 402, 424, 425, 426,
            427, 441, 442, 443, 444, 445, 446, 447, 448, 449,
            450, 451, 452, 453, 454, 455, 456, 457, 458, 459,
            460, 461, 462, 463, 464, 465, 642, 643, 644, 645,
            650, 651, 652, 653, 654, 655, 656, 657, 821, 822,
            823, 824, 825, 826, 827, 828, 833, 834, 835, 836,
            845, 846, 847, 848, 849, 850, 851, 852, 857, 858,
            859, 860, 951, 952, 953, 954, 955, 956, 957, 958,
            967, 968, 969, 970,
            1447, 1448, 1449, 1450, 1451, 1452, 1453, 1454, 1455,
            1456, 1457, 1458, 1611, 1612, 1613, 1614, 1615, 1616,
            1617, 1618, 1623, 1624, 1625, 1626, 1635, 1636, 1637,
            1638, 1639, 1640, 1641, 1642, 1647, 1648, 1649, 1650
        };
        #endregion
    }
}