using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.Drawing;
using System.Globalization;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace SCS.Web.UI.WebControls.Design
{
	public class TabItemCollectionEditor : CollectionEditor
	{
		private Type[] types;

		public TabItemCollectionEditor(Type type) : base(type)
		{
			types = new Type[] { typeof(TabItem) };
		}


		protected override Type[] CreateNewItemTypes()
		{
			return types;
		}
	}
}