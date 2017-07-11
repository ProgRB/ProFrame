using System;

namespace ProFrame
{
    public interface IWindowsForms_Viewer
    {
        System.Windows.Forms.Control ChildForm
        {
            get;
        }

        Type TypeChildForm
        {
            get;
        }

    }
}
