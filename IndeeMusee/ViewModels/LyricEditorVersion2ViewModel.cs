using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IndeeMusee.ViewModels
{
    class LyricEditorVersion2ViewModel : BaseViewModel
    {
        public static SongModel SongToEdit;

        List<LyricItemModel> songLyric = new List<LyricItemModel>();

        string[] lyric_data;

        public string SongTitle { get; set; }

        public string SaveStatus { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand InsertTimeStartCommand { get; set; }
        public ICommand InsertTimeEndCommand { get; set; }
        public ICommand EditLyricContentCommand { get; set; }
        public ICommand ClearTimeCommand { get; set; }
        public List<LyricItemModel> SongLyric { get => songLyric; set => songLyric = value; }

        public LyricEditorVersion2ViewModel()
        {
            SongTitle = SongToEdit.Title;
            SaveStatus = "SAVED";
            string lyricFilePath = $@"{GeneralDataManagement.LyricFolderPath}\{SongToEdit.LyricPath}";
            if (File.Exists(lyricFilePath) == true)
            {
                lyric_data = File.ReadAllLines(lyricFilePath);
                songLyric = GetLyric(lyric_data);
                OnPropertyChanged("SongLyric");
            }

            UpNextListModel.AddSongToUpNextListAndPlay(SongToEdit);
            UpNextListModel.IsEditingLyric = true;




            ImportCommand = new RelayCommand<object>(
               (p) =>
               {
                   return true;
               },
               (p) =>
               {
                   lyric_data = ExportToTextLyricData();
                   LyricEditor_FormAddLyric formEditLyric = new LyricEditor_FormAddLyric(lyric_data);
                   formEditLyric.ShowDialog();
                   if (formEditLyric.DialogResult == true)
                   {
                       SaveStatus = "SAVE*";
                       OnPropertyChanged("SaveStatus");

                       lyric_data = formEditLyric.LyricData;
                       songLyric = GetLyric(lyric_data);
                       OnPropertyChanged("SongLyric");
                   }


               });

            SaveCommand = new RelayCommand<object>(
               (p) =>
               {
                   return true;
               },
               (p) =>
               {
                   //FileStream fStream;

                   string LyricFilePath = $@"{GeneralDataManagement.LyricFolderPath}\{SongToEdit.LyricPath}";

                   lyric_data = ExportToTextLyricData();


                   if (File.Exists(LyricFilePath) == true)
                   {
                       System.IO.File.WriteAllLines(LyricFilePath, lyric_data);
                   }
                   else
                   {
                       LyricFilePath = $@"{GeneralDataManagement.LyricFolderPath}\{SongToEdit.Title}_lyric_{MyUtilites.MyFunction.GenerateCode()}.lrc";
                       System.IO.File.WriteAllLines(LyricFilePath, lyric_data);
                       SongToEdit.LyricPath = LyricFilePath.Replace($@"{GeneralDataManagement.LyricFolderPath}\", "");
                       SongDataAccess.UpdateSong(SongToEdit);
                   }

                   SaveStatus = "SAVED";
                   OnPropertyChanged("SaveStatus");


               });

            InsertTimeStartCommand = new RelayCommand<object>(
               (p) =>
               {
                   return true;
               },
               (p) =>
               {
                   SaveStatus = "SAVE*";
                   OnPropertyChanged("SaveStatus");

                   LyricItemModel lyricItem = new LyricItemModel(p as LyricItemModel);
                   lyricItem.TimeStart = NowPlayingSong.AppMediaPlayer.Position.ToString(@"mm\:ss\.ff");
                   UpdateLyricInfo(lyricItem);

               });

            InsertTimeEndCommand = new RelayCommand<object>(
               (p) =>
               {
                   return true;
               },
               (p) =>
               {
                   SaveStatus = "SAVE*";
                   OnPropertyChanged("SaveStatus");

                   LyricItemModel lyricItem = new LyricItemModel(p as LyricItemModel);
                   lyricItem.TimeEnd = NowPlayingSong.AppMediaPlayer.Position.ToString(@"mm\:ss\.ff");
                   UpdateLyricInfo(lyricItem);
               });

            EditLyricContentCommand = new RelayCommand<object>(
               (p) =>
               {
                   return true;
               },
               (p) =>
               {



               });

            ClearTimeCommand = new RelayCommand<object>(
               (p) =>
               {
                   return true;
               },
               (p) =>
               {
                   List<LyricItemModel> newSongLyric = new List<LyricItemModel>(SongLyric);
                   foreach (LyricItemModel lyric in newSongLyric)
                   {
                       lyric.TimeEnd = "";
                       lyric.TimeStart = "";
                   }
                   SongLyric = newSongLyric;
                   OnPropertyChanged("SongLyric");

               });
        }

        private void UpdateLyricInfo(LyricItemModel lyricToUpdate)
        {
            List<LyricItemModel> newListLyric = new List<LyricItemModel>(SongLyric);
            newListLyric[lyricToUpdate.Number] = lyricToUpdate;
            SongLyric = newListLyric;
            OnPropertyChanged("SongLyric");

        }

        private string[] ExportToTextLyricData()
        {
            List<string> LyricData = new List<string>();

            foreach (LyricItemModel lyric in SongLyric)
            {
                string line = "";
                if (lyric.TimeStart != "")
                {
                    line = $@"[{lyric.TimeStart}]{lyric.Content}";
                }
                else
                {
                    line = lyric.Content;
                }
                LyricData.Add(line);

                if (lyric.TimeEnd != "")
                {
                    LyricData.Add($@"[{lyric.TimeEnd}]");
                }
            }

            return LyricData.ToArray();

        }

        private List<LyricItemModel> GetLyric(string[] lyric_data)
        {
            List<LyricItemModel> newSongLyric = new List<LyricItemModel>();
            int index = 0;
            int number = 0;
            while (index < lyric_data.Length)
            {
                string data = lyric_data[index];
                string timeStart = "";
                string timeEnd = "";
                int i = data.IndexOf("[");
                int j = data.IndexOf("]");

                if (i < 0 || j < 0)
                {
                }
                else
                {
                    timeStart = data.Substring(i + 1, j - (i + 1));
                }

                string content = "";

                //lấy lời bài hát
                if (j + 1 < data.Length)
                {
                    content = data.Substring(j + 1);
                    content = content.Trim();
                }

                index++;
                if (content == "" || index == lyric_data.Length)
                {
                    continue;
                }


                //kiểm tra lời bài hát nếu dài quá thì sẽ cắt bớt
                string content1 = content;
                string content2 = null;

                int amountOfWord = content.Trim().Split(' ').Length;
                if (amountOfWord > 12)
                {
                    int flag = 0;
                    int startPosition = 0;
                    while (flag < 10)
                    {
                        startPosition = content.IndexOf(' ', startPosition + 1);
                        flag++;
                    }
                    content1 = content.Substring(0, startPosition);
                    content2 = content.Substring(startPosition).Trim();
                }

                //lấy thời gian kết thúc
                data = lyric_data[index];
                i = data.IndexOf("[");
                j = data.IndexOf("]");
                if (i == -1 || j == -1)
                {
                }
                else
                {
                    timeEnd = data.Substring(i + 1, j - (i + 1));

                }

                if (content2 == null)
                {
                    //khong tach loi
                    newSongLyric.Add(new LyricItemModel(number++, content1, timeStart, timeEnd));

                }
                else
                {
                    //tach loi
                    newSongLyric.Add(new LyricItemModel(number++, content1, timeStart, ""));
                    newSongLyric.Add(new LyricItemModel(number++, content2, "", ""));
                }



            }
            return newSongLyric;
        }
    }

    public class LyricItemModel
    {
        public string Content { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int Number { get; set; }

        public LyricItemModel(int number, string content, string timeStart, string timeEnd)
        {
            Number = number;
            Content = content;
            TimeStart = timeStart;
            TimeEnd = timeEnd;
        }

        public LyricItemModel(LyricItemModel lyric)
        {
            Number = lyric.Number;
            Content = lyric.Content;
            TimeStart = lyric.TimeStart;
            TimeEnd = lyric.TimeEnd;
        }
    }

}
