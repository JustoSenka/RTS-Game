using System.Collections.Generic;
using System;

[Serializable]
public class SortedList<T> : List<T>
{
	private bool isGenuine;
	private IComparer<T> comparer;

	public SortedList(IComparer<T> comparer) : base()
	{
		this.comparer = comparer;
	}

	private void InternalSort()
	{
		if (!isGenuine)
		{
			Sort(comparer);
			isGenuine = true;
		}
	}

	public new T this[int index]
	{
		get
		{
			InternalSort();
			return base[index];
		}
		set
		{
			InternalSort();
			base[index] = value;
		}
	}

	public new Enumerator GetEnumerator()
	{
		InternalSort();
		return base.GetEnumerator();
	}

	#region Add Remove Methods Override
	public new void Add(T item)
	{
		isGenuine = false;
		base.Add(item);
	}

	public new void AddRange(IEnumerable<T> collection)
	{
		isGenuine = false;
		base.AddRange(collection);
	}

	public new void Insert(int index, T item)
	{
		isGenuine = false;
		base.Insert(index, item);
	}

	public new void InsertRange(int index, IEnumerable<T> collection)
	{
		isGenuine = false;
		base.InsertRange(index, collection);
	}

	public new bool Remove(T item)
	{
		isGenuine = false;
		return base.Remove(item);
	}

	public new int RemoveAll(Predicate<T> match)
	{
		isGenuine = false;
		return base.RemoveAll(match);
	}

	public new void RemoveAt(int index)
	{
		isGenuine = false;
		base.RemoveAt(index);
	}

	public new void RemoveRange(int index, int count)
	{
		isGenuine = false;
		base.RemoveRange(index, count);
	}
	#endregion
}
