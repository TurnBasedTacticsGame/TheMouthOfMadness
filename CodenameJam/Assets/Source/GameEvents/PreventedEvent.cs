namespace Source.GameEvents
{
    /// <summary>
    /// Null object used to replace an event that was prevented.
    /// </summary>
    public class PreventedEvent : GameEvent
    {
        public PreventedEvent(GameEvent originalEvent)
        {
            OriginalEvent = originalEvent;
        }

        public GameEvent OriginalEvent { get; }

        public override string ToString()
        {
            return $"{GetType().Name} - {OriginalEvent}";
        }
    }
}
