using IndeeMusee.Models;
using IndeeMusee.MyUtilites;
using IndeeMusee.ViewModels;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for AddPlayListForm.xaml
    /// </summary>
    public partial class AddPlayListForm : Window
    {
        public static event ToggleFormDialogNotifyHandler ToggleForm;
        DispatcherTimer timer = new DispatcherTimer();

        DispatcherTimer timerUpdateButtonAddToPL = new DispatcherTimer();
        AddPlaylistFormViewModel AddPLFormVM;
        public AddPlayListForm(SongModel song)
        {
            InitializeComponent();
            ToggleForm();
            AddPLFormVM = new AddPlaylistFormViewModel(song);
            AddPLFormVM.ListPlaylistChange += AddPLFormVM_ListPlaylistChange;
            AddPLFormVM.ButtonStatusChange += AddPLFormVM_ButtonStatusChange;
            this.DataContext = AddPLFormVM;
            this.Closing += AddPlayListForm_Closing;


            timer.Tick += Timer_Tick;
            timerUpdateButtonAddToPL.Tick += TimerUpdateButtonAddToPL_Tick;
            timer.Start();

        }

        private void AddPLFormVM_ButtonStatusChange(int index)
        {
            Update_ButtonAddToPlaylist(index);
        }

        private void AddPLFormVM_ListPlaylistChange()
        {
            timerUpdateButtonAddToPL.Start();
        }

        private void TimerUpdateButtonAddToPL_Tick(object sender, EventArgs e)
        {
            if (ListView_ListPlaylist.Items.Count == AddPLFormVM.FilteredPlaylist.Count)
            {
                timerUpdateButtonAddToPL.Stop();
                for (int index = 0; index < AddPLFormVM.FilteredPlaylist.Count; index++)
                {
                    Update_ButtonAddToPlaylist(index);
                }
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ListView_ListPlaylist.Items.Count == AddPLFormVM.FilteredPlaylist.Count)
            {
                timer.Stop();
                Update_ListView_ListPlaylist();
            }
        }

        private void AddPlayListForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleForm();
        }

        private void Update_ButtonAddToPlaylist(int index)
        {
            ListViewItem lvi = ListView_ListPlaylist.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;

            if (lvi == null)
            {
                return;
            }

            List<Control> list_button = GetSpecificControl.AllChildren(lvi);

            Button ButtonAddToPL = list_button.Find(e => e.Name == "BtnAddToPL") as Button;
            Button ButtonAdded = list_button.Find(e => e.Name == "BtnAdded") as Button;


            if (AddPLFormVM.FilteredPlaylist[index].CheckExistedInPlaylist == true)
            {
                ButtonAddToPL.Visibility = Visibility.Hidden;
                ButtonAdded.Visibility = Visibility.Visible;
            }
            else
            {
                ButtonAddToPL.Visibility = Visibility.Visible;
                ButtonAdded.Visibility = Visibility.Hidden;
            }
        }

        private void Update_ListView_ListPlaylist()
        {

            for (int index = 0; index < AddPLFormVM.FilteredPlaylist.Count; index++)
            {
                Update_ButtonAddToPlaylist(index);
            }


            DoubleAnimation transparentAnimation = new DoubleAnimation();
            Storyboard myStoryboard = new Storyboard();

            transparentAnimation.From = 0;
            transparentAnimation.To = 1;
            transparentAnimation.Duration = TimeSpan.FromSeconds(0.3);
            Storyboard.SetTargetName(transparentAnimation, "FormAddPL");
            Storyboard.SetTargetProperty(transparentAnimation, new PropertyPath(Slider.OpacityProperty));
            myStoryboard.Children.Add(transparentAnimation);
            myStoryboard.Begin(this);

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
