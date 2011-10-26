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
using System.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SCS.Web.UI.WebControls.Design
{
	public partial class TabImageBuilder : WindowsFormsComponentEditor
	{
		public override bool EditComponent(ITypeDescriptorContext context, object component, IWin32Window owner)
		{
			TabStrip tabStrip = component as TabStrip;
		   
			if (tabStrip == null)			
				throw new ArgumentException("Component must be a TabStrip object.", "component");

			IServiceProvider site = tabStrip.Site;
			IComponentChangeService changeService = null;

			DesignerTransaction transaction = null;
			bool changed = false;

			try
			{
				if (site != null)
				{
					IDesignerHost designerHost = (IDesignerHost)site.GetService(typeof(IDesignerHost));
					transaction = designerHost.CreateTransaction("Image Builder");

					changeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
					
					if (changeService != null)
					{
						try
						{
							changeService.OnComponentChanging(tabStrip, null);
						}
						catch (CheckoutException ex)
						{
							if (ex == CheckoutException.Canceled)
								return false;
							throw ex;
						}
					}
				}

				try
				{
					TabImageBuilderForm form = new TabImageBuilderForm(tabStrip);
					
					if (form.ShowDialog(owner) == DialogResult.OK)
					{
						changed = true;
					}
				}
				finally
				{
					if (changed && changeService != null)
					{
						changeService.OnComponentChanged(tabStrip, null, null, null);
					}
				}
			}
			finally
			{
				if (transaction != null)
				{
					if (changed)
					{
						transaction.Commit();
					}
					else
					{
						transaction.Cancel();
					}
				}
			}

			return changed;
		}
	}
}