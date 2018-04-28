using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitmapToBytes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            openFileDialog1.Filter = "Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;

                Bitmap myImg = (Bitmap)Bitmap.FromFile(file);

                if (myImg.Width % 8 == 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{" + Environment.NewLine);
                    StringBuilder sbLine = new StringBuilder();
                    for (int ii = 0; ii < myImg.Height; ii++)
                    {
                        // loop each row of image
                        for (int jj = 0; jj < myImg.Width; jj++)
                        {
                            // loop each pixel in row and add to sbLine string to build
                            // string of bits for black and white image
                            Color pixelColor = myImg.GetPixel(jj, ii);
                            sbLine.Append(HexConverter(pixelColor));
                        }
                        // convert sbline string to byte array
                        byte[] buffer = GetBytes(sbLine.ToString());
                        // add first 0x to output row
                        sb.Append("0x");
                        // convert byte array to hex
                        sb.Append(BitConverter.ToString(buffer).Replace("-", ",0x"));
                        // clear line data
                        sbLine.Clear();
                        buffer = null;
                        // add comma to end of row
                        sb.Append(",");
                        // add new line
                        sb.Append(Environment.NewLine);
                    }
                    // write output to screen

                    sb.Append("};" + Environment.NewLine);
                    textBox1.Text = sb.ToString();
                }
                else
                {
                    // image isnt multiple of 8 wide
                    MessageBox.Show("Image is not a multiple of 8 pixels wide");
                }
            }
        }

        public byte[] GetBytes(string bitString)
        {
            return Enumerable.Range(0, bitString.Length / 8).
                Select(pos => Convert.ToByte(
                    bitString.Substring(pos * 8, 8),
                    2)
                ).ToArray();
        }

        private String HexConverter(System.Drawing.Color c)
        {
            if ((c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")).Equals("000000"))
            {
                // image black pixel
                return "0";
            }
            else
            {
                // image is not a black pixel
                return "1";
            }
        }

        // save text file
        private void saveTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                File.WriteAllText(saveFileDialog1.FileName, textBox1.Text);
                
            }
        }
    }
}
