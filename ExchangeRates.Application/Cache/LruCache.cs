using ExchangeRates.Application.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Algorithm copied from https://dotnetglance.com/Article/73/Implement-LRU-Cache-in-C- and edited for string key and values instead of int
namespace ExchangeRates.Application.Cache
{
    public class Node
    {
        public string Value { get; set; }
        public string Key { get; set; }
        public Node Previous { get; set; }
        public Node Next { get; set; }
    }
    public class LruCache
    {
        Node head;
        Node tail;
        Dictionary<string, Node> cacheWithData;
        readonly int capacity;

        public LruCache()
        {
            this.capacity = Constants.CacheCapacity;
            cacheWithData = new Dictionary<string, Node>();
        }

        public string Get(string key)
        {
            string value = "";
            Node temp;

            if (cacheWithData.TryGetValue(key, out temp))
            {
                value = temp.Value;
                // Now Move this Node to First location because it is most Recent Used
                MoveTo(temp);
            }
            return value;
        }

        private void MoveTo(Node node)
        {
            if (head == node)
            {
                return; // No Need to Perform any thing
            }
            if (node == tail)
            {
                // Last element 
                // so move last element to first location
                tail.Previous.Next = null;
                tail = tail.Previous;
            }
            else
            {
                // Middle Node
                var previous = node.Previous;
                previous.Next = node.Next;
                node.Next.Previous = previous;
            }
            // Moved node on to the first element
            head.Previous = node;
            node.Previous = null;
            node.Next = head;
            head = node;
        }

        public void Put(string key, string value)
        {
            // case 1 :- Key does't exist in Dictionary
            if (cacheWithData.ContainsKey(key))
            {
                Node node = cacheWithData[key];
                // Again moved to first location
                MoveTo(node);
                node.Value = value;
            }
            else
            {
                //So we have to add new key.
                Node node = new Node() { Key = key, Value = value };
                if (head == null)
                {
                    tail = head = node;
                }
                else
                {
                    // Add new Item on first location
                    node.Next = head;
                    head.Previous = node;
                    head = node;
                }

                cacheWithData.Add(key, node);
                if (cacheWithData.Count > capacity)
                {
                    // it's mean we need to removed last element from the cahce
                    cacheWithData.Remove(tail.Key);
                    if (tail.Previous != null)
                        tail.Previous.Next = null;
                    tail = tail.Previous;
                }
            }
        }
    }
}
