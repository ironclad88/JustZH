using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Items.ZuluIems;
using Server.Items.ZuluIems.GMItems.Tools;

namespace Server.Engines.Harvest
{
    public abstract class HarvestSystem
    {
        private readonly List<HarvestDefinition> m_Definitions;
        public HarvestSystem()
        {
            this.m_Definitions = new List<HarvestDefinition>();
        }

        public List<HarvestDefinition> Definitions
        {
            get
            {
                return this.m_Definitions;
            }
        }
        public virtual bool CheckTool(Mobile from, Item tool)
        {
            bool wornOut = (tool == null || tool.Deleted || (tool is IUsesRemaining && ((IUsesRemaining)tool).UsesRemaining <= 0));

            if (wornOut)
                from.SendLocalizedMessage(1044038); // You have worn out your tool!

            return !wornOut;
        }

        public virtual bool CheckHarvest(Mobile from, Item tool)
        {
            return this.CheckTool(from, tool);
        }

        public virtual bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            return this.CheckTool(from, tool);
        }

        public virtual bool CheckRange(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            bool inRange = (from.Map == map && from.InRange(loc, def.MaxRange));

            if (!inRange)
                def.SendMessageTo(from, timed ? def.TimedOutOfRangeMessage : def.OutOfRangeMessage);

            return inRange;
        }

        public virtual bool CheckResources(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);
            bool available = (bank != null && bank.Current >= def.ConsumedPerHarvest);

            if (!available)
                def.SendMessageTo(from, timed ? def.DoubleHarvestMessage : def.NoResourcesMessage);

            return available;
        }

        public virtual void OnBadHarvestTarget(Mobile from, Item tool, object toHarvest)
        {
        }

        public virtual object GetLock(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            /* Here we prevent multiple harvesting.
            * 
            * Some options:
            *  - 'return tool;' : This will allow the player to harvest more than once concurrently, but only if they use multiple tools. This seems to be as OSI.
            *  - 'return GetType();' : This will disallow multiple harvesting of the same type. That is, we couldn't mine more than once concurrently, but we could be both mining and lumberjacking.
            *  - 'return typeof( HarvestSystem );' : This will completely restrict concurrent harvesting.
            */
            return tool;
        }

        public virtual void OnConcurrentHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
        }

        public virtual void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
        }

        public virtual bool BeginHarvesting(Mobile from, Item tool)
        {
            if (!this.CheckHarvest(from, tool))
                return false;

            from.Target = new HarvestTarget(tool, this);
            return true;
        }

        public virtual void FinishHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked)
        {
            from.EndAction(locked);

            if (!this.CheckHarvest(from, tool))
                return;

            int tileID;
            Map map;
            Point3D loc;

            if (!this.GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                this.OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }
            else if (!def.Validate(tileID))
            {
                this.OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }
			
            if (!this.CheckRange(from, tool, def, map, loc, true))
                return;
            else if (!this.CheckResources(from, tool, def, map, loc, true))
                return;
            else if (!this.CheckHarvest(from, tool, def, toHarvest))
                return;

            if (this.SpecialHarvest(from, tool, def, map, loc))
                return;

            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

            if (bank == null)
                return;

            HarvestVein vein = bank.Vein;

            if (vein != null)
                vein = this.MutateVein(from, tool, def, bank, toHarvest, vein);

            if (vein == null)
                return;
            
            HarvestResource resource = this.MutateResource(from, tool, def, map, loc, vein);
            //CraftResourceInfo info;
            //if (primary.Types[0] is typeof(BaseLog))
            //{
            //    info = CraftResources.GetInfo(CraftResource.Iron);
            //}

            double skillBase = from.Skills[def.Skill].Base;
            double skillValue = from.Skills[def.Skill].Value;

            Type type = null;

            if (skillValue >= resource.MinSkill && from.CheckSkill(def.Skill, 0.95)) //resource.MinSkill, resource.MaxSkill))
            {
                type = this.GetResourceType(from, tool, def, map, loc, resource);
                if (type != null)
                    type = this.MutateType(type, from, tool, def, map, loc, resource);

                if (type != null)
                {
                    Item item = this.Construct(type, from);
                    if (item == null)
                    {
                        type = null;
                    }
                    else
                    {
                        //The whole harvest system is kludgy and I'm sure this is just adding to it.
                        double spec_bonus = 1.0;
                        if (item.Stackable)
                        {
                            // JustZH: amount: 1 plus 1 for every 20 harvesting skill
                            int amount = 1 + (int)(skillValue / 20);
                            if (item is IronOre || item is Log)
                            {
                                // Increase amount by 50% for regular logs or iron ore.
                                amount += (amount / 2);
                            }
                            // JustZH Gain more resources if specced
                            if (item is BaseLog || item is BaseOre || item is Sand)
                            {
                                spec_bonus = from.SpecBonus(SpecClasse.Crafter);
                            }
                            else if (item is Fish)
                            {
                                spec_bonus = from.SpecBonus(SpecClasse.Ranger);
                            }
                            amount = (int)(amount* spec_bonus);

                            item.Amount = GMToolChecker(amount, tool);
                            //int feluccaAmount = def.ConsumedPerFeluccaHarvest;

                            //int racialAmount = (int)Math.Ceiling(amount * 1.1);
                            //int feluccaRacialAmount = (int)Math.Ceiling(feluccaAmount * 1.1);

                            //bool eligableForRacialBonus = (def.RaceBonus && from.Race == Race.Human);
                            //bool inFelucca = (map == Map.Felucca);

                            //if (eligableForRacialBonus && inFelucca && bank.Current >= feluccaRacialAmount && 0.1 > Utility.RandomDouble())
                            //    item.Amount = feluccaRacialAmount;
                            //else if (inFelucca && bank.Current >= feluccaAmount)
                            //    item.Amount = feluccaAmount;
                            //else if (eligableForRacialBonus && bank.Current >= racialAmount && 0.1 > Utility.RandomDouble())
                            //    item.Amount = racialAmount;
                            //else
                            //    item.Amount = amount;
                        }

                        bank.Consume(1, from);

                        if (this.Give(from, item, def.PlaceAtFeetIfFull))
                        {
                            this.SendSuccessTo(from, item, resource);
                        }
                        else
                        {
                            this.SendPackFullTo(from, item, def, resource);
                            item.Delete();
                        }

                        // JustZH : add bonus resource bonus here, spec crafter, omero's/xarafax's?
                        BonusHarvestResource bonus = def.GetBonusResource(1.0 * spec_bonus); 

                        if (bonus != null && bonus.Type != null && skillBase >= bonus.ReqSkill)
                        {
                            if ((bonus.Type != typeof(ETSOre) &&
                                bonus.Type != typeof(RNDOre) &&
                                bonus.Type != typeof(DSROre)) ||
                                from.SpecClasse == SpecClasse.Crafter)
                            {
                                // JustZH: only get dsr/ets/rnd if spec crafter.
                                Item bonusItem = this.Construct(bonus.Type, from);

                                if (this.Give(from, bonusItem, true))   //Bonuses always allow placing at feet, even if pack is full irregrdless of def
                                {
                                    bonus.SendSuccessTo(from);
                                }
                                else
                                {
                                    item.Delete();
                                }
                            }
                        }

                        if (tool is IUsesRemaining)
                        {
                            IUsesRemaining toolWithUses = (IUsesRemaining)tool;

                            //toolWithUses.ShowUsesRemaining = true;
                            // JustZH Better tool decay with spec
                            int rand = (int)(10 * spec_bonus);
                            if (toolWithUses.UsesRemaining > 0 && Utility.Random(rand) == 0)
                                --toolWithUses.UsesRemaining;

                            if (toolWithUses.UsesRemaining < 1)
                            {
                                tool.Delete();
                                def.SendMessageTo(from, def.ToolBrokeMessage);
                            }
                        }
                    }
                }
            }

            if (type == null)
                def.SendMessageTo(from, def.FailMessage);

            this.OnHarvestFinished(from, tool, def, vein, bank, resource, toHarvest);
        }

        public virtual int GMToolChecker(int amount, Item tool)
        {
            if (tool is OmerosPickAxe)
            {
                amount = amount * 2;
               // Console.WriteLine("Omeros");
            }
            else if (tool is XarafaxAxe)
            {
                amount = amount * 2;
               // Console.WriteLine("XarafaxAxe");
            }
            else if (tool is PoseidonFishingpole)
            {
                amount = amount * 2;
               // Console.WriteLine("PoseidonFishingpole");
            }
            return amount;
        }

        public virtual void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
        }

        public virtual bool SpecialHarvest(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
        {
            return false;
        }

        public virtual Item Construct(Type type, Mobile from)
        {
            try
            {
                return Activator.CreateInstance(type) as Item;
            }
            catch
            {
                return null;
            }
        }

        public virtual HarvestVein MutateVein(Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
        {
            return vein;
        }

        public virtual void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
        {
            resource.SendSuccessTo(from);
        }

        public virtual void SendPackFullTo(Mobile from, Item item, HarvestDefinition def, HarvestResource resource)
        {
            def.SendMessageTo(from, def.PackFullMessage);
        }

        public virtual bool Give(Mobile m, Item item, bool placeAtFeet)
        {
            if (m.PlaceInBackpack(item))
                return true;

            if (!placeAtFeet)
                return false;

            Map map = m.Map;

            if (map == null)
                return false;

            List<Item> atFeet = new List<Item>();

            foreach (Item obj in m.GetItemsInRange(0))
                atFeet.Add(obj);

            for (int i = 0; i < atFeet.Count; ++i)
            {
                Item check = atFeet[i];

                if (check.StackWith(m, item, false))
                    return true;
            }

            item.MoveToWorld(m.Location, map);
            return true;
        }

        public virtual Type MutateType(Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            return from.Region.GetResource(type);
        }

        public virtual Type GetResourceType(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            if (resource.Types.Length > 0)
                return resource.Types[Utility.Random(resource.Types.Length)];

            return null;
        }

        public virtual HarvestResource MutateResource(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestVein vein)//, HarvestResource primary, HarvestResource fallback)
        {
            double skillValue = from.Skills[def.Skill].Value;
            HarvestResource new_resource = vein.PrimaryResource;
            HarvestResource fallback = vein.FallbackResource;
            // Find all resources that the miner is skilled enough for
            List<int> valid_indexes = new List<int>();
            for (int i = 0; i < def.Veins.Length; i++)
            {
                if (skillValue >= def.Veins[i].PrimaryResource.MinSkill)
                {
                    valid_indexes.Add(i);
                }
            }
            int rand_list_idx = Utility.RandomMinMax(0, valid_indexes.Count - 1);
            int rand_index = valid_indexes[rand_list_idx];
            if (null != def.Veins[rand_index].PrimaryResource)
            {
                new_resource = def.Veins[rand_index].PrimaryResource;
                fallback = def.Veins[rand_index].FallbackResource;
                //Console.WriteLine("");
                //Console.WriteLine("Switched resource: " + vein.PrimaryResource.Types[0].Name.ToString()
                //+ "to: " + new_resource.Types[0].Name.ToString());
                //Console.WriteLine("Rolled " + rand_list_idx + " between 0 and " + (valid_indexes.Count - 1) + " and got veins idx: " + rand_index);
                //Console.WriteLine("Total veins in HarvestSystem: " + def.Veins.Length);
            }

            if (def.Veins[rand_index].ChanceToFallback > (Utility.RandomDouble()))
                return fallback;


            // JustZH: we only look at min skill here, not req skill...
            if (fallback != null && (skillValue < new_resource.MinSkill))
            {
                return fallback;
            }

            return new_resource;
        }

        public virtual bool OnHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked, bool last)
        {
            bool dummy;
            return OnHarvesting( from,  tool,  def,  toHarvest,  locked,  last, out dummy);
        }

        public virtual bool OnHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked, bool last, out bool noResources)
        {
            noResources = false;
            if (!this.CheckHarvest(from, tool))
            {
                from.EndAction(locked);
                return false;
            }

            int tileID;
            Map map;
            Point3D loc;

            if (!this.GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                from.EndAction(locked);
                this.OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }
            else if (!def.Validate(tileID))
            {
                from.EndAction(locked);
                this.OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }
            else if (!this.CheckRange(from, tool, def, map, loc, true))
            {
                from.EndAction(locked);
                return false;
            }
            else if (!this.CheckResources(from, tool, def, map, loc, true))
            {
                noResources = true;
                from.EndAction(locked);
                return false;
            }
            else if (!this.CheckHarvest(from, tool, def, toHarvest))
            {
                from.EndAction(locked);
                return false;
            }

            this.DoHarvestingEffect(from, tool, def, map, loc);

            new HarvestSoundTimer(from, tool, this, def, toHarvest, locked, last).Start();

            return !last;
        }

        public virtual void DoHarvestingSound(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            if (def.EffectSounds.Length > 0)
                from.PlaySound(Utility.RandomList(def.EffectSounds));
        }

        public virtual void DoHarvestingEffect(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
        {
            from.Direction = from.GetDirectionTo(loc);

            if (!from.Mounted)
                from.Animate(Utility.RandomList(def.EffectActions), 5, 1, true, false, 0);
            else if (def.EffectActions[0] == 11 || def.EffectActions[0] == 13) //mining or lumberjacking effect
            {
              int[] effect = new int[] { 26 };
              from.Animate(Utility.RandomList(effect), 5, 1, true, false, 0);
            }
        }

        public virtual HarvestDefinition GetDefinition(int tileID)
        {
            HarvestDefinition def = null;

            for (int i = 0; def == null && i < this.m_Definitions.Count; ++i)
            {
                HarvestDefinition check = this.m_Definitions[i];

                if (check.Validate(tileID))
                    def = check;
            }

            return def;
        }

        public virtual void StartHarvesting(Mobile from, Item tool, object toHarvest)
        {
            if (!this.CheckHarvest(from, tool))
                return;

            int tileID;
            Map map;
            Point3D loc;

            if (!this.GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                    this.OnBadHarvestTarget(from, tool, toHarvest);
                    return;
            }

            HarvestDefinition def = this.GetDefinition(tileID);

            if (def == null)
            {
                this.OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            if (!this.CheckRange(from, tool, def, map, loc, false))
                return;
            else if (!this.CheckResources(from, tool, def, map, loc, false))
                return;
            else if (!this.CheckHarvest(from, tool, def, toHarvest))
                return;

            object toLock = this.GetLock(from, tool, def, toHarvest);

            if (!from.BeginAction(toLock))
            {
                this.OnConcurrentHarvest(from, tool, def, toHarvest);
                return;
            }

            new HarvestTimer(from, tool, this, def, toHarvest, toLock).Start();
            this.OnHarvestStarted(from, tool, def, toHarvest);
        }

        public virtual bool GetHarvestDetails(Mobile from, Item tool, object toHarvest, out int tileID, out Map map, out Point3D loc)
        {
            if (toHarvest is Static && !((Static)toHarvest).Movable)
            {
                Static obj = (Static)toHarvest;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = obj.Map;
                loc = obj.GetWorldLocation();
            }
            else if (toHarvest is StaticTarget)
            {
                StaticTarget obj = (StaticTarget)toHarvest;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = from.Map;
                loc = obj.Location;
            }
            else if (toHarvest is LandTarget)
            {
                LandTarget obj = (LandTarget)toHarvest;

                tileID = obj.TileID;
                map = from.Map;
                loc = obj.Location;
            }
            else if (toHarvest is YuccaTree)
            {
                YuccaTree test = (YuccaTree)toHarvest; // h�h�
                tileID = 0xd38;
                map = from.Map;
                loc = test.Location;
            }
            else
            {
                tileID = 0;
                map = null;
                loc = Point3D.Zero;
                return false;
            }

            return (map != null && map != Map.Internal);
        }
    }
}

namespace Server
{
    public interface IChopable
    {
        void OnChop(Mobile from);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FurnitureAttribute : Attribute
    {
        public FurnitureAttribute()
        {
        }

        public static bool Check(Item item)
        {
            return (item != null && item.GetType().IsDefined(typeof(FurnitureAttribute), false));
        }
    }
}