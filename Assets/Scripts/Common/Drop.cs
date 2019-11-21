﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;

public class Drop : MonoBehaviour, IDropHandler
{
    public int index;
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            Drag.draggingItem.transform.SetParent(this.transform);

            // 슬롯에 추가된 아이템을 GameData에 추가하기 위해 AddItem을 호출
            Item item = Drag.draggingItem.GetComponent<ItemInfo>().itemData;
            item.slotIndex = index;
            GameManager.instance.AddItem(item);
        }
    }
}
