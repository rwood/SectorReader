using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;

namespace SectorReader
{
	/// <summary>
	/// Simple SectorReader written for cs3400 by RogerWood
	/// </summary>
	public class SectorReader : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid HexTable;
		private System.Windows.Forms.DataGrid CharTable;
		//private System.ComponentModel.IContainer components;
		
		private System.Windows.Forms.Button CloseButton;
		private string selectedDrive;
		private uint BytesPerSector;
		private ulong Cylinders;
		private uint SectorsPerTrack;
		private uint TracksPerCylinder;
		private uint TotalSectors;
		private int currentSector; // Raw sector.
		private ulong currentCylinderOnDisk;
		private uint currentSectorOnTrack;
		private uint currentTrackOnCylinder;
		private int SectorsToShow;
		private byte[] buffer;
		private DataTable dataTableHex;
		private DataTable dataTableChar;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage HexadecimalTab;
		private System.Windows.Forms.TabPage CharactersTab;
		private System.Windows.Forms.ComboBox Drives;
		private System.Windows.Forms.Button PgDn;
		private System.Windows.Forms.Button PgUp;
		private string CommandLineArgs;
		private System.Windows.Forms.Label label1;
		private const uint IOCTL_DISK_GET_DRIVE_GEOMETRY = 0x70000;
		private System.Windows.Forms.Button Begin;
		private System.Windows.Forms.Button End;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblCurrentSector;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lblSectorsPerPage;
		private System.Windows.Forms.Label lblBytesPerSector;
		private System.Windows.Forms.Label lblCylindersPerDisk;
		private System.Windows.Forms.Label lblTracksPerCylinder;
		private System.Windows.Forms.Label lblSectorsPerTrack;
		private System.Windows.Forms.Label lblTotalSectors;
		private System.Windows.Forms.TextBox txtJmpSector;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtCylinder;
		private System.Windows.Forms.TextBox txtTrack;
		private System.Windows.Forms.TextBox txtSector;
		private System.Windows.Forms.Button btnGotoSpot;
		private System.Windows.Forms.Button GotoSector;
		private System.Windows.Forms.Label lblCyl;
		private System.Windows.Forms.Label lblTrk;
		private System.Windows.Forms.Label lblSctr;
		private DataTable list; //Drive List

		public SectorReader(string[] CLArgs)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			currentSector = 0;
			//maxSector = 0;
			if(CLArgs.Length != 0)
				CommandLineArgs = CLArgs[0];
			else
				CommandLineArgs = "";

			dataTableHex = new DataTable("hexs");
			dataTableChar= new DataTable("chars");
			ClearTables();
			string[] LogicalDrives = System.IO.Directory.GetLogicalDrives();
			list = new DataTable();
			list.Columns.Add(new DataColumn("Display", typeof(string)));
			DataRow inTempList = list.NewRow();
			inTempList["Display"] = string.Empty;
			list.Rows.Add(inTempList);
			inTempList = null;
			for(int i = 0; i < LogicalDrives.GetLength(0); i++)
			{
				inTempList = list.NewRow();
				inTempList["Display"] = LogicalDrives[i].Replace("\\", "");
				list.Rows.Add(inTempList);
				inTempList = null;
			}
			Drives.DataSource = list;
			Drives.DisplayMember = "Display";
			Drives.ValueMember = "Display"; 
		}

		private void ClearTables()
		{
			dataTableHex.Clear();
			dataTableChar.Clear();
			for(int i = 0; i < 32; i++)
			{
				dataTableHex.Columns.Add(i.ToString(),typeof(string));
				dataTableChar.Columns.Add(i.ToString(),typeof(string));
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.HexTable = new System.Windows.Forms.DataGrid();
			this.CharTable = new System.Windows.Forms.DataGrid();
			this.CloseButton = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.HexadecimalTab = new System.Windows.Forms.TabPage();
			this.CharactersTab = new System.Windows.Forms.TabPage();
			this.Drives = new System.Windows.Forms.ComboBox();
			this.PgDn = new System.Windows.Forms.Button();
			this.PgUp = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.Begin = new System.Windows.Forms.Button();
			this.End = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.lblCurrentSector = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lblSectorsPerPage = new System.Windows.Forms.Label();
			this.lblBytesPerSector = new System.Windows.Forms.Label();
			this.lblCylindersPerDisk = new System.Windows.Forms.Label();
			this.lblTracksPerCylinder = new System.Windows.Forms.Label();
			this.lblSectorsPerTrack = new System.Windows.Forms.Label();
			this.lblTotalSectors = new System.Windows.Forms.Label();
			this.txtJmpSector = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.txtCylinder = new System.Windows.Forms.TextBox();
			this.txtTrack = new System.Windows.Forms.TextBox();
			this.txtSector = new System.Windows.Forms.TextBox();
			this.lblCyl = new System.Windows.Forms.Label();
			this.lblTrk = new System.Windows.Forms.Label();
			this.lblSctr = new System.Windows.Forms.Label();
			this.btnGotoSpot = new System.Windows.Forms.Button();
			this.GotoSector = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.HexTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.CharTable)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.HexadecimalTab.SuspendLayout();
			this.CharactersTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// HexTable
			// 
			this.HexTable.AllowSorting = false;
			this.HexTable.CaptionVisible = false;
			this.HexTable.DataMember = "";
			this.HexTable.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.HexTable.Location = new System.Drawing.Point(0, 0);
			this.HexTable.Name = "HexTable";
			this.HexTable.ParentRowsVisible = false;
			this.HexTable.PreferredColumnWidth = 20;
			this.HexTable.ReadOnly = true;
			this.HexTable.RowHeadersVisible = false;
			this.HexTable.Size = new System.Drawing.Size(644, 568);
			this.HexTable.TabIndex = 0;
			// 
			// CharTable
			// 
			this.CharTable.AllowSorting = false;
			this.CharTable.CaptionVisible = false;
			this.CharTable.DataMember = "";
			this.CharTable.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.CharTable.Location = new System.Drawing.Point(0, 0);
			this.CharTable.Name = "CharTable";
			this.CharTable.ParentRowsVisible = false;
			this.CharTable.PreferredColumnWidth = 20;
			this.CharTable.ReadOnly = true;
			this.CharTable.RowHeadersVisible = false;
			this.CharTable.Size = new System.Drawing.Size(644, 658);
			this.CharTable.TabIndex = 1;
			// 
			// CloseButton
			// 
			this.CloseButton.Location = new System.Drawing.Point(8, 560);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(176, 23);
			this.CloseButton.TabIndex = 3;
			this.CloseButton.Text = "Close SectorReader";
			this.CloseButton.Click += new System.EventHandler(this.Close_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.HexadecimalTab);
			this.tabControl1.Controls.Add(this.CharactersTab);
			this.tabControl1.Location = new System.Drawing.Point(192, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(648, 588);
			this.tabControl1.TabIndex = 27;
			// 
			// HexadecimalTab
			// 
			this.HexadecimalTab.Controls.Add(this.HexTable);
			this.HexadecimalTab.Location = new System.Drawing.Point(4, 22);
			this.HexadecimalTab.Name = "HexadecimalTab";
			this.HexadecimalTab.Size = new System.Drawing.Size(640, 562);
			this.HexadecimalTab.TabIndex = 0;
			this.HexadecimalTab.Text = "Hexadecimal";
			// 
			// CharactersTab
			// 
			this.CharactersTab.Controls.Add(this.CharTable);
			this.CharactersTab.Location = new System.Drawing.Point(4, 22);
			this.CharactersTab.Name = "CharactersTab";
			this.CharactersTab.Size = new System.Drawing.Size(640, 562);
			this.CharactersTab.TabIndex = 1;
			this.CharactersTab.Text = "Characters";
			// 
			// Drives
			// 
			this.Drives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Drives.Location = new System.Drawing.Point(8, 32);
			this.Drives.Name = "Drives";
			this.Drives.Size = new System.Drawing.Size(168, 21);
			this.Drives.TabIndex = 28;
			this.Drives.SelectedIndexChanged += new System.EventHandler(this.NewDriveSelected);
			// 
			// PgDn
			// 
			this.PgDn.Location = new System.Drawing.Point(96, 96);
			this.PgDn.Name = "PgDn";
			this.PgDn.TabIndex = 29;
			this.PgDn.Text = "Page Down";
			this.PgDn.Click += new System.EventHandler(this.PgDn_Click);
			// 
			// PgUp
			// 
			this.PgUp.Location = new System.Drawing.Point(96, 64);
			this.PgUp.Name = "PgUp";
			this.PgUp.TabIndex = 30;
			this.PgUp.Text = "Page Up";
			this.PgUp.Click += new System.EventHandler(this.PgUp_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 31;
			this.label1.Text = "Select a drive:";
			// 
			// Begin
			// 
			this.Begin.Location = new System.Drawing.Point(8, 64);
			this.Begin.Name = "Begin";
			this.Begin.TabIndex = 32;
			this.Begin.Text = "Beginning";
			this.Begin.Click += new System.EventHandler(this.Begin_Click);
			// 
			// End
			// 
			this.End.Location = new System.Drawing.Point(8, 96);
			this.End.Name = "End";
			this.End.TabIndex = 33;
			this.End.Text = "End";
			this.End.Click += new System.EventHandler(this.End_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 128);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(160, 16);
			this.label2.TabIndex = 34;
			this.label2.Text = "Current Sector:";
			// 
			// lblCurrentSector
			// 
			this.lblCurrentSector.Location = new System.Drawing.Point(16, 144);
			this.lblCurrentSector.Name = "lblCurrentSector";
			this.lblCurrentSector.Size = new System.Drawing.Size(152, 16);
			this.lblCurrentSector.TabIndex = 35;
			this.lblCurrentSector.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 176);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 16);
			this.label3.TabIndex = 36;
			this.label3.Text = "Bytes/Sector:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 192);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 16);
			this.label4.TabIndex = 37;
			this.label4.Text = "Cylinders/Disk:";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 224);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(80, 16);
			this.label5.TabIndex = 38;
			this.label5.Text = "Sectors/Track:";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 208);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(88, 16);
			this.label6.TabIndex = 39;
			this.label6.Text = "Tracks/Cylinder:";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 240);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 16);
			this.label7.TabIndex = 40;
			this.label7.Text = "Total Sectors:";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 160);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(80, 16);
			this.label8.TabIndex = 41;
			this.label8.Text = "Sectors/Page:";
			// 
			// lblSectorsPerPage
			// 
			this.lblSectorsPerPage.Location = new System.Drawing.Point(88, 160);
			this.lblSectorsPerPage.Name = "lblSectorsPerPage";
			this.lblSectorsPerPage.Size = new System.Drawing.Size(80, 16);
			this.lblSectorsPerPage.TabIndex = 42;
			this.lblSectorsPerPage.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblBytesPerSector
			// 
			this.lblBytesPerSector.Location = new System.Drawing.Point(80, 176);
			this.lblBytesPerSector.Name = "lblBytesPerSector";
			this.lblBytesPerSector.Size = new System.Drawing.Size(88, 16);
			this.lblBytesPerSector.TabIndex = 43;
			this.lblBytesPerSector.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblCylindersPerDisk
			// 
			this.lblCylindersPerDisk.Location = new System.Drawing.Point(88, 192);
			this.lblCylindersPerDisk.Name = "lblCylindersPerDisk";
			this.lblCylindersPerDisk.Size = new System.Drawing.Size(80, 16);
			this.lblCylindersPerDisk.TabIndex = 44;
			this.lblCylindersPerDisk.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblTracksPerCylinder
			// 
			this.lblTracksPerCylinder.Location = new System.Drawing.Point(96, 208);
			this.lblTracksPerCylinder.Name = "lblTracksPerCylinder";
			this.lblTracksPerCylinder.Size = new System.Drawing.Size(72, 16);
			this.lblTracksPerCylinder.TabIndex = 45;
			this.lblTracksPerCylinder.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblSectorsPerTrack
			// 
			this.lblSectorsPerTrack.Location = new System.Drawing.Point(88, 224);
			this.lblSectorsPerTrack.Name = "lblSectorsPerTrack";
			this.lblSectorsPerTrack.Size = new System.Drawing.Size(80, 16);
			this.lblSectorsPerTrack.TabIndex = 46;
			this.lblSectorsPerTrack.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblTotalSectors
			// 
			this.lblTotalSectors.Location = new System.Drawing.Point(88, 240);
			this.lblTotalSectors.Name = "lblTotalSectors";
			this.lblTotalSectors.Size = new System.Drawing.Size(80, 16);
			this.lblTotalSectors.TabIndex = 47;
			this.lblTotalSectors.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtJmpSector
			// 
			this.txtJmpSector.Location = new System.Drawing.Point(16, 280);
			this.txtJmpSector.Name = "txtJmpSector";
			this.txtJmpSector.Size = new System.Drawing.Size(144, 20);
			this.txtJmpSector.TabIndex = 48;
			this.txtJmpSector.Text = "";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(8, 264);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(72, 16);
			this.label9.TabIndex = 49;
			this.label9.Text = "Goto Sector:";
			// 
			// txtCylinder
			// 
			this.txtCylinder.Location = new System.Drawing.Point(16, 376);
			this.txtCylinder.Name = "txtCylinder";
			this.txtCylinder.Size = new System.Drawing.Size(144, 20);
			this.txtCylinder.TabIndex = 50;
			this.txtCylinder.Text = "";
			// 
			// txtTrack
			// 
			this.txtTrack.Location = new System.Drawing.Point(16, 416);
			this.txtTrack.Name = "txtTrack";
			this.txtTrack.Size = new System.Drawing.Size(144, 20);
			this.txtTrack.TabIndex = 51;
			this.txtTrack.Text = "";
			// 
			// txtSector
			// 
			this.txtSector.Location = new System.Drawing.Point(16, 456);
			this.txtSector.Name = "txtSector";
			this.txtSector.Size = new System.Drawing.Size(144, 20);
			this.txtSector.TabIndex = 52;
			this.txtSector.Text = "";
			// 
			// lblCyl
			// 
			this.lblCyl.Location = new System.Drawing.Point(16, 360);
			this.lblCyl.Name = "lblCyl";
			this.lblCyl.Size = new System.Drawing.Size(144, 16);
			this.lblCyl.TabIndex = 53;
			this.lblCyl.Text = "Cylinder:";
			// 
			// lblTrk
			// 
			this.lblTrk.Location = new System.Drawing.Point(16, 400);
			this.lblTrk.Name = "lblTrk";
			this.lblTrk.Size = new System.Drawing.Size(144, 16);
			this.lblTrk.TabIndex = 54;
			this.lblTrk.Text = "Track:";
			// 
			// lblSctr
			// 
			this.lblSctr.Location = new System.Drawing.Point(16, 440);
			this.lblSctr.Name = "lblSctr";
			this.lblSctr.Size = new System.Drawing.Size(144, 16);
			this.lblSctr.TabIndex = 55;
			this.lblSctr.Text = "Sector:";
			// 
			// btnGotoSpot
			// 
			this.btnGotoSpot.Location = new System.Drawing.Point(88, 488);
			this.btnGotoSpot.Name = "btnGotoSpot";
			this.btnGotoSpot.TabIndex = 56;
			this.btnGotoSpot.Text = "Go";
			this.btnGotoSpot.Click += new System.EventHandler(this.btnGotoSpot_Click);
			// 
			// GotoSector
			// 
			this.GotoSector.Location = new System.Drawing.Point(88, 312);
			this.GotoSector.Name = "GotoSector";
			this.GotoSector.TabIndex = 57;
			this.GotoSector.Text = "Go";
			this.GotoSector.Click += new System.EventHandler(this.GotoSector_Click);
			// 
			// SectorReader
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(842, 591);
			this.Controls.Add(this.GotoSector);
			this.Controls.Add(this.btnGotoSpot);
			this.Controls.Add(this.lblSctr);
			this.Controls.Add(this.lblTrk);
			this.Controls.Add(this.lblCyl);
			this.Controls.Add(this.txtSector);
			this.Controls.Add(this.txtTrack);
			this.Controls.Add(this.txtCylinder);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.txtJmpSector);
			this.Controls.Add(this.lblTotalSectors);
			this.Controls.Add(this.lblSectorsPerTrack);
			this.Controls.Add(this.lblTracksPerCylinder);
			this.Controls.Add(this.lblCylindersPerDisk);
			this.Controls.Add(this.lblBytesPerSector);
			this.Controls.Add(this.lblSectorsPerPage);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblCurrentSector);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.End);
			this.Controls.Add(this.Begin);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.PgUp);
			this.Controls.Add(this.PgDn);
			this.Controls.Add(this.Drives);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.CloseButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.Name = "SectorReader";
			this.Text = "Sector Reader by Roger Wood";
			this.Load += new System.EventHandler(this.SectorReader_Load);
			((System.ComponentModel.ISupportInitialize)(this.HexTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.CharTable)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.HexadecimalTab.ResumeLayout(false);
			this.CharactersTab.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			Application.Run(new SectorReader(args));
		}

		private void SectorReader_Load(object sender, System.EventArgs e)
		{

		}

		private void Close_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		
		private void UpdateTables()
		{
			dataTableHex.Clear();
			dataTableChar.Clear();
			int ByteCount = 0;
			for(int i = 0; i < 32; i++)
			{
				int inTemp = 0;
				DataRow inTempHex = dataTableHex.NewRow();
				DataRow inTempChar = dataTableChar.NewRow();
				for(int n = 0; n < 32; n++)
				{
					
					inTemp = buffer[ByteCount];
					inTempHex[n] = String.Format("{0:x}", inTemp);
					inTempChar[n] = Convert.ToChar(inTemp);
					if(inTempHex[n].ToString().Length == 1)
						inTempHex[n] = "0" + inTempHex[n].ToString();
					ByteCount++;
				}
				dataTableChar.Rows.Add(inTempChar);
				dataTableHex.Rows.Add(inTempHex);
				inTemp = 0;
				inTempChar = null;
				inTempHex = null;
			}
			//currentByteOut.Text = currentByte.ToString();
			HexTable.DataSource = dataTableHex;
			CharTable.DataSource = dataTableChar;
			lblCurrentSector.Text = currentSector.ToString();
			UpdateRawSectorToCoordinates();
		}

		private void NewDriveSelected(object sender, System.EventArgs e)
		{
			if(Drives.SelectedIndex == 0)
			{
				dataTableHex.Clear();
				dataTableChar.Clear();
				return;
			}
			selectedDrive = Drives.SelectedValue.ToString();			
			//Get Drive Geometry
			IntPtr handle = Win32.GetHandle(selectedDrive);
			if(handle.ToInt32() == -1)
			{
				MessageBox.Show("Error: Invalid Handle" + handle.ToString());
				ClearAll();
				Drives.SelectedIndex = 0;
				return;
			}
			DISK_GEOMETRY diskParams = Win32.GetDiskGeometry(handle);
			if(diskParams.Cylinders > 0 && diskParams.BytesPerSector > 0)
			{
				BytesPerSector = diskParams.BytesPerSector;
				SectorsToShow = 1024 / (int)BytesPerSector;
				Cylinders = diskParams.Cylinders;
				SectorsPerTrack = diskParams.SectorsPerTrack;
				TracksPerCylinder = diskParams.TracksPerCylinder;
				TotalSectors = (uint)((ulong)SectorsPerTrack * (ulong)TracksPerCylinder * (ulong)Cylinders) - 1;
			}
			else
			{
				MessageBox.Show("Error: Couldn't get drive parameters.");
				ClearAll();
				Drives.SelectedIndex = 0;
				return;
			}
			
            currentSector = 0;
			buffer = new byte[(int)BytesPerSector * (int)SectorsToShow];
			lblSectorsPerPage.Text = SectorsToShow.ToString();
            lblBytesPerSector.Text = BytesPerSector.ToString();
			lblCylindersPerDisk.Text = Cylinders.ToString();
			lblTracksPerCylinder.Text = TracksPerCylinder.ToString();
			lblSectorsPerTrack.Text = SectorsPerTrack.ToString();
			lblTotalSectors.Text = TotalSectors.ToString();
			lblCyl.Text = "Cylinder: 1-" + Cylinders.ToString();
			lblTrk.Text = "Track: 1-" + TracksPerCylinder.ToString();
			lblSctr.Text = "Sector: 1-" + SectorsPerTrack.ToString();
			ReadSector(currentSector);
		}


		private void ReadSector(int SectorToRead)
		{
			for(int i = 0; i < ((int)BytesPerSector * (int)SectorsToShow); i++)
				buffer[i] = 0;
            IntPtr h = Win32.GetHandle(selectedDrive);
			if(h.ToInt32() == -1)
			{
				MessageBox.Show("Invalid Handle" + h.ToString());
				ClearAll();
				return;
            }
            int n = Win32.ReadFile(h,(uint)(SectorToRead * BytesPerSector),(uint)((int)BytesPerSector * (int)SectorsToShow),buffer);
			if(n<1024)
			{
				MessageBox.Show("Error. Couldn't read all of requested data.");
				dataTableHex.Clear();
				dataTableChar.Clear();
			}
			else
				UpdateTables();
		}

		private void ClearAll()
		{
			dataTableHex.Clear();
			dataTableChar.Clear();
		}

		#region control events
		private void PgDn_Click(object sender, System.EventArgs e)
		{
			if(Drives.SelectedIndex == 0)
				return;
			if(currentSector + 2 >= TotalSectors)
				currentSector = (int)TotalSectors - 2;
			else
				currentSector += 2;
			ReadSector(currentSector);
		}

		private void PgUp_Click(object sender, System.EventArgs e)
		{
			if(Drives.SelectedIndex == 0)
				return;
			if(currentSector > 1)
				currentSector -= 2;
			else
				currentSector = 0;
			ReadSector(currentSector);
		}

		private void Begin_Click(object sender, System.EventArgs e)
		{
			if(Drives.SelectedIndex == 0)
				return;
			currentSector = 0;
			ReadSector(currentSector);
		}

		private void End_Click(object sender, System.EventArgs e)
		{
			if(Drives.SelectedIndex == 0)
				return;
			currentSector = (int)TotalSectors - 1;
			ReadSector(currentSector);
		}

		private void UpdateRawSectorToCoordinates()
		{
			currentCylinderOnDisk = (ulong)currentSector / (TracksPerCylinder * SectorsPerTrack) + 1;
			ulong leftover = (ulong)currentSector % (TracksPerCylinder * SectorsPerTrack);
			currentTrackOnCylinder = (uint)((ulong)leftover / (ulong)SectorsPerTrack + 1);
			currentSectorOnTrack = (uint)((ulong)leftover % (ulong)SectorsPerTrack + 1);
			txtCylinder.Text = currentCylinderOnDisk.ToString();
			txtTrack.Text = currentTrackOnCylinder.ToString();
			txtSector.Text = currentSectorOnTrack.ToString();
			txtJmpSector.Text = currentSector.ToString();

		}

		private void UpdateCoordinatesToRawSector()
		{
			currentCylinderOnDisk = (ulong)Convert.ToInt32(txtCylinder.Text.ToString());
			if(currentCylinderOnDisk <= 0)
				currentCylinderOnDisk = 1;
			else if(currentCylinderOnDisk > Cylinders)
				currentCylinderOnDisk = Cylinders;
			currentTrackOnCylinder = (uint)Convert.ToInt32(txtTrack.Text.ToString());
			if(currentTrackOnCylinder < 0)
				currentTrackOnCylinder = 1;
			else if(currentTrackOnCylinder > TracksPerCylinder)
				currentTrackOnCylinder = TracksPerCylinder;
			currentSectorOnTrack = (uint)Convert.ToInt32(txtSector.Text.ToString());
			if(currentSectorOnTrack < 0)
				currentSectorOnTrack = 1;
			else if(currentSectorOnTrack > SectorsPerTrack)
				currentSectorOnTrack = SectorsPerTrack;

			currentSector = (int)(((ulong)(currentCylinderOnDisk-1) * (TracksPerCylinder * SectorsPerTrack) + ((ulong)(currentTrackOnCylinder - 1)  * SectorsPerTrack) + (ulong)currentSectorOnTrack-1));
			if(currentSector + 2 >= TotalSectors)
				currentSector = (int)TotalSectors - 1;
		}

		private void btnGotoSpot_Click(object sender, System.EventArgs e)
		{
			UpdateCoordinatesToRawSector();
			ReadSector(currentSector);
			return;
		}

		private void GotoSector_Click(object sender, System.EventArgs e)
		{
			if(!System.Text.RegularExpressions.Regex.IsMatch(txtJmpSector.Text, "^[0-9]"))
				return;
			int intTemp = Convert.ToInt32(txtJmpSector.Text);
			if(intTemp < 0)
			{
				currentSector = 0;
			}
			else if(intTemp >= TotalSectors - 2)
			{
				currentSector = (int)TotalSectors - 2;
			}
			else
			{
				currentSector = intTemp;
			}
			ReadSector(currentSector);
			return;
		}
		#endregion
	}
}
