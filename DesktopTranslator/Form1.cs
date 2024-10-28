using GTranslatorAPI;

namespace DesktopTranslator
{
    public partial class Form1 : Form
    {
        GTranslatorAPIClient translator = new GTranslatorAPIClient();

        Dictionary<string, Languages> dictLangs = new Dictionary<string, Languages>()
        {
            { "Русский", Languages.ru},
            { "Английский", Languages.en},
            { "Французский", Languages.fr},
            { "Немецкий", Languages.de },
            { "Испанский", Languages.es },
            { "Итальянский", Languages.it },
            { "Арабский", Languages.ar },
            { "Хинди", Languages.hi },
        };

        public Form1()
        {
            InitializeComponent();
            comboBoxSourceLang.Items.AddRange(dictLangs.Keys.ToArray());
            comboBoxSourceLang.SelectedIndex = 0;
            comboBoxTargetLang.Items.AddRange(dictLangs.Keys.ToArray());
            comboBoxTargetLang.SelectedIndex = 1;
            InitializeCheckBoxes();
            InitializeDataGridView();
        }

        private void InitializeCheckBoxes()
        {
            // Создание и добавление чекбоксов
            for (int i = 0; i < dictLangs.Keys.ToArray().Length; i++)
            {
                int x = (i % 2 == 0) ? 690 : 850;
                int y = (i % 2 == 0) ? 100 + (15 * i) : 100 + (15 * (i - 1));

                CheckBox checkBox = new CheckBox
                {
                    Text = dictLangs.Keys.ToArray()[i],
                    Location = new System.Drawing.Point(x, y),
                    Size = new System.Drawing.Size(150, 28)
                };
                this.Controls.Add(checkBox);
            }
        }

        private void InitializeDataGridView()
        {
            // Настройка DataGridView
            DataGridView dataGridView = new DataGridView
            {
                Location = new System.Drawing.Point(690, 225),
                Size = new System.Drawing.Size(457, 275),
                ColumnCount = 2
            };
            dataGridView.Columns[0].Name = "Язык";
            dataGridView.Columns[1].Name = "Текст";
            dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.Controls.Add(dataGridView);
        }

        private async void buttonTranslate_Click(object sender, EventArgs e)
        {
            string inputText = textBoxInput.Text;
            string sourseLanguage = comboBoxSourceLang.SelectedItem.ToString();
            string targetLanguage = comboBoxTargetLang.SelectedItem.ToString();

            if (string.IsNullOrEmpty(inputText) || inputText.Trim().Length == 0) return;
            
            try
            {
                var result = await translator.TranslateAsync(dictLangs[sourseLanguage], dictLangs[targetLanguage], inputText);
                textBoxOutput.Text = result.TranslatedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonCompare_Click(object sender, EventArgs e)
        {
            string inputText = textBoxInput.Text;
            string sourseLanguage = comboBoxSourceLang.SelectedItem.ToString();

            if (string.IsNullOrEmpty(inputText) || inputText.Trim().Length == 0) return;

            List<CheckBox> checkBoxes = new List<CheckBox>();
            foreach (Control control in this.Controls)
            {
                if (control is CheckBox checkBox)
                {
                    checkBoxes.Add(checkBox);
                }
            }

            // Создание таблицы
            DataGridView dataGridView = (DataGridView)this.Controls[16];
            dataGridView.Rows.Clear(); // Очистка предыдущих данных

            foreach (var checkBox in checkBoxes)
            {
                if (checkBox.Checked)
                {
                    string targetLanguage = checkBox.Text;
                    try
                    {
                        var result = await translator.TranslateAsync(dictLangs[sourseLanguage], dictLangs[targetLanguage], inputText);
                        dataGridView.Rows.Add(checkBox.Text, result.TranslatedText);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
    }
}
