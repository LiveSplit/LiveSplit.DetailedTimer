using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class DetailedTimerFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Detailed Timer"; }
        }

        public string Description
        {
            get { return "Displays the run timer, segment timer, and segment times for up to two comparisons."; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Timer; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new DetailedTimer(state);
        }

        public string UpdateName
        {
            get { return ComponentName; }
        }

        public string XMLURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/Components/update.LiveSplit.DetailedTimer.xml"; }
#else
            get { return "http://livesplit.org/update/Components/update.LiveSplit.DetailedTimer.xml"; }
#endif
        }

        public string UpdateURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/"; }
#else
            get { return "http://livesplit.org/update/"; }
#endif
        }

        public Version Version
        {
            get { return Version.Parse("1.3.0"); }
        }
    }
}
