//#define VERSION4
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using SCSWeb = SCS.Web.UI.WebControls;
using System.Collections.Specialized;

namespace Maintenance
{
	public class DynamicFormViewTemplate : ITemplate, INamingContainer, IBindableTemplate
	{
		ListItemType _type;
		DataColumnCollection _dccol;
		private System.Collections.Hashtable Fields;
		public bool ShowEdit = false;
		private int _iEditTabOrder = 0;
		private SCSWeb.TabStrip _TabStrip;
		public MultiView _TabMultiView;
		public OrderedDictionary objOD;

		// dropdownlist parameters
		public List<string> DataSourceTypeName;
		public List<string> DataSourceSelectMethod;
		public List<string> DropDownListColumns;
		public List<string> DropDownTextField;
		public List<string> DropDownValueField;
		public List<object> DropDownDataSource;

		public int NbrRows = 15;
		public int PixelsPerCharacter = 4; // 4 for smaller and 6 for small (CSS)
		public string CssClassTable = "FormViewTable";
		public string CssClassTableCell = "FormViewCell";
		public string CssClassPrimaryKey = "PrimaryKeyCell";
		public string CssClassEditControl = "FormViewEditControl";
		public string CssClassNegativeNbrs = "NegativeNumber";
		public string CssClassLateDates = "LateDate";
		public string CssClassUnselectedTab = "UnselectedTab";
		public string CssClassSelectedTab = "SelectedTab";
		public string DateFormat = "MM/dd/yyyy HH:mm:ss";
		public string DecimalFormat = "#0.00";
		public string IntegerFormat = "#0";
		public int TabInterval = 5;
		public bool GroupInTable = false;
		public bool GroupInTabs = true;
		public string ListOfTabs = "";

		public IOrderedDictionary ExtractValues(Control container)
		{
			// this is to make e.NewValues populated during ItemUpdating

			// only create the dictionary if not yet populated
			if( objOD == null )
				objOD = new OrderedDictionary();

			View objCurrentView = _TabMultiView.Views[_TabMultiView.ActiveViewIndex];
			CopyValuesToFromCurrentTab(objCurrentView, objCurrentView.ID, "FROM");

			return objOD;
		}

		public void TabStrip_TabItemClicked(object sender, SCSWeb.TabItemClickedEventArgs e)
		{
			if( objOD == null )
				objOD = new OrderedDictionary();

			// store the values from the current tab
			View objCurrentView = GetCurrentView();
			CopyValuesToFromCurrentTab(objCurrentView, objCurrentView.ID, "FROM");

			// select the view in the multiview that corresponds to the selected tab
			foreach( View objView in _TabMultiView.Views )
				if (objView.ID.Equals(e.SelectedTabItem.Text))
				{
					_TabMultiView.SetActiveView(objView);

					CopyValuesToFromCurrentTab(objView, objView.ID, "TO" );

					break;
				}			
		}

		public View GetCurrentView()
		{
			View objCurrentView = _TabMultiView.Views[_TabMultiView.ActiveViewIndex];
			return objCurrentView;
		}

		private void CopyValuesToFromCurrentTab(Control container, string strCategory, string strDirection )
		{
			// restore values in the selected tab
			foreach (DataColumn objDC in _dccol)
			{
				string strColumnName = objDC.ColumnName;

				// only retrieve values for columns that belong to the current tab
				// skip the rest
				if (!objDC.Namespace.Equals(strCategory))
					continue;

				// if column is not in dictionary, add it and skip to the next column
				if (!objOD.Contains(strColumnName))
				{
					objOD.Add(strColumnName, "");

					// since the column has just been added to the dictionary, we can just skip IF 
					// the direction is from the dictionary TO fields
					if (strDirection.Equals("TO"))
						continue;
				}

				// find the column's respective control to store/retrieve the values
				object objControl = container.FindControl("txt" + strColumnName);
				if (objControl != null)
				{
					TextBox objTB = (TextBox)objControl;

					// TO = from dictionary to field
					// FROM = from field to dictionary
					if (strDirection.Equals("TO"))
						objTB.Text = objOD[strColumnName].ToString(); //.Replace("<", "&lt;").Replace(">", "&gt;");
					else
						objOD[strColumnName] = objTB.Text;
				}
				else
				{
					// couldn't find it as text box, now try finding it as label
					objControl = container.FindControl("lbl" + strColumnName);
					if (objControl == null)
					{
						// try finding column as dropdownlist
						objControl = container.FindControl("ddl" + strColumnName);

						// skip it, unknown/unhandled control type
						if (objControl == null)
							continue;

						DropDownList objDdl = (DropDownList)objControl;

						// TO = from dictionary to field
						// FROM = from field to dictionary
						if (strDirection.Equals("TO"))
							objDdl.SelectedValue = objOD[strColumnName].ToString();
						else
							objOD[strColumnName] = objDdl.SelectedValue;

						continue;

					}

					Label objLbl = (Label)objControl;

					// TO = from dictionary to field
					// FROM = from field to dictionary
					if (strDirection.Equals("TO"))
						objLbl.Text = objOD[strColumnName].ToString();
					else
						objOD[strColumnName] = objLbl.Text;
				}
			}	// LOOP foreach (DataColumn objDC in _dccol)
		}

		public TextBox GetTextBoxFromColumnName(string strColumnName)
		{
			// find the column's respective control to store/retrieve the values
			View objView	= GetCurrentView();
			object objControl = objView.FindControl("txt" + strColumnName);
			TextBox objTB	= null;
			if (objControl != null)
			{
				objTB = (TextBox) objControl;
			}

			return objTB;
		}

		public DynamicFormViewTemplate(ListItemType type, DataColumnCollection dccol, bool showEdit)
		{
			_type = type;
			_dccol = dccol;
			ShowEdit = showEdit;
			_iEditTabOrder = 0;

			DataSourceTypeName = new List<string>();
			DataSourceSelectMethod = new List<string>();
			DropDownListColumns = new List<string>();
			DropDownValueField	= new List<string>();
			DropDownTextField	= new List<string>();
			DropDownDataSource	= new List<object>();
		}

		public void InstantiateIn( Control container)
		{
			CreateDictionary();

			if (GroupInTable)
				CreateFieldsInTable(container);

			if (GroupInTabs)
				CreateFieldsInTabs(container);
		}

		private void CreateDictionary()
		{
			// create the dictionary if not yet populated
			if (objOD == null)
				objOD = new OrderedDictionary();

			// store column-value pairs in the dictionary
			foreach (DataColumn objDC in _dccol)
			{
				string strColumnName = objDC.ColumnName;
				string strValue = objDC.Table.Rows[0][strColumnName].ToString();
				string strFormattedValue = GetFormattedValue(objDC.DataType.ToString(), strValue);

				// store column-value pairs in the dictionary
				if ( ! objOD.Contains(strColumnName) )
					objOD.Add(objDC.ColumnName, strFormattedValue);
			}
		}
		
		private void CreateFieldsInTabs(Control container)
		{
			Fields = new System.Collections.Hashtable();

			// create a tabstrip with the categories
			_TabStrip = new SCSWeb.TabStrip();

#if VERSION4
			_TabStrip.ViewStateMode = ViewStateMode.Enabled;
#endif

			_TabStrip.TabItemClicked += new SCSWeb.TabStrip.ItemClickedHandler(TabStrip_TabItemClicked);
			container.Controls.Add(_TabStrip);

			// create a multiview to store the tab contents
			_TabMultiView = new MultiView();
			container.Controls.Add(_TabMultiView);

			// collect list of categories from the columns
			ListOfTabs = "Main";
			foreach (DataColumn dc in _dccol)
				if (ListOfTabs.IndexOf(dc.Namespace) < 0)
					ListOfTabs = string.Format("{0},{1}", ListOfTabs, dc.Namespace);

			// create menu items as tabs
			string[] strTabs = ListOfTabs.Split(',');
			//_Tabs = new List<View>(strTabs.Length);

			foreach (string strCategory in strTabs)
			{
				// add tab to tabstrip
				SCSWeb.TabItem objTab = new SCSWeb.TabItem();
				objTab.Text = strCategory;
				objTab.Tag = strCategory;
				_TabStrip.Items.Add(objTab);

				// add corresponding view
				View objTabView = new View();
				objTabView.ID = strCategory;
				//_Tabs.Add(objTabView);
				_TabMultiView.Controls.Add(objTabView);

				// create a table within the view
				Table tbl = new Table();
				tbl.CssClass = CssClassTable;
				TableRow tr = null;
				TableCell td = null;

				objTabView.Controls.Add(tbl);

				int iColNbr = 0;
				foreach (DataColumn dc in _dccol)
				{
					if (dc.ColumnName != null && dc.Namespace == strCategory )
					{
						iColNbr++;
						// if there are more than the given nbr of rows, start adding columns instead of rows
						// making the formview grow sideways instead of downward
						if (iColNbr > NbrRows)
							// add columns starting from first row on
							tr = tbl.Rows[ ( iColNbr - 1 ) % NbrRows ];
						else
						{
							tr = new TableRow();
							tr.BorderWidth = Unit.Pixel(0);
							tr.VerticalAlign = VerticalAlign.Top;
							tbl.Rows.Add(tr);
						}

						// for each DataColum create a TableRow with 2 cells
						td = new TableCell();
						td.Wrap = false;
						tr.Cells.Add(td);

						td.Text = FormatColumnName(dc.ColumnName) + ":";
						td.CssClass = CssClassTableCell;

						td = new TableCell();
						td.Wrap = false;
						tr.Cells.Add(td);

						CreateColumnControl(td, dc);
					}
				}
			}

			// make the first tab selected
			_TabStrip.Items[0].Selected = true;
			_TabMultiView.SetActiveView(_TabMultiView.Views[0]);

			if (_type == ListItemType.Item)
			{
				LinkButton lnkEdit = new LinkButton();
				lnkEdit.Text = "Edit";
				lnkEdit.ToolTip = "Click here to turn on the EditItemTemplate of the FormView";
				lnkEdit.CommandName = "Edit";
				container.Controls.Add(lnkEdit);
			}
			else
			{
				// create a table to hold the link buttons
				Table tbl = new Table();
				tbl.CssClass = CssClassTable;
				TableRow tr = new TableRow();
				TableCell td = new TableCell();

				LinkButton lnkButton = new LinkButton();
				lnkButton.Text = "Cancel";
				lnkButton.ToolTip = "Click here to cancel the Edit process";
				lnkButton.CommandName = "Cancel";
				lnkButton.CausesValidation = false;
				td.Controls.Add(lnkButton);
				tr.Controls.Add(td);

				td = new TableCell();
				lnkButton = new LinkButton();
				lnkButton.Text = "Update";
				lnkButton.ToolTip = "Click here to update the record";
				lnkButton.CommandName = "Update";
				lnkButton.CausesValidation = false;
				td.Controls.Add(lnkButton);
				tr.Controls.Add(td);

				tbl.Controls.Add(tr);
				container.Controls.Add(tbl);
			}
		}

		private void CreateFieldsInTable(Control container)
		{
			Fields = new System.Collections.Hashtable();

			// create a table within the ItemTemplate
			Table tbl = new Table();
			tbl.CssClass = CssClassTable;
			TableRow tr = null;
			TableCell td = null;

			container.Controls.Add(tbl);

			int iColNbr = 0;
			foreach (DataColumn dc in _dccol)
			{
				if (dc.ColumnName != null)
				{
					iColNbr++;
					// if there are more than the given nbr of rows, start adding columns instead of rows
					// making the formview grow sideways instead of downward
					if (iColNbr > NbrRows)
						// add columns starting from first row on
						tr = tbl.Rows[ (iColNbr - 1) % NbrRows ];
					else
					{
						tr = new TableRow();
						tr.BorderWidth = Unit.Pixel(0);
						tr.VerticalAlign = VerticalAlign.Top;
						tbl.Rows.Add(tr);
					}

					// for each DataColum create a TableRow with 2 cells
					td = new TableCell();
					td.Wrap = false;
					tr.Cells.Add(td);

					td.Text = FormatColumnName(dc.ColumnName) + ":";
					td.CssClass = CssClassTableCell;

					td = new TableCell();
					td.Wrap = false;
					tr.Cells.Add(td);

					CreateColumnControl(td, dc);
				}
			}

			// if ShowEdit is set to true add a LinkButton for Edit within the ItemTemplate
			// and 2 linkButons (Update, Cancel) for the EditItemTemplate
			tr = new TableRow();
			tbl.Rows.Add(tr);
			td = new TableCell();
			tr.Cells.Add(td);

			if (_type == ListItemType.Item)
			{
				td.ColumnSpan = 2;
				LinkButton lnkEdit = new LinkButton();
				lnkEdit.Text = "Edit";
				lnkEdit.ToolTip = "Click here to turn on the EditItemTemplate of the FormView";
				lnkEdit.CommandName = "Edit";
				td.Controls.Add(lnkEdit);
			}
			else
			{
				LinkButton lnkButton = new LinkButton();
				lnkButton.Text = "Cancel";
				lnkButton.ToolTip = "Click here to cancel the Edit process";
				lnkButton.CommandName = "Cancel";
				td.Controls.Add(lnkButton);

				td = new TableCell();
				tr.Cells.Add(td);

				lnkButton = new LinkButton();
				lnkButton.Text = "Update";
				lnkButton.ToolTip = "Click here to update the record";
				lnkButton.CommandName = "Update";
				td.Controls.Add(lnkButton);
			}
		}

		public static string FormatColumnName( string strInput )
		{
			string strResult = null;

			strInput = strInput.Replace("_", " ");

			char previousChar = ' ';
			foreach ( char c in strInput )
			{
				if ( c < 'a' )
				{
					if ( previousChar >= 'a' )
						strResult += " " + c;
					else
						strResult += c;
				}
				else
					strResult += c;

				previousChar = c;
			}

			strResult = strResult.Replace("  ", " ");

			//strResult += ": ";

			return strResult;
		}

		private Control CreateColumnControl( TableCell td, DataColumn _dc )
		{
			TextBox txt = null;
			Label lbl = null;
			DropDownList ddl = null;
			RegularExpressionValidator rexpval = null;
			RangeValidator rangeVal = null;

			switch (_type)
			{
				case ListItemType.Item:

					// if datacolumn is readonly and unique then it is a primary key
					// render it as label a distinctive style
					lbl = CreateColumnAsLabel(td, _dc, lbl);

					break;

				case ListItemType.EditItem:

					_iEditTabOrder ++;

					if (_dc.ReadOnly )
					{
						// check if this column should be created as DropDownList
						if (DropDownListColumns.Contains(_dc.ColumnName))
						{
							ddl = CreateColumnAsDropDown(td, _dc, ddl);
						}
						else
							lbl = CreateColumnAsLabel(td, _dc, lbl);
					}
					else
					{
						switch (_dc.DataType.ToString())
						{
							case "System.Boolean":
								RadioButtonList rbl = new RadioButtonList();
								rbl.ID = "rbl" + _dc.ColumnName;
								Fields.Add(rbl.ID, _dc.ColumnName);
								ListItem li = new ListItem("Yes", "True");
								rbl.Items.Add(li);
								li = new ListItem("No", "False");
								rbl.Items.Add(li);
								rbl.RepeatLayout = RepeatLayout.Flow;
								rbl.RepeatDirection = RepeatDirection.Horizontal;
								rbl.DataBound += new EventHandler(rbl_DataBound);
								rbl.CssClass = CssClassEditControl;
								rbl.TabIndex = Convert.ToInt16( _iEditTabOrder * TabInterval );
								td.Controls.Add(rbl);
								break;

							case "System.String":
								txt = new TextBox();
								txt.ID = "txt" + _dc.ColumnName;
								Fields.Add(txt.ID, _dc.ColumnName);
								if (_dc.MaxLength <= 0)
									_dc.MaxLength = 50;
								txt.MaxLength = _dc.MaxLength;
								txt.Width = Unit.Pixel(_dc.MaxLength * PixelsPerCharacter);
								txt.CssClass = CssClassEditControl;
								txt.TabIndex = Convert.ToInt16( _iEditTabOrder * TabInterval );
								td.Controls.Add(txt);
								txt.DataBinding += new EventHandler(txt_DataBinding);
								break;

							case "System.Int16":
								txt = new TextBox();

								if (_dc.DefaultValue.Equals(DBNull.Value)) txt.ToolTip = _dc.ColumnName + " is of type " + _dc.DataType.ToString();
								if (!_dc.Expression.Equals(string.Empty)) txt.ToolTip += ", a calculated field based on " + _dc.Expression.ToString();

								txt.ID = "txt" + _dc.ColumnName;
								Fields.Add(txt.ID, _dc.ColumnName);

								txt.CssClass = CssClassEditControl;
								txt.TabIndex = Convert.ToInt16( _iEditTabOrder * TabInterval );
								td.Controls.Add(txt);
								txt.MaxLength = 5;
								txt.Width = Unit.Pixel(40);

								rangeVal = new RangeValidator();
								rangeVal.ControlToValidate = txt.ID;
								rangeVal.MaximumValue = "32767";
								rangeVal.MinimumValue = "-32767";
								rangeVal.Display = ValidatorDisplay.Dynamic;
								rangeVal.Text = "*";
								rangeVal.ErrorMessage = "Entered value for " + _dc.ColumnName + " is not valid for an integer numeric field";

								rangeVal.Type = ValidationDataType.Integer;
								td.Controls.Add(rangeVal);
								txt.DataBinding += new EventHandler(txt_DataBinding);
								break;

							case "System.Int32":
								txt = new TextBox();
								if (_dc.DefaultValue.Equals(DBNull.Value)) txt.ToolTip = _dc.ColumnName + " is of type " + _dc.DataType.ToString();
								if (!_dc.Expression.Equals(string.Empty)) txt.ToolTip += ", a calculated field based on " + _dc.Expression.ToString();

								txt.ID = "txt" + _dc.ColumnName;
								Fields.Add(txt.ID, _dc.ColumnName);
								txt.CssClass = CssClassEditControl;
								txt.TabIndex = Convert.ToInt16( _iEditTabOrder * TabInterval );
								td.Controls.Add(txt);
								txt.MaxLength = 10;
								txt.Width = Unit.Pixel(PixelsPerCharacter * txt.MaxLength );

								rangeVal = new RangeValidator();
								rangeVal.ControlToValidate = txt.ID;
								rangeVal.MaximumValue = "2147483647";
								rangeVal.MinimumValue = "-2147483648";
								rangeVal.Display = ValidatorDisplay.Dynamic;
								rangeVal.Text = "*";
								rangeVal.ErrorMessage = "Entered value for " + _dc.ColumnName + " is not valid for an integer numeric field";
								rangeVal.Type = ValidationDataType.Integer;
								td.Controls.Add(rangeVal);
								txt.DataBinding += new EventHandler(txt_DataBinding);
								break;

							case "System.Int64":
								txt = new TextBox();
								if (_dc.DefaultValue.Equals(DBNull.Value)) txt.ToolTip = _dc.ColumnName + " is of type " + _dc.DataType.ToString();
								if (!_dc.Expression.Equals(string.Empty)) txt.ToolTip += ", a calculated field based on " + _dc.Expression.ToString();

								txt.ID = "txt" + _dc.ColumnName;
								Fields.Add(txt.ID, _dc.ColumnName);
								txt.CssClass = CssClassEditControl;
								txt.TabIndex = Convert.ToInt16( _iEditTabOrder * TabInterval );
								td.Controls.Add(txt);
								txt.MaxLength = 19;
								txt.Width = Unit.Pixel(PixelsPerCharacter * 10);

								rangeVal = new RangeValidator();
								rangeVal.ControlToValidate = txt.ID;
								rangeVal.MaximumValue = "9223372036854775807";
								rangeVal.MinimumValue = "-9223372036854775808";
								rangeVal.Display = ValidatorDisplay.Dynamic;
								rangeVal.Text = "*";
								rangeVal.ErrorMessage = "Entered value for " + _dc.ColumnName + " is not valid for an integer numeric field";
								rangeVal.Type = ValidationDataType.Integer;
								td.Controls.Add(rangeVal);
								txt.DataBinding += new EventHandler(txt_DataBinding);
								break;

							case "System.Decimal":
								txt = new TextBox();
								if (_dc.DefaultValue.Equals(DBNull.Value)) 
									txt.ToolTip = _dc.ColumnName + " is of type " + _dc.DataType.ToString();
								if (!_dc.Expression.Equals(string.Empty)) 
									txt.ToolTip += ", a calculated field based on " + _dc.Expression.ToString();

								txt.ID = "txt" + _dc.ColumnName;
								Fields.Add(txt.ID, _dc.ColumnName);
								txt.CssClass = CssClassEditControl;
								txt.TabIndex = Convert.ToInt16( _iEditTabOrder * TabInterval );
								td.Controls.Add(txt);
								txt.MaxLength = 19;
								txt.Width = Unit.Pixel(PixelsPerCharacter * 10);

								rexpval = new RegularExpressionValidator();
								rexpval.ValidationExpression = @"^\d*.?\d{0,2}$";
								rexpval.Display = ValidatorDisplay.Dynamic;
								rexpval.Text = "*";
								rexpval.ErrorMessage = "Entered value for " + _dc.ColumnName + " is not valid for a decimal field";
								rexpval.ControlToValidate = txt.ID;
								td.Controls.Add(rexpval);
								txt.DataBinding += new EventHandler(txt_DataBinding);
								break;

							case "System.DateTime":
								txt = new TextBox();
								txt.ID = "txt" + _dc.ColumnName;
								Fields.Add(txt.ID, _dc.ColumnName);
								txt.MaxLength = 22;
								txt.Width = Unit.Pixel( PixelsPerCharacter * 35 );
								txt.CssClass = CssClassEditControl;
								txt.TabIndex = Convert.ToInt16( _iEditTabOrder * TabInterval );
								td.Controls.Add(txt);

								// this was causing trouble with the datetime format MM/dd/yyyy hh:mm:ss (military format)
								// need a different validation regex
								//rexpval = new RegularExpressionValidator();
								//rexpval.ValidationExpression = @"^\(?\d{3}[\)\-\s]?\d{3}[-\s]?\d{4}$";
								//rexpval.ControlToValidate = txt.ID;
								//rexpval.Display = ValidatorDisplay.Dynamic;
								//rexpval.Text = "*";
								//rexpval.ErrorMessage = "Entered value for " + _dc.ColumnName + " is not a valid date";
								//td.Controls.Add(rexpval);
								txt.DataBinding += new EventHandler(txt_DataBinding);
								break;

							default:
								txt = new TextBox();
								txt.ID = "txt" + _dc.ColumnName;
								Fields.Add(txt.ID, _dc.ColumnName);
								txt.CssClass = CssClassEditControl;
								txt.TabIndex = Convert.ToInt16( _iEditTabOrder * TabInterval );
								td.Controls.Add(txt);

								if (txt.MaxLength > 0)
								{
									txt.MaxLength = _dc.MaxLength;
									txt.Width = Unit.Pixel(_dc.MaxLength * PixelsPerCharacter);
								}

								txt.DataBinding += new EventHandler(txt_DataBinding);
								break;

						} // END switch ( _dc.DataType.ToString() )

					} // END if( _dc.ReadOnly ) else

				break;

			} // END switch( _type )

			return lbl;
		}

		private Label CreateColumnAsLabel(TableCell td, DataColumn _dc, Label lbl)
		{
			lbl = new Label();
			lbl.ID = "lbl" + _dc.ColumnName;
			Fields.Add(lbl.ID, _dc.ColumnName);
			if ( _dc.ReadOnly ) //&& _dc.Unique )
				lbl.CssClass = CssClassPrimaryKey;

			lbl.ToolTip	= GetDefaultTooltip(_dc);

			td.Controls.Add(lbl);
			lbl.DataBinding += new EventHandler(lbl_DataBinding);
			return lbl;
		}

		private DropDownList CreateColumnAsDropDown(TableCell td, DataColumn _dc, DropDownList ddl)
		{
			ddl = new DropDownList();
			ddl.ID = "ddl" + _dc.ColumnName;

			ddl.AppendDataBoundItems = true;
			ddl.Width = Unit.Pixel(_dc.MaxLength * PixelsPerCharacter);

			// locate the right DataTextField and DataSource 
			int iPosDDLCol		= DropDownListColumns.FindIndex( c => c.Equals( _dc.ColumnName ) );

			// prepare the data source to be bound
			ObjectDataSource objODS = new ObjectDataSource();
			objODS.TypeName	= DataSourceTypeName[ iPosDDLCol ];
			objODS.SelectMethod	= DataSourceSelectMethod[ iPosDDLCol ];
			//objODS.SelectParameters.Add(
			objODS.Select();

			// execute the data bind
			ddl.DataValueField	= DropDownValueField[ iPosDDLCol ];
			ddl.DataTextField	= DropDownTextField[ iPosDDLCol ];
			ddl.DataSource		= objODS;
			ddl.DataBind();
			
			Fields.Add(ddl.ID, _dc.ColumnName);

			if (_dc.ReadOnly)
				ddl.CssClass = CssClassPrimaryKey;

			ddl.ToolTip	= GetDefaultTooltip(_dc);

			td.Controls.Add(ddl);
			ddl.DataBinding += new EventHandler(ddl_DataBinding);

			return ddl;
		}

		private static string GetDefaultTooltip(DataColumn _dc)
		{
			// compose a tooltip from the column attributes
			string strToolTip = string.Concat( _dc.ColumnName, " is of type ", _dc.DataType.ToString() );

			if ( _dc.ReadOnly )
				strToolTip = string.Concat( strToolTip, ", Readonly" );

			if ( _dc.Unique )
				strToolTip = string.Concat( strToolTip, ", Unique" );

			if ( _dc.DefaultValue.Equals( DBNull.Value ) )
				strToolTip = string.Concat( strToolTip, ", Default value is DBNull" );

			if ( ! _dc.Expression.Equals( string.Empty ) )
				strToolTip = string.Concat( strToolTip, ", a calculated field based on ", _dc.Expression.ToString() );

			return strToolTip;
		}

		void txt_DataBinding(object sender, EventArgs e)
		{
			TextBox txt = (TextBox)sender;
			DataColumn _dc = _dccol[Fields[txt.ID].ToString()];
			DataRowView drv = (DataRowView)((FormView)txt.NamingContainer).DataItem;

			if (Convert.IsDBNull( drv[_dc.ColumnName] ))
			{
				txt.Text = "";
				return;
			}

			txt.Text = GetFormattedValue(_dc.DataType.ToString(), drv[_dc.ColumnName].ToString());
			txt.CssClass = string.Format( "{0},{1}", txt.CssClass, GetCssClass(_dc.DataType.ToString(), drv[_dc.ColumnName].ToString()) );
		}

		private string GetFormattedValue(string strDataType, string strValue)
		{
			if( strValue.Equals( "" ) )
				return "";

			switch (strDataType)
			{
				case "System.String":
					return strValue;

				case "System.Int16":
				case "System.Int32":
				case "System.Int64":
				case "int":
					return Convert.ToInt64(strValue).ToString(IntegerFormat);

				case "System.Decimal":
					return Convert.ToDecimal(strValue).ToString(DecimalFormat);

				case "System.DateTime":
					return Convert.ToDateTime(strValue).ToString(DateFormat);

				case "System.Boolean":
					return Convert.ToBoolean(strValue) ? "Yes" : "No";

				default:
					return strValue;
			}
		}

		private string GetCssClass(string strDataType, string strValue)
		{
			switch (strDataType)
			{
				case "System.String":
					return "";

				case "System.Int16":
				case "System.Int32":
				case "System.Int64":
				case "int":
					if (Convert.ToInt64(strValue) < 0) return CssClassNegativeNbrs;
					return "";

				case "System.Decimal":
					if (Convert.ToDecimal(strValue) < 0) return CssClassNegativeNbrs;
					return "";

				case "System.DateTime":
					if (Convert.ToDateTime(strValue) > DateTime.Now) return CssClassLateDates;
					return "";

				default:
					return "";
			}
		}

		void rbl_DataBound(object sender, EventArgs e)
		{
			RadioButtonList rbl = ((RadioButtonList)sender);
			DataColumn _dc = _dccol[Fields[rbl.ID].ToString()];
			DataRowView drv = (DataRowView)((FormView)rbl.NamingContainer).DataItem;
			rbl.SelectedValue = drv[_dc.ColumnName].ToString();
		}

		void lbl_DataBinding(object sender, EventArgs e)
		{
			Label lbl = ((Label)sender);
			DataRowView drv = (DataRowView)((FormView)lbl.NamingContainer).DataItem;
			DataColumn _dc = _dccol[Fields[lbl.ID].ToString()];

			// change the data format based on the data if the datacolumn is not a primary key field
			if (_dc.ReadOnly && _dc.Unique)
				lbl.Text = drv[_dc.ColumnName].ToString();
			else
			{
				if (Convert.IsDBNull(drv[_dc.ColumnName]))
					lbl.Text = "";
				else
				{
					lbl.Text = GetFormattedValue(_dc.DataType.ToString(), drv[_dc.ColumnName].ToString());
					string strCssClass = GetCssClass(_dc.DataType.ToString(), drv[_dc.ColumnName].ToString());
					if (!strCssClass.Equals(""))
						lbl.CssClass = string.Format("{0},{1}", lbl.CssClass, strCssClass);
				}

			}
		}

		void ddl_DataBinding(object sender, EventArgs e)
		{
			DropDownList ddl = ((DropDownList)sender);
			DataRowView drv = (DataRowView)((FormView)ddl.NamingContainer).DataItem;
			DataColumn _dc = _dccol[Fields[ddl.ID].ToString()];

			// change the data format based on the data if the datacolumn is not a primary key field
			if (_dc.ReadOnly && _dc.Unique)
				ddl.SelectedValue = drv[_dc.ColumnName].ToString();
			else
			{
				if (Convert.IsDBNull(drv[_dc.ColumnName]))
					ddl.SelectedValue = "";
				else
				{
					ddl.SelectedValue = GetFormattedValue(_dc.DataType.ToString(), drv[_dc.ColumnName].ToString());
					string strCssClass  = GetCssClass(_dc.DataType.ToString(), drv[_dc.ColumnName].ToString());
					if( ! strCssClass.Equals( "" ) )
						ddl.CssClass = string.Format("{0},{1}", ddl.CssClass, strCssClass);
				}
			}
		}
	}
}