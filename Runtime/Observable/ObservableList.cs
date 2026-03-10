using System;
using System.Collections;
using System.Collections.Generic;

namespace HUtil.Runtime.Observable
{
    /// <summary>
    /// 데이터의 변경을 관찰할 수 있는 리스트
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObservableList<T> : IList<T>, IReadOnlyList<T>
    {
        // 실제 데이터를 담을 내부 리스트
        private readonly List<T> _list = new List<T>();
        
        // 구독자들에게 변화를 알릴 액션
        private event Action<ListChangeEvent<T>> _onChanged;

        public int Count => _list.Count;
        public bool IsReadOnly => false;

        // 인덱서 (Replace 이벤트 발생)
        public T this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                Notify(ListChangeAction.Replace, value, index);
            }
        }

        /// <summary>
        /// 리스트의 변경을 관찰할 수 있는 구독을 추가합니다
        /// </summary>
        /// <param name="onChanged">변경이 발생했을 때 호출될 델리게이트</param>
        /// <param name="notifyImmediately">구독 즉시 기존에 있던 아이템들을 'Add' 액션으로 쏴줍니다</param>
        /// <returns>구독을 취소할 수 있는 IDisposable</returns>
        public IDisposable Subscribe(Action<ListChangeEvent<T>> onChanged, bool notifyImmediately = true)
        {
            if (onChanged == null) throw new ArgumentNullException(nameof(onChanged));

            _onChanged += onChanged;

            // 구독 즉시 기존에 있던 아이템들을 'Add' 액션으로 쏴줍니다.
            // (UI 초기화 로직을 분리할 필요가 없어집니다!)
            if (notifyImmediately)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    onChanged(new ListChangeEvent<T>(ListChangeAction.Add, _list[i], i));
                }
            }

            // 이전과 동일한 구독 해제 패턴 반환
            return new ScriptableDisposable(() => _onChanged -= onChanged);
        }

        // --- IList<T> 구현부 (데이터 변경 + 알림) ---
        public void Add(T item)
        {
            _list.Add(item);
            Notify(ListChangeAction.Add, item, _list.Count - 1);
        }

        public bool Remove(T item)
        {
            int index = _list.IndexOf(item);
            if (index >= 0)
            {
                _list.RemoveAt(index);
                Notify(ListChangeAction.Remove, item, index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            T item = _list[index];
            _list.RemoveAt(index);
            Notify(ListChangeAction.Remove, item, index);
        }

        public void Clear()
        {
            _list.Clear();
            // Clear는 아이템이나 인덱스가 무의미하므로 default로 쏩니다.
            Notify(ListChangeAction.Clear, default, -1);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            Notify(ListChangeAction.Add, item, index); // Insert도 UI 입장에선 Add 처리
        }

        private void Notify(ListChangeAction action, T item, int index)
        {
            _onChanged?.Invoke(new ListChangeEvent<T>(action, item, index));
        }

        // 나머지 IList 구현 (알림 불필요)
        public bool Contains(T item) => _list.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public int IndexOf(T item) => _list.IndexOf(item);
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}