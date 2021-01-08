using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for LyricEditorView.xaml
    /// </summary>
    public partial class LyricEditorView : UserControl
    {
        public static event ChangePageHandler ChangePage;

        LyricEditorViewModel LyricEditorVM;
        SongModel songToUpdateLyric;
        string LyricFilePath;
        public LyricEditorView()
        {
            InitializeComponent();
            LyricEditorVM = this.DataContext as LyricEditorViewModel;
            //FlowDocument mcFlowDoc = new FlowDocument();
            //// Create a paragraph with text  
            //Paragraph para = new Paragraph();
            //para.Inlines.Add(new Run("I am a flow document. Would you like to edit me? "));
            //para.Inlines.Add(new Bold(new Run("Go ahead.")));
            //para.Inlines.Add(new Run(LyricEditorViewModel.SongToEdit.SongName));
            //// Add the paragraph to blocks of paragraph  
            //mcFlowDoc.Blocks.Add(para);
            //// Create RichTextBox, set its hegith and width  
            //RichTBLyricContent.Document = mcFlowDoc;

            TextRange range;
            FileStream fStream;
            songToUpdateLyric = LyricEditorViewModel.SongToEdit;
            LyricFilePath = $@"{GeneralDataManagement.DatabaseFolderPath}\{songToUpdateLyric.LyricPath}".Replace(".lrc", "");
            range = new TextRange(RichTBLyricContent.Document.ContentStart, RichTBLyricContent.Document.ContentEnd);

            if (File.Exists(LyricFilePath + ".txt") == true)
            {
                fStream = new FileStream(LyricFilePath + ".txt", FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.XamlPackage);
                fStream.Close();
            }
            else if (File.Exists(LyricFilePath + ".lrc") == true)
            {
                fStream = new FileStream(LyricFilePath + ".txt", FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
            }
            else
            {
                TextRange TRTime = new TextRange(RichTBLyricContent.Document.ContentStart, RichTBLyricContent.Document.ContentStart);
                TRTime.Text = $@"[00:00.00]";
                TRTime.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                TRTime.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                EditingCommands.MoveToDocumentEnd.Execute(null,RichTBLyricContent);

                TextRange TRTitle = new TextRange(RichTBLyricContent.CaretPosition, RichTBLyricContent.CaretPosition);
                TRTitle.Text = "Bài hát: " + LyricEditorViewModel.SongToEdit.Title +'\n';
                TRTitle.ApplyPropertyValue(TextElement.FontSizeProperty,25.0);
                TRTitle.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                TRTitle.ApplyPropertyValue(TextElement.FontWeightProperty,FontWeights.Bold);
                EditingCommands.MoveToDocumentEnd.Execute(null,RichTBLyricContent);

                TRTitle = new TextRange(RichTBLyricContent.CaretPosition, RichTBLyricContent.CaretPosition);
                TRTitle.Text =LyricEditorViewModel.SongToEdit.ContributingArtists + "\n";
                TRTitle.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                TRTitle.ApplyPropertyValue(TextElement.FontSizeProperty, 20.0);
                TRTitle.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);
                TRTitle.ApplyPropertyValue(TextElement.FontStyleProperty,FontStyles.Italic);

                EditingCommands.MoveToDocumentEnd.Execute(null,RichTBLyricContent);
                TRTitle = new TextRange(RichTBLyricContent.CaretPosition, RichTBLyricContent.CaretPosition);
                TRTitle.Text = " ";
                TRTitle.ApplyPropertyValue(TextElement.FontSizeProperty, 15.0);
                TRTitle.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                TRTitle.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                TRTitle.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

                RichTBLyricContent.Focus();

                EditingCommands.MoveToDocumentEnd.Execute(null,RichTBLyricContent);
                RichTBLyricContent.FontSize = 15.0;
                RichTBLyricContent.Foreground = Brushes.Black;
                RichTBLyricContent.FontStyle = FontStyles.Normal;
                RichTBLyricContent.FontWeight = FontWeights.Normal;




            }
        }

        private void BtnInsertTime_Click(object sender, RoutedEventArgs e)
        {
            string[] lyric_data = (new TextRange(RichTBLyricContent.Document.ContentStart, RichTBLyricContent.Document.ContentEnd)).Text.Split('\n');

            int someBigNumber = int.MaxValue;
            int lineMoved, currentLineNumber;
            RichTBLyricContent.Selection.Start.GetLineStartPosition(-someBigNumber, out lineMoved);
            currentLineNumber = -lineMoved;


            string lyric = lyric_data[currentLineNumber];

            int i = lyric.IndexOf("[");
            int j = lyric.IndexOf("]");
            if (i != -1 && j != -1 && i < j)
            {
                //đã có thời gian
                lyric = lyric.Substring(j + 1);
            }

            EditingCommands.MoveToLineStart.Execute(null, RichTBLyricContent);
            EditingCommands.SelectToLineEnd.Execute(null, RichTBLyricContent);
            EditingCommands.Delete.Execute(null, RichTBLyricContent);


            TextRange TRTime = new TextRange(RichTBLyricContent.CaretPosition, RichTBLyricContent.CaretPosition);
            TRTime.Text = $@"[{(this.DataContext as LyricEditorViewModel).CurrentSongTimePosition}]";
            TRTime.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
            TRTime.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

            TextRange TRLyric = new TextRange(RichTBLyricContent.CaretPosition, RichTBLyricContent.CaretPosition);
            TRLyric.Text = lyric + '\n';
            TRLyric.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
            TRLyric.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

            EditingCommands.MoveUpByLine.Execute(null, RichTBLyricContent);
            EditingCommands.MoveToLineEnd.Execute(null, RichTBLyricContent);


        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            TextRange range;
            FileStream fStream;


            if (File.Exists(LyricFilePath + ".lrc") == true)
            {
                fStream = new FileStream(LyricFilePath + ".lrc", FileMode.OpenOrCreate);
                range = new TextRange(RichTBLyricContent.Document.ContentStart, RichTBLyricContent.Document.ContentEnd);
                range.Save(fStream, DataFormats.Text);
                fStream.Close();


                //save file format
                fStream = new FileStream(LyricFilePath + ".txt", FileMode.OpenOrCreate);
                range.Save(fStream, DataFormats.XamlPackage);
                fStream.Close();
            }

            else
            {
                //save file text
                LyricFilePath = $@"{GeneralDataManagement.DatabaseFolderPath}\{songToUpdateLyric.Title}_lyric_{MyUtilites.MyFunction.GenerateCode()}";
                fStream = new FileStream(LyricFilePath + ".lrc", FileMode.OpenOrCreate);
                range = new TextRange(RichTBLyricContent.Document.ContentStart, RichTBLyricContent.Document.ContentEnd);
                range.Save(fStream, DataFormats.Text);

                songToUpdateLyric.LyricPath = LyricFilePath.Replace($@"{GeneralDataManagement.DatabaseFolderPath}\", "") + ".lrc";
                SongDataAccess.UpdateSong(songToUpdateLyric);
                fStream.Close();

                //save file format
                fStream = new FileStream(LyricFilePath + ".txt", FileMode.OpenOrCreate);
                range.Save(fStream, DataFormats.XamlPackage);
                fStream.Close();
            }

        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            UpNextListModel.IsEditingLyric = false;
            MyMusicControl.UserSongListHasUpdated();
            MusicInPlaylistControl.UserSongListHasUpdated();
            ChangePage("MyMusic");

        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (CheckboxInsertMode.IsChecked == false) return;
            if (e.Key == Key.Space)
            {
                NowPlayingSong.IsPlaying = !NowPlayingSong.IsPlaying;
                e.Handled = true;
            }

            if (e.Key == Key.I)
            {
                BtnInsertTime_Click(null, null);
                e.Handled = true;
            }
        }

        private void RichTBLyricContent_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void RichTBLyricContent_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
