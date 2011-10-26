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
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel.Design.Serialization;

namespace SCS.Web.UI.WebControls
{
	public class TabItem : IStateManager
	{
		#region Fields
		private bool _isTrackingViewState;
		private bool _postBack = true;
		private StateBag _viewState;

		private string _tag = string.Empty;
		private string _text = string.Empty;
		private string _url = string.Empty;
		//private string _clientId = string.Empty;
		private string _id = string.Empty;

		private int _leftPadding = 0;
		private int _rightPadding = 0;

		private bool _selected = false;
		#endregion

		public TabItem()
		{

		}
		public TabItem(string text)
		{
			this._text = text;
		}
		public TabItem(string text, string url)
		{
			this._text = text;
			this._url = url;
		}
		public TabItem(string text, string url, bool selected)
		{
			this._text = text;
			this._url = url;
			this._selected = selected;
		}
		public TabItem(string text, string url, bool selected, bool postBack)
		{
			_text = text;
			_url = url;
			_selected = selected;
			_postBack = postBack;
		}
		public TabItem(string text, string url, bool selected, bool postBack, string tag)
		{
			_text = text;
			_url = url;
			_selected = selected;
			_postBack = postBack;
			_tag = tag;
		}

		protected StateBag ViewState
		{
			get
			{
				if (_viewState == null)
				{
					_viewState = new StateBag(false);
					if (_isTrackingViewState) ((IStateManager)_viewState).TrackViewState();
				}
				return _viewState;
			}
		}
		internal void SetDirty()
		{
			if (_viewState != null)
			{
				ICollection Keys = _viewState.Keys;
				foreach (string key in Keys)
				{
					_viewState.SetItemDirty(key, true);
				}
			}
		}

		public override string ToString()
		{
			return string.Format("ID={0}, Text={1}, Url={2}, Tag={3}, PostBack={4}, Selected={5}, RightPadding={6}, LeftPadding={7}",
				_id.ToString(),
				_text,
				_url,
				_tag,
				_postBack.ToString(),
				_selected.ToString(),
				_rightPadding.ToString(),
				_leftPadding.ToString());
		}

		#region IStatemanager implementation

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return _isTrackingViewState;
			}
		}
		void IStateManager.LoadViewState(object savedState)
		{
			if (savedState != null)
			{
				((IStateManager)ViewState).LoadViewState(savedState);

				string text = (string)ViewState["text"];

				if (text != null)
					this.Text = text;

				string tag = (string)ViewState["tag"];

				if (tag != null)
					this.Tag = tag;

				string url = (string)ViewState["url"];

				if (url != null)
					this.Url = url;

				string id = (string)ViewState["id"];

				if (id != null)
					this.Id_internal = id;

				object postBack = ViewState["postBack"];

				if (postBack != null)
					this.PostBack = Convert.ToBoolean(postBack);

				object selected = ViewState["selected"];

				if (selected != null)
					this.Selected = Convert.ToBoolean(selected);

				object rightPadding = ViewState["rightPadding"];

				if (rightPadding != null)
					this.RightPadding = (int)rightPadding;

				object leftPadding = ViewState["leftPadding"];

				if (leftPadding != null)
					this.LeftPadding = (int)leftPadding;
			}
		}
		object IStateManager.SaveViewState()
		{
			string currentText = this.Text;
			string initialText = (string)ViewState["text"];

			if (currentText.Equals(initialText) == false)
			{
				ViewState["text"] = currentText;
			}

			string currentTag = this.Tag;
			string initialTag = (string)ViewState["tag"];

			if (currentTag.Equals(initialTag) == false)
			{
				ViewState["tag"] = currentTag;
			}

			string currentUrl = this.Url;
			string initialUrl = (string)ViewState["url"];

			if (currentUrl.Equals(initialUrl) == false)
			{
				ViewState["url"] = currentUrl;
			}

			string currentId = this.Id;
			string initialId = (string)ViewState["id"];

			if (currentId.Equals(initialId) == false)
			{
				ViewState["id"] = currentId;
			}

			bool currentPostBack = this.PostBack;
			bool initialPostBackTyped = (ViewState["postBack"] == null) ? false : (bool)ViewState["postBack"];

			if (currentPostBack != initialPostBackTyped)
			{
				ViewState["postBack"] = currentPostBack;
			}

			bool currentSelected = this.Selected;
			bool initialSelectedTyped = (ViewState["selected"] == null) ? false : (bool)ViewState["selected"];

			if (currentSelected != initialSelectedTyped)
			{
				ViewState["selected"] = currentSelected;
			}

			int currentRightPadding = this.RightPadding;
			int initialRightPadding = (ViewState["rightPadding"] == null) ? 0 : (int)ViewState["rightPadding"];

			if (currentRightPadding.Equals(initialRightPadding) == false)
			{
				ViewState["rightPadding"] = currentRightPadding;
			}

			int currentLeftPadding = this.LeftPadding;
			int initialLeftPadding = (ViewState["leftPadding"] == null) ? 0 : (int)ViewState["leftPadding"];

			if (currentLeftPadding.Equals(initialLeftPadding) == false)
			{
				ViewState["leftPadding"] = currentLeftPadding;
			}

			if (_viewState != null)
			{
				return ((IStateManager)_viewState).SaveViewState();
			}
			return null;
		}
		void IStateManager.TrackViewState()
		{
			if (_text.Length > 0)
				ViewState["text"] = _text;

			if (_tag.Length > 0)
				ViewState["tag"] = _tag;

			if (_url.Length > 0)
				ViewState["url"] = _url;

			if (_id.Length > 0)
				ViewState["id"] = _id;

			if (!_postBack)
				ViewState["postBack"] = !_postBack;

			if (_rightPadding > 0)
				ViewState["rightPadding"] = _rightPadding;

			if (_leftPadding > 0)
				ViewState["leftPadding"] = _leftPadding;

			_isTrackingViewState = true;

			if (_viewState != null)
			{
				((IStateManager)_viewState).TrackViewState();
			}
		}

		#endregion

		#region Properties

		//[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		//public string ClientId
		//{
		//	get { return this._clientId; }
		//	set { this._clientId = value; }
		//}

		[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		[DefaultValue(""), Description("The Id to use for the tab."), Bindable(false),]
		public string Id
		{
			get { return this._id; }
		}

		[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		internal string Id_internal
		{
			get { return this._id; }
			set { this._id = value; }
		}

		[DefaultValue(""), Description("The tag to use for the tab."), Bindable(false),]
		public string Tag
		{
			get { return this._tag; }
			set { this._tag = value; }
		}

		[DefaultValue(""), Description("The text to use for the tab."), Bindable(false),]
		public string Text
		{
			get { return this._text; }
			set { this._text = value; }
		}

		[DefaultValue(""), Description("The Url to use for the tab."), Bindable(false),]
		[Editor(typeof(UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string Url
		{
			get { return this._url; }
			set { this._url = value; }
		}

		//[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		//[DefaultValue(""), Description("The width of the tab."), Bindable(false),]
		//public int Width
		//{
		//	get { return this._width; }
		//	set { this._width = value; }
		//}

		[DefaultValue(false), Description("Indicates the currently active tab."), Bindable(false),]
		public bool Selected
		{
			get { return this._selected; }
			set { this._selected = value; }
		}

		[DefaultValue(true), Description("Indicates if post back is enabled for this tab."), Bindable(false),]
		public bool PostBack
		{
			get { return this._postBack; }
			set { this._postBack = value; }
		}

		[Category("Appearance"), DefaultValue(0), Description("The amount of padding right of the tab text."), Bindable(false),]
		public int RightPadding
		{
			get { return this._rightPadding; }
			set { this._rightPadding = value; }
		}

		[Category("Appearance"), DefaultValue(0), Description("The amount of padding left of the tab text."), Bindable(false),]
		public int LeftPadding
		{
			get { return this._leftPadding; }
			set { this._leftPadding = value; }
		}

		#endregion
	}
}
