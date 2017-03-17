using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsMagickPOC
{
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.Remoting.Messaging;

    using ImageMagick;
    

    public partial class Form1 : Form
    {
        private ImageMagick.MagickImage magickImage;

        private MagickImage layer1;

        private MagickImage layer2;

        //private ImageMagick.MagickImage imageMagickImage;

        public Form1()
        {
            InitializeComponent();
            this.mainPictureBox.Dock = DockStyle.Fill;
            this.mainPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.Dock = DockStyle.Fill;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.imageToolStripMenuItem.Visible = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.tif;*.tiff;";
                dialog.InitialDirectory = @"C:\Users\Pavel_Hubich\Downloads";
                dialog.Title = "Please select an image";
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var fileInfo = new FileInfo(dialog.FileName);
                    if (fileInfo.Exists)
                    {

                        this.magickImage = new ImageMagick.MagickImage(
                                               new FileInfo(fileInfo.FullName),
                                               new MagickReadSettings { Debug = true, Verbose = true });
                        this.layer1 = new ImageMagick.MagickImage(
                                               new FileInfo(fileInfo.FullName),
                                               new MagickReadSettings { Debug = true, Verbose = true, FrameIndex = 1 });
                        this.layer2 = new ImageMagick.MagickImage(
                                               new FileInfo(fileInfo.FullName),
                                               new MagickReadSettings { Debug = true, Verbose = true, FrameIndex = 2 });
                        var img = new MagickImage();
                        this.magickImage.Settings.Verbose = true;
                        this.richTextBox1.Clear();
                        var sb = new StringBuilder();
                        foreach (var attr in this.magickImage.AttributeNames)
                        {
                            sb.AppendLine(string.Format("{0} : {1}", attr, this.magickImage.GetAttribute(attr)));
                        }
                        this.richTextBox1.Text = sb.ToString();
                        this.mainPictureBox.Image = this.magickImage.ToBitmap();
                        this.imageToolStripMenuItem.Visible = true;
                        //var ololo = 
                    }
                }
            }
        }

        private void changeBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.FullOpen = true;
                colorDialog.AnyColor = true;
                var result = colorDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var col = new MagickImageCollection();
                    var nImg = new MagickImage(
                                   new MagickColor(colorDialog.Color),
                                   this.magickImage.BaseWidth,
                                   this.magickImage.BaseHeight);
                    nImg.Compose = CompositeOperator.SrcIn;
                    var layer = new MagickImage(this.magickImage.ToByteArray(), new MagickReadSettings {FrameIndex = 1 });
                    col.Add(nImg);
                    col.Add(layer);
                    col.Add(this.magickImage);
                    this.pictureBox1.Image = col.Flatten().ToBitmap();
                }
            }
            
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void addBackgroundImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.tif;*.tiff;";
                dialog.InitialDirectory = @"C:\Users\Pavel_Hubich\Downloads";
                dialog.Title = "Please select an image";
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var fileInfo = new FileInfo(dialog.FileName);
                    if (fileInfo.Exists)
                    {
                        var newImage = new ImageMagick.MagickImage(new FileInfo(fileInfo.FullName));
                        var col = new MagickImageCollection();
                        //{
                            newImage.Compose = CompositeOperator.Over;
                            col.Add(newImage);
                            col.Add(this.magickImage);
                            this.pictureBox1.Image = col.Flatten().ToBitmap();
                        //}
                    }
                }
            }
        }

        private void deconstructToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //var newImage = new ImageMagick.MagickImage(new FileInfo(fileInfo.FullName));
            var col = new MagickImageCollection();
            //{
            //this.layer1.Compose = CompositeOperator.Over;
            //this.layer2.Compose = CompositeOperator.SrcIn;
            col.Add(this.layer1);
            col.Add(this.layer2);
            //col.Combine();
            //col.Flatten();
            var flattened = col.Merge();
            this.pictureBox1.Image = flattened.ToBitmap();
            //}

        }
    }
}
