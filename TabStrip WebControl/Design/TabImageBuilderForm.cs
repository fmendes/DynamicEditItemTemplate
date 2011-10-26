using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

using SCS.Tools.ImageManipulation;

namespace SCS.Web.UI.WebControls.Design
{
	public partial class TabImageBuilderForm : Form
	{
		#region Enums
		private enum CornerType { Rounded = 1, Chamfered = 2 }
		private enum TabType { Symetrical = 1, RightLean = 2, LeftLean = 3 }
		private enum CornerSideType { Left = 1, Right = 2 }
		private enum TabStateType { Normal = 1, Selected = 2 }
		#endregion

		#region Fields
		private TabStrip _tabStrip;

		private Bitmap _tabRightImage;
		private Bitmap _tabLeftImage;
		private Bitmap _tabRightSelectedImage;
		private Bitmap _tabLeftSelectedImage;
		private Bitmap _tabLeftEdgeImage;
		private Bitmap _tabBottomImage;

		private Color _tabColor;
		private Color _borderColor;
		private Color _selectedColor;

		private CornerType _cornerType = CornerType.Rounded;
		private TabType _tabType = TabType.Symetrical;

		private int _tabWidth;
		private int _tabHeight;
		private int _borderWidth;
		private int _cornerWidth;
		private int _cornerHeight;

		private string _outputDirectory;
		private bool _outputDirectoryChanged = false;

		private static string _overwriteMessage = "TabStrip images already exist in this location. Are you sure you want to overwrite them?";
		private static string _saveSucceedMessage = "Images saved successfully to {0}.";
		private static string _saveFailedMessage = "Unable to save to {0}.";
		private static string _saveFailedUnauthorizedMessage = "Unable to save images to {0}. Please verify that you have write permissions assigned to all tab images and that they are checked out.";
		private static string _saveFailedNoDirectoryMessage = "Unable to save files because the specified directory does not exist. Would you like create this directory?";
		private static string _saveFailedFileInUseMessage = "Unable to save files at this time because another process is using them. Please save them to another directory.";
		private static string _foundSettingsMessage = "A saved settings file is located in this directory. Would you like to load it's settings?";
		private static string _overrideOutputDirMessage = "The saved settings file has another output directory listed. Would you like to change you current output directory to it?";

		private static string _settingsFileName = "TabStripSettings.xml";
		#endregion

		//public TabImageBuilderForm()
		//{
		//	InitializeComponent();
		//}
		public TabImageBuilderForm(TabStrip component)
		{
			InitializeComponent();

			_tabStrip = component;
			this.SetOutputPath();
		}

		private void CreateImages()
		{
			if (!this.VerifyInput())
				return;

			_tabLeftImage = this.CreateTabLeftImage(TabStateType.Normal);
			_tabRightImage = this.CreateTabRightImage(TabStateType.Normal);
			_tabLeftSelectedImage = this.CreateTabLeftImage(TabStateType.Selected);
			_tabRightSelectedImage = this.CreateTabRightImage(TabStateType.Selected);
			_tabLeftEdgeImage = this.CreateLeftEdgeImage();
			_tabBottomImage = this.CreateTabBottomImage();

			this.SaveCommand.Enabled = true;
			this.DisplayPreview();
		}
		private void DisplayPreview()
		{
			if (PreviewNormalSelect.Checked)
			{
				this.TabLeftImage.Image = _tabLeftImage;
				this.TabRightImage.Image = _tabRightImage;
			}
			else
			{
				this.TabLeftImage.Image = _tabLeftSelectedImage;
				this.TabRightImage.Image = _tabRightSelectedImage;
			}

			this.TabLeftImage.Height = _tabLeftImage.Height;
			this.TabLeftImage.Width = _tabLeftImage.Width;

			this.TabRightImage.Height = _tabRightImage.Height;
			this.TabRightImage.Width = _tabRightImage.Width;

			this.TabRightImage.Left = this.TabLeftImage.Left + this.TabLeftImage.Width;
		}
		private void SaveImages()
		{
			try
			{
				if (!_outputDirectory.EndsWith("\\"))
					_outputDirectory += "\\";

				string path = _outputDirectory + "tab_left.gif";

				if (File.Exists(path))
				{
					DialogResult overwriteResult = MessageBox.Show(_overwriteMessage + Environment.NewLine + path, this.Text, MessageBoxButtons.OKCancel,
						MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

					if (overwriteResult == DialogResult.Cancel)
						return;
				}

				OctreeQuantizer quantizer = new OctreeQuantizer(255, 8);

				if (_tabLeftImage != null)
				{
					this.DeleteImage(path);

					using (Bitmap quantized = quantizer.Quantize(_tabLeftImage))
					{
						quantized.Save(path, ImageFormat.Gif);
					}
				}

				path = _outputDirectory + "tab_right.gif";

				if (_tabRightImage != null)
				{
					this.DeleteImage(path);

					using (Bitmap quantized = quantizer.Quantize(_tabRightImage))
					{
						quantized.Save(path, ImageFormat.Gif);
					}
				}

				path = _outputDirectory + "tab_right_on.gif";

				if (_tabRightSelectedImage != null)
				{
					this.DeleteImage(path);

					using (Bitmap quantized = quantizer.Quantize(_tabRightSelectedImage))
					{
						quantized.Save(path, ImageFormat.Gif);
					}
				}

				path = _outputDirectory + "tab_left_on.gif";

				if (_tabLeftSelectedImage != null)
				{
					this.DeleteImage(path);

					using (Bitmap quantized = quantizer.Quantize(_tabLeftSelectedImage))
					{
						quantized.Save(path, ImageFormat.Gif);
					}
				}

				path = _outputDirectory + "tab_underline.gif";

				if (_tabBottomImage != null)
				{
					this.DeleteImage(path);

					//_tabBottomImage.Save(path, ImageFormat.Gif);

					//ImageHelper helper = new ImageHelper();
					//helper.SaveGifWithNewColorTable(_tabBottomImage, path, 1024, true);

					using (Bitmap quantized = quantizer.Quantize(_tabBottomImage))
					{
						quantized.Save(path, ImageFormat.Gif);
					}
				}

				path = _outputDirectory + "spacer.gif";

				if (_tabLeftEdgeImage != null)
				{
					this.DeleteImage(path);

					using (Bitmap quantized = quantizer.Quantize(_tabLeftEdgeImage))
					{
						quantized.Save(path, ImageFormat.Gif);
					}
				}

				MessageBox.Show(string.Format(_saveSucceedMessage, _outputDirectory),
					this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

				//DTEHelper helper = new DTEHelper();

				//if (helper.IsFileInCurrentProject(_outputDirectory))
				//{
				//	string message = string.Format(_saveSucceedMessageQuestion, _outputDirectory, _tabStrip.ID);

				//	DialogResult enableResult = MessageBox.Show(message, this.Text, MessageBoxButtons.YesNo,
				//		MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

				//	if (enableResult == DialogResult.Yes)
				//	{
				//		string imageFolder = helper.ResolveLocalPathToCurrentWeb(_outputDirectory);

				//		_tabStrip.LeftTabImage = imageFolder + "tab_left.gif";
				//		_tabStrip.RightTabImage = imageFolder + "tab_right.gif";
				//		_tabStrip.SelectedLeftTabImage = imageFolder + "tab_left_on.gif";
				//		_tabStrip.SelectedRightTabImage = imageFolder + "tab_right_on.gif";
				//		_tabStrip.BottomImage = imageFolder + "tab_underline.gif";						
				//	}

				//	 TODO: need to figure out how to perist the files in the markup.
				//}
				//else
				//{
				//	MessageBox.Show(string.Format(_saveSucceedMessage, _outputDirectory),
				//		this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				//}

				this.SaveSettings();
			}
			catch (UnauthorizedAccessException e)
			{
				string message = string.Format(_saveFailedUnauthorizedMessage, _outputDirectory);

				Console.Write("Exception Details: " + e.Message);
				Console.Write("Exception Type: " + e.GetType().ToString());

				if (e.InnerException != null)
					message += " " + e.InnerException;

				MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (DirectoryNotFoundException dex)
			{
				DialogResult createDirResult = MessageBox.Show(_saveFailedNoDirectoryMessage, this.Text, MessageBoxButtons.OKCancel,
						MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

				Console.Write("Exception Details: " + dex.Message);
				Console.Write("Exception Type: " + dex.GetType().ToString());

				if (createDirResult != DialogResult.OK)
					return;

				Directory.CreateDirectory(_outputDirectory);
				this.SaveImages();
			}
			catch (IOException ioe)
			{
				MessageBox.Show(_saveFailedFileInUseMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

				Console.Write("Exception Details: " + ioe.Message);
				Console.Write("Exception Type: " + ioe.GetType().ToString());

			}
			catch (Exception ex)
			{
				string message = string.Format(_saveFailedMessage, _outputDirectory);

				message += " Exception Details: " + ex.Message;
				message += " Exception Type: " + ex.GetType().ToString();

				if (ex.InnerException != null)
					message += " " + ex.InnerException;

				MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void DeleteImage(string path)
		{
			if (File.Exists(path))
			{
				File.SetAttributes(path, FileAttributes.Normal);
				File.Delete(path);
			}
		}
		private void SetDefaults()
		{
			// set default values
			_tabHeight = Convert.ToInt32(this.TabHeightEntry.Text);
			_tabWidth = Convert.ToInt32(this.TabWidthEntry.Text);
			_borderWidth = Convert.ToInt32(this.BorderWidthEntry.Text);

			_cornerHeight = Convert.ToInt32(this.CornerHeightEntry.Text);
			_cornerHeight *= 2;
			_cornerWidth = Convert.ToInt32(this.CornerWidthEntry.Text);
			_cornerWidth *= 2;

			_tabColor = ColorTranslator.FromHtml(this.TabColorEntry.Text);
			_borderColor = ColorTranslator.FromHtml(this.BorderColorEntry.Text);
			_selectedColor = ColorTranslator.FromHtml(this.SelectedColorEntry.Text);

		}
		private void SetOutputPath()
		{
			_outputDirectory = DTEHelper.GetActiveDocumentDirectory();

			//_outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			//string basePath = helper.GetCurrentProjectPropertyValue(ProjectPropertyType.LocalPath);
			//string imageUrl = _tabStrip.BottomImage;

			//if (imageUrl.StartsWith("~"))
			//	imageUrl = imageUrl.Substring(1);

			//if (imageUrl.StartsWith("/"))
			//	imageUrl = imageUrl.Substring(1);

			//imageUrl = imageUrl.Substring(0, imageUrl.LastIndexOf("/") + 1);
			//imageUrl = imageUrl.Replace("/", @"\");

			//_outputDirectory = basePath + imageUrl.Replace("/'", @"\");
			//this.OutputPathEntry.Text = _outputDirectory;

			if (!_outputDirectory.EndsWith(@"\"))
				_outputDirectory += @"\";

			this.OutputPathEntry.Text = _outputDirectory;
		}
		private bool VerifyInput()
		{
			if (_tabWidth < 20)
			{
				MessageBox.Show("Please enter a tab width of 20 pixels or more.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_tabHeight < 10)
			{
				MessageBox.Show("Please enter a tab height of 10 pixels or more.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_borderWidth < 0 || _borderWidth > 20)
			{
				MessageBox.Show("Please enter a border width between 1 and 10.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_cornerWidth < 0 || _cornerWidth > _tabWidth)
			{
				MessageBox.Show("Please enter a corner width greater than 0 and less than the width of the tab.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_cornerHeight < 0 || _cornerHeight > _tabHeight)
			{
				MessageBox.Show("Please enter a corner height greater than 0 and less than the height of the tab.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_tabColor.IsEmpty)
			{
				MessageBox.Show("Please select or enter a tab color.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_borderColor.IsEmpty)
			{
				MessageBox.Show("Please select or enter a border color.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_tabColor.Equals(_borderColor))
			{
				MessageBox.Show("Please select or enter a border color different than the tab color.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_cornerWidth > _tabWidth)
			{
				MessageBox.Show("Please enter tab width greater than the corner width.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			if (_cornerHeight > _tabHeight)
			{
				MessageBox.Show("Please enter a tab height greater than the corner height.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			return true;
		}
		private void SaveSettings()
		{
			TabStripSettings settings = new TabStripSettings();

			settings.SavedTabStripSettings.TabColor = ColorTranslator.ToHtml(_tabColor);
			settings.SavedTabStripSettings.BorderColor = ColorTranslator.ToHtml(_borderColor);
			settings.SavedTabStripSettings.SelectedTabColor = ColorTranslator.ToHtml(_selectedColor);

			settings.SavedTabStripSettings.BorderWidth = _borderWidth;
			settings.SavedTabStripSettings.BottomPadding = -1;
			settings.SavedTabStripSettings.TabWidth = _tabWidth;
			settings.SavedTabStripSettings.TabHeight = _tabHeight;

			settings.SavedTabStripSettings.CornerHeight = _cornerHeight / 2;
			settings.SavedTabStripSettings.CornerWidth = _cornerWidth / 2;

			settings.SavedTabStripSettings.TabType = Enum.GetName(typeof(TabType), _tabType);
			settings.SavedTabStripSettings.TabCornerType = Enum.GetName(typeof(CornerType), _cornerType);

			settings.SavedTabStripSettings.OutputDirectory = _outputDirectory;

			try
			{
				settings.Serialize(_outputDirectory + _settingsFileName, settings);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void LoadSettings()
		{
			TabStripSettings settings = new TabStripSettings();

			settings = settings.Deserialize(_outputDirectory + _settingsFileName);

			if (!string.IsNullOrEmpty(settings.SavedTabStripSettings.TabColor))
			{
				_tabColor = ColorTranslator.FromHtml(settings.SavedTabStripSettings.TabColor);
				this.TabColorEntry.Text = settings.SavedTabStripSettings.TabColor;
			}

			if (!string.IsNullOrEmpty(settings.SavedTabStripSettings.BorderColor))
			{
				_borderColor = ColorTranslator.FromHtml(settings.SavedTabStripSettings.BorderColor);
				this.BorderColorEntry.Text = settings.SavedTabStripSettings.BorderColor;
			}

			if (!string.IsNullOrEmpty(settings.SavedTabStripSettings.SelectedTabColor))
			{
				_selectedColor = ColorTranslator.FromHtml(settings.SavedTabStripSettings.SelectedTabColor);
				this.SelectedColorEntry.Text = settings.SavedTabStripSettings.SelectedTabColor;
			}

			_borderWidth = settings.SavedTabStripSettings.BorderWidth;
			this.BorderWidthEntry.Text = _borderWidth.ToString();

			//_pottomPadding = settings.SavedTabStripSettings.BottomPadding;
			_tabWidth = settings.SavedTabStripSettings.TabWidth;
			this.TabWidthEntry.Text = _tabWidth.ToString();

			_tabHeight = settings.SavedTabStripSettings.TabHeight;
			this.TabHeightEntry.Text = _tabHeight.ToString();

			_cornerHeight = settings.SavedTabStripSettings.CornerHeight * 2;
			this.CornerHeightEntry.Text = settings.SavedTabStripSettings.CornerHeight.ToString();

			_cornerWidth = settings.SavedTabStripSettings.CornerWidth * 2;
			this.CornerWidthEntry.Text = settings.SavedTabStripSettings.CornerWidth.ToString();

			if (!string.IsNullOrEmpty(settings.SavedTabStripSettings.TabType))
			{
				_tabType = (TabType)Enum.Parse(typeof(TabType), settings.SavedTabStripSettings.TabType);

				switch (_tabType)
				{
					case TabType.LeftLean:
						this.LeanLeftOption.Checked = true;
						break;

					case TabType.RightLean:
						this.LeanRightOption.Checked = true;
						break;

					default:
						this.SymetricalOption.Checked = true;
						break;
				}
			}

			if (!string.IsNullOrEmpty(settings.SavedTabStripSettings.TabCornerType))
			{
				_cornerType = (CornerType)Enum.Parse(typeof(CornerType), settings.SavedTabStripSettings.TabCornerType);

				switch (_cornerType)
				{
					case CornerType.Chamfered:
						this.ChamferedOption.Checked = true;
						break;

					default:
						this.RoundedOption.Checked = true;
						break;
				}
			}

			if (this.OutputPathEntry.Text.Trim() != settings.SavedTabStripSettings.OutputDirectory.Trim())
			{
				DialogResult result = MessageBox.Show(_overrideOutputDirMessage, 
					this.Text, MessageBoxButtons.YesNo,
					MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

				if (result == DialogResult.Yes)
				{
					_outputDirectory = settings.SavedTabStripSettings.OutputDirectory;
					this.OutputPathEntry.Text = _outputDirectory;
				}
			}
		}

		private Bitmap CreateTabLeftImage(TabStateType tabState)
		{
			int cornerHeight = 0;
			int cornerWidth = 0;

			if (_tabType == TabType.RightLean || _tabType == TabType.Symetrical)
			{
				cornerHeight = _cornerHeight;
				cornerWidth = _cornerWidth;
			}

			int imageWidth = _cornerWidth / 2 + 1;

			Bitmap tabLeftImage = new Bitmap(imageWidth, _tabHeight, PixelFormat.Format32bppPArgb);
			Graphics graphics = Graphics.FromImage(tabLeftImage);

			this.FillBackground(graphics, imageWidth, tabState);
			Pen pen = this.GetPen();

			if (cornerWidth > 0 && cornerHeight > 0)
			{
				if (_cornerType == CornerType.Rounded)
					this.DrawCornerArc(graphics, pen, CornerSideType.Left);
				else
					this.DrawCornerLine(graphics, pen, CornerSideType.Left, imageWidth);
			}

			// draw left vertical line

			Point pointA = new Point();
			Point pointB = new Point();

			int penWidth = Convert.ToInt32(pen.Width);

			pointA.X = penWidth / 2;
			pointA.Y = cornerHeight / 2;
			pointB.X = penWidth / 2;
			pointB.Y = _tabHeight;

			graphics.DrawLine(pen, pointA, pointB);

			// draw horizontal line

			if (cornerWidth == 0 && cornerHeight == 0)
			{
				pointA.X = 0;
				pointA.Y = penWidth / 2;
				pointB.X = _cornerWidth;
				pointB.Y = penWidth / 2; ;
			}

			graphics.DrawLine(pen, pointA, pointB);

			Color currentTabColor = (tabState == TabStateType.Normal) ? _tabColor : _selectedColor;

			for (int y = 0; y < cornerHeight / 2; y++)
			{
				for (int x = 0; x < imageWidth - 1; x++)
				{
					if (!this.ClearPixel(tabLeftImage, currentTabColor, x, y))
						break;
				}
			}

			return tabLeftImage;
		}
		private Bitmap CreateTabRightImage(TabStateType tabState)
		{
			int cornerHeight = 0;
			int cornerWidth = 0;

			if (_tabType == TabType.LeftLean || _tabType == TabType.Symetrical)
			{
				cornerHeight = _cornerHeight;
				cornerWidth = _cornerWidth;
			}

			int imageWidth = _tabWidth - (_cornerWidth / 2) - 1;

			Bitmap tabRightImage = new Bitmap(imageWidth, _tabHeight, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(tabRightImage);

			this.FillBackground(graphics, imageWidth, tabState);
			Pen pen = this.GetPen();

			if (cornerWidth > 0 && cornerHeight > 0)
			{
				if (_cornerType == CornerType.Rounded)
					this.DrawCornerArc(graphics, pen, CornerSideType.Right);
				else
					this.DrawCornerLine(graphics, pen, CornerSideType.Right, imageWidth);
			}

			// draw right image vertical line
			Point pointA = new Point();
			Point pointB = new Point();

			int penWidth = Convert.ToInt32(pen.Width);

			pointA.X = imageWidth - (1 + penWidth / 2);
			pointA.Y = cornerHeight / 2;
			pointB.X = imageWidth - (1 + penWidth / 2);
			pointB.Y = _tabHeight;

			graphics.DrawLine(pen, pointA, pointB);

			// draw right image horizontal line
			pointA.X = 0;
			pointA.Y = penWidth / 2;
			pointB.X = (imageWidth - cornerWidth + (cornerWidth / 2)) - 1;
			pointB.Y = penWidth / 2;

			graphics.DrawLine(pen, pointA, pointB);

			Color currentTabColor = (tabState == TabStateType.Normal) ? _tabColor : _selectedColor;

			for (int y = 0; y < cornerHeight / 2; y++)
			{
				for (int x = imageWidth - 1; x > pointB.X; x--)
				{
					if (!this.ClearPixel(tabRightImage, currentTabColor, x, y))
						break;
				}
			}

			return tabRightImage;

		}
		private Bitmap CreateLeftEdgeImage()
		{
			Bitmap leftEdgeImage = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

			// set to clear pixel
			leftEdgeImage.SetPixel(0, 0, Color.FromArgb(0, 255, 255, 255));

			return leftEdgeImage;
		}
		private Bitmap CreateTabBottomImage()
		{
			Bitmap bottomImage = new Bitmap(22, 25, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(bottomImage);

			Brush brush = new SolidBrush(_tabColor);
			graphics.FillRectangle(brush, 0, 0, 22, 25);

			Pen pen = this.GetPen();
			graphics.DrawLine(pen, 0, 24, 21, 24);

			for (int y = 0; y < 24; y++)
			{
				for (int x = 0; x < 22; x++)
				{
					bottomImage.SetPixel(x, y, Color.FromArgb(0, 255, 255, 255));
				}
			}

			return bottomImage;
		}

		private Pen GetPen()
		{
			return new Pen(_borderColor, _borderWidth);
		}

		private void FillBackground(Graphics graphics, int imageWidth, TabStateType tabState)
		{
			Brush brush = new SolidBrush((tabState == TabStateType.Normal) ? _tabColor : _selectedColor);
			graphics.FillRectangle(brush, 0, 0, imageWidth, _tabHeight);
		}
		private void DrawCornerArc(Graphics graphics, Pen pen, CornerSideType side)
		{
			int x;
			int y;
			int penWidth = Convert.ToInt32(pen.Width);

			float sweepAngle;

			if (side == CornerSideType.Left)
			{
				x = penWidth / 2;
				y = penWidth / 2;
				sweepAngle = -90F;
			}
			else
			{
				//x = ((_tabWidth - 1) - (_cornerWidth * 2) - 1) - (penWidth / 2);

				x = ((_tabWidth - 1) - (_cornerWidth * 2) - 1) + (_cornerWidth / 2) - (penWidth / 2);

				y = penWidth / 2;
				sweepAngle = 90F;
			}

			graphics.DrawArc(pen, x, y, _cornerWidth, _cornerHeight, 270, sweepAngle);
		}
		private void DrawCornerLine(Graphics graphics, Pen pen, CornerSideType side, int imageWidth)
		{
			Point point1;
			Point point2;

			if (side == CornerSideType.Left)
			{
				point1 = new Point(_cornerWidth / 2, 0);
				point2 = new Point(0, _cornerHeight / 2);
			}
			else
			{
				point1 = new Point(imageWidth - (_cornerWidth / 2) - 1, 0);
				point2 = new Point(imageWidth, _cornerHeight / 2 + 1);
			}
			graphics.DrawLine(pen, point1, point2);
		}

		private bool ClearPixel(Bitmap image, Color currentTabColor, int x, int y)
		{
			Color color = image.GetPixel(x, y);

			if (color.A.Equals(currentTabColor.A)
				&& color.R.Equals(currentTabColor.R)
				&& color.G.Equals(currentTabColor.G)
				&& color.B.Equals(currentTabColor.B))
			{
				image.SetPixel(x, y, Color.FromArgb(0, 255, 255, 255));
			}
			else if (color.A.Equals(_borderColor.A)
				&& color.R.Equals(_borderColor.R)
				&& color.G.Equals(_borderColor.G)
				&& color.B.Equals(_borderColor.B))
			{
				return false;
			}
			return true;
		}

		#region Event Handlers

		#region Text Boxes
		private void TabWidthEntry_Leave(object sender, EventArgs e)
		{
			if (!int.TryParse(this.TabWidthEntry.Text, out _tabWidth))
			{
				MessageBox.Show("Please enter an integer value.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.TabWidthEntry.Focus();
				return;
			}
			this.CreateImages();
		}
		private void TabHeightEntry_Leave(object sender, EventArgs e)
		{
			if (!int.TryParse(this.TabHeightEntry.Text, out _tabHeight))
			{
				MessageBox.Show("Please enter an integer value.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.TabHeightEntry.Focus();
				return;
			}
			this.CreateImages();
		}
		private void BorderWidthEntry_Leave(object sender, EventArgs e)
		{
			if (!int.TryParse(this.BorderWidthEntry.Text, out _borderWidth))
			{
				MessageBox.Show("Please enter an integer value.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.BorderWidthEntry.Focus();
				return;
			}
			this.CreateImages();
		}
		private void TabColorEntry_Leave(object sender, EventArgs e)
		{
			try
			{
				_tabColor = ColorTranslator.FromHtml(this.TabColorEntry.Text);
			}
			catch
			{
				MessageBox.Show("Please enter a valid HTML color.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.TabColorEntry.Focus();
				return;
			}
			this.CreateImages();
		}
		private void CornerWidthEntry_Leave(object sender, EventArgs e)
		{
			if (!int.TryParse(this.CornerWidthEntry.Text, out _cornerWidth))
			{
				MessageBox.Show("Please enter an integer value.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.CornerWidthEntry.Focus();
				return;
			}
			_cornerWidth *= 2;
			this.CreateImages();
		}
		private void CornerHeightEntry_Leave(object sender, EventArgs e)
		{
			if (!int.TryParse(this.CornerHeightEntry.Text, out _cornerHeight))
			{
				MessageBox.Show("Please enter an integer value.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.CornerHeightEntry.Focus();
				return;
			}
			_cornerHeight *= 2;
			this.CreateImages();
		}
		private void SelectedColorEntry_Leave(object sender, EventArgs e)
		{
			try
			{
				_selectedColor = ColorTranslator.FromHtml(this.SelectedColorEntry.Text);
			}
			catch
			{
				MessageBox.Show("Please enter a valid HTML color.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.SelectedColorEntry.Focus();
				return;
			}
			this.CreateImages();
		}
		private void BorderColorEntry_Leave(object sender, EventArgs e)
		{
			try
			{
				_borderColor = ColorTranslator.FromHtml(this.BorderColorEntry.Text);
			}
			catch
			{
				MessageBox.Show("Please enter a valid HTML color.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.BorderColorEntry.Focus();
			}
			this.CreateImages();
		}
		private void OutputPathEntry_Leave(object sender, EventArgs e)
		{
			_outputDirectory = OutputPathEntry.Text;

			if (!_outputDirectory.EndsWith(@"\"))
				_outputDirectory += @"\";

			this.OutputPathEntry.Text = _outputDirectory;

			if (_outputDirectoryChanged && File.Exists(_outputDirectory + _settingsFileName))
			{
				DialogResult overwriteResult = MessageBox.Show(_foundSettingsMessage, this.Text, MessageBoxButtons.YesNo,
					MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

				if (overwriteResult == DialogResult.No)
					return;

				try
				{
					this.LoadSettings();
					this.CreateImages();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				_outputDirectoryChanged = false;
			}
		}
		private void OutputPathEntry_TextChanged(object sender, EventArgs e)
		{
			_outputDirectoryChanged = true;
		}
		#endregion

		#region Buttons
		private void TabColorSelectCommand_Click(object sender, EventArgs e)
		{
			ColorDialog dialog = new ColorDialog();

			dialog.AllowFullOpen = true;
			dialog.ShowHelp = false;
			dialog.Color = _tabColor;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_tabColor = dialog.Color;
				this.TabColorEntry.Text = ColorTranslator.ToHtml(_tabColor);
				this.CreateImages();
			}
		}
		private void BorderColorSelectCommand_Click(object sender, EventArgs e)
		{
			ColorDialog dialog = new ColorDialog();

			dialog.AllowFullOpen = true;
			dialog.ShowHelp = false;
			dialog.Color = _borderColor;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_borderColor = dialog.Color;
				this.BorderColorEntry.Text = ColorTranslator.ToHtml(_borderColor);
				this.CreateImages();
			}
		}
		private void SelectedColorSelectCommand_Click(object sender, EventArgs e)
		{
			ColorDialog dialog = new ColorDialog();

			dialog.AllowFullOpen = true;
			dialog.ShowHelp = false;
			dialog.Color = _selectedColor;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_selectedColor = dialog.Color;
				this.SelectedColorEntry.Text = ColorTranslator.ToHtml(_selectedColor);
				this.CreateImages();
			}
		}
		private void PathSelectCommand_Click(object sender, EventArgs e)
		{
			this.FolderBrowserDialog.SelectedPath = _outputDirectory;
			DialogResult result = this.FolderBrowserDialog.ShowDialog();
			
			if (result == DialogResult.OK)
			{
				_outputDirectory = FolderBrowserDialog.SelectedPath;

				if (!_outputDirectory.EndsWith(@"\"))
					_outputDirectory += @"\";

				this.OutputPathEntry.Text = _outputDirectory;

				if (File.Exists(_outputDirectory + _settingsFileName))
				{
					DialogResult overwriteResult = MessageBox.Show(_foundSettingsMessage, this.Text, MessageBoxButtons.YesNo,
						MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

					if (overwriteResult == DialogResult.No)
						return;

					try
					{
						this.LoadSettings();
						this.CreateImages();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
			}
		}

		private void CreateCommand_Click(object sender, EventArgs e)
		{
			this.CreateImages();
		}
		private void SaveCommand_Click(object sender, EventArgs e)
		{
			this.SaveImages();
		}
		private void CloseCommand_Click(object sender, EventArgs e)
		{
			this.Hide();

			// comment this out for VS design mode
			//Application.Exit();
		}
		#endregion

		#region Radio Buttons
		private void TabType_CheckedChanged(object sender, EventArgs e)
		{
			if ((((RadioButton)sender).Checked))
			{
				_tabType = (TabType)int.Parse((string)(((RadioButton)sender).Tag));
				this.CreateImages();
			}
		}
		private void TabCornerType_CheckedChanged(object sender, EventArgs e)
		{
			if ((((RadioButton)sender).Checked))
			{
				_cornerType = (CornerType)int.Parse((string)(((RadioButton)sender).Tag));
				this.CreateImages();
			}
		}
		private void TabPreviewTypeSelect_CheckedChanged(object sender, EventArgs e)
		{
			this.DisplayPreview();
		}
		#endregion

		private void TabImageBuilderForm_Load(object sender, EventArgs e)
		{
			try
			{
				_outputDirectory = this.OutputPathEntry.Text;
				this.LoadSettings();
			}
			catch (FileNotFoundException fex)
			{
				MessageBox.Show(fex.Message + " Default settings will be applied.",
					this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

				this.SetDefaults();
				this.CreateImages();
				return;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			this.CreateImages();
		}
		private void TabImageBuilderForm_FormClosing(object sender, EventArgs e)
		{
			if (_tabRightImage != null)
				_tabRightImage.Dispose();

			if (_tabRightImage != null)
				_tabRightImage.Dispose();

			if (_tabLeftImage != null)
				_tabLeftImage.Dispose();

			if (_tabRightSelectedImage != null)
				_tabRightSelectedImage.Dispose();

			if (_tabLeftSelectedImage != null)
				_tabLeftSelectedImage.Dispose();

			if (_tabLeftEdgeImage != null)
				_tabLeftEdgeImage.Dispose();

			if (_tabBottomImage != null)
				_tabBottomImage.Dispose();
		}

		#endregion
	}
}