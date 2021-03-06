using System;
using Server.Mobiles;

using Server;

namespace Server.Items
{
    public class EventRobe : HoodedShroudOfShadows
	{       

		[Constructable]
        public EventRobe(int hue)
        {

            Hue = hue;
			Weight = 0;
			Name = "EventRobe";
			Layer = Layer.OuterTorso;
            LootType = LootType.Blessed;
            this.Attributes.LowerRegCost = 100;
		}
        public override bool OnEquip(Mobile from)
        {
            if (from.Player)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm.IsInEvent != true)
                {
                    if (pm.AccessLevel >= AccessLevel.GameMaster)
                    {
                        return true;
                    }
                    else
                        from.SendMessage("Oh cool one of these event Items!");
                    this.Delete();

                    return false;
                }
                else
                {
                    return true;
                }


            }
            else
            {
                return false;
            }
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            if (from.Player)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm.IsInEvent != true)
                {
                    from.SendMessage("Oh cool one of these Event Items!");
                    this.Delete();
                    return false;
                }
                else
                {
                    return true;
                }


            }
            else
            {
                return false;
            }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (from.Player)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm.IsInEvent != true)
                {
                    if (pm.AccessLevel >= AccessLevel.GameMaster)
                    {
                        return true;
                    }
                    else
                        from.SendMessage("Oh cool one of these Event Items!");
                    this.Delete();
                    return false;

                }
                else
                {
                    return true;
                }


            }
            else
            {
                return false;
            }
        }

        // And to really make sure nothing will ever happen....

        public override bool OnDragLift(Mobile from)
        {

            if (from.Player)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm.IsInEvent != true)
                {
                    if (pm.AccessLevel >= AccessLevel.GameMaster)
                    {
                        return true;
                    }
                    else
                        from.SendMessage("Oh cool one of these Event Items!");
                    this.Delete();
                    return false;

                }
                else
                {
                    return true;
                }


            }
            else
            {
                return false;
            }

        }
		public EventRobe( Serial serial ) : base( serial )
		{
		}
		
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			if ( Weight == 1.0 )
				 Weight = 10.0;
		}
	}
}
