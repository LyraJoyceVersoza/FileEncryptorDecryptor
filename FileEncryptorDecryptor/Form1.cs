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

namespace FileEncryptorDecryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = openFileDialog1.FileName;
            byte[] fileBytes = File.ReadAllBytes(filePath);

            int key = 3; 
            byte[] encryptedBytes = RailFenceEncrypt(fileBytes, key);

            File.WriteAllBytes(filePath, encryptedBytes);
            label1.Text = "File ENCRYPTED.";
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = openFileDialog2.FileName;
            byte[] fileBytes = File.ReadAllBytes(filePath);

            int key = 3; 
            byte[] decryptedBytes = RailFenceDecrypt(fileBytes, key);

            File.WriteAllBytes(filePath, decryptedBytes);
            label1.Text = "File DECRYPTED.";
        }

        private static byte[] RailFenceEncrypt(byte[] data, int rails)
        {
            if (rails <= 1 || data.Length < 2) return data;

            int length = data.Length;
            byte[] encrypted = new byte[length];

            int index = 0;
            for (int rail = 0; rail < rails; rail++)
            {
                int step1 = (rails - rail - 1) * 2;
                int step2 = rail * 2;
                int pos = rail;
                bool toggle = true;

                while (pos < length)
                {
                    encrypted[index++] = data[pos];
                    if (step1 == 0) pos += step2;
                    else if (step2 == 0) pos += step1;
                    else
                    {
                        pos += toggle ? step1 : step2;
                        toggle = !toggle;
                    }
                }
            }
            return encrypted;
        }

        private static byte[] RailFenceDecrypt(byte[] data, int rails)
        {
            if (rails <= 1 || data.Length < 2) return data;

            int length = data.Length;
            byte[] decrypted = new byte[length];
           
            bool[,] railMatrix = new bool[rails, length];
            int row = 0, direction = 1;

            for (int i = 0; i < length; i++)
            {
                railMatrix[row, i] = true;
                row += direction;

                if (row == 0 || row == rails - 1)
                    direction *= -1;
            }
                        
            int dataIndex = 0;
            byte[,] railData = new byte[rails, length];

            for (int r = 0; r < rails; r++)
            {
                for (int c = 0; c < length; c++)
                {
                    if (railMatrix[r, c])
                        railData[r, c] = data[dataIndex++];
                }
            }

            row = 0;
            direction = 1;
            for (int i = 0; i < length; i++)
            {
                decrypted[i] = railData[row, i];
                row += direction;

                if (row == 0 || row == rails - 1)
                    direction *= -1;
            }

            return decrypted;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
