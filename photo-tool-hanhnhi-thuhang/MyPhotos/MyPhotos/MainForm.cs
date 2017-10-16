using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Manning.MyPhotoAlbum;

namespace MyPhotos
{
    public partial class MainForm : Form
    {
        private AlbumManager _manager;
        private AlbumManager Manager
        {
            get
            {
                return _manager;
            }
            set
            {
                _manager = value;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            NewAlbum();
        }

        private void NewAlbum()
        {
            // TODO: clean up,  save  existing album
            Manager = new AlbumManager();
            DisplayAlbum();
        }

        private void DisplayAlbum()
        {
            pbxPhoto.Image = Manager.CurrentImage;
            SetTitleBar();
            SetStatusStrip(null);
        }

        private void SetTitleBar()
        {
            Version ver = new Version(Application.ProductVersion);
            String name = Manager.FullName;
            Text = String.Format("{2} - MyPhotos {0:0}.{1:0}",
                                 ver.Major, ver.Minor,
                                 string.IsNullOrEmpty(name) ? "Untitled" : name);
        }
        private void mnuFileLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Photo";
            dlg.Filter = "jpg files (*.jpg)|*.jpg"
            + "|All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pbxPhoto.Image = new Bitmap(dlg.OpenFile());
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Unable to load file: " + ex.Message);
                    pbxPhoto.Image = null;
                }
                SetStatusStrip(dlg.FileName);
            }
            dlg.Dispose();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuImage_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ProcessImageClick(e);

        }
        private void ProcessImageClick(ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            string enumVal = item.Tag as string;
            if (enumVal != null)
            {
                pbxPhoto.SizeMode = (PictureBoxSizeMode)Enum.Parse(typeof(PictureBoxSizeMode), enumVal);
            }
        }

        private void mnuImage_Click(object sender, EventArgs e)
        {

        }

        private void mnuImage_DropDownOpening(object sender, EventArgs e)
        {
            ProcessImageOpening(sender as ToolStripDropDownItem);
        }

        private void ProcessImageOpening(ToolStripDropDownItem parent)
        {
            if (parent != null)
            {
                string enumVal = pbxPhoto.SizeMode.ToString();
                foreach (ToolStripMenuItem item in parent.DropDownItems)
                {
                    item.Enabled = (pbxPhoto.Image != null);
                    item.Checked = item.Tag.Equals(enumVal);
                }
            }
        }
        private void SetStatusStrip(string path)
        {
            if (pbxPhoto.Image != null)
            {
                sttInfo.Text = Manager.Current.Caption;
                sttImageSize.Text = String.Format("{0:#}x{1:#}",
                                                 pbxPhoto.Image.Width,
                                                 pbxPhoto.Image.Height);
                sttAlbumPos.Text = String.Format("0:0/{1:0}",
                                                Manager.Index + 1,
                                                Manager.Album.Count);
            }
            else
            {
                sttInfo.Text = null;
                sttImageSize.Text = null;
                sttAlbumPos.Text = null;
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            NewAlbum();
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Album";
            dlg.Filter = "Album files (*.abm)|*.abm"
                         + "|All files (*.*)|*.*";
            dlg.InitialDirectory = AlbumManager.DefaultPath;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // TODO: save any existing album

                // Open the new album
                // TODO: handle invalid album file
                Manager = new AlbumManager(dlg.FileName);
                DisplayAlbum();
            }
            dlg.Dispose();
        }

        private void SaveAlbum(string name)
        {
            Manager.Save(name, true);
        }

        private void SaveAlbum()
        {
            if (string.IsNullOrEmpty(Manager.FullName))
                SaveAsAlbum();
            else
            {
                // Save the album under the existing name
                SaveAlbum(Manager.FullName);
            }
        }

        private void SaveAsAlbum()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save Album";
            dlg.DefaultExt = "abm";
            dlg.Filter = "Album files (*.abm)|*.abm"
                         + "|All files (*.*)|*.*";
            dlg.InitialDirectory = AlbumManager.DefaultPath;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SaveAlbum(dlg.FileName);
                // Update title bar to include new name
                SetTitleBar();
            }
            dlg.Dispose();
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            SaveAlbum();
        }

        private void mnuFiveSaveAs_Click(object sender, EventArgs e)
        {
            SaveAsAlbum();
        }

        private void mnuEditAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = " Add Photos";
            dlg.Multiselect = true;
            dlg.Filter = " Image Files (JPEG, GIF, BMP, etc.)|*.jpg;*.gif;*.bmp;*.tiff;*.png|" +
                "JPEG files  (* .jpg;*.jeg)|*.jpg;*.jpeg|" +
                "GIF file (*.gif)|*.gif|" +
                "BMP files (*.bmp)|*.bmp|" +
                "TIFF files (*.tif;*.tiff)|*.tif;*.tiff|" +
                "PNG files (*.png)|*.png|" +
                "All files (*.*)|*.*";
            dlg.InitialDirectory = Environment.CurrentDirectory;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] files = dlg.FileNames;

                int index = 0;
                foreach (string s in files)
                {
                    Photograph photo = new Photograph(s);

                    index = Manager.Album.IndexOf(photo);
                    if (index < 0)
                        Manager.Album.Add(photo);
                    else
                        photo.Dispose();
                }
                Manager.Index = Manager.Album.Count - 1;
            }
            dlg.Dispose();
            DisplayAlbum();
        }

        private void mnuEditRemove_Click(object sender, EventArgs e)
        {
            if (Manager.Album.Count > 0)
            {
                Manager.Album.RemoveAt(Manager.Index);
                DisplayAlbum();
            }
        }

        private void mnuNext_Click(object sender, EventArgs e)
        {
            if (Manager.Index < Manager.Album.Count - 1)
            {
                Manager.Index++;
                DisplayAlbum();
            }
        }

        private void mnuPrevious_Click(object sender, EventArgs e)
        {
            if (Manager.Index > 0)
            {
                Manager.Index--;
                DisplayAlbum();
            }
        }

        private void ctxMenuPhoto_Opening(object sender, CancelEventArgs e)
        {
            mnuNext.Enabled = (Manager.Index < Manager.Album.Count - 1);
            mnuPrevious.Enabled = (Manager.Index > 0);
        }
    }
}


