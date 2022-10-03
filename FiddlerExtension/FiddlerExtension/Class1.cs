using Fiddler;
using System;
using System.Windows.Forms;

namespace FiddlerExtension
{
    public class Class1 : IFiddlerExtension
    {
        TabPage tabPage;

        public Class1()
        {
            tabPage = new TabPage("Video2");
            tabPage.Controls.Add(new UserControl1());
            tabPage.ImageIndex = (int)SessionIcons.Video;
        }

        public void OnBeforeUnload()
        {
        }

        public void OnLoad()
        {
            FiddlerApplication.UI.tabsViews.TabPages.Add(tabPage);
            FiddlerApplication.UI.mnuTools.MenuItems.Add("2333", (_,__) => {
                MessageBox.Show("2333");
            });
            FiddlerApplication.UI.mnuMain.MenuItems.Add("55rrrr", (_, __) => {
                MessageBox.Show("55rrrr");
            });

        }
    }
}
