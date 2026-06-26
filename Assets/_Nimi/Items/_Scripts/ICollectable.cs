using System;

namespace EMR.Medal
{
    public interface ICollectable
    {
        public int Count { get; }

        public event Action OnCollect;

        public void Collect();
    }
}