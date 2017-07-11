using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using System.Linq;

namespace ProFrame
{
    public class ViewTabCollection : NotificationObject
    {
        static ObservableCollection<ViewTabBase> _openTabs;
        public static ObservableCollection<ViewTabBase> OpenTabs
        {
            get
            {
                if (_openTabs == null)
                    _openTabs = new ObservableCollection<ViewTabBase>();
                return _openTabs;
            }
        }
        ViewTabBase _selectedTab;
        public ViewTabBase SelectedTab
        {
            get
            {
                return _selectedTab;
            }
            set
            {
                _selectedTab = value;
                RaisePropertyChanged(() => SelectedTab);
                if (_selectedTab != null)
                    _selectedTab.ContentData.Focus();
            }
        }

        public ViewTabBase AddNewTab(string tabheader, UserControl content)
        {
            ViewTabBase v = new ViewTabBase(tabheader, content);
            OpenTabs.Add(v);
            SelectedTab = v;
            return v;
        }

        public ViewTabBase AddNewTab(string tabheader, UserControl content, Uri uriIcon)
        {
            ViewTabBase v = new ViewTabBase(tabheader, content, uriIcon);
            OpenTabs.Add(v);
            SelectedTab = v;
            return v;
        }

        public ViewTabBase AddNewTab(string tabheader, Type typeContent)
        {
            return AddNewTab(tabheader, typeContent, false, null);
        }
        public ViewTabBase AddNewTab(string tabheader, Type typeContent, bool openExisting)
        {
            return AddNewTab(tabheader, typeContent, openExisting, null);
        }
        public ViewTabBase AddNewTab(string tabheader, Type typeContent, Uri uriIcon)
        {
            return AddNewTab(tabheader, typeContent, false, uriIcon);
        }

        public ViewTabBase AddNewTab(string tabheader, Type typeContent, bool openExisting, Uri uriIcon)
        {
            ViewTabBase v;
            if (openExisting)
            {
                if (typeContent.BaseType.FullName == "System.Windows.Forms.UserControl")
                {
                    v = GetOpenTabForm(typeContent);
                    if (v == null)
                    {
                        v = new ViewTabBase(tabheader, new WindowsForms_Viewer(Activator.CreateInstance(typeContent) as System.Windows.Forms.UserControl), uriIcon);
                        OpenTabs.Add(v);
                    }
                }
                else
                {
                    v = GetOpenTab(typeContent);
                    if (v == null)
                    {
                        v = new ViewTabBase(tabheader, Activator.CreateInstance(typeContent) as UserControl, uriIcon);
                        OpenTabs.Add(v);
                    }
                }
            }
            else
            {
                v = new ViewTabBase(tabheader,
                    (typeContent.BaseType.FullName == "System.Windows.Forms.UserControl" ?
                    new WindowsForms_Viewer(Activator.CreateInstance(typeContent) as System.Windows.Forms.UserControl) :
                    Activator.CreateInstance(typeContent) as UserControl), uriIcon);
                OpenTabs.Add(v);
            }
            SelectedTab = v;
            return v;
        }

        public void CloseTab(ViewTabBase v)
        {
            CancelEventArgs e = new CancelEventArgs();
            if (v != null) v.ValidateClose(e);
            if (!e.Cancel)
            {
                OpenTabs.Remove(v);
            }
        }

        /// <summary>
        /// Возвращает вкладку с указанным типом или налл
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ViewTabBase GetOpenTab(Type t)
        {
            return _openTabs.Where(r => r.ContentData.GetType() == t).FirstOrDefault();
        }

        /// <summary>
        /// Возвращает вкладку с указанным типом или налл
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ViewTabBase[] GetOpenTabArray(Type t)
        {
            return _openTabs.Where(r => r.ContentData.GetType() == t).ToArray();
        }
        /// <summary>
        /// Возвращает вкладку с указанным типом или налл
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ViewTabBase GetOpenTab(string title)
        {
            return _openTabs.Where(r => r.HeaderText == title).FirstOrDefault();
        }
        /// <summary>
        /// Возвращает вкладку с указанным типом или налл
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ViewTabBase GetOpenTabForm(Type t)
        {
            return _openTabs.Where(r => r.ContentData is IWindowsForms_Viewer).Where(r => ((IWindowsForms_Viewer)r.ContentData).TypeChildForm == t).FirstOrDefault();
        }
    }
}
