using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Item
{
    public abstract class BaseItem:MonoBehaviour
    {
       public enum ItemType
        {
            Firearms,
            Attachment,
            Others
        }

        public ItemType CurrentItemType;

        public int ItemId;
        public string ItemName;
    }
}
