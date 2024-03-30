using Cysharp.Threading.Tasks;

namespace Source.GameEvents
{
    public abstract class GameEvent
    {
        public virtual UniTask Apply(EventTracker tracker)
        {
            return UniTask.CompletedTask;
        }

        public override string ToString()
        {
            var name = GetType().Name;

            // Remove generic argument
            var genericArgIndex = name.IndexOf('`');
            if (genericArgIndex > 0)
            {
                name = name.Substring(0, genericArgIndex);
            }

            return name;
        }
    }
}
