using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AshMind.Extensions;

namespace Expressive.Common {
    public class PartialList<T> : IList<T> {
        private readonly IList<T> list;
        private readonly int startIndex;

        public PartialList(IList<T> list, int startIndex, int count) {
            this.list = list;
            this.startIndex = startIndex;
            this.Count = count;
        }

        private int GetEndIndex() {
            return this.startIndex + this.Count;
        }

        private void VerifyIsInList(int index) {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            if (this.startIndex + index > this.GetEndIndex())
                throw new ArgumentOutOfRangeException("index");
        }

        public int IndexOf(T item) {
            var indexInFullList = list.IndexOf(item);
            if (indexInFullList == -1)
                return -1;

            if (indexInFullList < startIndex)
                return -1;

            if (indexInFullList > this.GetEndIndex())
                return -1;

            return indexInFullList - startIndex;
        }

        public void Insert(int index, T item) {
            this.VerifyIsInList(index);
            this.list.Insert(startIndex + index, item);
        }

        public void RemoveAt(int index) {
            this.VerifyIsInList(index);
            this.list.RemoveAt(startIndex + index);
            this.Count -= 1;
        }

        public T this[int index] {
            get {
                this.VerifyIsInList(index);
                return this.list[startIndex + index];
            }
            set {
                this.VerifyIsInList(index);
                this.list[startIndex + index] = value;
            }
        }

        #region ICollection<T> Members

        public void Add(T item) {
            if (this.GetEndIndex() == this.list.Count - 1) {
                this.list.Add(item);
                this.Count += 1;
            }
            else {
                this.list.Insert(this.GetEndIndex() + 1, item);
                this.Count += 1;
            }
        }

        public void Clear() {
            this.list.RemoveRange(this.startIndex, this.Count);
            this.Count = 0;
        }

        public bool Contains(T item) {
            return this.IndexOf(item) > -1;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            var fullArray = new T[this.list.Count];
            this.list.CopyTo(fullArray, 0);
            Array.Copy(fullArray, this.startIndex, array, arrayIndex, this.Count);
        }

        public int Count { get; private set; }

        public bool IsReadOnly {
            get { return this.list.IsReadOnly; }
        }

        public bool Remove(T item) {
            var index = this.IndexOf(item);
            if (index == -1)
                return false;

            this.list.RemoveAt(this.startIndex + index);
            this.Count -= 1;
            return true;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator() {
            return this.list.EnumerateRange(this.startIndex, this.Count).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion
    }
}
