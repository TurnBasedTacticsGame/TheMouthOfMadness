using System;

namespace Source.GameEvents.Core
{
    public class GameEventNode
    {
        public GameEvent Event { get; private set; }
        public bool IsLocked { get; private set; }

        public GameEventNode(GameEvent gameEvent)
        {
            Event = gameEvent;
        }

        public void Prevent()
        {
            if (IsLocked)
            {
                throw new InvalidOperationException("Event has been locked. Events can only be prevented during the OnEventRaised event.");
            }

            if (Event is PreventedEvent)
            {
                throw new InvalidOperationException("Event has already been prevented. Events can only be prevented once.");
            }

            Event = new PreventedEvent(Event);
        }

        public void Lock()
        {
            IsLocked = true;
        }

        public override string ToString()
        {
            return Event.ToString();
        }
    }
}
