using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndeeMusee
{
    public delegate void SongProgressChangeHandler();
    public delegate void EventClickHandler();
    public delegate void PropertiesChangeHandler();
    public delegate void ChangePageHandler(string page);
    public delegate void ChangeSongToPlayHandler(int index);
    public delegate void ChangePlaylistInfoViewHandler(string playlistKey);

    //notify my music view to scroll to end of list view
    public delegate void AddNewSongHandler();

    //notify lyric
    public delegate void ChangeLyricHandler(LyricModel lyric,string nextLyric);

    //notify delegate
    public delegate void NotifyHandler();

    //selected index change --> my music, xu ly truong hop bam vao button nhung list item ko selected
    public delegate void ChangeSelectedIndex(int index,int status);

    //new form is show --> opacity mainform decrease
    public delegate void ToggleFormDialogNotifyHandler();

    //add, remove song to playlist --> update button 
    public delegate void AddToPlaylistStatusHandler(int index);

    //search music request 
    public delegate void SearchMusicHandler(string searchContent);
}
