using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Lab1_Survey_WPF
{
    public partial class MainWindow : Window
    {
        private readonly List<string> _questions = new List<string>
        {
            "1) Як вас звати?",
            "2) Скільки вам років?",
            "3) Які цілі на цей семестр?",
            "4) Скільки лабораторних вже зробили?",
            "5) Який прогрес з дипломом?"
        };

        private readonly Dictionary<int, string> _answers = new Dictionary<int, string>();
        private int _index = 0;

        private string _outputFilePath; // стане непорожнім після першого збереження

        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void UpdateUI()
        {
            TxtQuestion.Text = _questions[_index];

            string a;
            TxtAnswer.Text = _answers.TryGetValue(_index, out a) ? a : "";

            BtnPrev.IsEnabled = _index > 0;
            BtnNext.IsEnabled = _index < _questions.Count - 1;

            TxtStatus.Text = "Питання " + (_index + 1) + " з " + _questions.Count;
        }

        private void SaveCurrentAnswer()
        {
            _answers[_index] = (TxtAnswer.Text ?? "").Trim();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentAnswer();
            if (_index > 0) _index--;
            UpdateUI();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentAnswer();
            if (_index < _questions.Count - 1) _index++;
            UpdateUI();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentAnswer();

            if (string.IsNullOrWhiteSpace(_outputFilePath))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Куди зберегти відповіді?";
                sfd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.FileName = "survey_answers.txt";

                bool? result = sfd.ShowDialog();
                if (result != true)
                {
                    TxtStatus.Text = "Збереження скасовано.";
                    return;
                }

                _outputFilePath = sfd.FileName;
            }

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== Опитування ===");
                sb.AppendLine("Дата/час: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine();

                for (int i = 0; i < _questions.Count; i++)
                {
                    string ans;
                    if (!_answers.TryGetValue(i, out ans)) ans = "";

                    sb.AppendLine(_questions[i]);
                    sb.AppendLine("Відповідь: " + ans);
                    sb.AppendLine(new string('-', 40));
                }

                sb.AppendLine();
                sb.AppendLine("=== Кінець запису ===");
                sb.AppendLine();

                File.AppendAllText(_outputFilePath, sb.ToString(), Encoding.UTF8);

                TxtStatus.Text = "Збережено: " + _outputFilePath;
                MessageBox.Show("Відповіді збережено успішно!", "OK",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка збереження: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}