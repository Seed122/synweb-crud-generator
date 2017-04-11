using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace SPGenerator.UI.ViewModels
{
    class InitConnectionDialogViewModel: ViewAware
    {
        #region Props

        private string _dataSource;
        private string _login;
        private string _password;
        private string _database;

        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                if (value == _dataSource) return;
                _dataSource = value;
                NotifyOfPropertyChange();
            }
        }

        public string Login
        {
            get { return _login; }
            set
            {
                if (value == _login) return;
                _login = value;
                NotifyOfPropertyChange();
            }
        }

        public string Password // TODO: Hiding characters
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
                NotifyOfPropertyChange();
            }
        }

        public string Database
        {
            get { return _database; }
            set
            {
                if (value == _database) return;
                _database = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        public void Connect()
        {
            var window = (Window)GetView();
            window.DialogResult = true;
            window.Close();
        }

        public void Cancel()
        {
            var window = (Window) GetView();
            window.DialogResult = false;
            window.Close();
        }

        #endregion

    }
}
