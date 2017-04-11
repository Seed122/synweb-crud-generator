using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using SPGenerator.DataModel;
using SPGenerator.UI.Commands;
using SPGenerator.UI.Models;

namespace SPGenerator.UI.ViewModels
{
    class SettingsDialogViewModel : ViewAware
    {
        readonly SettingsModel _model;
        public SettingsDialogViewModel()
        {
            _model = new SettingsModel();
            LoadSettings();
        }

        #region Properties

        public string PrefixWhereParameter
        {
            get { return _prefixWhereParameter; }
            set
            {
                if (value == _prefixWhereParameter) return;
                _prefixWhereParameter = value;
                NotifyOfPropertyChange();
            }
        }

        public string PrefixInputParameter
        {
            get { return _prefixInputParameter; }
            set
            {
                if (value == _prefixInputParameter) return;
                _prefixInputParameter = value;
                NotifyOfPropertyChange();
            }
        }
        

        public string PostfixInsertSp
        {
            get { return _postfixInsertSp1; }
            set
            {
                if (value == _postfixInsertSp1) return;
                _postfixInsertSp1 = value;
                NotifyOfPropertyChange();
            }
        }


        public string PostfixUpdateSp
        {
            get { return _postfixUpdateSp; }
            set
            {
                if (value == _postfixUpdateSp) return;
                _postfixUpdateSp = value;
                NotifyOfPropertyChange();
            }
        }


        public string PostfixDeleteSp
        {
            get { return _postfixDeleteSp; }
            set
            {
                if (value == _postfixDeleteSp) return;
                _postfixDeleteSp = value;
                NotifyOfPropertyChange();
            }
        }

        public string ErrorHandling
        {
            get { return _errorHandling; }
            set
            {
                if (value == _errorHandling) return;
                _errorHandling = value;
                NotifyOfPropertyChange();
            }
        }

        public string[] ErrorHandlingOptions
        {
            get
            {
                return new[] { "Yes", "No" };
               
            }
        }


        #endregion

        #region Commands
        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new RelayCommand(Save);
                return saveCommand;
            }
        }

        private RelayCommand cancelCommand;
        private string _prefixWhereParameter;
        private string _prefixInputParameter;
        private string _postfixInsertSp;
        private string _postfixInsertSp1;
        private string _postfixUpdateSp;
        private string _postfixDeleteSp;
        private string _errorHandling;

        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                    cancelCommand = new RelayCommand(Cancel);
                return cancelCommand;
            }
        }

        private void Cancel(object param)
        {
            ((Window)param).Close();
        }

        private void Save(object param)
        {
            var settings = new Settings();
            settings.PrefixInputParameter = PrefixInputParameter;
            settings.PostfixInsertSp = PostfixInsertSp;
            settings.PostfixUpdateSp = PostfixUpdateSp;
            settings.PostfixDeleteSp = PostfixDeleteSp;
            settings.PrefixWhereParameter = PrefixWhereParameter;
            settings.errorHandling = ErrorHandling;
            _model.SaveSettings(settings);
            ((Window)param).Close();
        }


        #endregion

        private void LoadSettings()
        {
            var settings = _model.GetSettings();
            PrefixInputParameter = settings.PrefixInputParameter;
            PostfixInsertSp = settings.PostfixInsertSp;
            PostfixUpdateSp = settings.PostfixUpdateSp;
            PostfixDeleteSp = settings.PostfixDeleteSp;
            PrefixWhereParameter = settings.PrefixWhereParameter;
            ErrorHandling = settings.errorHandling;
        }
    }
}
