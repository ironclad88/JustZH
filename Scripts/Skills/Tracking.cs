using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Spells;
using Server.Spells.Necromancy;

namespace Server.SkillHandlers
{
    public delegate bool TrackTypeDelegate(Mobile m);

    public class Tracking
    {
        private static readonly Dictionary<Mobile, TrackingInfo> m_Table = new Dictionary<Mobile, TrackingInfo>();
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Tracking].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.SendLocalizedMessage(1011350); // What do you wish to track?

            m.CloseGump(typeof(TrackWhatGump));
            m.CloseGump(typeof(TrackWhoGump));
            m.SendGump(new TrackWhatGump(m));

            if (m.SpecClasse == SpecClasse.Ranger)
            {
                if (m.SpecLevel == 1)
                {
                    return TimeSpan.FromSeconds(6.0);
                }
                else if (m.SpecLevel == 2)
                {
                    return TimeSpan.FromSeconds(5.5);
                }
                else if (m.SpecLevel == 3)
                {
                    return TimeSpan.FromSeconds(5.0);
                }
                else if (m.SpecLevel == 4)
                {
                    return TimeSpan.FromSeconds(4.0);
                }
                else if (m.SpecLevel == 5)
                {
                    return TimeSpan.FromSeconds(3.5);
                }
            }
            return TimeSpan.FromSeconds(7.0); // 7 second delay before beign able to re-use a skill
        }

        public static void AddInfo(Mobile tracker, Mobile target)
        {
            TrackingInfo info = new TrackingInfo(tracker, target);
            m_Table[tracker] = info;
        }

        public static double GetStalkingBonus(Mobile tracker, Mobile target)
        {
            TrackingInfo info = null;
            m_Table.TryGetValue(tracker, out info);

            if (info == null || info.m_Target != target || info.m_Map != target.Map)
                return 0.0;

            int xDelta = info.m_Location.X - target.X;
            int yDelta = info.m_Location.Y - target.Y;

            double bonus = Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));

            m_Table.Remove(tracker);	//Reset as of Pub 40, counting it as bug for Core.SE.


            if (Core.ML)
                return Math.Min(bonus, 10 + tracker.Skills.Tracking.Value / 10);

            return bonus;
        }

        public static void ClearTrackingInfo(Mobile tracker)
        {
            m_Table.Remove(tracker);
        }

        public class TrackingInfo
        {
            public Mobile m_Tracker;
            public Mobile m_Target;
            public Point2D m_Location;
            public Map m_Map;
            public TrackingInfo(Mobile tracker, Mobile target)
            {
                this.m_Tracker = tracker;
                this.m_Target = target;
                this.m_Location = new Point2D(target.X, target.Y);
                this.m_Map = target.Map;
            }
        }
    }

    public class TrackWhatGump : Gump
    {
        private readonly Mobile m_From;
        private readonly bool m_Success;
        public TrackWhatGump(Mobile from)
            : base(20, 30)
        {
            this.m_From = from;
            this.m_Success = from.CheckSkill(SkillName.Tracking, 0.0, 21.1);

            this.AddPage(0);

            this.AddBackground(0, 0, 440, 135, 5054);

            this.AddBackground(10, 10, 420, 75, 2620);
            this.AddBackground(10, 85, 420, 25, 3000);

            this.AddItem(20, 20, 9682);
            this.AddButton(20, 110, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(20, 90, 100, 20, 1018087, false, false); // Animals

            this.AddItem(120, 20, 9607);
            this.AddButton(120, 110, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(120, 90, 100, 20, 1018088, false, false); // Monsters

            this.AddItem(220, 20, 8454);
            this.AddButton(220, 110, 4005, 4007, 3, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(220, 90, 100, 20, 1018089, false, false); // Human NPCs

            this.AddItem(320, 20, 8455);
            this.AddButton(320, 110, 4005, 4007, 4, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(320, 90, 100, 20, 1018090, false, false); // Players
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID >= 1 && info.ButtonID <= 4)
                TrackWhoGump.DisplayTo(this.m_Success, this.m_From, info.ButtonID - 1);
        }
    }

    public class TrackWhoGump : Gump
    {
        private static readonly TrackTypeDelegate[] m_Delegates = new TrackTypeDelegate[]
        {
            new TrackTypeDelegate(IsAnimal),
            new TrackTypeDelegate(IsMonster),
            new TrackTypeDelegate(IsHumanNPC),
            new TrackTypeDelegate(IsPlayer)
        };
        private readonly Mobile m_From;
        private readonly int m_Range;
        private readonly List<Mobile> m_List;
        private TrackWhoGump(Mobile from, List<Mobile> list, int range)
            : base(20, 30)
        {
            this.m_From = from;
            this.m_List = list;
            this.m_Range = range;

            this.AddPage(0);

            this.AddBackground(0, 0, 440, 155, 5054);

            this.AddBackground(10, 10, 420, 75, 2620);
            this.AddBackground(10, 85, 420, 45, 3000);

            if (list.Count > 4)
            {
                this.AddBackground(0, 155, 440, 155, 5054);

                this.AddBackground(10, 165, 420, 75, 2620);
                this.AddBackground(10, 240, 420, 45, 3000);

                if (list.Count > 8)
                {
                    this.AddBackground(0, 310, 440, 155, 5054);

                    this.AddBackground(10, 320, 420, 75, 2620);
                    this.AddBackground(10, 395, 420, 45, 3000);
                }
            }

            for (int i = 0; i < list.Count && i < 12; ++i)
            {
                Mobile m = list[i];

                this.AddItem(20 + ((i % 4) * 100), 20 + ((i / 4) * 155), ShrinkTable.Lookup(m));
                this.AddButton(20 + ((i % 4) * 100), 130 + ((i / 4) * 155), 4005, 4007, i + 1, GumpButtonType.Reply, 0);

                if (m.Name != null)
                    this.AddHtml(20 + ((i % 4) * 100), 90 + ((i / 4) * 155), 90, 40, m.Name, false, false);
            }
        }

        public static void DisplayTo(bool success, Mobile from, int type)
        {
            if (!success)
            {
                from.SendLocalizedMessage(1018092); // You see no evidence of those in the area.
                return;
            }

            Map map = from.Map;

            if (map == null)
                return;

            TrackTypeDelegate check = m_Delegates[type];

            // JustZH i think this is a passive gain while tracking, should not be used (not used like this in zulu)
            // from.CheckSkill(SkillName.Tracking, 21.1, 100.0); // Passive gain           // hmmmm?

            int range = 40 + (int)(from.Skills[SkillName.Tracking].Value / 10);

            // JustZH Tracking bonus for Rangers using Tracking
            if (from.SpecClasse == SpecClasse.Ranger) // spec rangers is tracking larger area
                range *= ((int)from.SpecBonus(SpecClasse.Ranger) + 10 / 10);


            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in from.GetMobilesInRange(range))
            {
                //if (m != from && (!Core.AOS || m.Alive) && (!m.Hidden || m.IsPlayer() || from.AccessLevel > m.AccessLevel) && check(m) && CheckDifficulty(from, m))
                if (m != from && (!Core.AOS || m.Alive) && (m.IsPlayer() || from.AccessLevel > m.AccessLevel) && check(m) && !m.Hidden) // removed checkDifficulty, can no longer detect hidden players
                    list.Add(m);
            }

            if (list.Count > 0)
            {
                from.CheckSkill(SkillName.Tracking, 21.1, 130.0); // made skillgain if you succeed to find anyone
                list.Sort(new InternalSorter(from));

                from.SendGump(new TrackWhoGump(from, list, range));
                from.SendLocalizedMessage(1018093); // Select the one you would like to track.
            }
            else
            {
                if (type == 0)
                    from.SendLocalizedMessage(502991); // You see no evidence of animals in the area.
                else if (type == 1)
                    from.SendLocalizedMessage(502993); // You see no evidence of creatures in the area.
                else
                    from.SendLocalizedMessage(502995); // You see no evidence of people in the area.
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int index = info.ButtonID - 1;

            if (index >= 0 && index < this.m_List.Count && index < 12)
            {
                Mobile m = this.m_List[index];

                this.m_From.QuestArrow = new TrackArrow(this.m_From, m, this.m_Range * 2);

                if (Core.SE)
                    Tracking.AddInfo(this.m_From, m);
            }
        }

        // Tracking players uses tracking and detect hidden vs. hiding and stealth 
        private static bool CheckDifficulty(Mobile from, Mobile m)
        {
            return true; // meh, make a chance check later
        }

        private static bool IsAnimal(Mobile m)
        {
            return (!m.Player && m.Body.IsAnimal);
        }

        private static bool IsMonster(Mobile m)
        {
            return (!m.Player && m.Body.IsMonster);
        }

        private static bool IsHumanNPC(Mobile m)
        {
            return (!m.Player && m.Body.IsHuman);
        }

        private static bool IsPlayer(Mobile m)
        {
            return m.Player;
        }

        private class InternalSorter : IComparer<Mobile>
        {
            private readonly Mobile m_From;
            public InternalSorter(Mobile from)
            {
                this.m_From = from;
            }

            public int Compare(Mobile x, Mobile y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                return this.m_From.GetDistanceToSqrt(x).CompareTo(this.m_From.GetDistanceToSqrt(y));
            }
        }
    }

    public class TrackArrow : QuestArrow
    {
        private readonly Timer m_Timer;
        private Mobile m_From;
        public TrackArrow(Mobile from, Mobile target, int range)
            : base(from, target)
        {
            this.m_From = from;
            this.m_Timer = new TrackTimer(from, target, range, this);
            this.m_Timer.Start();
        }

        public override void OnClick(bool rightClick)
        {
            if (rightClick)
            {
                Tracking.ClearTrackingInfo(this.m_From);

                this.m_From = null;

                this.Stop();
            }
        }

        public override void OnStop()
        {
            this.m_Timer.Stop();

            if (this.m_From != null)
            {
                Tracking.ClearTrackingInfo(this.m_From);

                this.m_From.SendLocalizedMessage(503177); // You have lost your quarry.
            }
        }
    }

    public class TrackTimer : Timer
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly int m_Range;
        private readonly QuestArrow m_Arrow;
        private int m_LastX, m_LastY;
        public TrackTimer(Mobile from, Mobile target, int range, QuestArrow arrow)
            : base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2.5))
        {
            this.m_From = from;
            this.m_Target = target;
            this.m_Range = range;

            this.m_Arrow = arrow;
        }

        protected override void OnTick()
        {
            if (!this.m_Arrow.Running)
            {
                this.Stop();
                return;
            }
            else if (this.m_From.NetState == null || this.m_From.Deleted || this.m_Target.Deleted || this.m_From.Map != this.m_Target.Map || !this.m_From.InRange(this.m_Target, this.m_Range) || (this.m_Target.Hidden && this.m_Target.AccessLevel > this.m_From.AccessLevel))
            {
                this.m_Arrow.Stop();
                this.Stop();
                return;
            }

            if (this.m_LastX != this.m_Target.X || this.m_LastY != this.m_Target.Y)
            {
                this.m_LastX = this.m_Target.X;
                this.m_LastY = this.m_Target.Y;

                this.m_Arrow.Update();
            }
        }
    }
}