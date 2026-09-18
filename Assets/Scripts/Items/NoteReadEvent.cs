using Backrooms.Core;

namespace Backrooms.Items
{
    public struct NoteReadEvent : IGameEvent
    {
        public ItemData Item;

        public NoteReadEvent(ItemData item)
        {
            Item = item;
        }
    }
}