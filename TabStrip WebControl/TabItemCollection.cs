/*
Copyright (c) 2007 Bill Davidsen (wdavidsen@yahoo.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;

namespace SCS.Web.UI.WebControls
{
	public sealed class TabItemCollection : IList, IStateManager
	{
		#region Fields
		private ArrayList _tabItems;
		private bool _isTrackingViewState;
		private bool _saveAll;
		#endregion

		internal TabItemCollection()
		{
			_tabItems = new ArrayList();
		}

		public TabItem this[int index]
		{
			get
			{
				return (TabItem)_tabItems[index];
			}
		}
		object IList.this[int index]
		{
			get
			{
				return _tabItems[index];
			}
			set
			{
				_tabItems[index] = (TabItem)value;
			}
		}
		public int Add(TabItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			_tabItems.Add(item);

			if (_isTrackingViewState)
			{
				((IStateManager)item).TrackViewState();
				item.SetDirty();
			}

			return _tabItems.Count - 1;
		}
		public void Clear()
		{
			_tabItems.Clear();
			if (_isTrackingViewState)
			{
				_saveAll = true;
			}
		}
		public bool Contains(TabItem item)
		{
			if (item == null)
			{
				return false;
			}
			return _tabItems.Contains(item);
		}
		public int IndexOf(TabItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return _tabItems.IndexOf(item);
		}
		public void Insert(int index, TabItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			_tabItems.Insert(index, item);

			if (_isTrackingViewState)
			{
				((IStateManager)item).TrackViewState();
				_saveAll = true;
			}
		}
		public void RemoveAt(int index)
		{
			_tabItems.RemoveAt(index);
			if (_isTrackingViewState)
			{
				_saveAll = true;
			}
		}
		public void Remove(TabItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			int index = IndexOf(item);
			if (index >= 0)
			{
				RemoveAt(index);
			}
		}

		#region IEnumerable Implementation
		public IEnumerator GetEnumerator()
		{
			return _tabItems.GetEnumerator();
		}
		#endregion IEnumerable Implementation

		#region ICollection Implementation
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int Count
		{
			get
			{
				return _tabItems.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			_tabItems.CopyTo(array, index);
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}
		#endregion ICollection Implementation

		#region IList Implementation
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}
		int IList.Add(object item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!(item is TabItem))
			{
				throw new ArgumentException("item must be a TabItem");
			}

			return Add((TabItem)item);
		}
		void IList.Clear()
		{
			Clear();
		}
		bool IList.Contains(object item)
		{
			return Contains(item as TabItem);
		}
		int IList.IndexOf(object item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!(item is TabItem))
			{
				throw new ArgumentException("item must be a TabItem");
			}

			return IndexOf((TabItem)item);
		}
		void IList.Insert(int index, object item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!(item is TabItem))
			{
				throw new ArgumentException("item must be a TabItem");
			}

			Insert(index, (TabItem)item);
		}
		void IList.Remove(object item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!(item is TabItem))
			{
				throw new ArgumentException("item must be a TabItem");
			}

			Remove((TabItem)item);
		}
		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}
		#endregion IList Implementation

		#region IStateManager Implementation
		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return _isTrackingViewState;
			}
		}
		void IStateManager.LoadViewState(object savedState)
		{
			if (savedState == null)
			{
				return;
			}

			if (savedState is Pair)
			{
				// All items were saved.
				// Create new TabItem collection using view state.
				_saveAll = true;
				Pair p = (Pair)savedState;
				ArrayList types = (ArrayList)p.First;
				ArrayList states = (ArrayList)p.Second;
				int count = types.Count;

				_tabItems = new ArrayList(count);
				for (int i = 0; i < count; i++)
				{
					TabItem tabItem = null;
					if (((char)types[i]).Equals('c'))
						//{
						tabItem = new TabItem();
					//}
					//else 
					//{

					//}
					Add(tabItem);
					((IStateManager)tabItem).LoadViewState(states[i]);
				}
			}
			else
			{
				// Load modified items.
				Triplet t = (Triplet)savedState;
				ArrayList indices = (ArrayList)t.First;
				ArrayList types = (ArrayList)t.Second;
				ArrayList states = (ArrayList)t.Third;

				for (int i = 0; i < indices.Count; i++)
				{
					int index = (int)indices[i];
					if (index < this.Count)
					{
						((IStateManager)_tabItems[index]).LoadViewState(states[i]);
					}
					else
					{
						TabItem tabItem = null;
						//if (((char)types[i]).Equals('c')) 
						//{
						tabItem = new TabItem();
						//}
						//else 
						//{

						//}
						Add(tabItem);
						((IStateManager)tabItem).LoadViewState(states[i]);
					}
				}
			}
		}
		void IStateManager.TrackViewState()
		{
			_isTrackingViewState = true;

			foreach (TabItem tabItem in _tabItems)
			{
				((IStateManager)tabItem).TrackViewState();
			}
		}
		object IStateManager.SaveViewState()
		{
			if (_saveAll == true)
			{
				// Save all items.
				ArrayList types = new ArrayList(Count);
				ArrayList states = new ArrayList(Count);
				for (int i = 0; i < Count; i++)
				{
					TabItem tabItem = (TabItem)_tabItems[i];
					tabItem.SetDirty();

					//if (tabItem is TabItem) 
					//{
					types.Add('c');
					//}
					//else 
					//{
					//	types.Add('r');
					//}
					states.Add(((IStateManager)tabItem).SaveViewState());
				}
				if (types.Count > 0)
				{
					return new Pair(types, states);
				}
				else
				{
					return null;
				}
			}
			else
			{
				// Save only the dirty items.
				ArrayList indices = new ArrayList();
				ArrayList types = new ArrayList();
				ArrayList states = new ArrayList();

				for (int i = 0; i < Count; i++)
				{
					TabItem tabItem = (TabItem)_tabItems[i];
					object state = ((IStateManager)tabItem).SaveViewState();
					if (state != null)
					{
						states.Add(state);
						indices.Add(i);

						//if (tabItem is TabItem) 
						//{
						types.Add('c');
						//}
						//else 
						//{
						//	types.Add('r');
						//}
					}
				}

				if (indices.Count > 0)
				{
					return new Triplet(indices, types, states);
				}

				return null;
			}
		}
		#endregion IStateManager Implementation
	}
}
