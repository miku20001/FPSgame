using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Item;

public class AttachmentItem : BaseItem
{
    public enum AttachmentType
    {
        Scope,
        Other
    }

    public AttachmentType CurrentAttackmentType;

}
