using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for LyricEditor_FormAddLyric.xaml
    /// </summary>
    public partial class LyricEditor_FormAddLyric : Window
    {
        public string[] LyricData { get; set; }

        public static event ToggleFormDialogNotifyHandler ToggleForm;
        public LyricEditor_FormAddLyric(string[] lyric_data)
        {
            InitializeComponent();

            if (ToggleForm != null) ToggleForm();
            this.Closing += LyricEditor_FormAddLyric_Closing;
            if (lyric_data == null) return;
            LbText.Visibility = Visibility.Hidden;

            FlowDocument myFlowDoc = new FlowDocument();

            if (lyric_data.Length==0)
            {
                Paragraph myParagraph = new Paragraph();
                myParagraph.Inlines.Add("Enter your lyric here");
                myFlowDoc.Blocks.Add(myParagraph);
            }

            foreach (string lyric in lyric_data)
            {
                if (lyric.Trim() == "") continue;
                Paragraph myParagraph = new Paragraph();
                myParagraph.Inlines.Add(lyric);
                myFlowDoc.Blocks.Add(myParagraph);
            }

            RichTBLyricContent.Document = myFlowDoc;

        }

        private void LyricEditor_FormAddLyric_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ToggleForm != null) ToggleForm();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.MoveToDocumentStart.Execute(null, RichTBLyricContent);
            LyricData = (new TextRange(RichTBLyricContent.Document.ContentStart, RichTBLyricContent.Document.ContentEnd)).Text.Split('\n');
            this.DialogResult = true;
            this.Close();
        }

        private void RichTBLyricContent_MouseEnter(object sender, MouseEventArgs e)
        {
            if(LyricData==null)
                LbText.Visibility = Visibility.Hidden;
        }
    }
}
