using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components;

public class SegmentTimer : Timer
{
    public override TimeSpan? GetTime(LiveSplitState state, TimingMethod method)
    {
        TimeSpan? lastSplit = TimeSpan.Zero;
        int runEndedDelay = state.CurrentPhase == TimerPhase.Ended ? 1 : 0;
        if (state.CurrentSplitIndex > 0 + runEndedDelay)
        {
            lastSplit = state.Run[state.CurrentSplitIndex - 1 - runEndedDelay].SplitTime[method];
        }

        if (state.CurrentPhase == TimerPhase.NotRunning)
        {
            return state.Run.Offset;
        }
        else
        {
            return state.CurrentTime[method] - lastSplit;
        }
    }
}
