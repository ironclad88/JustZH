﻿using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

/*
 *  Author Oscar Ternström
 */

namespace Server.Gumps.Zulugumps
{

    public class SureCheckGump : Gump
    {

        private Skill _skill;

        public SureCheckGump(Mobile owner, Skill skilltype, string name)
            : this(owner, ResurrectMessage.Generic, false, skilltype, name)
        {
        }


        public SureCheckGump(Mobile owner, ResurrectMessage msg, bool fromSacrifice, Skill skilltype, string name)
            : this(owner, msg, skilltype, name)
        {
        }

        public SureCheckGump(Mobile owner, ResurrectMessage msg, Skill skilltype, string name)
            : base(100, 0)
        {

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            string _name = name;
            _skill = skilltype;

            this.AddPage(0);
            this.AddBackground(76, 93, 393, 139, 9200);
           // this.AddButton(195, 130, 210, 211, 2, GumpButtonType.Reply, 0, "Fencing");
            this.AddButton(100, 200, 247, 248, 1, GumpButtonType.Reply, 0, "OK");
            this.AddButton(380, 200, 241, 243, 2, GumpButtonType.Reply, 0, "NAY");
            this.AddLabel(85, 150, 0, @"Are you sure you want to drop " + _name + " to 0?");
            

        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            // button.ID            switch (info.ButtonID)

            switch (info.ButtonID)
            {
                case 1:
                    _skill.Base = 0;
                    from.CloseGump(typeof(SureCheckGump));
                    break;
                case 2:
                    from.CloseGump(typeof(SureCheckGump));
                    break;
            }

        }

    }
}