using System;
using System.Collections.Generic;

namespace UI.Runtime.Utility
{
    /** 模拟栈行为的列表 */
    public sealed class StackList<T>
    {
        private readonly List<T> _list = new List<T>();

        public int Count => _list.Count;
        public int Capacity => _list.Capacity;

        public StackList()
        {
        }

        public StackList(int capacity)
        {
            _list.Capacity = capacity;
        }

        public void Push(T value)
        {
            _list.Add(value);
        }

        public T Pop()
        {
            T t = Peek();
            _list.RemoveAt(Count - 1);
            return t;
        }

        public T Peek()
        {
            if (Count == 0)
                throw new Exception("StackList 为空，禁止Pop/Peek");

            return _list[Count - 1];
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void Remove(T value)
        {
            _list.Remove(value);
        }

        public bool Contains(T value)
        {
            return _list.Contains(value);
        }

        /** 返回栈底到栈顶的列表 */
        public List<T> GetList()
        {
            return _list;
        }
    }
}