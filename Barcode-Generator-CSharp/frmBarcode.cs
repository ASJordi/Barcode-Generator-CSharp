using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using BarcodeLib;

namespace Barcode_Generator_CSharp
{
    public partial class frmBarcode : Form
    {
        public frmBarcode()
        {
            InitializeComponent();
        }

        public class OpcionCombo
        {
            public int Valor { get; set; }
            public string Texto { get; set; }
        }

        private void frmBarcode_Load(object sender, EventArgs e)
        {
            LoadBarcodeTypes();
        }

        private void LoadBarcodeTypes()
        {
            //Load barcode types
            int index = 0;
            foreach (var name in Enum.GetNames(typeof(TYPE)))
            {
                cmTypesBarcode.Items.Add(new OpcionCombo() { Valor = index, Texto = name });
                index++;
            }

            cmTypesBarcode.DisplayMember = "Texto";
            cmTypesBarcode.ValueMember = "Valor";
            cmTypesBarcode.SelectedIndex = 31;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                Image CodeImage;
                int index = (cmTypesBarcode.SelectedItem as OpcionCombo).Valor;
                TYPE codeType = (TYPE)index;

                Barcode code = new Barcode();
                code.IncludeLabel = true;
                code.LabelPosition = LabelPositions.BOTTOMCENTER;

                //set code of txtCode
                CodeImage = code.Encode(codeType, txtCode.Text.Trim(), Color.Black, Color.White, 300, 100);

                //set txtTitle as extra in image
                Bitmap imageTitle = ConvertTextToImage(txtTitle.Text.Trim(), 300, Color.White);
                int NewHeightImage = CodeImage.Height + imageTitle.Height;
                Bitmap newImage = new Bitmap(300, NewHeightImage);
                Graphics draw = Graphics.FromImage(newImage);
                draw.DrawImage(imageTitle, new Point(0, 0));
                draw.DrawImage(CodeImage, new Point(0, imageTitle.Height));

                //set image in PictureBox
                pbCode.BackgroundImage = newImage;
            }
            catch (Exception)
            {
                MessageBox.Show("Not supported operation" + Environment.NewLine + "Please review the text and code input", "Barcode Generator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Image codeImage = pbCode.BackgroundImage.Clone() as Image;

            SaveFileDialog windowFile = new SaveFileDialog();
            windowFile.FileName = string.Format("{0}.png", txtCode.Text.Trim());
            windowFile.Filter = "Imagen |*.png";

            if (windowFile.ShowDialog() == DialogResult.OK)
            {
                codeImage.Save(windowFile.FileName, ImageFormat.Png);
                MessageBox.Show("Barcode generated", "Barcode Generator", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static Bitmap ConvertTextToImage(string texto, int _width, Color color)
        {
            //Create object Bitmap
            Bitmap objBitmap = new Bitmap(1, 1);
            int Width = 0;
            int Height = 0;
            //format the font (font type, size)
            Font objFont = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            //Create Graphics object from Bitmap obejct
            Graphics objGraphics = Graphics.FromImage(objBitmap);

            //Set the size according to the length of the text
            Width = _width;
            Height = (int)objGraphics.MeasureString(texto, objFont).Height + 5;
            objBitmap = new Bitmap(objBitmap, new Size(Width, Height));

            objGraphics = Graphics.FromImage(objBitmap);

            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            objGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            objGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            StringFormat drawFormat = new StringFormat();
            objGraphics.Clear(color);

            drawFormat.Alignment = StringAlignment.Center;
            objGraphics.DrawString(texto, objFont, new SolidBrush(Color.Black), new RectangleF(0, (objBitmap.Height / 2) - (objBitmap.Height - 10), objBitmap.Width, objBitmap.Height), drawFormat);
            objGraphics.Flush();

            return objBitmap;
        }
    }
}
