namespace HUtil.Runtime.Observable
{
    /// <summary>
    /// <see cref="ObservableList{T}"/>에서 발생한 행동의 종류
    /// </summary>
    public enum ListChangeAction
    {
        Add,
        Remove,
        Replace,
        Clear
    }

    /// <summary>
    /// <see cref="ObservableList{T}"/>에서 발생한 행동의 이벤트
    /// </summary>
    /// <typeparam name="T">리스트의 아이템 타입</typeparam>
    public readonly struct ListChangeEvent<T>
    {
        public readonly ListChangeAction Action;
        public readonly T Item;
        public readonly int Index;

        public ListChangeEvent(ListChangeAction action, T item, int index)
        {
            Action = action;
            Item = item;
            Index = index;
        }
    }
}