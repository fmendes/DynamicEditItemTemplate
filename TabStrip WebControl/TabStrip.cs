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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Windows.Forms.Design;
using SCS.Web.UI.WebControls.Design;

namespace SCS.Web.UI.WebControls
{
	enum ImageRepeatType { NotSpecified = 0, RepeatXBottom, NoRepeatLeftTop, NoRepeatRightTop }

	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]	
	[DefaultProperty("Items"), DefaultEvent("TabItemClicked"), ToolboxData("<{0}:TabStrip runat=server></{0}:TabStrip>")]
	[Designer(typeof(SCS.Web.UI.WebControls.Design.TabStripDesigner)), ParseChildren(true), PersistChildren(false)]
	[Editor(typeof(SCS.Web.UI.WebControls.Design.TabImageBuilder), typeof(ComponentEditor))]
	
	public class TabStrip : Control, INamingContainer, IPostBackEventHandler
	{
		#region Fields
		private TabItemCollection _tabItems = null;

		private Style _tabTextStyle = null;
		private Style _tabSelectedTextStyle = null;
		private Style _tabTextHoverStyle = null;

		private TabItem _selectedTabItem = null;
		private TabItem _lastSelectedTabItem = null;

		private int _leftImageWidth;

		private bool _stylesheetWritten = false;
		private bool _passIdOnQuerystring = false;

		internal string _designTimeBasePath = string.Empty;

		private static string _stylesheetHtml = "<style>\r\n{0}</style>";
		private static string _imageResourcePrefix = "SCS.Web.UI.WebControls.Resources.Images.";

		private int _defaultLeftImageWidth = 7;
		#endregion

		public delegate void ItemClickedHandler(object sender, TabItemClickedEventArgs e);
		private static readonly object tabItemClickedEvent = new object();

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
//#if DEBUG
//			string[] resources = this.GetType().Assembly.GetManifestResourceNames();
//			foreach (string resourceName in resources)
//			{
//				Debug.WriteLine(resourceName);
//			} 
//#endif
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!this.DesignMode)
			{
				if (!Page.IsPostBack || !this.EnableViewState)
				{
					// set selected tab item using tab id passed on query string
					if (Page.Request["tabid"] != null && !Page.IsPostBack)
					{
						this.SetSelectedItemFromQueryString();
					}
				}
			}
		}
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
					   
			if (!Page.IsPostBack)
				this.SetTabItemIds();

			if (Page.Header != null)
			{
				Literal styleHolder = new Literal();
				styleHolder.Text = string.Format(_stylesheetHtml, this.BuildStyles());

				Page.Header.Controls.Add(styleHolder);
				_stylesheetWritten = true;
			}
		}
		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			this.WriteTabs(writer);
		}

		internal void WriteTabs(HtmlTextWriter writer)
		{
			// start outer div
			writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.Write(Environment.NewLine);

			// if stylesheet was unable to be written to the header, write it here
			if (!_stylesheetWritten)
				writer.Write(string.Format(_stylesheetHtml, this.BuildStyles()));

			// start unordered list
			writer.RenderBeginTag(HtmlTextWriterTag.Ul);
			writer.Write(Environment.NewLine);

			// write tab left edge			
			//writer.AddAttribute(HtmlTextWriterAttribute.Class, "edge");
			//writer.RenderBeginTag(HtmlTextWriterTag.Li);

			//writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/spacer.gif");
			//writer.AddAttribute(HtmlTextWriterAttribute.Width, "1");
			//writer.RenderBeginTag(HtmlTextWriterTag.Img);
			//writer.RenderEndTag();
			//writer.RenderEndTag();
			writer.Write(Environment.NewLine);

			int ctr = 1;
			foreach (TabItem tabItem in Items)
			{
				// this is for when the id is empty (when the TabStrip is inside of a formview)
				if( string.IsNullOrEmpty( tabItem.Id ) )
					tabItem.Id_internal = string.Format( "{0}Item{1}", this.ClientID, ctr.ToString() );

				// write tab			   
				writer.AddAttribute(HtmlTextWriterAttribute.Id, tabItem.Id);

				if (tabItem.Selected)
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "linkOn");

				//if (tabItem.Width > 0)
				//	writer.AddStyleAttribute(HtmlTextWriterStyle.Width, tabItem.Width.ToString());

				if (tabItem.PostBack)
					writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "javascript:this.firstChild.click();");

				writer.RenderBeginTag(HtmlTextWriterTag.Li);

				// write tab link				
				writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "Link" + ctr.ToString());

				if (!string.IsNullOrEmpty(tabItem.Url) && !tabItem.PostBack)
				{
					string url = tabItem.Url;

					if (_passIdOnQuerystring && url.Length > 0)
					{
						url += (url.IndexOf("?") == -1) ? "?tabid=" : "&tabid=";
						url += tabItem.Id.Replace(":", "__");
					}
					writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
				}
				else if (tabItem.PostBack)
				{
					// this is a bit redundant, but neccessary for the css anchor hovering to work correctly.
					writer.AddAttribute(HtmlTextWriterAttribute.Href, 
						Page.ClientScript.GetPostBackClientHyperlink(this, tabItem.Id));
				}

				string padding = string.Empty;

				// write right and left padding
				if (tabItem.RightPadding > 0)
				{
					padding = Convert.ToString(_leftImageWidth + tabItem.RightPadding) + "px";
					writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, padding);
				}

				if (tabItem.LeftPadding > 0)
				{
					padding = Convert.ToString(tabItem.LeftPadding) + "px";
					writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, padding);
				}

				writer.RenderBeginTag(HtmlTextWriterTag.A);

				// write tab label
				writer.Write(tabItem.Text);

				// close list item and link
				writer.RenderEndTag();
				writer.RenderEndTag();
				writer.Write(Environment.NewLine);

				ctr++;
			}
			// close tab ul tag
			writer.RenderEndTag();
			writer.Write(Environment.NewLine);

			// close tab div tag
			writer.RenderEndTag();
			writer.Write(Environment.NewLine);
		}
		internal string BuildStyles()
		{
			StringBuilder style = new StringBuilder(1024);

			string imagePath = this.GetImagePath(this.BottomImage);

			if (string.IsNullOrEmpty(imagePath))
				imagePath = Page.ClientScript.GetWebResourceUrl(this.GetType(), _imageResourcePrefix + "tab_underline.gif");
			
			// control wrapper style
			//

			style.Append(this.GetStyleOpen("#" + this.ClientID));
			style.Append(FormatStyleAttribute("cursor", "hand"));
			style.Append(FormatStyleAttribute("float", "left"));

			if (this.Width.Length > 0)
				style.Append(FormatStyleAttribute("width", this.Width));

			style.Append(FormatStyleAttribute("background",
				this.FormatImageBackgroundUrl(imagePath, ImageRepeatType.RepeatXBottom)));
			style.Append(FormatStyleAttribute("line-height", "normal"));
			style.Append(FormatStyleAttribute("margin", "0"));
			style.Append(this.GetStyleClose());

			// whole list style
			// 

			style.Append(this.GetStyleOpen("#" + this.ClientID + " ul"));
			style.Append(FormatStyleAttribute("padding", "0"));
			style.Append(FormatStyleAttribute("margin", "0"));
			style.Append(FormatStyleAttribute("list-style", "none"));
			style.Append(this.GetStyleClose());

			imagePath = this.GetImagePath(this.LeftTabImage);

			if (string.IsNullOrEmpty(imagePath))
			{
				imagePath = Page.ClientScript.GetWebResourceUrl(this.GetType(), _imageResourcePrefix + "tab_left.gif");
				_leftImageWidth = _defaultLeftImageWidth;
			}
			else
			{
				_leftImageWidth = this.GetLeftImageWidth(imagePath);
			}
			// tab style
			// 

			style.Append(this.GetStyleOpen("#" + this.ClientID + " li"));
			style.Append(FormatStyleAttribute("float", "left"));
			style.Append(FormatStyleAttribute("background",
				this.FormatImageBackgroundUrl(imagePath, ImageRepeatType.NoRepeatLeftTop)));
			style.Append(FormatStyleAttribute("margin", "0"));
			style.Append(FormatStyleAttribute("padding", string.Format("0 0 0 {0}px", _leftImageWidth)));
			style.Append(this.GetStyleClose());

			//imagePath = this.GetImagePath(this.LeftEdgeImage, DesignTimeBasePath);

			//style.Append(this.GetStyleOpen("#" + this.UniqueID + " li.edge"));
			//style.Append(FormatStyleAttribute("background",
			//	this.FormatImageBackgroundUrl(imagePath, ImageRepeatType.NoRepeatLeftTop)));
			//style.Append(FormatStyleAttribute("padding", "5px 0"));
			//style.Append(this.GetStyleClose());

			// anchor style
			// 

			imagePath = this.GetImagePath(this.RightTabImage);

			if (string.IsNullOrEmpty(imagePath))
				imagePath = Page.ClientScript.GetWebResourceUrl(this.GetType(), _imageResourcePrefix + "tab_right.gif");

			style.Append(this.GetStyleOpen("#" + this.ClientID + " a"));
			style.Append(FormatStyleAttribute("float", "left"));
			style.Append(FormatStyleAttribute("display", "block"));

			string styleList = this.GetCssStyle(_tabTextStyle);
			if (styleList.Trim().Length > 0)
				style.Append(this.FormatStyleList(styleList));
			
			style.Append(FormatStyleAttribute("background",
				this.FormatImageBackgroundUrl(imagePath, ImageRepeatType.NoRepeatRightTop)));
			style.Append(FormatStyleAttribute("padding", string.Format("4px {0}px 4px {1}px", 
				_leftImageWidth + LeftPadding, RightPadding)));
			style.Append(this.GetStyleClose());

			// anchor hover style
			//

			style.Append(this.GetStyleOpen("#" + this.ClientID + " a:hover"));

			styleList = this.GetCssStyle(_tabTextHoverStyle);
			if (styleList.Trim().Length > 0)
				style.Append(this.FormatStyleList(styleList));

			//style.Append(FormatStyleAttribute("text-decoration", "underline"));
			//style.Append(FormatStyleAttribute("color", "#4747ff"));
			style.Append(this.GetStyleClose());

			imagePath = this.GetImagePath(this.SelectedLeftTabImage);
			
			if (string.IsNullOrEmpty(imagePath))
				imagePath = Page.ClientScript.GetWebResourceUrl(this.GetType(), _imageResourcePrefix + "tab_left_on.gif");

			// selected tab style
			//

			style.Append(this.GetStyleOpen("#" + this.ClientID + " .linkOn"));
			style.Append(FormatStyleAttribute("background-image",
				this.FormatImageBackgroundUrl(imagePath, ImageRepeatType.NotSpecified)));
			style.Append(FormatStyleAttribute("border-width", "0"));
			style.Append(this.GetStyleClose());

			imagePath = this.GetImagePath(this.SelectedRightTabImage);
			
			if (string.IsNullOrEmpty(imagePath))
				imagePath = Page.ClientScript.GetWebResourceUrl(this.GetType(), _imageResourcePrefix + "tab_right_on.gif");

			// selected anchor tab style
			//

			style.Append(this.GetStyleOpen("#" + this.ClientID + " .linkOn a"));
			style.Append(FormatStyleAttribute("background-image",
				this.FormatImageBackgroundUrl(imagePath, ImageRepeatType.NotSpecified)));

			styleList = this.GetCssStyle(_tabSelectedTextStyle);
			if (styleList.Trim().Length > 0)
				style.Append(this.FormatStyleList(styleList));

			//style.Append(FormatStyleAttribute("color", "#333"));
			style.Append(FormatStyleAttribute("padding-bottom", "5px"));
			style.Append(this.GetStyleClose());

			return style.ToString();
		}
	   
		private string FormatStyleAttribute(string name, string value)
		{
			return string.Format("\t{0}:{1};\r\n", name, value);
		}
		private string FormatStyleList(string styleList)
		{
			string styles = "\t" + styleList.Replace(";", ";\r\n\t");
			styles = styles.Substring(0, styles.Length - 1);
			
			return styles;
		}
		private string GetStyleOpen(string id)
		{
			return string.Format("{0} {1}\r\n", id, "{");
		}
		private string GetStyleClose()
		{
			return "}\r\n\r\n";
		}

		private string FormatImageBackgroundUrl(string path, ImageRepeatType repeatType)
		{
			string repeatInstruction = string.Empty;

			switch (repeatType)
			{
				case ImageRepeatType.NoRepeatLeftTop:
					repeatInstruction = "no-repeat left top";
					break;

				case ImageRepeatType.NoRepeatRightTop:
					repeatInstruction = "no-repeat right top";
					break;

				case ImageRepeatType.RepeatXBottom:
					repeatInstruction = "repeat-x bottom";
					break;
			}

			return string.Format("url(\"{0}\") {1}", path, repeatInstruction);
		}
		private string GetImagePath(string url)
		{
			string path = string.Empty;

			if (!this.DesignMode)
			{
				path = Page.ResolveUrl(url);
			}
			else
			{
				if (!string.IsNullOrEmpty(url))
				{
					if (url.StartsWith("~"))
						url = url.Substring(1);

					if (url.StartsWith("/"))
						url = url.Substring(1);

					path = DesignTimeBasePath + url.Replace("/", @"\");
				}
			}
			return path;
		}
		private string GetCssStyle(Style style)
		{
			if (style == null)
				return this.GetDefaultStyle(style);

			StringBuilder sb = new StringBuilder(256);
			Color c;

			c = style.ForeColor;
			if (!c.IsEmpty)
			{
				sb.Append("color:");
				sb.Append(ColorTranslator.ToHtml(c));
				sb.Append(";");
			}
			else
			{
				sb.Append("color:black;");
			}

			FontInfo fi = style.Font;
			string s;

			s = fi.Name;
			if (s.Length != 0)
			{
				sb.Append("font-family:'");
				sb.Append(s);
				sb.Append("';");
			}
			if (fi.Bold)
			{
				sb.Append("font-weight:bold;");
			}
			else
			{
				sb.Append("font-weight:normal;");
			}

			if (fi.Italic)
			{
				sb.Append("font-style:italic;");
			}
			else
			{
				sb.Append("font-style:normal;");
			}

			s = String.Empty;
			if (fi.Underline)
				s += "underline";

			if (fi.Strikeout)
				s += " line-through";

			if (fi.Overline)
				s += " overline";

			if (s.Length != 0)
			{
				sb.Append("text-decoration:");
				sb.Append(s);
				sb.Append(';');
			}
			else
			{
				sb.Append("text-decoration:none;");
			}

			FontUnit fu = fi.Size;
			if (fu.IsEmpty == false)
			{
				sb.Append("font-size:");
				sb.Append(fu.ToString(CultureInfo.InvariantCulture));
				sb.Append(';');
			}

			Unit u = style.Width;
			if (!u.IsEmpty)
			{
				sb.Append("width:");
				sb.Append(u.ToString());
				sb.Append(';');
			}

			u = style.Height;
			if (!u.IsEmpty)
			{
				sb.Append("height:");
				sb.Append(u.ToString());
				sb.Append(';');
			}

			return sb.ToString();
		}
		private string GetDefaultStyle(Style style)
		{
			StringBuilder sb = new StringBuilder(256);
			
			sb.Append("color:black;");
			sb.Append("text-decoration:none;");
			sb.Append("font-size:small;");
			
			return sb.ToString();
		}

		private int GetLeftImageWidth(string path)
		{
			Bitmap leftImage = null;

			try
			{
				if (Context != null && Context is HttpContext)
				{
					leftImage = new Bitmap(Page.MapPath(path));
				}
				else
				{
					leftImage = new Bitmap(path);
				}
				return leftImage.Width;
			}
			catch (ArgumentException)
			{
				return _defaultLeftImageWidth; // use default image width
			}
			finally
			{
				if (leftImage != null)
					leftImage.Dispose();
			}
		}
		private void SetSelectedItem(string id)
		{
			int ctr = 1;
			foreach (TabItem tabItem in _tabItems)
			{
				tabItem.Id_internal = this.ClientID + "Item" + ctr.ToString();

				if (tabItem.Selected)
				{
					this._lastSelectedTabItem = tabItem;
					tabItem.Selected = false;
				}

				if (tabItem.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase))
				{
					tabItem.Selected = true;
					this._selectedTabItem = tabItem;
				}
				else
				{
					tabItem.Selected = false;
				}		

				ctr++;
			}
		}
		private void SetSelectedItemFromQueryString()
		{
			string id = string.Empty;

			if (Page.Request["tabid"] != null)
			{
				id = Page.Request["tabid"].Replace("__", ":");
			}

			int ctr = 1;
			foreach (TabItem tabItem in _tabItems)
			{
				tabItem.Id_internal = this.ClientID + "Item" + ctr.ToString();
			
				if (tabItem.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase))
				{
					tabItem.Selected = true;
					this._selectedTabItem = tabItem;
				}
				else
				{
					tabItem.Selected = false;
				}				

				ctr++;
			}
		}
		private void SetTabItemIds()
		{
			int ctr = 1;
			foreach (TabItem tabItem in Items)
			{
				if (string.IsNullOrEmpty(tabItem.Id))
				{
					tabItem.Id_internal = this.ClientID + "Item" + ctr.ToString();
					ctr++;
				}
			}
		}

		#region INamingContainter Implementation (View State)

		protected override object SaveViewState()
		{
			object[] state = new object[5];

			state[0] = base.SaveViewState();
			state[1] = (_tabItems != null) ? ((IStateManager)_tabItems).SaveViewState() : null;
			state[2] = (_tabTextStyle != null) ? ((IStateManager)_tabTextStyle).SaveViewState() : null;
			state[3] = (_tabTextHoverStyle != null) ? ((IStateManager)_tabTextHoverStyle).SaveViewState() : null;
			state[4] = (_tabSelectedTextStyle != null) ? ((IStateManager)_tabSelectedTextStyle).SaveViewState() : null;
					   
			for (int i = 0; i < 5; i++)
				if (state[i] != null)
					return state;

			// Another perfomance optimization. If no modifications were made to any
			// properties from their persisted state, the view state for this control
			// is null. Returning null, rather than an array of null values helps
			// minimize the view state significantly.
			return null;
		}
		protected override void LoadViewState(object savedState)
		{
			object baseState = null;
			object[] state = null;

			if (savedState != null)
			{
				state = (object[])savedState;
				baseState = state[0];
			}

			// Always call the base class, even if the state is null, so
			// the base class gets a chance to fully implement its LoadViewState
			// functionality.
			base.LoadViewState(baseState);

			if (state == null)
				return;

			if (state[1] != null)
				((IStateManager)Items).LoadViewState(state[1]);

			if (state[2] != null)
				((IStateManager)TabTextStyle).LoadViewState(state[2]);

			if (state[3] != null)
				((IStateManager)TabTextHoverStyle).LoadViewState(state[3]);

			if (state[4] != null)
				((IStateManager)TabSelectedTextStyle).LoadViewState(state[4]);
		}
		protected override void TrackViewState()
		{
			base.TrackViewState();

			if (_tabItems != null)
				((IStateManager)_tabItems).TrackViewState();

			if (_tabTextStyle != null)
				((IStateManager)_tabTextStyle).TrackViewState();

			if (_tabTextHoverStyle != null)
				((IStateManager)_tabTextHoverStyle).TrackViewState();

			if (_tabSelectedTextStyle != null)
				((IStateManager)_tabSelectedTextStyle).TrackViewState();
		}

		#endregion

		#region IPostBackEventHandler Implementation

		public void RaisePostBackEvent(string eventArgument)
		{
			this.SetSelectedItem(eventArgument);

			if (this._selectedTabItem != null)
			{
				TabItemClickedEventArgs eventArgs =
					new TabItemClickedEventArgs(_selectedTabItem, _lastSelectedTabItem);

				OnTabItemClicked(eventArgs);

				if (eventArgs.Cancel)
				{
					this._selectedTabItem.Selected = false;

					if (this._lastSelectedTabItem != null)
						this._lastSelectedTabItem.Selected = true;

					this._selectedTabItem = this._lastSelectedTabItem;
				}
			}
		}
		
		#endregion

		#region Event

		private void OnTabItemClicked(TabItemClickedEventArgs e)
		{
			ItemClickedHandler tabItemClickedEventDelegate = (ItemClickedHandler)Events[tabItemClickedEvent];

			if (tabItemClickedEventDelegate != null)
			{
				tabItemClickedEventDelegate(this, e);
			}
		}	   
		public event ItemClickedHandler TabItemClicked
		{
			add
			{
				Events.AddHandler(tabItemClickedEvent, value);
			}
			remove
			{
				Events.RemoveHandler(tabItemClickedEvent, value);
			}
		}

		#endregion

		#region Properties

		internal string DesignTimeBasePath
		{
			get { return this._designTimeBasePath; }
			set { this._designTimeBasePath = value; }
		}

		#region Appearance

		[Category("Appearance"), Description("The color used behind the tabs."), Bindable(false),]
		[EditorAttribute(typeof(System.Drawing.Design.ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public Color BackColor
		{
			get
			{
				object o = ViewState["BackColor"];
				return ((o == null) ? new Color() : (Color)o);
			}
			set
			{
				ViewState["BackColor"] = value;
			}
		}

		[Category("Appearance"), Description("The image that makes up the left side of the tab."), Bindable(true)]
		[Editor(typeof(ImageUrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string LeftTabImage
		{
			get
			{
				object o = ViewState["LeftTabImage"];
				return ((o == null) ? string.Empty : (string)o);
			}
			set
			{
				ViewState["LeftTabImage"] = value;
			}
		}

		[Category("Appearance"), Description("The image that makes up the right side of the tab."), Bindable(true)]
		[Editor(typeof(ImageUrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string RightTabImage
		{
			get
			{
				object o = ViewState["RightTabImage"];
				return ((o == null) ? string.Empty : (string)o);
			}
			set
			{
				ViewState["RightTabImage"] = value;
			}
		}

		[Category("Appearance"), Description("The image that makes up the left side of the selected tab."), Bindable(true)]
		[Editor(typeof(ImageUrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectedLeftTabImage
		{
			get
			{
				object o = ViewState["SelectedLeftTabImage"];
				return ((o == null) ? string.Empty : (string)o);
			}
			set
			{
				ViewState["SelectedLeftTabImage"] = value;
			}
		}

		[Category("Appearance"), Description("The image that makes up the right side of the selected tab."), Bindable(true)]
		[Editor(typeof(ImageUrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectedRightTabImage
		{
			get
			{
				object o = ViewState["SelectedRightTabImage"];
				return ((o == null) ? string.Empty : (string)o);
			}
			set
			{
				ViewState["SelectedRightTabImage"] = value;
			}
		}

		[Category("Appearance"), Description("The image that makes up the bottom of the TabStrip."), Bindable(true)]
		[Editor(typeof(ImageUrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string BottomImage
		{
			get
			{
				object o = ViewState["BottomImage"];
				return ((o == null) ? string.Empty : (string)o);
			}
			set
			{
				ViewState["BottomImage"] = value;
			}
		}

		[Category("Appearance"), DefaultValue(4), Description("The amount of padding above tab text."), Bindable(true),]
		public int TopPadding
		{
			get
			{
				object o = ViewState["topPadding"];
				return ((o == null) ? 4 : (int)o);
			}
			set
			{
				ViewState["topPadding"] = value;
			}
		}

		[Category("Appearance"), DefaultValue(4), Description("The amount of padding below tab text."), Bindable(true),]
		public int BottomPadding
		{
			get
			{
				object o = ViewState["bottomPadding"];
				return ((o == null) ? 4 : (int)o);
			}
			set
			{
				ViewState["bottomPadding"] = value;
			}
		}

		[Category("Appearance"), DefaultValue(0), Description("The amount of padding right of the tab text."), Bindable(true),]
		public int RightPadding
		{
			get
			{
				object o = ViewState["rightPadding"];
				return ((o == null) ? 0 : (int)o);
			}
			set
			{
				ViewState["rightPadding"] = value;
			}
		}

		[Category("Appearance"), DefaultValue(0), Description("The amount of padding left of the tab text."), Bindable(true),]
		public int LeftPadding
		{
			get
			{
				object o = ViewState["leftPadding"];
				return ((o == null) ? 0 : (int)o);
			}
			set
			{
				ViewState["leftPadding"] = value;
			}
		}

		[Category("Appearance"), DefaultValue("100%"), Description("The width of the TabStrip."), Bindable(true),]
		public string Width
		{
			get
			{
				object o = ViewState["width"];
				return ((o == null) ? "100%" : (string)o);
			}
			set
			{
				ViewState["width"] = value;
			}
		}

		#endregion

		#region Behavior
		[Bindable(true), Category("Behavior"), DefaultValue(false), Description("Passes selected tab item Id on query string for setting current tab item.")]
		public bool PassIdOnQueryString
		{
			get
			{
				return this._passIdOnQuerystring;
			}
			set
			{
				this._passIdOnQuerystring = value;
			}
		}
		#endregion

		#region Misc
		[Category("Misc"), Description("The collection of tabs."), Bindable(false),]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Editor(typeof(TabItemCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public TabItemCollection Items
		{
			get
			{
				if (_tabItems == null)
				{
					_tabItems = new TabItemCollection();
					if (IsTrackingViewState)
					{
					   ((IStateManager)_tabItems).TrackViewState();
					}
				}
				return _tabItems;
			}
		}
	   
		#endregion

		#region Style

		[Category("Style"), Description("The style applied to the tab text.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual Style TabTextStyle
		{
			get
			{
				if (_tabTextStyle == null)
				{
					_tabTextStyle = new Style();
					if (IsTrackingViewState)
					{
						((IStateManager)_tabTextStyle).TrackViewState();
					}
				}
				return _tabTextStyle;
			}
		}

		[Category("Style"), Description("The style applied to the tab text.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual Style TabTextHoverStyle
		{
			get
			{
				if (_tabTextHoverStyle == null)
				{
					_tabTextHoverStyle = new Style();
					if (IsTrackingViewState)
					{
						((IStateManager)_tabTextHoverStyle).TrackViewState();
					}
				}
				return _tabTextHoverStyle;
			}
		}

		[Category("Style"), Description("The style applied to the tab text.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual Style TabSelectedTextStyle
		{
			get
			{
				if (_tabSelectedTextStyle == null)
				{
					_tabSelectedTextStyle = new Style();
					if (IsTrackingViewState)
					{
						((IStateManager)_tabSelectedTextStyle).TrackViewState();
					}
				}
				return _tabSelectedTextStyle;
			}
		}

		#endregion
		
		#endregion
	}
}
