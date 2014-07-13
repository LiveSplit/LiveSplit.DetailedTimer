using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.UI.Components
{
    public class SegmentTimer : Timer
    {
        public override TimeSpan? GetTime(Model.LiveSplitState state)
        {
            TimeSpan? lastSplit = TimeSpan.Zero;
            var runEndedDelay = state.CurrentPhase == TimerPhase.Ended ? 1 : 0;
            if (state.CurrentSplitIndex > 0 + runEndedDelay)
            {
                if (state.Run[state.CurrentSplitIndex - 1 - runEndedDelay].SplitTime[state.CurrentTimingMethod] != null)
                    lastSplit = state.Run[state.CurrentSplitIndex - 1 - runEndedDelay].SplitTime[state.CurrentTimingMethod].Value;
                else
                    lastSplit = null;
            }
            if (state.CurrentPhase == TimerPhase.NotRunning)
                return state.Run.Offset;
            else
                return state.CurrentTime[state.CurrentTimingMethod] - lastSplit;
        }
    }
}
