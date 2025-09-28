using System;

public class MinHeap<T> where T : IComparable<T>
{
    private T[] data;
    private int count;

    public int Count => count;

    public MinHeap(int capacity)
    {
        data = new T[capacity];
        count = 0;
    }

    public void Clear()
    {
        count = 0;
    }

    public void AddUpdate(T item)
    {
        if (count >= data.Length)
            Array.Resize(ref data, data.Length * 2);

        data[count] = item;
        HeapifyUp(count);
        count++;
    }

    public T Pop()
    {
        if (count == 0) throw new InvalidOperationException("Heap is empty");

        T min = data[0];
        count--;
        data[0] = data[count];
        HeapifyDown(0);
        return min;
    }

    public T Peek()
    {
        if (count == 0) throw new InvalidOperationException("Heap is empty");
        return data[0];
    }

    private void HeapifyUp(int index)
    {
        int parentIndex;
        while (index > 0)
        {
            parentIndex = (index - 1) / 2;
            if (data[index].CompareTo(data[parentIndex]) >= 0) break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        int left, right, smallest;
        while (true)
        {
            left = 2 * index + 1;
            right = 2 * index + 2;
            smallest = index;

            if (left < count && data[left].CompareTo(data[smallest]) < 0)
                smallest = left;
            if (right < count && data[right].CompareTo(data[smallest]) < 0)
                smallest = right;

            if (smallest == index) break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        T temp = data[a];
        data[a] = data[b];
        data[b] = temp;
    }
}
