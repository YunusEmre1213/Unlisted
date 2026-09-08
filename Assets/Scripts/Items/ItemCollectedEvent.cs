using Backrooms.Core;

namespace Backrooms.Items
{
    public struct ItemCollectedEvent : IGameEvent
    {
        public ItemData Item;

        public ItemCollectedEvent(ItemData item)
        {
            Item = item;
        }
    }
}