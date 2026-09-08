using Backrooms.Core;

namespace Backrooms.AI
{
    public struct ThreatStateChangedEvent : IGameEvent
    {
        public ThreatStateType NewState;

        public ThreatStateChangedEvent(ThreatStateType newState)
        {
            NewState = newState;
        }
    }
}