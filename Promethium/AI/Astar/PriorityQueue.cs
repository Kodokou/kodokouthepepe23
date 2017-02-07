using System.Collections.Generic;

namespace Promethium.AI.Astar
{
    internal sealed class PriorityQueue<T>
    {
        private readonly List<T> InnerList = new List<T>();
        private readonly IComparer<T> mComparer;

        public PriorityQueue(IComparer<T> comparer, int capacity)
        {
            mComparer = comparer;
            InnerList.Capacity = capacity;
        }

        private void SwitchElements(int i, int j)
        {
            T h = InnerList[i];
            InnerList[i] = InnerList[j];
            InnerList[j] = h;
        }

        private int OnCompare(int i, int j)
        {
            return mComparer.Compare(InnerList[i], InnerList[j]);
        }

        public int Push(T item)
        {
            int p = InnerList.Count, p2;
            InnerList.Add(item);
            while (true)
            {
                if (p == 0) break;
                p2 = (p - 1) / 2;
                if (OnCompare(p, p2) < 0)
                {
                    SwitchElements(p, p2);
                    p = p2;
                }
                else break;
            }
            return p;
        }

        public T Pop()
        {
            var result = InnerList[0];
            int p = 0, p1, p2, pn;
            InnerList[0] = InnerList[InnerList.Count - 1];
            InnerList.RemoveAt(InnerList.Count - 1);
            while (true)
            {
                pn = p;
                p1 = 2 * p + 1;
                p2 = 2 * p + 2;
                if (InnerList.Count > p1 && OnCompare(p, p1) > 0) p = p1;
                if (InnerList.Count > p2 && OnCompare(p, p2) > 0) p = p2;
                if (p == pn) break;
                SwitchElements(p, pn);
            }
            return result;
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public int Count
        {
            get { return InnerList.Count; }
        }
    }
}