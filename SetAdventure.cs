using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

public class SetAdventure
{
    const int SW_HIDE = 0;
    const int SW_SHOW = 5;

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void Main()
    {
        var handle = GetConsoleWindow();

        ShowWindow(handle, SW_HIDE);
		
		Display theForm = new Display();
		theForm.Icon = new Icon("res/PurpleStar.ico");
		
		objForm = new OBJEdit();
		objForm.Icon = theForm.Icon;
		
		new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            objForm.ShowDialog();
        }).Start();
		
		theForm.ShowDialog();
	}
	
	//When sorting, what should we sort by
	public static int organizeType = 0;
	
	//List of the objects we are working with
	public static List<SETObject> objList = new List<SETObject>();
	
	//Main form controls
	public static ListBox lstBox = new ListBox();
	public static Label lbl = new Label();
	public static Label lblObjCount = new Label();
	
	public static OBJEdit objForm = null;
	
	public partial class Display : Form
	{
		public Display()
		{
			//TODO:
			//
			// Analyze tab
			//
			// analyze->search  
			//  search for objects with id, maybe other params as well like position being between some range
			
			
			// Create an empty MainMenu.
			MainMenu mainMenu = new MainMenu();

			// File submenu
			MenuItem[] subMenuFile = new MenuItem[6];
			subMenuFile[0] = new MenuItem("Open");
			subMenuFile[1] = new MenuItem("Merge");
			subMenuFile[2] = new MenuItem("Save As SADX");
			subMenuFile[3] = new MenuItem("Save As SA2");
			subMenuFile[4] = new MenuItem("Export");
			subMenuFile[5] = new MenuItem("Close");
			
			subMenuFile[0].Click += eventFileOpen;
			subMenuFile[1].Click += eventFileMerge;
			subMenuFile[2].Click += eventFileSaveAsSADX;
			subMenuFile[3].Click += eventFileSaveAsSA2;
			subMenuFile[4].Click += eventFileExport;
			subMenuFile[5].Click += eventFileClose;
		   
			MenuItem menuItemFile = new MenuItem("&File", subMenuFile);
			
			
			
			// Organize submenu
			MenuItem[] menusOrganize = new MenuItem[10];
			menusOrganize[0] = new MenuItem("by ID");
			menusOrganize[1] = new MenuItem("by ID, X Rotation");
			menusOrganize[2] = new MenuItem("by ID, Y Rotation");
			menusOrganize[3] = new MenuItem("by ID, Z Rotation");
			menusOrganize[4] = new MenuItem("by ID, X");
			menusOrganize[5] = new MenuItem("by ID, Y");
			menusOrganize[6] = new MenuItem("by ID, Z");
			menusOrganize[7] = new MenuItem("by ID, Variable 1");
			menusOrganize[8] = new MenuItem("by ID, Variable 2");
			menusOrganize[9] = new MenuItem("by ID, Variable 3");
			
			menusOrganize[0].Click += eventOrganizeID;
			menusOrganize[1].Click += eventOrganizeIDXRot;
			menusOrganize[2].Click += eventOrganizeIDYRot;
			menusOrganize[3].Click += eventOrganizeIDZRot;
			menusOrganize[4].Click += eventOrganizeIDX;
			menusOrganize[5].Click += eventOrganizeIDY;
			menusOrganize[6].Click += eventOrganizeIDZ;
			menusOrganize[7].Click += eventOrganizeIDVar1;
			menusOrganize[8].Click += eventOrganizeIDVar2;
			menusOrganize[9].Click += eventOrganizeIDVar3;
			
			// Edit submenu
			MenuItem[] menusEdit = new MenuItem[5];
			menusEdit[0] = new MenuItem("Organize", menusOrganize);
			menusEdit[1] = new MenuItem("Add Blank Object");
			menusEdit[2] = new MenuItem("Duplicate Selected");
			menusEdit[3] = new MenuItem("Remove Selected");
			menusEdit[4] = new MenuItem("Remove Duplicates");
			
			menusEdit[1].Click += eventEditAddBlankObject;
			menusEdit[2].Click += eventEditDuplicateSelected;
			menusEdit[3].Click += eventEditRemoveSelected;
			menusEdit[4].Click += eventEditRemoveDuplicates;
		   
			MenuItem menuItemEdit = new MenuItem("&Edit", menusEdit);
			
			
			
			// Add two MenuItem objects to the MainMenu.
			mainMenu.MenuItems.Add(menuItemFile);
			mainMenu.MenuItems.Add(menuItemEdit);
			
			// Set the selection mode to multiple and extended.
			lstBox.SelectionMode = SelectionMode.MultiExtended;
			lstBox.ScrollAlwaysVisible = true;
			lstBox.Left = 0;
			lstBox.Top = 16;
			lstBox.DoubleClick += eventDoubleClick;
			
			this.Controls.Add(lstBox);
			lstBox.Font = new Font("Courier New", 11, FontStyle.Bold);
			
			lbl.AutoSize = false;
			lbl.Left = 0;
			lbl.Top = 0;
			lbl.Text = "ID     X Rot   Y Rot   Z Rot      X          Y          Z              Var 1      Var 2      Var 3";
			
			this.Controls.Add(lbl);
			lbl.Font = new Font("Courier New", 10.95f, FontStyle.Regular);
			
			this.Controls.Add(lblObjCount);
			lblObjCount.Font = new Font("Courier New", 10.95f, FontStyle.Regular);
			
			this.Text = "";
			this.Size = new Size(976, 800);
			this.Location = new System.Drawing.Point(278, 16);
			this.StartPosition = FormStartPosition.Manual;
			this.Menu = mainMenu;
			this.Resize += eventFormResize;
		}
		
		public void eventFormResize(object sender, EventArgs e)
		{
			lstBox.Size = new Size(this.ClientSize.Width, this.ClientSize.Height-32);
			lbl.Size = new Size(this.ClientSize.Width, 16);
			lblObjCount.Size = new Size(this.ClientSize.Width, 16);
			lblObjCount.Location = new System.Drawing.Point(0, this.ClientSize.Height-16);
		}
		
		public void eventDoubleClick(object sender, EventArgs e)
		{
			objForm.selectNewObject();
		}
		
		public void eventEditAddBlankObject(object sender, EventArgs e)
		{
			SETObject blank = new SETObject();
			blank.id = 0;
			blank.clip = 0;
			blank.rotX = 0;
			blank.rotY = 0;
			blank.rotZ = 0;
			blank.x = 0;
			blank.y = 0;
			blank.z = 0;
			blank.var1 = 0;
			blank.var2 = 0;
			blank.var3 = 0;
			blank.genDisp();
			objList.Insert(0, blank);
			updateObjectList();
		}
		
		public void eventEditDuplicateSelected(object sender, EventArgs e)
		{
			List<SETObject> toBeDuped = new List<SETObject>();
			
			foreach (SETObject o in lstBox.SelectedItems)
			{
				toBeDuped.Add(o);
			}
			
			foreach (SETObject o in toBeDuped)
			{
				objList.Add(o);
			}
			
			updateObjectList();
			
			if (toBeDuped.Count > 0)
			{
				if (toBeDuped.Count == 1)
				{
					MessageBox.Show("Duplicated "+toBeDuped.Count+" object");
				}
				else
				{
					MessageBox.Show("Duplicated "+toBeDuped.Count+" objects");
				}
			}
		}
		
		public void eventEditRemoveSelected(object sender, EventArgs e)
		{
			deleteSelectedObjects();
		}
		
		public static void deleteSelectedObjects()
		{
			List<SETObject> toBeDeleted = new List<SETObject>();
			
			foreach (SETObject o in lstBox.SelectedItems)
			{
				toBeDeleted.Add(o);
			}
			
			foreach (SETObject o in toBeDeleted)
			{
				objList.Remove(o);
			}
			
			updateObjectList();
			
			if (toBeDeleted.Count > 0)
			{
				if (toBeDeleted.Count == 1)
				{
					MessageBox.Show("Removed "+toBeDeleted.Count+" object");
				}
				else
				{
					MessageBox.Show("Removed "+toBeDeleted.Count+" objects");
				}
			}
		}
		
		public void eventEditRemoveDuplicates(object sender, EventArgs e)
		{
			//More efficient way would be just putting them 
			//in a hash map that doesn't allow duplicates.
			//But I want to preserve oder, so for now, n^2
			//will have to do.
			
			List<SETObject> removed = new List<SETObject>();
			
			for (int i = 0; i < objList.Count-1; i++)
			{
				for (int j = i+1; j < objList.Count; j++)
				{
					if (objList[i].isEqualTo(objList[j]))
					{
						removed.Add(objList[j]);
						objList.RemoveAt(j);
						j--;
					}
				}
			}
			
			updateObjectList();
			
			if (removed.Count < 15  && removed.Count != 0) //Some arbitrary number of how many objects to show
			{
				string messg = "Removed "+removed.Count+" duplicate objects:\n";
				if (removed.Count == 1)
				{
					messg = "Removed "+removed.Count+" duplicate object:\n";
				}
				
				foreach (SETObject o in removed)
				{
					messg  = messg+o.toCompressedString()+"\n";
				}
				
				MessageBox.Show(messg);
			}
			else
			{
				MessageBox.Show("Removed "+removed.Count+" duplicate objects");
			}
		}
		
		public void eventOrganizeID(object sender, EventArgs e)
        {
			organizeType = 0;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDXRot(object sender, EventArgs e)
        {
			organizeType = 1;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDYRot(object sender, EventArgs e)
        {
			organizeType = 2;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDZRot(object sender, EventArgs e)
        {
			organizeType = 3;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDX(object sender, EventArgs e)
        {
			organizeType = 4;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDY(object sender, EventArgs e)
        {
			organizeType = 5;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDZ(object sender, EventArgs e)
        {
			organizeType = 6;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDVar1(object sender, EventArgs e)
        {
			organizeType = 7;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDVar2(object sender, EventArgs e)
        {
			organizeType = 8;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventOrganizeIDVar3(object sender, EventArgs e)
        {
			organizeType = 9;
			objList.Sort();
			updateObjectList();
		}
		
		public void eventFileClose(object sender, EventArgs e)
        {
			objList.Clear();
			updateObjectList();
			this.Text = "";
		}
		
		public void eventFileSaveAsSA2(object sender, EventArgs e)
        {
			SaveFileDialog  dlg = new SaveFileDialog();
			dlg.ShowHelp = true;
			dlg.Filter  = "SET files|*.bin";
			dlg.Title = "Export SET file";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName;
                fileName = dlg.FileName;
				
				saveSETFileSA2(fileName);
				this.Text = fileName;
			}
		}
		
		public void eventFileSaveAsSADX(object sender, EventArgs e)
        {
			SaveFileDialog  dlg = new SaveFileDialog();
			dlg.ShowHelp = true;
			dlg.Filter  = "SET files|*.bin";
			dlg.Title = "Export SET file";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName;
                fileName = dlg.FileName;
				
				saveSETFileSADX(fileName);
				this.Text = fileName;
			}
		}
		
		public void eventFileExport(object sender, EventArgs e)
        {
			SaveFileDialog  dlg = new SaveFileDialog();
			dlg.ShowHelp = true;
			dlg.Filter  = "txt files|*.txt";
			dlg.Title = "Export as a text file";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName;
                fileName = dlg.FileName;
				
				if (fileName.EndsWith(".txt") == false)
				{
					fileName = fileName+".txt";
				}
				
				using (System.IO.StreamWriter file = 
					new System.IO.StreamWriter(fileName))
				{				
					foreach (SETObject o in objList)
					{
						file.WriteLine(o.toFullString());
					}
				}
			}
		}
		
		public void eventFileOpen(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
			dlg.ShowHelp = true;
			dlg.Filter  = "SET files|*.bin";
			dlg.Title = "Open SET file";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName;
                fileName = dlg.FileName;
				
				this.Text = fileName;
				
				try
				{
					objList.Clear();
					
					byte[] buf = System.IO.File.ReadAllBytes(fileName);
					
					addObjectsFromRawBytes(buf);
				}
				catch
				{
					MessageBox.Show("Trouble loading file '"+fileName+"'");
				}
				
				updateObjectList();
            }
        }
		
		public void eventFileMerge(object sender, EventArgs e)
        {
			if (this.Text == "")
			{
				MessageBox.Show("Open a file first");
				return;
			}
			
            OpenFileDialog dlg = new OpenFileDialog();
			dlg.ShowHelp = true;
			dlg.Filter  = "SET files|*.bin";
			dlg.Title = "Open SET file to merge";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName;
                fileName = dlg.FileName;
				
				string newPart = fileName.Substring(fileName.LastIndexOf("\\")+1); //get the new filename
				
				string oldPart = this.Text;
				oldPart = oldPart.Substring(0, oldPart.LastIndexOf("."));
				
				this.Text = oldPart+"_MERGE_"+newPart;
				
				try
				{
					byte[] buf = System.IO.File.ReadAllBytes(fileName);
					
					addObjectsFromRawBytes(buf);
				}
				catch
				{
					MessageBox.Show("Trouble loading file '"+fileName+"'");
				}
				
				updateObjectList();
            }
        }
		
		public static void addObjectsFromRawBytes(byte[] buf)
		{
			//Since the object count is stored in 4 bytes, 
			// and since theres usually a "small" (less than 65k) number of objects
			// in these files, we can assume the endianess by computing
			// both counts and comparing the sizes
			
			uint endianCheck1 = (uint)0;
			endianCheck1 += (uint)(buf[0]);
			endianCheck1 += (uint)(buf[1] << 8);
			endianCheck1 += (uint)(buf[2] << 16);
			endianCheck1 += (uint)(buf[3] << 24);
			
			uint endianCheck2 = (uint)0;
			endianCheck2 += (uint)(buf[3]);
			endianCheck2 += (uint)(buf[2] << 8);
			endianCheck2 += (uint)(buf[1] << 16);
			endianCheck2 += (uint)(buf[0] << 24);
			
			if (endianCheck1 < endianCheck2) //Little endian: SADX
			{
				uint objCount = endianCheck1;
				
				for (uint i = 1; i < objCount+1; i++)
				{
					SETObject obj = new SETObject();
					
					uint off = i*32;
					
					byte id = 0;
					id = buf[off+0];
					//id += (buf[off+0] << 8); //Its only 4 bits of this?
					
					byte clip = 0;
					clip = buf[off+1]; //Only 4 bits of it?
					
					ushort rotX = 0;
					rotX += (ushort)(buf[off+2]);
					rotX += (ushort)(buf[off+3] << 8);
					
					ushort rotY = 0;
					rotY += (ushort)(buf[off+4]);
					rotY += (ushort)(buf[off+5] << 8);
					
					ushort rotZ = 0;
					rotZ += (ushort)(buf[off+6]);
					rotZ += (ushort)(buf[off+7] << 8);
					
					byte[] tmp = new byte[4];
					tmp[3] = buf[off+11];
					tmp[2] = buf[off+10];
					tmp[1] = buf[off+9];
					tmp[0] = buf[off+8];
					float x =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[3] = buf[off+15];
					tmp[2] = buf[off+14];
					tmp[1] = buf[off+13];
					tmp[0] = buf[off+12];
					float y =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[3] = buf[off+19];
					tmp[2] = buf[off+18];
					tmp[1] = buf[off+17];
					tmp[0] = buf[off+16];
					float z =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[3] = buf[off+23];
					tmp[2] = buf[off+22];
					tmp[1] = buf[off+21];
					tmp[0] = buf[off+20];
					float var1 =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[3] = buf[off+27];
					tmp[2] = buf[off+26];
					tmp[1] = buf[off+25];
					tmp[0] = buf[off+24];
					float var2 =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[3] = buf[off+31];
					tmp[2] = buf[off+30];
					tmp[1] = buf[off+29];
					tmp[0] = buf[off+28];
					float var3 =  System.BitConverter.ToSingle(tmp, 0);
					
					obj.id = id;
					obj.clip = clip;
					obj.x = x;
					obj.y = y;
					obj.z = z;
					obj.rotX = rotX;
					obj.rotY = rotY;
					obj.rotZ = rotZ;
					obj.var1 = var1;
					obj.var2 = var2;
					obj.var3 = var3;
					
					obj.genDisp();
					
					objList.Add(obj);
				}
			}
			else //Big endian : SA2
			{
				uint objCount = endianCheck2;
				
				for (uint i = 1; i < objCount+1; i++)
				{
					SETObject obj = new SETObject();
					
					uint off = i*32;
					
					byte id = 0;
					id = buf[off+1];
					//id += (buf[off+0] << 8); //Its only 4 bits of this?
					
					byte clip = 0;
					clip = buf[off+0]; //Only 4 bits of it?
					
					ushort rotX = 0;
					rotX += (ushort)(buf[off+3]);
					rotX += (ushort)(buf[off+2] << 8);
					
					ushort rotY = 0;
					rotY += (ushort)(buf[off+5]);
					rotY += (ushort)(buf[off+4] << 8);
					
					ushort rotZ = 0;
					rotZ += (ushort)(buf[off+7]);
					rotZ += (ushort)(buf[off+6] << 8);
					
					byte[] tmp = new byte[4];
					tmp[0] = buf[off+11];
					tmp[1] = buf[off+10];
					tmp[2] = buf[off+9];
					tmp[3] = buf[off+8];
					float x =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[0] = buf[off+15];
					tmp[1] = buf[off+14];
					tmp[2] = buf[off+13];
					tmp[3] = buf[off+12];
					float y =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[0] = buf[off+19];
					tmp[1] = buf[off+18];
					tmp[2] = buf[off+17];
					tmp[3] = buf[off+16];
					float z =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[0] = buf[off+23];
					tmp[1] = buf[off+22];
					tmp[2] = buf[off+21];
					tmp[3] = buf[off+20];
					float var1 =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[0] = buf[off+27];
					tmp[1] = buf[off+26];
					tmp[2] = buf[off+25];
					tmp[3] = buf[off+24];
					float var2 =  System.BitConverter.ToSingle(tmp, 0);
					
					tmp[0] = buf[off+31];
					tmp[1] = buf[off+30];
					tmp[2] = buf[off+29];
					tmp[3] = buf[off+28];
					float var3 =  System.BitConverter.ToSingle(tmp, 0);
					
					obj.id = id;
					obj.clip = clip;
					obj.x = x;
					obj.y = y;
					obj.z = z;
					obj.rotX = rotX;
					obj.rotY = rotY;
					obj.rotZ = rotZ;
					obj.var1 = var1;
					obj.var2 = var2;
					obj.var3 = var3;
					
					obj.genDisp();
					
					objList.Add(obj);
				}
			}
        }
		
		public static void saveSETFileSA2(string fileName)
		{
			try
			{
				using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
				{
					byte[] hBuf = new byte[4];
					uint size = (uint)objList.Count;
					hBuf[0] = (byte)((size >> 24) & 0xFF);
					hBuf[1] = (byte)((size >> 16) & 0xFF);
					hBuf[2] = (byte)((size >> 8)  & 0xFF);
					hBuf[3] = (byte)((size >> 0)  & 0xFF);
					writer.Write(hBuf);
					
					hBuf[0] = 0;
					hBuf[1] = 0;
					hBuf[2] = 0;
					hBuf[3] = 0;
					
					writer.Write(hBuf);
					writer.Write(hBuf);
					writer.Write(hBuf);
					
					writer.Write(hBuf);
					writer.Write(hBuf);
					writer.Write(hBuf);
					writer.Write(hBuf);
					
					foreach (SETObject obj in objList)
					{
						writer.Write(obj.clip);
						writer.Write(obj.id);
						
						byte[] shortBuf = new byte[2];
						shortBuf[0] = (byte)((obj.rotX >> 8) & 0xFF);
						shortBuf[1] = (byte)((obj.rotX >> 0) & 0xFF);
						writer.Write(shortBuf);
						
						shortBuf[0] = (byte)((obj.rotY >> 8) & 0xFF);
						shortBuf[1] = (byte)((obj.rotY >> 0) & 0xFF);
						writer.Write(shortBuf);
						
						shortBuf[0] = (byte)((obj.rotZ >> 8) & 0xFF);
						shortBuf[1] = (byte)((obj.rotZ >> 0) & 0xFF);
						writer.Write(shortBuf);
						
						byte[] buf = BitConverter.GetBytes(obj.x);
						byte[] fBuf = new byte[4];
						fBuf[0] = buf[3];
						fBuf[1] = buf[2];
						fBuf[2] = buf[1];
						fBuf[3] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.y);
						fBuf[0] = buf[3];
						fBuf[1] = buf[2];
						fBuf[2] = buf[1];
						fBuf[3] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.z);
						fBuf[0] = buf[3];
						fBuf[1] = buf[2];
						fBuf[2] = buf[1];
						fBuf[3] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.var1);
						fBuf[0] = buf[3];
						fBuf[1] = buf[2];
						fBuf[2] = buf[1];
						fBuf[3] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.var2);
						fBuf[0] = buf[3];
						fBuf[1] = buf[2];
						fBuf[2] = buf[1];
						fBuf[3] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.var3);
						fBuf[0] = buf[3];
						fBuf[1] = buf[2];
						fBuf[2] = buf[1];
						fBuf[3] = buf[0];
						writer.Write(fBuf);
					}
				}
			}
			catch
			{
				MessageBox.Show("problem writing");
			}
		}
		
		public static void saveSETFileSADX(string fileName)
		{
			try
			{
				using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
				{
					byte[] hBuf = new byte[4];
					uint size = (uint)objList.Count;
					hBuf[3] = (byte)((size >> 24) & 0xFF);
					hBuf[2] = (byte)((size >> 16) & 0xFF);
					hBuf[1] = (byte)((size >> 8)  & 0xFF);
					hBuf[0] = (byte)((size >> 0)  & 0xFF);
					writer.Write(hBuf);
					
					hBuf[0] = 0;
					hBuf[1] = 0;
					hBuf[2] = 0;
					hBuf[3] = 0;
					
					writer.Write(hBuf);
					writer.Write(hBuf);
					writer.Write(hBuf);
					
					writer.Write(hBuf);
					writer.Write(hBuf);
					writer.Write(hBuf);
					writer.Write(hBuf);
					
					foreach (SETObject obj in objList)
					{
						writer.Write(obj.id);
						writer.Write(obj.clip);
						
						byte[] shortBuf = new byte[2];
						shortBuf[1] = (byte)((obj.rotX >> 8) & 0xFF);
						shortBuf[0] = (byte)((obj.rotX >> 0) & 0xFF);
						writer.Write(shortBuf);
						
						shortBuf[1] = (byte)((obj.rotY >> 8) & 0xFF);
						shortBuf[0] = (byte)((obj.rotY >> 0) & 0xFF);
						writer.Write(shortBuf);
						
						shortBuf[1] = (byte)((obj.rotZ >> 8) & 0xFF);
						shortBuf[0] = (byte)((obj.rotZ >> 0) & 0xFF);
						writer.Write(shortBuf);
						
						byte[] buf = BitConverter.GetBytes(obj.x);
						byte[] fBuf = new byte[4];
						fBuf[3] = buf[3];
						fBuf[2] = buf[2];
						fBuf[1] = buf[1];
						fBuf[0] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.y);
						fBuf[3] = buf[3];
						fBuf[2] = buf[2];
						fBuf[1] = buf[1];
						fBuf[0] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.z);
						fBuf[3] = buf[3];
						fBuf[2] = buf[2];
						fBuf[1] = buf[1];
						fBuf[0] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.var1);
						fBuf[3] = buf[3];
						fBuf[2] = buf[2];
						fBuf[1] = buf[1];
						fBuf[0] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.var2);
						fBuf[3] = buf[3];
						fBuf[2] = buf[2];
						fBuf[1] = buf[1];
						fBuf[0] = buf[0];
						writer.Write(fBuf);
						
						buf = BitConverter.GetBytes(obj.var3);
						fBuf[3] = buf[3];
						fBuf[2] = buf[2];
						fBuf[1] = buf[1];
						fBuf[0] = buf[0];
						writer.Write(fBuf);
					}
				}
			}
			catch
			{
				MessageBox.Show("problem writing");
			}
		}
		
		//Call whenever the objList changes
		public static void updateObjectList()
		{
			lstBox.DataSource = null;
			lstBox.DataSource = objList;
			
			int currIndex = 0;
			foreach (SETObject o in objList)
			{
				o.index = currIndex;
				currIndex++;
			}
			
			//Let the object editing form check to see if its object is still in the list
			objForm.checkObjStillExists();
			
			lblObjCount.Text = "Object count: "+objList.Count;
		}
	}
	
	public partial class OBJEdit : Form
	{
		public static SETObject currObj = null;
		
		public static Button btnApply = new Button();
		
		public static TextBox editID   = new TextBox();
		public static TextBox editXRot = new TextBox();
		public static TextBox editYRot = new TextBox();
		public static TextBox editZRot = new TextBox();
		public static TextBox editX    = new TextBox();
		public static TextBox editY    = new TextBox();
		public static TextBox editZ    = new TextBox();
		public static TextBox editVar1 = new TextBox();
		public static TextBox editVar2 = new TextBox();
		public static TextBox editVar3 = new TextBox();
		
		public static Label lblID   = new Label();
		public static Label lblXRot = new Label();
		public static Label lblYRot = new Label();
		public static Label lblZRot = new Label();
		public static Label lblX    = new Label();
		public static Label lblY    = new Label();
		public static Label lblZ    = new Label();
		public static Label lblVar1 = new Label();
		public static Label lblVar2 = new Label();
		public static Label lblVar3 = new Label();
		
		public OBJEdit()
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = new System.Drawing.Point(16, 16);
			this.Size = new Size(256+16-32, 384-16+32+16);
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.Text = "Object Editor";
			
			btnApply.Size = new Size(256-16-32-8, 32);
			btnApply.Location = new System.Drawing.Point(16, 16);
			btnApply.Text = "Apply";
			btnApply.Click += eventClickApplyButton;
			
			initializeTextBox(editID  , 16+16+32*1);
			initializeTextBox(editXRot, 16+16+32*2);
			initializeTextBox(editYRot, 16+16+32*3);
			initializeTextBox(editZRot, 16+16+32*4);
			initializeTextBox(editX   , 16+16+32*5);
			initializeTextBox(editY   , 16+16+32*6);
			initializeTextBox(editZ   , 16+16+32*7);
			initializeTextBox(editVar1, 16+16+32*8);
			initializeTextBox(editVar2, 16+16+32*9);
			initializeTextBox(editVar3, 16+16+32*10);
			
			initializeLabel(lblID  , 16+16+32*1, "ID");
			initializeLabel(lblXRot, 16+16+32*2, "X Rotation");
			initializeLabel(lblYRot, 16+16+32*3, "Y Rotation");
			initializeLabel(lblZRot, 16+16+32*4, "Z Rotation");
			initializeLabel(lblX   , 16+16+32*5, "X");
			initializeLabel(lblY   , 16+16+32*6, "Y");
			initializeLabel(lblZ   , 16+16+32*7, "Z");
			initializeLabel(lblVar1, 16+16+32*8, "Variable 1");
			initializeLabel(lblVar2, 16+16+32*9, "Variable 2");
			initializeLabel(lblVar3, 16+16+32*10, "Variable 3");
			
			this.Controls.Add(btnApply);
			
			this.Controls.Add(editID);
			this.Controls.Add(editXRot);
			this.Controls.Add(editYRot);
			this.Controls.Add(editZRot);
			this.Controls.Add(editX);
			this.Controls.Add(editY);
			this.Controls.Add(editZ);
			this.Controls.Add(editVar1);
			this.Controls.Add(editVar2);
			this.Controls.Add(editVar3);
			
			this.Controls.Add(lblID  );
			this.Controls.Add(lblXRot);
			this.Controls.Add(lblYRot);
			this.Controls.Add(lblZRot);
			this.Controls.Add(lblX   );
			this.Controls.Add(lblY   );
			this.Controls.Add(lblZ   );
			this.Controls.Add(lblVar1);
			this.Controls.Add(lblVar2);
			this.Controls.Add(lblVar3);
		}
		
		public void selectNewObject()
		{
			if (lstBox.SelectedItem != null)
			{
				currObj = (SETObject)lstBox.SelectedItem;
				
				editID.Text   = currObj.id.ToString("X");
				editXRot.Text = ""+currObj.rotX;
				editYRot.Text = ""+currObj.rotY;
				editZRot.Text = ""+currObj.rotZ;
				editX.Text    = ""+currObj.x;
				editY.Text    = ""+currObj.y; 
				editZ.Text    = ""+currObj.z;  
				editVar1.Text = ""+currObj.var1;
				editVar2.Text = ""+currObj.var2;
				editVar3.Text = ""+currObj.var3;
			}
			
			this.Show();
		}
		
		public void checkObjStillExists()
		{
			if (objList.Contains(currObj) == false)
			{
				currObj = null;
				this.Hide();
			}
		}
		
		public void initializeTextBox(TextBox txtBox, int top)
		{
			txtBox.Size = new Size(128, 16);
			txtBox.Location = new System.Drawing.Point(128-8-32, top);
			txtBox.Text = "";
		}
		
		public void initializeLabel(Label lbl, int top, string txt)
		{
			lbl.AutoSize = false;
			lbl.Size = new Size(128-32-32, 16);
			lbl.Location = new System.Drawing.Point(16, top+2);
			lbl.TextAlign = ContentAlignment.MiddleRight;
			lbl.Text = txt;
		}
		
		public void eventClickApplyButton(object sender, EventArgs e)
		{
			checkObjStillExists();
			if (currObj == null)
			{
				MessageBox.Show("No object selected");
			}
			else
			{
				try
				{
					byte id     = Convert.ToByte(editID.Text, 16);
					ushort rotX = Convert.ToUInt16(editXRot.Text, 10);
					ushort rotY = Convert.ToUInt16(editYRot.Text, 10);
					ushort rotZ = Convert.ToUInt16(editZRot.Text, 10);
					float x     = Convert.ToSingle(editX.Text);
					float y     = Convert.ToSingle(editY.Text);
					float z     = Convert.ToSingle(editZ.Text);
					float var1  = Convert.ToSingle(editVar1.Text);
					float var2  = Convert.ToSingle(editVar2.Text);
					float var3  = Convert.ToSingle(editVar3.Text);
					
					currObj.id = id;
					currObj.rotX = rotX;
					currObj.rotY = rotY;
					currObj.rotZ = rotZ;
					currObj.x = x;
					currObj.y = y;
					currObj.z = z;
					currObj.var1 = var1;
					currObj.var2 = var2;
					currObj.var3 = var3;
					
					currObj.genDisp();
					
					Display.updateObjectList();
				}
				catch
				{
					MessageBox.Show("Error parsing new object parameters");
				}
			}
		}
		
		protected override void OnFormClosing(FormClosingEventArgs  e)
		{
			e.Cancel = true;
			this.Hide();
		}
	}
	
	public class SETObject : IComparable
	{
		public string dispString;
		public int index;
		
		public byte id;
		public byte clip;
		public float x;
		public float y;
		public float z;
		public ushort rotX;
		public ushort rotY;
		public ushort rotZ;
		public float var1;
		public float var2;
		public float var3;
		
		public SETObject() {}
		
		public void genDisp()
		{
			dispString = fixString(id.ToString("X"), 5)+"  "+
					     fixString(""+rotX, 8)+
					     fixString(""+rotY, 8)+
					     fixString(""+rotZ, 8)+"   "+
					     fixFloatString(x, 11)+
					     fixFloatString(y, 11)+
					     fixFloatString(z, 11)+"    "+
					     fixFloatString(var1, 11)+
					     fixFloatString(var2, 11)+
					     fixFloatString(var3, 11);
		}
		
		public override string ToString()
		{
			return dispString;
		}
		
		public string toCompressedString()
		{
			return (id.ToString("X")+
				" "+rotX+" "+rotY+" "+rotZ+" "+
				" "+String.Format("{0:0.0}", x)+
				" "+String.Format("{0:0.0}", y)+
				" "+String.Format("{0:0.0}", z)+
				" "+String.Format("{0:0.0}", var1)+
				" "+String.Format("{0:0.0}", var2)+
				" "+String.Format("{0:0.0}", var3));
		}
		
		public string toFullString()
		{
			return (id.ToString("X")+
				" "+rotX+" "+rotY+" "+rotZ+" "+
				" "+x+" "+y+" "+z+
				" "+var1+" "+var2+" "+var3);
		}
		
		public string fixString(string old, int size)
		{
			while (old.Length < size)
			{
				old = old+" ";
			}
			
			return old;
		}
		
		public string fixFloatString(float val, int size)
		{
			string old = String.Format("{0:0.00}", val);
			
			while (old.Length < size)
			{
				old = old+" ";
			}
			return old;
		}
		
		public bool isEqualTo(SETObject other)
		{
			return (
				other.id   == id   &&
				other.clip == clip &&
				other.x    == x    &&
				other.y    == y    &&
				other.z    == z    &&
				other.rotX == rotX &&
				other.rotY == rotY &&
				other.rotZ == rotZ &&
				other.var1 == var1 &&
				other.var2 == var2 &&
				other.var3 == var3);
		}
		
		public int CompareTo(object obj)
		{
			SETObject orderToCompare = obj as SETObject;
			
			//Always organize by ID first
			if (orderToCompare.id < id)
			{
				return 1;
			}
			else if (orderToCompare.id > id)
			{
				return -1;
			}
			
			switch (organizeType)
			{
				//Just organize by ID
				case 0:
					break;
				
				//Organize by X Rotation
				case 1:
					if (orderToCompare.rotX < rotX)
					{
						return 1;
					}
					else if (orderToCompare.rotX > rotX)
					{
						return -1;
					}
					break;
					
				//Organize by Y Rotation
				case 2:
					if (orderToCompare.rotY < rotY)
					{
						return 1;
					}
					else if (orderToCompare.rotY > rotY)
					{
						return -1;
					}
					break;
					
				//Organize by Z Rotation
				case 3:
					if (orderToCompare.rotZ < rotZ)
					{
						return 1;
					}
					else if (orderToCompare.rotZ > rotZ)
					{
						return -1;
					}
					break;
					
				//Organize by X
				case 4:
					if (orderToCompare.x < x)
					{
						return 1;
					}
					else if (orderToCompare.x > x)
					{
						return -1;
					}
					break;
					
				//Organize by Y
				case 5:
					if (orderToCompare.y < y)
					{
						return 1;
					}
					else if (orderToCompare.y > y)
					{
						return -1;
					}
					break;
					
				//Organize by Z
				case 6:
					if (orderToCompare.z < z)
					{
						return 1;
					}
					else if (orderToCompare.z > z)
					{
						return -1;
					}
					break;
					
				//Organize by Variable 1
				case 7:
					if (orderToCompare.var1 < var1)
					{
						return 1;
					}
					else if (orderToCompare.var1 > var1)
					{
						return -1;
					}
					break;
					
				//Organize by Variable 2
				case 8:
					if (orderToCompare.var2 < var2)
					{
						return 1;
					}
					else if (orderToCompare.var2 > var2)
					{
						return -1;
					}
					break;
					
				//Organize by Variable 3
				case 9:
					if (orderToCompare.var3 < var3)
					{
						return 1;
					}
					else if (orderToCompare.var3 > var3)
					{
						return -1;
					}
					break;
					
				default:
					break;
			}
			
			//Still a tie? At least make it stable
			if (orderToCompare.index < index)
			{
				return 1;
			}
			
			return -1;
		}
	}
}
