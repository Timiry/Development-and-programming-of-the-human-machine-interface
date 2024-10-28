using System.Text;
using System.IO;

namespace Rhymer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBoxIn.Text = "Опустел наш садик,\r\nулетели птицы,\r\nЛистья, облетая,\r\nтихо шелестят:\r\nНеужели лето\r\nвновь не возвратится\r\nВ наш забытый Богом,\r\nопустевший сад?";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            wordsDictionary = LoadWordsFromFile(wordsFilePath);
        }

        string wordsFilePath = "russian.txt"; // Путь к файлу со словами
        Dictionary<(int, string), List<string>> wordsDictionary;

        private void buttonRhyme_Click(object sender, EventArgs e)
        {
            string poem = textBoxIn.Text;
            if (string.IsNullOrEmpty(poem)) MessageBox.Show("Введите стихотворение");
            
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

                    // Поиск подходящего слова
                    if (wordsDictionary.TryGetValue((syllableCount, ending), out var candidates))
                    {
                        // Замена на случайное подходящее слово
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

                // Определение количества слогов и окончания
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
            // Простой алгоритм для подсчета слогов
            return word.Count(c => "аеёиоуыэюяАЕЁИОУЫЭЮЯ".Contains(c));
        }

        static string GetEnding(string word)
        {
            // Получение окончания (последние 2-3 буквы)
            return word.Length >= 3 ? word.Substring(word.Length - 3) : word;
        }
    }
}
