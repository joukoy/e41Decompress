﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace e41Decompress
{
    public partial class frmFileSelection : Form
    {
        public frmFileSelection()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listFiles.ListViewItemSorter = lvwColumnSorter;

        }

        public string filter = ".bin";
        private ListViewColumnSorter lvwColumnSorter;

        public void LoadFiles(string Folder)
        {
            listFiles.Enabled = true;
            listFiles.Items.Clear();
            listFiles.View = View.Details;
            listFiles.Columns.Add("File");
            listFiles.Columns.Add("Folder");
            listFiles.Columns.Add("Size");
            listFiles.Columns[0].Width = 300;
            listFiles.Columns[1].Width = 300;
            listFiles.Columns[2].Width = 200;

            listFiles.CheckBoxes = true;
            if (Folder == "")
                Folder = Application.StartupPath;
            DirectoryInfo d = new DirectoryInfo(Folder);
            FileInfo[] Files;
            if (chkSubfolders.Checked)
                Files = d.GetFiles("*.*", SearchOption.AllDirectories);
            else
                Files = d.GetFiles("*.*");

            string[] filters;
            filters = new string[1];
            filters[0] = filter;
            foreach (FileInfo file in Files)
            {
                for (int f = 0; f < filters.Length; f++)
                {
                    if (file.Extension.ToLower() == filters[f].ToLower() || chkAllfiles.Checked)
                    {
                        var item = new ListViewItem(file.Name);
                        item.Tag = file.FullName;
                        item.SubItems.Add(file.DirectoryName);
                        string filesize = file.Length.ToString();
                        //if (file.Length > 1023)
                          //  filesize = ((int)(file.Length / 1024)).ToString() + " k";
                        item.SubItems.Add(filesize);
                        listFiles.Items.Add(item);
                        break;
                    }
                }
            }
            txtFolder.Text = Folder;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string Folder = Helpers.SelectFolder("Select folder", txtFolder.Text);
            if (Folder.Length > 0)
                LoadFiles(Folder);
        }


        private void txtFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadFiles(txtFolder.Text);

        }


        private void chkSubfolders_CheckedChanged(object sender, EventArgs e)
        {
            LoadFiles(txtFolder.Text);
        }

        private void chkIncludeCustomFileTypes_CheckedChanged(object sender, EventArgs e)
        {
            LoadFiles(txtFolder.Text);
        }

        private void frmFileSelection_Load(object sender, EventArgs e)
        {
            listFiles.ColumnClick += ListFiles_ColumnClick;
        }

        private void ListFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;                    
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listFiles.Sort();
        }

        private void frmFileSelection_FormClosing(object sender, EventArgs e)
        {
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listFiles.Items.Count; i++)
            {
                if (chkSelectAll.Checked)
                    listFiles.Items[i].Checked = true;
                else
                    listFiles.Items[i].Checked = false;
            }

        }

        private void chkAllfiles_CheckedChanged(object sender, EventArgs e)
        {
            LoadFiles(txtFolder.Text);
        }
    }
}
