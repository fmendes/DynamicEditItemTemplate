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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.WebControls;
using EnvDTE;

namespace SCS.Web.UI.WebControls.Design
{
	public class TabStripDesigner : ControlDesigner
	{
		private DesignerVerbCollection designerVerbs;

		public override void Initialize(IComponent component)
		{
			if (!(component is TabStrip))
			{
				throw new ArgumentException("Component must be a TabStrip control.", "component");
			}
			base.Initialize(component);
		}
		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (designerVerbs == null)
				{
					designerVerbs = new DesignerVerbCollection();
					designerVerbs.Add(new DesignerVerb("TabStrip Image Builder...", new EventHandler(this.OnPropertyBuilder)));
				}

				return designerVerbs;
			}
		}
		public override string GetDesignTimeHtml()
		{
			string html = string.Empty;

			try
			{
				TabStrip tabStrip = (TabStrip)Component;

				if (tabStrip.Items.Count == 0)
				{
					html = this.GetEmptyDesignTimeHtml();
				}
				else
				{
					StringWriter stringWriter = new StringWriter();

					tabStrip.DesignTimeBasePath
						= (string)DTEHelper.GetCurrentProjectPropertyValue(ProjectPropertyType.LocalPath);
				   
					HtmlTextWriter writer = new HtmlTextWriter(stringWriter);					
					tabStrip.WriteTabs(writer); 

					html += stringWriter.ToString();

					//html += "<br/><br/>" + tabStrip.DesignTimeBasePath;
				}
			}
			catch (Exception ex)
			{
				html = this.GetErrorDesignTimeHtml(ex);
			}

			return html;
		}

		protected override string GetErrorDesignTimeHtml(Exception e)
		{
			return "Error: " + base.GetErrorDesignTimeHtml(e);
		}
		protected override string GetEmptyDesignTimeHtml()
		{
			TabStrip tabStrip = (TabStrip)Component;
			tabStrip.Items.Add(new TabItem("Tab 1"));			
			return this.GetDesignTimeHtml();
		}

		private void OnPropertyBuilder(object sender, EventArgs e)
		{
			TabImageBuilder imageBuilder = new TabImageBuilder();
			imageBuilder.EditComponent(Component);
		}	   
	}
}
