using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace ProFrame
{
    public class MainWindowBase : Window
    {
        TextBlock _statusBarText = null;

        public override void OnApplyTemplate()
        {
            object content = this.Content;
            ContentPresenter cp = this.GetTemplateChild("cpMain") as ContentPresenter;
            cp.Content = content;
            _statusBarText = this.GetTemplateChild("lbMessage") as TextBlock;
            LocalizationManager.Manager = new CustomLocalizationManager();
            (this.GetTemplateChild("dmOpenTabs") as Xceed.Wpf.AvalonDock.DockingManager).DocumentClosed += DockingManager_DocumentClosed;
        }

        private void DockingManager_DocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            OpenTabs.CloseTab((ViewTabBase)e.Document.Content);
        }

        public void AddStatusMessage(string p_message)
        {
            _statusBarText.Text = p_message;
        }

        private bool _visibleStatusBar = false;
        public bool VisibleStatusBar
        {
            get
            {
                return _visibleStatusBar;
            }
            set
            {
                _visibleStatusBar = value;
                StackPanel sp = this.GetTemplateChild("sbStatus") as StackPanel;
                sp.Visibility = _visibleStatusBar ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public ViewTabCollection OpenTabs
        {
            get
            {
                return (ViewTabCollection)this.FindResource("OpenTabs");
            }
        }
    }
}
