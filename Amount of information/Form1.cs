using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amount_of_information
{
    public partial class Form1 : Form
    {
        public string text;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "D:\\Daniar\\C#_project\\Amount of information";
                openFileDialog.Filter = "txt Files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileStream = openFileDialog.OpenFile();
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        text = reader.ReadToEnd();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Regex reg = new Regex("[0-9а-яёА-ЯЁa-zA-Z',.;( )*#@!&?+_=^%$`~<>|/{}]");
            var letters = new List<string>();
            if (reg.IsMatch(text))
            {
                foreach (Match match in reg.Matches(text))
                {
                    letters.Add(match.Value);
                }

            }

            int symbol_count = 0;
            var found_unique_symbols = new List<string>();

            foreach (string letter in letters)
            {
                if (!found_unique_symbols.Contains(letter))
                {
                    found_unique_symbols.Add(letter);
                }

                symbol_count++;
            }
            var unique_symbols = new List<Tuple<string, int>>();

            foreach (string letter in found_unique_symbols)
            {
                unique_symbols.Add(Tuple.Create(letter, 0));
            }

            foreach (string letter in letters)
            {
                for (int i = 0; i < unique_symbols.Count; i++)
                {
                    if (unique_symbols[i].Item1 == letter)
                    {
                        unique_symbols[i] = Tuple.Create(unique_symbols[i].Item1, unique_symbols[i].Item2 + 1);
                    }
                }
            }

            double Entropia = 0;

            for (int i = 0; i < unique_symbols.Count; i++)
            {

                Entropia += ((double)unique_symbols[i].Item2 / (double)symbol_count) * Math.Log((double)1 / ((double)unique_symbols[i].Item2 / (double)symbol_count), 2.0);
            }

            Letters.Text = Entropia.ToString();

            AllEntropia.Text = (Entropia * symbol_count).ToString();

            textBox3.Text = (1 - ((double)Entropia / 8)).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // создаём Bitmap из изображения, находящегося в pictureBox1
                Bitmap input = new Bitmap(pictureBox1.Image);
                // создаём Bitmap для черно-белого изображения
                Bitmap output = new Bitmap(input.Width, input.Height);
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16);
                        float G = (float)((pixel & 0x0000FF00) >> 8);
                        float B = (float)(pixel & 0x000000FF);
                        R = G = B = (R + G + B) / 3.0f;
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                pictureBox1.Image = output;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Загрузка изображения
            Bitmap image = new Bitmap(pictureBox1.Image);

            // Подсчет частоты появления каждого значения пикселя
            int[] pixelCount = new int[256];
            int totalPixels = 0;

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color pixelColor = image.GetPixel(i, j);
                    byte pixelValue = pixelColor.R;

                    pixelCount[pixelValue]++;
                    totalPixels++;
                }
            }

            // Вычисление энтропии
            double entropyImage = 0;

            for (int i = 0; i < 256; i++)
            {
                if (pixelCount[i] > 0)
                {
                    double probability = (double)pixelCount[i] / totalPixels;
                    entropyImage -= probability * Math.Log(probability, 2);
                }
            }

            EntImg.Text = entropyImage.ToString();
            AllEntPik.Text = (image.Width * image.Height * entropyImage).ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.Text = Letters.Text;
            textBox1.Text = AllEntropia.Text;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox2.Text = EntImg.Text;
            textBox1.Text = AllEntPik.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = "D:\\Daniar\\C#_project\\Amount of information";
            ofd.Filter = "txt Files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Huffman huffman = new Huffman();
                huffman.CompressFile(ofd.FileName, "D:\\Daniar\\C#_project\\Прошлое\\Amount of information\\TextArh.txt.huf");
            }
        }



        class Huffman
        {
            public void CompressFile(string fileName, string arhFileName)
            {
                byte[] data = File.ReadAllBytes(fileName);
                byte[] arch = CompressBytes(data);
                File.WriteAllBytes(arhFileName, arch);

            }

            private byte[] CompressBytes(byte[] data)
            {
                int[] freqs = CalculateFreq(data);
                byte[] head = CreateHeader(data.Length, freqs);
                Node root = CreateHuffmanTree(freqs);
                string[] codes = CreateHuffmanCode(root);
                byte[] bits = Compress(data, codes);
                return head.Concat(bits).ToArray();
            }

            private byte[] CreateHeader(int datalength, int[] freqs)
            {
                List<byte> head = new List<byte>();
                head.Add((byte)(datalength & 255));
                head.Add((byte)((datalength >> 8) & 255));
                head.Add((byte)((datalength >> 16) & 255));
                head.Add((byte)((datalength >> 24) & 255));
                head.Add((byte)((datalength >> 32) & 255));
                head.Add((byte)((datalength >> 40) & 255));

                for (int j = 0; j < 256; j++)
                {
                    head.Add((byte)freqs[j]);
                }
                return head.ToArray();
            }

            private byte[] Compress(byte[] data, string[] codes)
            {
                List<byte> bits = new List<byte>();
                byte sum = 0;
                byte bit = 1;
                foreach (byte symbol in data)
                    foreach (char c in codes[symbol])
                    {
                        if (c == '1')
                        {
                            sum |= bit;
                        }
                        if (bit < 128)
                        {
                            bit <<= 1;
                        }
                        else
                        {
                            bits.Add(sum);
                            sum = 0;
                            bit = 1;
                        }
                    }
                if (bit > 1)
                {
                    bits.Add(sum);
                }
                return bits.ToArray();
            }

            private string[] CreateHuffmanCode(Node root)
            {
                string[] codes = new string[256];
                Next(root, "");
                return codes;

                void Next(Node node, string code)
                {
                    if (node.bit0 == null)
                    {
                        codes[node.symbol] = code;
                    }
                    else
                    {
                        Next(node.bit0, code + "0");
                        Next(node.bit1, code + "1");

                    }
                }
            }

            private int[] CalculateFreq(byte[] data)
            {
                int[] freqs = new int[256];
                foreach (byte b in data)
                {
                    freqs[b]++;
                }
                return freqs;
            }

            private Node CreateHuffmanTree(int[] freqs)
            {
                PrioretyQueue<Node> pq = new PrioretyQueue<Node>();
                for (int j = 0; j < 256; j++)
                {
                    if (freqs[j] > 0)
                    {
                        pq.Enqueue(freqs[j], new Node((byte)j, (byte)freqs[j]));
                    }
                }
                while (pq.Size() > 1)
                {
                    Node byte0 = pq.Dequeue();
                    Node byte1 = pq.Dequeue();
                    int freq = byte0.freq + byte1.freq;
                    Node next = new Node(byte0, byte1, (byte)freq);
                    pq.Enqueue(freq, next);
                }
                return pq.Dequeue();
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = "D:\\Daniar\\C#_project\\Amount of information";
            ofd.Filter = "txt Files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LZW lzw = new LZW();
                lzw.CompressFile(ofd.FileName, "D:\\Daniar\\C#_project\\Прошлое\\Amount of information\\TextArh.txt.lzw");
            }
        }

        class LZW
        {
            public void CompressFile(string fileName, string arhFileName)
            {
                // Чтение данных из исходного файла
                string data = File.ReadAllText(fileName);
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                int index = 0;
                foreach (string word in data.Split(' '))
                {
                    if (!dictionary.ContainsKey(word))
                    {
                        dictionary.Add(word, index);
                        index++;
                        Console.WriteLine(word);
                    }
                }
                // Архивация данных
                List<int> compressedData = new List<int>();
                foreach (string word in data.Split(' '))
                {
                    compressedData.Add(dictionary[word]);
                }

                using (BinaryWriter writer = new BinaryWriter(File.Open(arhFileName, FileMode.Create)))
                {
                    foreach (int code in compressedData)
                    {
                        writer.Write(code);
                    }
                }
            }
        }
    }
}