using UnityEngine;

namespace Backrooms.Items
{
    public enum ItemCategory
    {
        Functional,
        Personal
    }

    [CreateAssetMenu(fileName = "NewItem", menuName = "Backrooms/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string itemId;
        public string displayName;
        [TextArea] public string description;
        public ItemCategory category;
    }
}