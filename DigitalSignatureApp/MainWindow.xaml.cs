using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace DigitalSignatureApp
{
    public partial class MainWindow : Window
    {
        private DSACryptoServiceProvider dsa;
        private string selectedFilePath;
        private string selectedSignatureFilePath;

        public MainWindow()
        {
            InitializeComponent();
            dsa = new DSACryptoServiceProvider(1024); // Генерація DSA ключа 1024 біт
            InputTextBox_LostFocus(null, null); // Встановлюємо текст-заповнювач при запуску
        }


        private byte[] SignData(byte[] data)
        {
            return dsa.SignData(data, HashAlgorithmName.SHA1);
        }

        private bool VerifySignature(byte[] data, byte[] signature)
        {
            return dsa.VerifyData(data, signature, HashAlgorithmName.SHA1);
        }

        // Подія для очищення тексту-заповнювача, коли TextBox отримує фокус
        private void InputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text == "Enter text to sign...")
            {
                InputTextBox.Text = "";
                InputTextBox.Foreground = Brushes.Black;
            }
        }

        // Подія для встановлення тексту-заповнювача, коли TextBox втрачає фокус
        private void InputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                InputTextBox.Text = "Enter text to sign...";
                InputTextBox.Foreground = Brushes.Gray;
            }
        }

        private void SignTextButton_Click(object sender, RoutedEventArgs e)
        {
            var data = Encoding.UTF8.GetBytes(InputTextBox.Text);
            var signature = SignData(data);
            TextSignatureBox.Text = BitConverter.ToString(signature).Replace("-", "");
        }

        private void SaveTextSignatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextSignatureBox.Text))
            {
                MessageBox.Show("Please sign the text first.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                DefaultExt = ".txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, TextSignatureBox.Text);
                MessageBox.Show("Text signature saved.");
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                MessageBox.Show($"File selected: {selectedFilePath}");
            }
        }

        private void SignFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select a file to sign.");
                return;
            }

            var data = File.ReadAllBytes(selectedFilePath);
            var signature = SignData(data);
            FileSignatureBox.Text = BitConverter.ToString(signature).Replace("-", "");
        }

        private void SaveFileSignatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FileSignatureBox.Text))
            {
                MessageBox.Show("Please sign the file first.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                DefaultExt = ".txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, FileSignatureBox.Text);
                MessageBox.Show("File signature saved.");
            }
        }

        private void SelectVerifyFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
               // MessageBox.Show($"File selected for verification: {selectedFilePath}");
            }
        }

        private void SelectSignatureFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                DefaultExt = ".txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedSignatureFilePath = openFileDialog.FileName;
                //MessageBox.Show($"Signature file selected: {selectedSignatureFilePath}");
            }
        }

        private void VerifySignatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || string.IsNullOrEmpty(selectedSignatureFilePath))
            {
                MessageBox.Show("Please select both a file and its signature to verify.");
                return;
            }

            var data = File.ReadAllBytes(selectedFilePath);
            var signatureHex = File.ReadAllText(selectedSignatureFilePath);
            var signature = HexStringToByteArray(signatureHex);
            bool isValid = VerifySignature(data, signature);

            MessageBox.Show(isValid ? "Signature is valid!" : "ERROR: Invalid signature.");
        }

        private byte[] HexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
