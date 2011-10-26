using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SCSWeb = SCS.Web.UI.WebControls;

namespace NewMCIS
{
	public partial class _Default : System.Web.UI.Page
	{
		private const string CACHED_TABLE = "IncidentDataTable";
		private const string CACHED_TEMPLATE = "FormViewTemplate";
		private const string CACHED_CHANGEDCOLS = "ChangedColumnsDataTable";
		private const string PREVIOUS_INDEX = "PreviousIndex";

		private DataTable objDT;
		private DataTable objChangedColumns;
		private int iPreviousIndex = -1;
		DBAccess objDB;

		protected void Page_Load(object sender, EventArgs e)
		{
			// initialize database helper
			objDB = new DBAccess();
			objDB.ErrorLabel = lblError;
			
			btnOk.OnClientClick = string.Format("javascript: {0}.style.visibility = 'hidden'; return false; ", pnlMessage.ClientID);

			pnlMessage.Visible = false;
			
			if (!Page.IsPostBack)
			{
				// do the initial retrieval of the list of CADs
				DataTable objCADTable = objDB.GetCADs();
				ddlCADId.DataSource = objCADTable;
				ddlCADId.DataTextField = "CAD";
				ddlCADId.DataValueField = "CAD";
				ddlCADId.DataBind();
			}
			else
			{
				// retrieve table to be edited from the Session
				objDT = (DataTable)Session[ CACHED_TABLE ];
				if (objDT != null)
					fvEditor.DataSource = objDT;

				// retrieve the item template containing the tabs and the columns to use in the formview
				DynamicItemTemplate objTemplate = (DynamicItemTemplate)Session[ CACHED_TEMPLATE ];
				if (objTemplate != null)
				{
					fvEditor.EditItemTemplate = objTemplate;
					try
					{
						fvEditor.DataBind();
					}
					catch (Exception)
					{
					}
				}

				// retrieve the table that stores the changed columns and reasons
				if (objChangedColumns == null)
				{
					objChangedColumns = (DataTable)Session[ CACHED_CHANGEDCOLS ];

					// if it is not Sessiond, create and Session it
					if (objChangedColumns == null)
					{
						// if table does not exist, create it and Session the structure
						objChangedColumns = new DataTable("ChangedColumns");
						...
						
						Session[ CACHED_CHANGEDCOLS ] = objChangedColumns;					
					}
				}

				// retrieve the index of the currently selected change in the list box of changes
				iPreviousIndex = Convert.ToInt32( Session[PREVIOUS_INDEX] );
			}
		}

		private void RetrieveTable(string strTableName, string strMasterIncidentId, string strCADID
									, string strVehicleRecId, string strRecId)
		{
			lblError.Text = "";

			// retrieve incident
			objDT = objDB.GetIncident(strTableName, strMasterIncidentId, strCADID, strVehicleRecId, strRecId);

			if (objDT.Rows.Count <= 0 && objDB.ErrorMessage.Equals(string.Empty))
			{
				lblError.Text = "Incident not found.";
				SetFocus(tbIncidentId);
				return;
			}

			Session.Remove(CACHED_TABLE);
			Session[ CACHED_TABLE ] = objDT;

			string strCheckUsersADGroup = System.Configuration.ConfigurationManager.AppSettings["CheckUsersADGroup"];
			if( strCheckUsersADGroup.Equals( "1" ) ) 
			{
				// check whether the user is enabled to edit incidents from the selected CAD
				bool bAuthorizedToEdit	= false;
				bAuthorizedToEdit = UserBelongsToADGroupNamed(strCADID);

				// if the user doesn't belong to the AD group for the selected CAD, check if he belongs to the AD group for the region 
				// where the CAD is
				if ( ! bAuthorizedToEdit )
				{
					// get the region to which the CAD belongs
					string strRegion = objDB.GetCADRegion(strCADID);

					if (strRegion.Equals(""))
						return;

					bAuthorizedToEdit = UserBelongsToADGroupNamed(strRegion);
				}

				if (!bAuthorizedToEdit)
				{
				    lblError.Text = string.Concat( "User is not authorized to edit incidents from the CAD ", strCADID, "." );
					lblIncidentAddress.Text = "";
					lblScrubbed.Text = "";
					SetFocus(tbIncidentId);
				    return;
				}
			}

			// retrieve table list of columns
			DataTable objColList = objDB.GetTableInfo(strTableName);

			// get list of read only column names from the Web.Config file
			string strReadOnly = System.Configuration.ConfigurationManager.AppSettings["ReadOnlyColumns"];
			strReadOnly = string.Format(",{0},", strReadOnly);

			// get list of calculated column names from the Web.Config file
			string strCalculated = System.Configuration.ConfigurationManager.AppSettings["CalculatedColumns"];
			strCalculated = string.Format(",{0},", strCalculated);

			fvEditor.DataKeyNames = new string[] { };

			// configure MaxLength, ReadOnly and other properties for each of the columns
			foreach (DataColumn objCol in objDT.Columns)
			{
				// find column in the list of columns
				DataRow objRow = objColList.Rows.Find(objCol.ColumnName);
				if (objRow != null)
				{
					// set the column length to the column length found in the database
					if (!Convert.IsDBNull(objRow["Length"]))
						objCol.MaxLength = Convert.ToInt32(objRow["Length"]);

					// primary keys will be flagged as read only
					if (strReadOnly.IndexOf(objCol.ColumnName) > 0)
					{
						objCol.ReadOnly = true;

						// add column to the form view's keys
						string[]	strKeys = new string[ fvEditor.DataKeyNames.Count() + 1 ];
						fvEditor.DataKeyNames.CopyTo( strKeys, 0 );
						strKeys[ fvEditor.DataKeyNames.Count() ] = objCol.ColumnName;
						fvEditor.DataKeyNames = strKeys;
					}

					// calculated columns will be flagged as read only
					if (strCalculated.IndexOf(objCol.ColumnName) > 0)
						objCol.ReadOnly = true;

					AssignCategoryToColumn(objCol);
				}
			}


			// create a template with the columns
			DynamicItemTemplate objTemplate = new DynamicItemTemplate(ListItemType.EditItem, objDT.Columns, true);
			objTemplate.PixelsPerCharacter = 4;
			objTemplate.NbrRows = 17;

			// load template with dropdownlist configuration parameters from the Web.Config file
			string strDDLColumns = System.Configuration.ConfigurationManager.AppSettings["DropDownListColumns"];
			string[] strDDLColumnsArray = strDDLColumns.Split(',');
			foreach (string strColumn in strDDLColumnsArray)
				objTemplate.DropDownListColumns.Add(strColumn);

			string strDDLValueFields = System.Configuration.ConfigurationManager.AppSettings["DropDownValueFields"];
			string[] strDDLValueFieldsArray = strDDLValueFields.Split(',');
			foreach (string strField in strDDLValueFieldsArray)
				objTemplate.DropDownValueField.Add(strField);

			string strDDLTextFields = System.Configuration.ConfigurationManager.AppSettings["DropDownTextFields"];
			string[] strDDLTextFieldsArray = strDDLTextFields.Split(',');
			foreach (string strField in strDDLTextFieldsArray)
				objTemplate.DropDownTextField.Add(strField);

			string strDDLDSTypes = System.Configuration.ConfigurationManager.AppSettings["DataSourceTypeNames"];
			string[] strDDLDSTypesArray = strDDLDSTypes.Split(',');
			foreach (string strDS in strDDLDSTypesArray)
				objTemplate.DataSourceTypeName.Add(strDS);

			string strDDLDSMethods = System.Configuration.ConfigurationManager.AppSettings["DataSourceSelectMethods"];
			string[] strDDLDSMethodsArray = strDDLDSMethods.Split(',');
			foreach (string strDSMethod in strDDLDSMethodsArray)
				objTemplate.DataSourceSelectMethod.Add(strDSMethod);

			// assign template to form view and add it to Session
			fvEditor.EditItemTemplate = objTemplate;
			Session.Remove(CACHED_TEMPLATE);
			Session[ CACHED_TEMPLATE ] = objTemplate;

			// bind the incident detail to the formview
			fvEditor.DataSource = null;
			fvEditor.DataSource = objDT;
			fvEditor.DataBind();
			SetFocus(fvEditor);
		}

		private bool UserBelongsToADGroupNamed( string strCADID )
		{
			bool bAuthorizedToEdit	= false;

			// look in the list of AD groups to which the user belongs
			foreach (System.Security.Principal.IdentityReference objIR in Request.LogonUserIdentity.Groups)
			{
				// if the name of the AD group contains the CAD abbreviation, the user is authorized for the CAD
				System.Security.Principal.IdentityReference objAccount = objIR.Translate(typeof(System.Security.Principal.NTAccount));
				if (objAccount.Value.Contains(strCADID))
				{
					bAuthorizedToEdit = true;
					break;
				}
			}
			return bAuthorizedToEdit;
		}

		private static void AssignCategoryToColumn(DataColumn objCol)
		{
			// get list of categories to get a list of columns per category from the Web.Config file
			string strCategoryList = System.Configuration.ConfigurationManager.AppSettings["ListOfCategories"];
			string[] strCategories = strCategoryList.Split(',');

			// 1st category is the default (Main)
			objCol.Namespace = strCategories[0];

			foreach (string strCategory in strCategories)
			{
				// skip first category (which is the default)
				//if (strCategory.Equals(strCategories[0]))
				//    continue;

				// get a list of columns for this category from the Web.Config file
				string strListName = string.Format("{0}CategoryColumns", strCategory);
				string strColumnList = System.Configuration.ConfigurationManager.AppSettings[strListName];
				string[] strColumns = strColumnList.Split(',');

				// test each partial column name to see if the column should be assigned to this category
				foreach (string strColumnNamePart in strColumns)
				{
					if ( objCol.ColumnName.Equals( strColumnNamePart ) )
					{
						// assign category and return to the main process in order to process the next column
						objCol.Namespace = strCategory;
						return;
					}

					if (objCol.ColumnName.IndexOf(strColumnNamePart) >= 0)
					{
						// assign category and check the next category to see if it fits better
						objCol.Namespace = strCategory;
						break;
					}
				}
			}
		}

		protected void fvEditor_ItemUpdating(object sender, FormViewUpdateEventArgs e)
		{
			// IMPORTANT:  e.OldValues will be empty because this is a dynamic templated formview
			// and the bound controls are nested in a multiview/view/table/tablerow/tablecell like below:
			//					   TableCell   MultiView   View		Table	   TableRow	TableCell   TextBox
			//((TextBox)fvEditor.Row.Controls[0].Controls[1].Controls[0].Controls[0].Controls[4].Controls[1].Controls[0] )

			string strNewValue = "", strFormattedOldValue = "", strColumnName = "", strFormat = "";

			// reset list of changed fields
			objChangedColumns.Rows.Clear();

			// the formview's datarowview will have the old values
			DataRowView objDRV = (DataRowView)fvEditor.DataItem;
			if (objDRV == null) return;

			// get old and new values for each column on the table
			foreach (DataColumn objDC in objDRV.Row.Table.Columns)
			{
				strColumnName = objDC.ColumnName;

				// fetch format masks for each column type
				string strType = objDC.DataType.ToString();
				switch (strType)
				{
					case "System.DateTime":
						strFormat = "{0:" + ((DynamicItemTemplate)fvEditor.EditItemTemplate).DateFormat + "}";
						break;

					case "System.Decimal":
						strFormat = "{0:" + ((DynamicItemTemplate)fvEditor.EditItemTemplate).DecimalFormat + "}";
						break;

					case "System.Int16":
					case "System.Int32":
					case "System.Int64":
					case "int":
						strFormat = "{0:" + ((DynamicItemTemplate)fvEditor.EditItemTemplate).IntegerFormat + "}";
						break;

					default:
						strFormat = "{0}";
						break;
				}

				if (strType.Equals("System.Boolean") && ! Convert.IsDBNull( objDRV.Row[strColumnName] ))
					strFormattedOldValue = ((bool) objDRV.Row[strColumnName] ? "Yes" : "No");
				else
					// apply format mask to the value
					strFormattedOldValue = string.Format(strFormat, objDRV.Row[strColumnName]);

				strNewValue = e.NewValues[strColumnName].ToString();

				// if old value differs from the new value (has been changed), add column to the list of changes
				if (!strFormattedOldValue.Equals(strNewValue))
				{
					// validate new value according to type
					if (!strNewValue.Equals(""))
						switch (strType)
						{
							case "System.DateTime":
								DateTime dtValue;
								if (!DateTime.TryParseExact(strNewValue
										, "MM/dd/yyyy hh:mm:ss tt"
										, new System.Globalization.CultureInfo("en-US")
										, System.Globalization.DateTimeStyles.AllowWhiteSpaces
										, out dtValue))
								{

									if (!DateTime.TryParseExact(strNewValue
											, "MM/dd/yyyy HH:mm:ss"
											, new System.Globalization.CultureInfo("en-US")
											, System.Globalization.DateTimeStyles.AllowWhiteSpaces
											, out dtValue))
									{
										// failed validation
										lblError.Text = string.Concat("The date for column ", strColumnName
											, " is invalid. The format must be either MM/dd/yyyy hh:mm:ss or MM/dd/yyyy hh:mm AM/PM.");
										return;
									}
								}
								strNewValue = dtValue.ToString();
								break;

							case "System.Decimal":
								Decimal dcValue;
								if (!Decimal.TryParse(strNewValue
										, out dcValue))
								{
									// failed validation
									lblError.Text = string.Concat("The decimal value for column ", strColumnName, " is invalid.");
									return;
								}
								strNewValue = dcValue.ToString();

								break;

							case "System.Int16":
							case "System.Int32":
							case "System.Int64":
							case "int":
								int iValue;
								if (!int.TryParse(strNewValue
										, out iValue))
								{
									// failed validation
									lblError.Text = string.Concat("The value for column ", strColumnName, " is invalid.");
									return;
								}
								strNewValue = iValue.ToString();

								break;

							default:
								break;
						}

					if (strNewValue.Equals(""))
						strNewValue = "NULL";

					string strLegibleColumn = DynamicItemTemplate.FormatColumnName(strColumnName);

					// if a field value was changed, add row to table in order to prompt for a reason
					DataRow objRow  = objChangedColumns.NewRow();
					objRow["Table_Name"]		= ViewState["Table_Name"].ToString();
					...
					objRow["Field_Name"]		= strColumnName;
					objRow["LegibleColumnName"] = strLegibleColumn + " (*)";
					objRow["Reason"]			= "";
					objRow["Changed_To"]		= strNewValue;
					objRow["Changed_From"]		= strFormattedOldValue;
					objRow["UserName"]			= Page.User.Identity.Name;
					objRow["FromHost"]			= Request.UserHostAddress;
					objRow["ChangeKey"]		= 0;
					objChangedColumns.Rows.Add(objRow);

					Session[ CACHED_CHANGEDCOLS ] = objChangedColumns;
				}

			}   // END foreach (DataColumn objDC in objDRV.Row.Table.Columns)

			// make the list of changed columns appear if a column was changed
			if (objChangedColumns.Rows.Count > 0)
			{
				pnlChanges.Visible = true;
				pnlChanges.CssClass = "ChangePanel";
				SwitchDialogHeader( false );
				pnlMessage.Visible = false;

				lbChanges.DataTextField = "LegibleColumnName";
				lbChanges.DataValueField = "Field_Name";
				lbChanges.DataSource = objChangedColumns;
				lbChanges.DataBind();
				lbChanges.SelectedIndex = 0;
				RetrieveReasonForNewSelectedChange();
				//iPreviousIndex = -1;
			}
		}

		protected void SwitchDialogHeader(bool bError)
		{
			if (bError)
				lblChangesError.CssClass = "Error";
			else
			{
				lblChangesError.Text = "Enter the reason for each change:";
				lblChangesError.CssClass = "DialogHeader";
			}
		}


		protected void fvEditor_ItemUpdated(Object sender, FormViewUpdatedEventArgs e)
		{
			if (e.Exception != null)
			{
				lblError.Text = e.Exception.Message;
			}

			e.KeepInEditMode = true;
		}
...

		protected void fvEditor_ModeChanging(object sender, FormViewModeEventArgs e)
		{
			// clear the incident detail binding in the formview
			fvEditor.DataSource = null;
			fvEditor.DataBind();

			// clear the list of changes from the Session
			Session.Remove(CACHED_CHANGEDCOLS);

			lblIncidentAddress.Text = "";

			if (e.CancelingEdit)
				if (tvIncident.Nodes.Count > 0)
				{
					tvIncident.Nodes[0].Select();
					tvIncident_SelectedNodeChanged(sender, new EventArgs());
				}
			SetFocus(tvIncident);
		}

		protected void btnOk_Click(object sender, EventArgs e)
		{
			pnlMessage.Visible = false;
		}

	}
}
