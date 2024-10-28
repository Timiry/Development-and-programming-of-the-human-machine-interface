using System.Text;
using System.IO;

namespace Rhymer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBoxIn.Text = "������� ��� �����,\r\n������� �����,\r\n������, �������,\r\n���� ��������:\r\n������� ����\r\n����� �� �����������\r\n� ��� ������� �����,\r\n���������� ���?";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            wordsDictionary = LoadWordsFromFile(wordsFilePath);
        }

        string wordsFilePath = "russian.txt"; // ���� � ����� �� �������
        Dictionary<(int, string), List<string>> wordsDictionary;

        private void buttonRhyme_Click(object sender, EventArgs e)
        {
            string poem = textBoxIn.Text;
            if (string.IsNullOrEmpty(poem)) MessageBox.Show("������� �������������");
            
            string modifiedPoem = ReplaceWordsInPoem(poem, wordsDictionary);
            textBoxOut.Text = modifiedPoem;
        }

        static string ReplaceWordsInPoem(string poem, Dictionary<(int, string), List<string>> wordsDictionary)
        {
            var lines = poem.Split('\n');


            for (int i = 0; i < lines.Length; i++)
            {
                var words = lines[i].Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];
                    int syllableCount = CountSyllables(word);
                    string ending = GetEnding(word);

                    // ����� ����������� �����
                    if (wordsDictionary.TryGetValue((syllableCount, ending), out var candidates))
                    {
                        // ������ �� ��������� ���������� �����
                        words[j] = candidates[new Random().Next(candidates.Count)];
                    }
                }
                lines[i] = string.Join(" ", words);
            }

            return string.Join("\n", lines);
        }

        static Dictionary<(int, string), List<string>> LoadWordsFromFile(string filePath)
        {
            var wordsDictionary = new Dictionary<(int, string), List<string>>();

            foreach (var line in File.ReadLines(filePath, Encoding.GetEncoding(1251)))
            {
                string word = line.Trim();

                // ����������� ���������� ������ � ���������
                int syllableCount = CountSyllables(word);
                string ending = GetEnding(word);

                var key = (syllableCount, ending);
                if (!wordsDictionary.ContainsKey(key))
                {
                    wordsDictionary[key] = new List<string>();
                }
                wordsDictionary[key].Add(word);
            }

            return wordsDictionary;
        }

        static int CountSyllables(string word)
        {
            // ������� �������� ��� �������� ������
            return word.Count(c => "����������Ũ�������".Contains(c));
        }

        static string GetEnding(string word)
        {
            // ��������� ��������� (��������� 2-3 �����)
            return word.Length >= 3 ? word.Substring(word.Length - 3) : word;
        }
    }
}
