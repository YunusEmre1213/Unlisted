using UnityEngine;
using Backrooms.Core;

namespace Backrooms.AI
{
    public class FakeClueState : IState
    {
        public void Enter()
        {
            Debug.Log("[ThreatState] Sahte ipucu tetiklendi");
            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.FakeClue));
        }

        public void Tick()
        {
        }

        public void Exit()
        {
            Debug.Log("[ThreatState] Sahte ipucu durumundan cikildi");
        }
    }
}