using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

namespace Maintenance
{
	public class DynamicGridViewService
	{
		private GridView _objGV;
		private DataTable _objDT;
		private string _strTableDefsSessionVar;
		private OrderedDictionary _objODDataSource;

		public delegate void OnUpdateEvent( object sender, GridViewUpdateEventArgs e );
		public OnUpdateEvent OnUpdate;
		public delegate void OnDeleteEvent( object sender, GridViewDeleteEventArgs e );
		public OnDeleteEvent OnDelete;

		public bool EnableForeignKeyLinks = false;

		// the DataColumn sent in the DataTable _objDT will have ExtendedProperties named ForeignKey and ForeignKeyTable
		// which will tell the class to create a DropDownList and data bind it to the method described in DataSourceSelectMethod 
		// (and DataSourceTypeName)
		public string DataSourceTypeName = "Maintenance.DBAccess";	// class that has select method ex.: NewMCIS.DBAccess
		public string DataSourceSelectMethod = "GetRowsFromTable";	// method that retrieves a datatable with dropdownlist content ex.: GetBusinessUnits

		// this constructor gets the gridview and retrieves the associated datatable from Session
		public DynamicGridViewService( GridView objGridView, string strTableDefsSessionVar = "" )
		{
			_objGV = objGridView;
			_strTableDefsSessionVar = strTableDefsSessionVar;

			// retrieve datatable from session
			string strSessionGVName = string.Concat("DGVS_", _objGV.ID, "_Table");
			//string strSessionTemplatesName = string.Concat("DGVS_", objGV.ID, "_Templates");
			Page objPg = new Page();
			if (objPg.Session[strSessionGVName] == null)
				return;
			_objDT = (DataTable)objPg.Session[strSessionGVName];

			// retrieve dictionary from session
			strSessionGVName = string.Concat("DGVS_", _objGV.ID, "_Dictionary");
			if (objPg.Session[strSessionGVName] == null)
				return;
			_objODDataSource = (OrderedDictionary)objPg.Session[strSessionGVName];
		}

		// this constructor receives the datatable to be associated with the gridview and stored in Session
		public DynamicGridViewService(GridView objGridView, DataTable objDataTable, string strTableDefsSessionVar = "" )
		{
			_objGV = objGridView;
			_objDT = objDataTable;
			_strTableDefsSessionVar = strTableDefsSessionVar;

			string strSessionGVName = string.Concat("DGVS_", _objGV.ID, "_Table");

			Page objPg = new Page();

			objPg.Session[strSessionGVName] = _objDT;

			//Activate();
		}

		public void Activate()
		{
			CreateAndAssignTemplates();
			BindGridView();
			BindEvents();
		}

		private void BindGridView()
		{
			_objGV.DataSource = _objDT;
			_objGV.DataBind(); 
		}

		private void BindEvents()
		{
			_objGV.RowEditing += new GridViewEditEventHandler(objGV_RowEditing);
			_objGV.RowCancelingEdit += new GridViewCancelEditEventHandler(objGV_RowCancelingEdit);
			_objGV.RowDeleting += new GridViewDeleteEventHandler(objGV_RowDeleting);
			_objGV.RowUpdating += new GridViewUpdateEventHandler(objGV_RowUpdating);
		}

		private void objGV_RowDeleting( object sender, GridViewDeleteEventArgs e )
		{
			DataTable objDTDef = null;
			if (!_strTableDefsSessionVar.Equals(""))
				objDTDef = (DataTable)(new Page()).Session[_strTableDefsSessionVar];

			int iGridRow = _objGV.PageIndex * _objGV.PageSize + e.RowIndex;
			bool bIdentityColumnFound = false;
			foreach (DataColumn objDC in _objDT.Columns)
			{
				// get original values and format it to make it ready for the UPDATE statement
				string strName = objDC.ColumnName;
				string strValue = _objDT.Rows[iGridRow][strName].ToString();
				bool bEncloseInQuotes = objDC.DataType.Name.Equals("String") || objDC.DataType.Name.Equals("DateTime");
				if (bEncloseInQuotes)
					strValue = string.Concat("'", strValue, "'");

				// check whether the column is a key
				if ( objDTDef != null 
					&& !e.Keys.Contains( strName ) )
				{
					DataRow objDR = objDTDef.Rows.Find( strName );
					// if column is key/identity and not in e.Keys, add it to e.Keys with the OLD value
					if (objDR != null)
					{
						// consider identity column as key
						if (Convert.ToInt32(objDR["IsIdentity"]) == 1)
						{
							e.Keys.Add(strName, strValue);
							bIdentityColumnFound = true;
						}

						// if no identity column was found earlier, use the available key
						if (Convert.ToInt32(objDR["PrimaryKey"]) == 1
								&& ! bIdentityColumnFound )
							e.Keys.Add(strName, strValue);
					}
				}

				// adjust new values, format it to make it ready for the UPDATE statement
				strValue = "";
				if (e.Values[strName] != null)
				{
					strValue = e.Values[strName].ToString();

					// update the datatable with the new value, so to display the change when databound again
					_objDT.Rows[iGridRow][strName] = e.Values[strName];
				}
				else
				{
					strValue = GetValueFromGridViewColumn(e.RowIndex, strName);

					// update the datatable with the new value, so to display the change when databound again
					if (strValue.Equals("NULL"))
						_objDT.Rows[iGridRow][strName] = Convert.DBNull;
					else
						_objDT.Rows[iGridRow][strName] = strValue;
				}

				if (bEncloseInQuotes)
					strValue = string.Concat("'", strValue, "'");

				// adjust new values
				e.Values[strName] = strValue;
			}

			if (OnDelete != null)
				OnDelete(sender, e);

			if (e.Cancel)
				return;

			_objGV.EditIndex = -1;
			//BindGridView();
		}

		private void objGV_RowUpdating( object sender, GridViewUpdateEventArgs e )
		{
			DataTable objDTDef = null;
			if (!_strTableDefsSessionVar.Equals(""))
				objDTDef = (DataTable) (new Page()).Session[_strTableDefsSessionVar];

			int iGridRow = _objGV.PageIndex * _objGV.PageSize + e.RowIndex;
			bool bIdentityColumnFound = false;
			foreach (DataColumn objDC in _objDT.Columns)
			{
				// get original values and format it to make it ready for the UPDATE statement
				string strName = objDC.ColumnName;
				string strValue = _objDT.Rows[iGridRow][strName].ToString();
				string strType = objDC.DataType.Name;

				// if empty string and a datetime, set it to null
				if (strValue.Equals("") && ! strType.Equals("String") )
					strValue = "NULL";

				bool bEncloseInQuotes = strType.Equals("String") || strType.Equals("DateTime");
				if (bEncloseInQuotes && !strValue.Equals("NULL"))
					strValue = string.Concat("'", strValue, "'");

				// check whether the column is a key
				if (objDTDef != null
					&& !e.Keys.Contains(strName))
				{
					DataRow objDR = objDTDef.Rows.Find(strName);
					// if column is key/identity and not in e.Keys, add it to e.Keys with the OLD value
					if (objDR != null)
					{
						// consider identity column as key
						if (Convert.ToInt32(objDR["IsIdentity"]) == 1)
						{
							e.Keys.Add(strName, strValue);
							bIdentityColumnFound = true;
						}

						// if no identity column was found earlier, use the available key
						if (Convert.ToInt32(objDR["PrimaryKey"]) == 1
								&& !bIdentityColumnFound)
							e.Keys.Add(strName, strValue);
					}
				}

				// add original values
				e.OldValues.Add(strName, strValue);

				// adjust new values, format it to make it ready for the UPDATE statement
				strValue = "";
				if (e.NewValues[strName] != null)
					strValue = e.NewValues[strName].ToString();
				else
				{
					strValue = GetValueFromGridViewColumn(e.RowIndex, strName);

					// if empty string and a datetime, set it to null
					if (strValue.Equals("") && !strType.Equals("String"))
						strValue = "NULL";
				}

				if (bEncloseInQuotes && !strValue.Equals("NULL"))
					strValue = string.Concat("'", strValue, "'");

				// adjust new values
				e.NewValues[strName] = strValue;
			}

			if (OnUpdate != null)
				OnUpdate(sender, e);

			if (e.Cancel)
				return;

			// set new values in the table
			foreach (DataColumn objDC in _objDT.Columns)
			{
				string strName = objDC.ColumnName;
				string strValue = "";
				string strType = objDC.DataType.Name;

				// adjust new values, format it to make it ready for the UPDATE statement
				if (e.NewValues[strName] != null)
					strValue = e.NewValues[strName].ToString();
				else
				{
					strValue = GetValueFromGridViewColumn(e.RowIndex, strName);

					// if empty string and a datetime, set it to null
					if (strValue.Equals("") && !strType.Equals("String"))
						strValue = "NULL";
				}

				if (strValue.StartsWith("'"))
					strValue = strValue.Substring(1);
				if (strValue.EndsWith("'"))
					strValue = strValue.Substring(0, strValue.Length - 1);

				// update the datatable with the new value, so to display the change when databound again
				if (strValue.Equals("NULL"))
					_objDT.Rows[iGridRow][strName] = Convert.DBNull;
				else
					_objDT.Rows[iGridRow][strName] = strValue;
			}

			_objGV.EditIndex = -1;
			BindGridView();
		}

		private string GetValueFromGridViewColumn(int iRow, string strColumnName)
		{
			string strValue	= "NULL";

			// handle the case when the e.NewValues was not populated
			Control objCtrl = _objGV.Rows[iRow].FindControl(strColumnName);
			if (objCtrl != null)
			{
				if (objCtrl is Label)
					strValue = ((Label)objCtrl).Text;
				if (objCtrl is RadioButtonList)
					strValue = ((RadioButtonList)objCtrl).SelectedValue;
				if (objCtrl is DropDownList)
					strValue = ((DropDownList)objCtrl).SelectedValue;
				if (objCtrl is TextBox)
					strValue = ((TextBox)objCtrl).Text;
			}

			return strValue;
		}

		private void objGV_RowEditing( object sender, GridViewEditEventArgs e )
		{
			_objGV.EditIndex = e.NewEditIndex;
			BindGridView();
		}

		private void objGV_RowCancelingEdit( object sender, GridViewCancelEditEventArgs e )
		{
			_objGV.EditIndex = -1;
			BindGridView();
		}

		private void CreateAndAssignTemplates()
		{
			_objGV.Columns.Clear();

			if( _objODDataSource == null)
				_objODDataSource = new OrderedDictionary();

			// create template for commands such as Edit and Delete and assign it to the gridview
			TemplateField objLinkButtonTemplates = new TemplateField();
			objLinkButtonTemplates.ItemTemplate = new DynamicGridViewTemplate(ListItemType.Item, "...", "Command");
			objLinkButtonTemplates.HeaderTemplate = new DynamicGridViewTemplate(ListItemType.Header, "...", "Command");
			objLinkButtonTemplates.EditItemTemplate = new DynamicGridViewTemplate(ListItemType.EditItem, "...", "Command");
			_objGV.Columns.Add(objLinkButtonTemplates);

			if (_objDT != null)
			{
				// set the primary key if available
				if (_objDT.PrimaryKey.Length > 0)
					_objGV.DataKeyNames = new string[] { _objDT.PrimaryKey[0].ColumnName };

				foreach (DataColumn objDC in _objDT.Columns)
				{
					// for each column, create header/display/editing templates
					TemplateField objFieldTemplate = new TemplateField();

					// determine the size of the column
					string strType = objDC.DataType.Name;
					int iSize = objDC.MaxLength;
					switch (strType)
					{
						case "Decimal":
						case "Int64": iSize = 19; break;
						case "DateTime": iSize = 22; break;
						case "Int16": iSize = 5; break;
						case "Int32": iSize = 10; break;
					}

					// default the size to 50
					if (iSize < 0)
						iSize = 50;

					// check whether the column is a foreign key
					string strFK = "", strFKTable = "";
					if (objDC.ExtendedProperties.ContainsKey("ForeignKey"))
					{
						strFK = objDC.ExtendedProperties["ForeignKey"].ToString();
						strFKTable = objDC.ExtendedProperties["ForeignKeyTable"].ToString();

						// create data source for the dropdownlist
						ObjectDataSource objODS;

						if (_objODDataSource.Contains(strFKTable))
							objODS = (ObjectDataSource)_objODDataSource[strFKTable];
						else
						{
							// get value samples from the first/last rows
							string strMaxValue = _objDT.Compute( string.Concat( "MAX(", objDC.ColumnName, ")" ), "" ).ToString();
							string strMinValue = _objDT.Compute( string.Concat( "MIN(", objDC.ColumnName, ")" ), "" ).ToString();

							objODS = new ObjectDataSource();

							// caches data until inactive for 60 seconds
							objODS.EnableCaching = true;
							objODS.CacheExpirationPolicy = DataSourceCacheExpiry.Sliding;
							objODS.CacheDuration = 60;

							objODS.TypeName = DataSourceTypeName;
							objODS.SelectMethod = DataSourceSelectMethod;
							objODS.SelectParameters.Add("strTableName", strFKTable);
							objODS.SelectParameters.Add("strKey", strFK);
							// give the sample value so the query will try to get 500 rows surrounding that value
							objODS.SelectParameters.Add("strMinValue", strMinValue);
							objODS.SelectParameters.Add("strMaxValue", strMaxValue);
							objODS.Select();
							_objODDataSource.Add(strFKTable, objODS);
						}

						// create display/editing templates that will become dropdownlists
						objFieldTemplate.ItemTemplate = new DynamicGridViewTemplate(ListItemType.Item, objDC.ColumnName, strType
								, iSize, strFK, strFKTable, objODS);
						objFieldTemplate.EditItemTemplate = new DynamicGridViewTemplate(ListItemType.EditItem, objDC.ColumnName
								, strType, iSize, strFK, strFKTable, objODS);
					}
					else
					{
						objFieldTemplate.ItemTemplate = new DynamicGridViewTemplate(ListItemType.Item, objDC.ColumnName, strType, iSize);
						objFieldTemplate.EditItemTemplate = new DynamicGridViewTemplate(ListItemType.EditItem, objDC.ColumnName, strType, iSize);
					}

					// create header/display/editing templates
					objFieldTemplate.HeaderTemplate = new DynamicGridViewTemplate(ListItemType.Header, objDC.ColumnName, strType);

					// assign templates to the gridview
					_objGV.Columns.Add(objFieldTemplate);

				}	// end foreach

			}	// end if

			string strSessionGVName = string.Concat("DGVS_", _objGV.ID, "_Dictionary");
			Page objPg = new Page();
			if (objPg.Session[strSessionGVName] != null)
				return;
			objPg.Session[strSessionGVName] = _objODDataSource;
		}
	}

	public class DynamicGridViewTemplate : ITemplate, INamingContainer, IBindableTemplate
	{
		ListItemType _ItemType;
		string _FieldName, _Type, _ForeignKey = "", _ForeignKeyTable = "";
		int _Size	= 0;
		ObjectDataSource _objODS;

		public string CssEditField = "GridViewFields";

		public int PixelsPerCharacter = 5; // 4 for smaller and 6 for small (CSS)
		public string DateFormat = "MM/dd/yyyy HH:mm:ss";
		public string DecimalFormat = "#0.00";
		public string IntegerFormat = "#0";

		public DynamicGridViewTemplate(ListItemType objItemType, string strFieldName, string strType )
		{
			_ItemType = objItemType;
			_FieldName = strFieldName;
			_Type = strType;
		}

		public DynamicGridViewTemplate(ListItemType objItemType, string strFieldName, string strType, int iSize )
		{
			_ItemType = objItemType;
			_FieldName = strFieldName;
			_Type = strType;
			_Size = iSize;
		}

		public DynamicGridViewTemplate(ListItemType objItemType, string strFieldName, string strType, int iSize
			, string strForeignKey, string strForeignKeyTable, ObjectDataSource objODS)
		{
			_ItemType = objItemType;
			_FieldName = strFieldName;
			_Type = strType;
			_Size = iSize;
			_ForeignKey	= strForeignKey;
			_ForeignKeyTable = strForeignKeyTable;
			_objODS = objODS;
		}

		public void InstantiateIn(Control container)
		{
			switch (_ItemType)
			{
				case ListItemType.Header:
					Literal objHeader = new Literal();
					objHeader.Text = string.Concat("<b>", FormatColumnName(_FieldName), "</b>");
					container.Controls.Add(objHeader);
					break;


				case ListItemType.Item:
					switch (_Type)
					{
						case "Command":
							LinkButton lbtnEdit = new LinkButton();
							lbtnEdit.ID = "edit_button";
							lbtnEdit.Text = "Edit";
							lbtnEdit.CommandName = "Edit";
							lbtnEdit.ToolTip = "Edit";
							container.Controls.Add(lbtnEdit);

							Literal objLit = new Literal();
							objLit.Text = " ";
							container.Controls.Add(objLit);

							LinkButton lbtnDelete = new LinkButton();
							lbtnDelete.ID = "delete_button";
							lbtnDelete.Text = "Delete";
							lbtnDelete.CommandName = "Delete";
							lbtnDelete.ToolTip = "Delete";
							lbtnDelete.OnClientClick = "return confirm('Are you sure to delete the record?')";
							container.Controls.Add(lbtnDelete);

							break;

						default:
							// check whether this is a foreign key
							if (_ForeignKey.Equals(""))
							{
								Label objLabel = new Label();
								objLabel.ID = _FieldName;
								objLabel.CssClass = CssEditField;
								objLabel.Text = String.Empty; //we will bind it later through 'OnDataBinding' event
								objLabel.DataBinding += new EventHandler(OnDataBinding);
								container.Controls.Add(objLabel);
							}
							else
								CreateDropDownList(container, false);
							break;
					}
					break;

				case ListItemType.EditItem:
					switch (_Type)
					{
						case "Command":
							LinkButton lbtnUpdate = new LinkButton();
							lbtnUpdate.ID = "update_button";
							lbtnUpdate.CommandName = "Update";
							lbtnUpdate.Text = "Update";
							lbtnUpdate.ToolTip = "Update";
							container.Controls.Add(lbtnUpdate);

							Literal objLit = new Literal();
							objLit.Text = " ";
							container.Controls.Add(objLit);

							LinkButton lbtnCancel = new LinkButton();
							lbtnCancel.ID = "cancel_button";
							lbtnCancel.CommandName = "Cancel";
							lbtnCancel.Text = "Cancel";
							lbtnCancel.ToolTip = "Cancel";
							container.Controls.Add(lbtnCancel);
							break;

						case "System.Boolean":
							RadioButtonList objRBL = new RadioButtonList();
							objRBL.ID = string.Concat( "rbl", _FieldName );
							objRBL.CssClass = CssEditField;
							ListItem li = new ListItem("Yes", "True");
							objRBL.Items.Add(li);
							li = new ListItem("No", "False");
							objRBL.Items.Add(li);
							objRBL.RepeatLayout = RepeatLayout.Flow;
							objRBL.RepeatDirection = RepeatDirection.Horizontal;
							objRBL.DataBound += new EventHandler(rbl_DataBound);
							container.Controls.Add(objRBL);
							break;

						default:							
							// check whether this is a foreign key
							if (_ForeignKey.Equals(""))
							{
								TextBox objTB = new TextBox();
								objTB.ID = _FieldName;
								objTB.CssClass = CssEditField;
								objTB.Text = String.Empty;
								objTB.DataBinding += new EventHandler(OnDataBinding);
								container.Controls.Add(objTB);
							}
							else
								CreateDropDownList(container, true );
							break;

					}
					break;
			}			
		}

		private void CreateDropDownList(Control container, bool bEnabled)
		{
			// if so then create dropdownlist
			DropDownList objDDL = new DropDownList();
			objDDL.ID = _FieldName;
			objDDL.CssClass = CssEditField;
			objDDL.Enabled = bEnabled;
			objDDL.Text = String.Empty; // we will bind it later through 'OnDataBinding' event
			objDDL.DataValueField = _ForeignKey;
			objDDL.DataTextField = "Description";	// this is an alias
			objDDL.AppendDataBoundItems = true;
			objDDL.Width = Unit.Pixel(_Size * PixelsPerCharacter);

			// populate this dropdown
			objDDL.DataSource = _objODS;
			try
			{
				// skip problem when dropdown doesn't have the value the column needs
				objDDL.DataBind();
			}
			catch (ArgumentOutOfRangeException)
			{ }

			objDDL.DataBinding += new EventHandler(OnDataBinding);
			container.Controls.Add(objDDL);
		}

		private void OnDataBinding(object sender, EventArgs e)
		{
			Control objCtrl = (Control)sender;
			IDataItemContainer data_item_container = (IDataItemContainer)objCtrl.NamingContainer;
			object objValue = DataBinder.Eval(data_item_container.DataItem, _FieldName);
			string strValue = "";
			if (!Convert.IsDBNull(objValue))
				strValue = objValue.ToString();

			// check whether this is a foreign key
			if (_ForeignKey.Equals(""))
			{
				switch (_ItemType)
				{
					case ListItemType.Item:
						Label objLabel = (Label)sender;
						objLabel.Text = GetFormattedValue(_Type, strValue);
						if (_Size == 0)
							objLabel.Width = Unit.Pixel(strValue.Length * PixelsPerCharacter);
						else
							objLabel.Width = Unit.Pixel(_Size * PixelsPerCharacter);
						break;

					case ListItemType.EditItem:
						TextBox objTB = (TextBox)sender;
						objTB.Text = GetFormattedValue(_Type, strValue);
						if (_Size == 0)
							objTB.Width = Unit.Pixel(strValue.Length * PixelsPerCharacter);
						else
							objTB.Width = Unit.Pixel(_Size * PixelsPerCharacter);
						break;
				}
			}
			else
			{
				DropDownList objDDL = (DropDownList)sender;
				if (!Convert.IsDBNull(strValue))
				{
					if (objDDL.Items.Count > 0)
					{
						// assure that the value is in the dropdown, if not, add it
						ListItem objLI = objDDL.Items.FindByValue(strValue);
						if (objLI == null)
						{
							objLI = new ListItem(strValue, strValue);
							objDDL.Items.Insert(0, objLI);
						}
					}
					objDDL.SelectedValue = strValue;
				}
			}
		}

		private void rbl_DataBound(object sender, EventArgs e)
		{
			Control objCtrl = (Control)sender;
			IDataItemContainer data_item_container = (IDataItemContainer)objCtrl.NamingContainer;
			object objValue = DataBinder.Eval(data_item_container.DataItem, _FieldName);
			RadioButtonList objRBL = (RadioButtonList)sender;
			objRBL.SelectedValue = objValue.ToString();
		}

		private string GetFormattedValue(string strDataType, string strValue)
		{
			if (strValue.Equals(""))
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

		public IOrderedDictionary ExtractValues(Control container)
		{
			// this is to make e.NewValues populated during ItemUpdating

			OrderedDictionary objOD = new OrderedDictionary();
			Control objCtrl = container.FindControl( _FieldName );

			if (objCtrl is Label)
			{
				Label objLabel = (Label)objCtrl;
				objOD.Add(_FieldName, objLabel.Text);
			}

			if (objCtrl is TextBox)
			{
				TextBox objTB = (TextBox)objCtrl;
				objOD.Add(_FieldName, objTB.Text);
			}

			if (objCtrl is DropDownList)
			{
				DropDownList objDDL = (DropDownList)objCtrl;
				objOD.Add(_FieldName, objDDL.SelectedValue);
			}

			if (objCtrl is RadioButtonList)
			{
				RadioButtonList objRBL = (RadioButtonList)objCtrl;
				objOD.Add(_FieldName, objRBL.SelectedValue);
			}

			return objOD;
		}

		public static string FormatColumnName(string strInput)
		{
			string strResult = null;

			strInput = strInput.Replace("_", " ");

			char previousChar = ' ';
			foreach (char c in strInput)
			{
				if (c < 'a')
				{
					if (previousChar >= 'a')
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
	}
}