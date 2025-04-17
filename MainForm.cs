using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CopyrighterApp
{
    public partial class MainForm : Form
    {
        private string selectedImagePath;
        private string copyrightText = "© MyCopyright";
        private string copyrightDirectory;

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Копирайтер";
            this.Width = 1000;
            this.Height = 600;

            MenuStrip menu = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Open...", null, OpenImage);
            fileMenu.DropDownItems.Add("Open directory...", null, OpenDirectory);
            fileMenu.DropDownItems.Add("Copyright directory...", null, SetCopyrightDir);
            fileMenu.DropDownItems.Add("Copyright text...", null, SetCopyrightText);
            menu.Items.Add(fileMenu);
            this.MainMenuStrip = menu;
            this.Controls.Add(menu);

            ListView thumbnailList = new ListView
            {
                View = View.LargeIcon,
                Dock = DockStyle.Left,
                Width = 200
            };
            thumbnailList.SelectedIndexChanged += ThumbnailList_SelectedIndexChanged;
            thumbnailList.Name = "ThumbnailList";
            this.Controls.Add(thumbnailList);

            PictureBox pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            pictureBox.Name = "MainPictureBox";
            this.Controls.Add(pictureBox);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };

            Button addCopyrightBtn = new Button { Text = "Add Copyright" };
            addCopyrightBtn.Click += AddCopyright;

            Button saveImageBtn = new Button { Text = "Save image..." };
            saveImageBtn.Click += SaveImage;

            Button batchBtn = new Button { Text = "Batch mode" };
            batchBtn.Click += BatchMode;

            buttonPanel.Controls.Add(addCopyrightBtn);
            buttonPanel.Controls.Add(saveImageBtn);
            buttonPanel.Controls.Add(batchBtn);

            this.Controls.Add(buttonPanel);
        }

        private void OpenImage(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.jpg;*.png;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = ofd.FileName;
                LoadImageToMainBox(selectedImagePath);
            }
        }

        private void OpenDirectory(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var files = Directory.GetFiles(fbd.SelectedPath, "*.*");
                var thumbnailList = this.Controls.Find("ThumbnailList", true)[0] as ListView;
                thumbnailList.Items.Clear();
                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(64, 64);

                int index = 0;
                foreach (var file in files)
                {
                    if (file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".bmp"))
                    {
                        Image img = Image.FromFile(file);
                        imageList.Images.Add(img.GetThumbnailImage(64, 64, null, IntPtr.Zero));
                        var item = new ListViewItem(Path.GetFileName(file), index++);
                        item.Tag = file;
                        thumbnailList.Items.Add(item);
                    }
                }

                thumbnailList.LargeImageList = imageList;
            }
        }

        private void LoadImageToMainBox(string path)
        {
            var pictureBox = this.Controls.Find("MainPictureBox", true)[0] as PictureBox;
            pictureBox.Image = Image.FromFile(path);
        }

        private void AddCopyright(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedImagePath)) return;

            Image img = Image.FromFile(selectedImagePath);
            Graphics g = Graphics.FromImage(img);
            Font font = new Font("Arial", 20);
            Brush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
            g.DrawString(copyrightText, font, brush, new PointF(10, img.Height - 40));
            g.Dispose();

            var pictureBox = this.Controls.Find("MainPictureBox", true)[0] as PictureBox;
            pictureBox.Image = img;
        }

        private void SaveImage(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var pictureBox = this.Controls.Find("MainPictureBox", true)[0] as PictureBox;
                pictureBox.Image.Save(sfd.FileName);
            }
        }

        private void BatchMode(object sender, EventArgs e)
        {
            MessageBox.Show("Batch mode: not yet implemented fully");
        }

        private void SetCopyrightDir(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                copyrightDirectory = fbd.SelectedPath;
            }
        }

        private void SetCopyrightText(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Введите текст копирайта", "Копирайт", copyrightText);
            if (!string.IsNullOrEmpty(input))
            {
                copyrightText = input;
            }
        }

        private void ThumbnailList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var list = sender as ListView;
            if (list.SelectedItems.Count > 0)
            {
                selectedImagePath = list.SelectedItems[0].Tag.ToString();
                LoadImageToMainBox(selectedImagePath);
            }
        }
    }
}