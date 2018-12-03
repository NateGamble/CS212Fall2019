using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using static HashExample;
using System.Security.Cryptography;

namespace Project02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window { 

        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        private int babbleOrder;
        private Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input, @"\s+");       // split into array of words
                //  Order 1 hash table
                hashTable = HashExample.makeHashtable(words);
            }
        }

        private void analyzeInput(int order)
        {
            babbleOrder = order;
            if (babbleOrder > 0)
            {
                MessageBox.Show("Analyzing at order: " + babbleOrder);
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            if (babbleOrder == 0)
            {
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                    textBlock1.Text += " " + words[i];
            } else if (babbleOrder == 1) {
                String currentWord = words[0];
                String nextWord;
                ArrayList currentWordList = new ArrayList();
                RandomNumberGenerator rnd = RandomNumberGenerator.Create();
                byte[] rndArray = new byte[4];
                int randomNumber;
                //  Adds the first word to the text block
                textBlock1.Text += " " + currentWord;
                //  Magically prints out correct babble
                for (int i = 0; i < wordCount; i++)
                {
                    //  gets the array for the key currentWord
                    hashTable.TryGetValue(currentWord, out currentWordList);
                    //  Creates random number
                    rnd.GetBytes(rndArray);
                    randomNumber = (int)(BitConverter.ToUInt32(rndArray, 0) % currentWordList.Count);
                    //  gets next word using the current word as a key and a random element for that key
                    nextWord = currentWordList[randomNumber].ToString();

                    textBlock1.Text += " " + nextWord;
                    currentWord = nextWord;
                }
            }
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
        }
    }
    }
