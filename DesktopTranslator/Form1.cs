using GTranslatorAPI;

namespace DesktopTranslator
{
    public partial class Form1 : Form
    {
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
        }

        private async void buttonTranslate_Click(object sender, EventArgs e)
        {
            string inputText = textBoxInput.Text;
            string sourseLanguage = comboBoxSourceLang.SelectedItem.ToString();
            string targetLanguage = comboBoxTargetLang.SelectedItem.ToString();

            GTranslatorAPIClient translator = new GTranslatorAPIClient();
            var result = await translator.TranslateAsync(dictLangs[sourseLanguage], dictLangs[targetLanguage], inputText);

            textBoxOutput.Text = result.TranslatedText;
        }
    }
}
