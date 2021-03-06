﻿using Server.Mobiles;

namespace Server.Gumps.Zulugumps
{

    public class ProtsGump : Gump
    {
        public ProtsGump(Mobile owner)
            : base(100, 0)
        {

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            PlayerMobile player = owner as PlayerMobile;

            int armor = (int)(player.ArmorRating + 0.5);
            int phys = owner.PhysicalResistance;
            int pois = owner.PoisonResistance;
            int fire = owner.FireResistance;
            int water = owner.ColdResistance;
            int air = owner.EnergyResistance;
            int earth = owner.EarthResistance;
            int necro = owner.NecroResistance;
            int holy = owner.HolyResistance;
            int magic_eff = owner.MagicEfficency;

            AddPage(0);
            AddBackground(90, 30, 400, 325, 9200);
            AddLabel(150, 50, 0, @"Protections & Mods");
            AddItem(100, 50, 7107);
            int startX = 100;
            int startY = 100;
            AddLabel(startX, startY, 0, @"Armor Rating");
            AddLabel(startX, startY += 25, 0, @"Physical Protection");
            AddLabel(startX, startY += 25, 0, @"Poison Protection");
            AddLabel(startX, startY += 25, 0, @"Fire Protection");
            AddLabel(startX, startY += 25, 0, @"Water Protection");
            AddLabel(startX, startY += 25, 0, @"Air Protection");
            AddLabel(startX, startY += 25, 0, @"Earth Protection");
            AddLabel(startX, startY += 25, 0, @"Necro Protection");
            AddLabel(startX, startY += 25, 0, @"Holy Protection");
            AddLabel(startX, startY += 25, 0, @"Magic Efficency");
            // add healing, free action, blackrock, moonstone here later

            startX += 250;
            startY = 100;
            AddLabel(startX, startY, 0, armor.ToString());
            AddLabel(startX, startY += 25, 0, phys.ToString() + @"%");
            AddLabel(startX, startY += 25, 0, pois.ToString() + @"%");
            AddLabel(startX, startY += 25, 0, fire.ToString() + @"%");
            AddLabel(startX, startY += 25, 0, water.ToString() + @"%");
            AddLabel(startX, startY += 25, 0, air.ToString() + @"%");
            AddLabel(startX, startY += 25, 0, earth.ToString() + @"%");
            AddLabel(startX, startY += 25, 0, necro.ToString() + @"%");
            AddLabel(startX, startY += 25, 0, holy.ToString() + @"%");
            AddLabel(startX, startY += 25, 0, magic_eff.ToString() + @"%");

        }
    }
}