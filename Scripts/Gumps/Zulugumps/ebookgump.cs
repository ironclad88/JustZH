﻿using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Zulu.EarthSpells;

/*
 *  Author Oscar Ternström
 */

namespace Server.Gumps.Zulugumps
{

    public class ebookgump : Gump
    {

        Mobile test;

        public ebookgump(Mobile owner, bool[] array)
            : base(100, 0)
        {
            try
            {
                test = owner;
                int xName1 = 80;
                int xName2 = 240;
                int yName1 = 65;
                int yName2 = 65;

                int btnX1 = 60;
                int btnX2 = 220;
                int btnY1 = 70;
                int btnY2 = 70;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddPage(0);
                this.AddImage(30, 30, 2203);

                this.AddLabel(70, 40, 28, @"Circle 1 Spells");
                this.AddLabel(230, 40, 28, @"Circle 2 Spells");

                if (array[1])
                { // starts with 1 instead of 0, 0 is the event for book close, if you start with 0 you cast antidote when you close the damn book
                    this.AddLabel(xName1, yName1, 66, @"Antidote");
                    this.AddButton(btnX1, btnY1, 2104, 2103, 1, GumpButtonType.Reply, 0);
                }

                if (array[2])
                {
                    this.AddLabel(xName1, yName1 += 20, 66, @"Owl Sight");
                    this.AddButton(btnX1, btnY1 += 20, 2104, 2103, 2, GumpButtonType.Reply, 0);
                }

                if (array[3])
                {
                    this.AddLabel(xName1, yName1 += 20, 66, @"Shifting Earth");
                    this.AddButton(btnX1, btnY1 += 20, 2104, 2103, 3, GumpButtonType.Reply, 0);
                }

                if (array[4])
                {
                    this.AddLabel(xName1, yName1 += 20, 66, @"Summon Mammals");
                    this.AddButton(btnX1, btnY1 += 20, 2104, 2103, 4, GumpButtonType.Reply, 0);
                }

                if (array[5])
                {
                    this.AddLabel(xName1, yName1 += 20, 66, @"Call Lightning");
                    this.AddButton(btnX1, btnY1 += 20, 2104, 2103, 5, GumpButtonType.Reply, 0);
                }

                if (array[6])
                {
                    this.AddLabel(xName1, yName1 += 20, 66, @"Earth Blessing");
                    this.AddButton(btnX1, btnY1 += 20, 2104, 2103, 6, GumpButtonType.Reply, 0);
                }

                if (array[7])
                {
                    this.AddLabel(xName1, yName1 += 20, 66, @"Earth Portal");
                    this.AddButton(btnX1, btnY1 += 20, 2104, 2103, 7, GumpButtonType.Reply, 0);
                }

                if (array[8])
                {
                    this.AddLabel(xName1, yName1 += 20, 66, @"Nature´s Touch");
                    this.AddButton(btnX1, btnY1 += 20, 2104, 2103, 8, GumpButtonType.Reply, 0);
                }

                if (array[9])
                {
                    this.AddLabel(xName2, yName2, 66, @"Gust of Air");
                    this.AddButton(btnX2, btnY2, 2104, 2103, 9, GumpButtonType.Reply, 0);
                }

                if (array[10])
                {
                    this.AddLabel(xName2, yName2 += 20, 66, @"Rising Fire");
                    this.AddButton(btnX2, btnY2 += 20, 2104, 2103, 10, GumpButtonType.Reply, 0);
                }

                if (array[11])
                {
                    this.AddLabel(xName2, yName2 += 20, 66, @"Shapeshift");
                    this.AddButton(btnX2, btnY2 += 20, 2104, 2103, 11, GumpButtonType.Reply, 0);
                }

                if (array[12])
                {
                    this.AddLabel(xName2, yName2 += 20, 66, @"Ice Strike");
                    this.AddButton(btnX2, btnY2 += 20, 2104, 2103, 12, GumpButtonType.Reply, 0);
                }

                if (array[13])
                {
                    this.AddLabel(xName2, yName2 += 20, 66, @"Earth Spirit");
                    this.AddButton(btnX2, btnY2 += 20, 2104, 2103, 13, GumpButtonType.Reply, 0);
                }

                if (array[14])
                {
                    this.AddLabel(xName2, yName2 += 20, 66, @"Fire Spirit");
                    this.AddButton(btnX2, btnY2 += 20, 2104, 2103, 14, GumpButtonType.Reply, 0);
                }

                if (array[15])
                {
                    this.AddLabel(xName2, yName2 += 20, 66, @"Storm Spirit");
                    this.AddButton(btnX2, btnY2 += 20, 2104, 2103, 15, GumpButtonType.Reply, 0);
                }

                if (array[16])
                {
                    this.AddLabel(xName2, yName2 += 20, 66, @"Water Spirit");
                    this.AddButton(btnX2, btnY2 += 20, 2104, 2103, 16, GumpButtonType.Reply, 0);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }


        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            Mobile caster = test;
            switch (info.ButtonID)
            {
                case 1:
                    new Antidote(caster, null).Cast();
                    break;
                case 2:
                    new OwlSight(caster, null).Cast();
                    break;
                case 3:
                    new ShiftingEarth(caster, null).Cast();
                    break;
                case 4:
                    new SummonMammal(caster, null).Cast();
                    break;
                case 5:
                    new CallLightning(caster, null).Cast();
                    break;
                case 6:
                    new EarthBless(caster, null).Cast();
                    break;
                case 7:
                    new Earthportal(caster, null).Cast();
                    break;
                case 8:
                    new NaturesTouch(caster, null).Cast();
                    break;
                case 9:
                    new Gustofair(caster, null).Cast();
                    break;
                case 10:
                    new RisingFire(caster, null).Cast();
                    break;
                case 11:
                    new Gustofair(caster, null).Cast();
                    break;
                case 12:
                    new IceStrike(caster, null).Cast();
                    break;
                case 13:
                    new EarthSpirit(caster, null).Cast();
                    break;
                case 14:
                    new FireSpirit(caster, null).Cast();
                    break;
                case 15:
                    new StormSpirit(caster, null).Cast();
                    break;
                case 16:
                    new WaterSpirit(caster, null).Cast();
                    break;

            }
        }
    }
}