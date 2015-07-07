using SPGenerator.UI.Commands;
using SPGenerator.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SPGenerator.UI.ViewModels
{
    class SettingsVM : ViewModelBase
    {
        SettingsModel model;
        public SettingsVM()
        {
            model = new SettingsModel();
            LoadSettings();
        }

        #region Properties
        string prefixWhereParameter;
        public string PrefixWhereParameter
        {
            get
            {
                return prefixWhereParameter;
            }
            set
            {
                prefixWhereParameter = value;
                NotifyPropertyChanged("prefixWhereParameter");
            }
        }

        string prefixInputParameter;
        public string PrefixInputParameter
        {
            get
            {
                return prefixInputParameter;
            }
            set
            {
                prefixInputParameter = value;
                NotifyPropertyChanged("prefixInputParameter");
            }
        }

        string postfixInsertSp;
        public string PostfixInsertSp
        {
            get
            {
                return postfixInsertSp;
            }
            set
            {
                postfixInsertSp = value;
                NotifyPropertyChanged("postfixInsertSp");
            }
        }

        string postfixUpdateSp;
        public string PostfixUpdateSp
        {
            get
            {
                return postfixUpdateSp;
            }
            set
            {
                postfixUpdateSp = value;
                NotifyPropertyChanged("postfixUpdateSp");
            }
        }

        string postfixDeleteSp;
        public string PostfixDeleteSp
        {
            get
            {
                return postfixDeleteSp;
            }
            set
            {
                postfixDeleteSp = value;
                NotifyPropertyChanged("postfixDeleteSp");
            }
        }

        string errorHandling;
        public string ErrorHandling
        {
            get
            {
                return errorHandling;
            }
            set
            {
                errorHandling = value;
                NotifyPropertyChanged("ErrorHandling");
            }
        }

        public string[] ErrorHandlingOptions
        {
            get
            {
                return new string[] { "Yes", "No" };
               
            }
        }


        #endregion

        #region Commands
        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null) saveCommand = new RelayCommand(param => this.Save(param));
                return saveCommand;
            }
        }

        private RelayCommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null) cancelCommand = new RelayCommand(param => this.Cancel(param));
                return cancelCommand;
            }
        }

        private void Cancel(object param)
        {
            ((Window)param).Close();
        }

        private void Save(object param)
        {
            var settings = new SPGenerator.DataModel.Settings();
            settings.PrefixInputParameter = PrefixInputParameter;
            settings.PostfixInsertSp = PostfixInsertSp;
            settings.PostfixUpdateSp = PostfixUpdateSp;
            settings.PostfixDeleteSp = PostfixDeleteSp;
            settings.PrefixWhereParameter = PrefixWhereParameter;
            settings.errorHandling = ErrorHandling;
            model.SaveSettings(settings);
            ((Window)param).Close();
        }


        #endregion

        private void LoadSettings()
        {
            var settings = model.GetSettings();
            PrefixInputParameter = settings.PrefixInputParameter;
            PostfixInsertSp = settings.PostfixInsertSp;
            PostfixUpdateSp = settings.PostfixUpdateSp;
            PostfixDeleteSp = settings.PostfixDeleteSp;
            PrefixWhereParameter = settings.PrefixWhereParameter;
            ErrorHandling = settings.errorHandling;
        }
    }
}
