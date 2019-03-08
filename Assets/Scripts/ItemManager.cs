using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{


    public class ItemManager : NetworkBehaviour
    {
        public static float ItemDisableTime { get { return 60f; } }
        /// <summary>list of disabled items</summary>
        private static List<Item> disabledItems = new List<Item>();
        private static List<Item> toRemove = new List<Item>();

        private void Update()
        {
            // go through diabled item list
            foreach (Item i in disabledItems)
            {
                i.gameObject.GetComponent<Item>().deactivatedTime -= Time.deltaTime;
                if (i.gameObject.GetComponent<Item>().deactivatedTime <= 0)
                {
                    // Set to remove list
                    toRemove.Add(i);
                }
            }

            // go through to remove item list
            foreach (Item i in toRemove)
            {
                // set timer to 0
                i.gameObject.GetComponent<Item>().deactivatedTime = 0;
                // reset position
                i.gameObject.transform.position =
                new Vector3(
                    i.gameObject.transform.position.x,
                    0,
                    i.gameObject.transform.position.z);
                disabledItems.Remove(i);
            }

            // clear to remove list
            toRemove.Clear();
        }

        /// <summary>
        /// Add one Item to list
        /// </summary>
        /// <param name="_item">Item to add</param>
        public static void AddItem(Item _item)
        {
            // add item to list
            disabledItems.Add(_item);
            // set timer
            _item.deactivatedTime = ItemDisableTime;
            // locate item 10000 unity below
            _item.gameObject.transform.position =
                new Vector3(
                    _item.gameObject.transform.position.x,
                    -10000,
                    _item.gameObject.transform.position.z);
        }
    }

    public enum ItemType
    {
        NONE,
        BLIND,
        CONFUSED
    }
}
