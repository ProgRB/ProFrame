using DbProviderConfiguration;
using System;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace ProFrame
{
    /// <summary>
    /// Логика взаимодействия для MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        public MessageControl()
        {
            InitializeComponent();
        }
    }
    public class MessageModel : NotificationObject
    {
        public MessageModel()
        {
            try
            {
                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                    return;
                _ds = new DataSet();
                _ds.Tables.Add("MESSAGE");
                _daMessage = new UniDbAdapter(
                    @"select MAX(M.MESSAGE_ID) OVER() MAX_MESSAGE_ID, 
	                    ROW_NUMBER() over (order by date_message desc) MESSAGE_NUMBER,
                        M.CONTENT_MESSAGE, M.DATE_MESSAGE, MESSAGE_ID
                    from APSTAFF.MESSAGE M
                    where M.APP_NAME_ID = :p_APP_NAME_ID and 
                        ((M.MESSAGE_ID > :p_MESSAGE_ID and :p_MESSAGE_ID is not null)
                        or
                        (M.DATE_MESSAGE between trunc(:p_begin_date) and trunc(:p_end_date)+86399/86400))
                    ORDER BY DATE_MESSAGE DESC", UniDbConnection.Current);
                _daMessage.SelectCommand.Parameters.Add("p_APP_NAME_ID", UniDbType.Int);
                _daMessage.SelectCommand.Parameters.Add("p_MESSAGE_ID", UniDbType.Decimal);
                _daMessage.SelectCommand.Parameters.Add("p_begin_date", UniDbType.DateTime);
                _daMessage.SelectCommand.Parameters.Add("p_end_date", UniDbType.DateTime);
            }
            catch (Exception ex)
            { }
        }

        private bool isLastLoaded = false;
        private DateTime _selectedDateBegin = DateTime.Today;
        public DateTime SelectedDateBegin
        {
            get { return this._selectedDateBegin; }
            set
            {
                if (value != this._selectedDateBegin)
                {
                    this._selectedDateBegin = value;
                    RaisePropertyChanged(() => SelectedDateBegin);
                    LoadMessage(_selectedDateBegin, _selectedDateEnd);
                    RaisePropertyChanged(() => Messages);
                }
            }
        }

        private DateTime _selectedDateEnd = DateTime.Today;
        public DateTime SelectedDateEnd
        {
            get { return this._selectedDateEnd; }
            set
            {
                if (value != this._selectedDateEnd)
                {
                    this._selectedDateEnd = value;
                    RaisePropertyChanged(() => SelectedDateEnd);
                    LoadMessage(_selectedDateBegin, _selectedDateEnd);
                    RaisePropertyChanged(() => Messages);
                }
            }
        }

        private string _app_name = "";
        public string AppName
        {
            get { return _app_name; }
            set
            {
                _app_name = value;
                RaisePropertyChanged(() => AppName);
            }
        }

        private int _app_name_ID = 0;
        public int AppName_ID
        {
            get { return _app_name_ID; }
            set
            {
                _app_name_ID = value;
                RaisePropertyChanged(() => AppName_ID);
            }
        }
        private DataSet _ds;
        private UniDbAdapter _daMessage;

        Decimal? _lastMessageID = null;
        public decimal? LastMessageID
        {
            get
            {
                return _lastMessageID;
            }
            set
            {
                _lastMessageID = value;
            }
        }

        public DataView Messages
        {
            get
            {
                if (_ds != null && _ds.Tables.Contains("MESSAGE"))
                    return _ds.Tables["MESSAGE"].DefaultView;
                else return null;
            }
        }


        private void LoadMessage(DateTime? begin_date, DateTime? end_date)
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            if (!isLastLoaded)
            {
                AppName = AppConstants.App_Name;
                AppName_ID = AppConstants.App_Name_ID;
                LastMessageID = Convert.ToDecimal(appSettings[AppName + "Message_ID"]);
            }
            _ds.Tables["MESSAGE"].Rows.Clear();
            _daMessage.SelectCommand.Parameters["p_APP_NAME_ID"].Value = AppName_ID;
            _daMessage.SelectCommand.Parameters["p_MESSAGE_ID"].Value = LastMessageID;
            _daMessage.SelectCommand.Parameters["p_begin_date"].Value = begin_date;
            _daMessage.SelectCommand.Parameters["p_end_date"].Value = end_date;
            _daMessage.Fill(_ds.Tables["MESSAGE"]);
            object last_value = _ds.Tables["MESSAGE"].Compute("MAX(MESSAGE_ID)", "");
            if (last_value != null && last_value != DBNull.Value && Convert.ToDecimal(last_value) > LastMessageID)
            {
                appSettings[AppName + "Message_ID"] = last_value.ToString();
                //appSettings.Save();
                LastMessageID = Convert.ToDecimal(last_value);
                _isHidded = false;// значит есть новые сообщения - при первом запуске у нас покажется список
            }
            isLastLoaded = true;
        }

        bool _isHidded = true;
        public bool IsHidden
        {
            get
            {
                if (!_lastMessageID.HasValue)
                    LoadMessage(_selectedDateBegin, _selectedDateEnd);
                return _isHidded;
            }
        }
    }
}
