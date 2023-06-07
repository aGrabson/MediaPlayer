using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MediaPlayer
{
    public partial class Form1 : Form
    {
        List<string> filteredFiles = new List<string>();
        FolderBrowserDialog browser = new FolderBrowserDialog();
        int currentFile = 0;


        public Form1()
        {
            InitializeComponent();
        }

        private void LoadFolderEvent(object sender, EventArgs e)
        {
            VideoPlayer.Ctlcontrols.stop();

            if (filteredFiles.Count > 1)
            {
                filteredFiles.Clear();
                filteredFiles = null;
                PlayList.Items.Clear();
                currentFile = 0;
            }

            DialogResult result = browser.ShowDialog();

            if(result == DialogResult.OK)
            {
                filteredFiles = Directory.GetFiles(browser.SelectedPath, "*.*").
                    Where(file => file.ToLower().EndsWith("webm") ||
                          file.ToLower().EndsWith("mp4") ||
                          file.ToLower().EndsWith("wmv") ||
                          file.ToLower().EndsWith("mkv") ||
                          file.ToLower().EndsWith("avi")).ToList();
                LoadPlayList();
            }    
        }

        private void ShowAboutEvent(object sender, EventArgs e)
        {
            MessageBox.Show("This app is made by Artur Graba" +
                Environment.NewLine + "Hope you'll like simple media player" +
                Environment.NewLine + "Click on Open Folder Button to load the video folder to the app and it will start auto playing" +
                Environment.NewLine + "Enjoy", "GONZU SAYS: ");

        }

        private void MediaPlayerStateChangeEvent(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if(e.newState == 0)
            {
                lblDuration.Text = "Media Player is Ready to be loaded";
            }
            else if (e.newState == 1)
            {
                lblDuration.Text = "Media Player stopped";
            }
            else if (e.newState == 2)
            {
                lblDuration.Text = "Duration:" + VideoPlayer.currentMedia.durationString;
            }
            else if (e.newState == 8)
            {
                if(currentFile>=filteredFiles.Count-1)
                {
                    currentFile = 0;
                }
                else
                {
                    currentFile += 1;
                }
                PlayList.SelectedIndex = currentFile;
                ShowFileName(fileName);
            }
            else if (e.newState == 9)
            {
                lblDuration.Text = "Loading new video";
            }
            else if (e.newState == 10)
            {
                timer1.Start();
            }
        }

        private void PlayListChanged(object sender, EventArgs e)
        {
            currentFile = PlayList.SelectedIndex;
            PlayFile(PlayList.SelectedItem.ToString());
            ShowFileName(fileName);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void TimerEvent(object sender, EventArgs e)
        {
            VideoPlayer.Ctlcontrols.play();
            timer1.Stop();
        }
        private void LoadPlayList()
        {
            VideoPlayer.currentPlaylist = VideoPlayer.newPlaylist("Playlist", "");

            foreach (string videos in filteredFiles)
            {
                VideoPlayer.currentPlaylist.appendItem(VideoPlayer.newMedia(videos));
                PlayList.Items.Add(videos);
            }    

            if(filteredFiles.Count > 0)
            {
                fileName.Text = "Files Found " + filteredFiles.Count;
                PlayList.SelectedIndex = currentFile;
                PlayFile(PlayList.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("No Video Files Found in this folder");
            }
        }
        private void PlayFile(string url)
        {
            VideoPlayer.URL = url;
        }
        private void ShowFileName(Label name)
        {
            string file = Path.GetFileName(PlayList.SelectedItem.ToString());
            name.Text = "Currently playing" + file;
        }
    }
}
