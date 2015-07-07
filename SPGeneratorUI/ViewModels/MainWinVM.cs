using SPGenerator;
using SPGenerator.Common;
using SPGenerator.DataModel;
using SPGenerator.UI.Commands;
using SPGenerator.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TreeViewControl.ViewModels;
using TreeViewControl.Views;


namespace SPGenerator.UI.ViewModels
{
    internal class MainWinVM : ViewModelBase
    {

        #region vairable
        TreeViewNode rootNode;
        MainWinModel model;
        string conString = @"Server=.\sqlexpress;Database=TestDb;User ID=mahesh;Password=mypassword;";
        private ICollection<DBTableInfo> _dbInfo;
        //string defaultDisplayText = "Enter Connection String Here";
        #endregion

        public MainWinVM()
        {
            model = new MainWinModel();
            ConnectionString = conString;
        }

        #region Properties
        string sqlScript;
        public string SqlScript
        {
            get
            {
                return sqlScript;
            }
            set
            {
                sqlScript = value;
                NotifyPropertyChanged("SqlScript");
            }
        }

        string connectionString;
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
                NotifyPropertyChanged("ConnectionString");
            }
        }

        bool isConnectedToServer;
        public bool IsConnectedToServer
        {
            get
            {
                return isConnectedToServer;
            }
            set
            {
                isConnectedToServer = value;
                NotifyPropertyChanged("IsConnectedToServer");
            }
        }

        #endregion

        #region Commands
        private RelayCommand connectServerCommand;
        public ICommand ConnectServerCommand
        {
            get
            {
                if (connectServerCommand == null) connectServerCommand = new RelayCommand(param => this.ConnectToServer(param));
                return connectServerCommand;
            }
        }
        private void ConnectToServer(object param)
        {
            var currentCursor = Mouse.OverrideCursor;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                _dbInfo = model.GetDbInfo(ConnectionString);
                PopulateTree(_dbInfo, (TreeViewWithCheckBox)param);
                IsConnectedToServer = true;
            }

            finally
            {
                Mouse.OverrideCursor = currentCursor;
            }
        }

        private RelayCommand generateSPCommand;
        public ICommand GenerateSPCommand
        {
            get
            {
                if (generateSPCommand == null) generateSPCommand = new RelayCommand(param => this.GenerateSps());
                return generateSPCommand;
            }
        }

        private RelayCommand settingCommand;
        public ICommand SettingCommand
        {
            get
            {
                if (settingCommand == null) settingCommand = new RelayCommand(param => this.Settings());
                return settingCommand;
            }
        }
        private void Settings()
        {
            Views.Settings setting = new Views.Settings();
            setting.Show();
        }

        private RelayCommand copyCommand;
        public ICommand CopyCommand
        {
            get
            {
                if (copyCommand == null) copyCommand = new RelayCommand(param => this.Copy());
                return copyCommand;
            }
        }
        private void Copy()
        {
            if (!string.IsNullOrEmpty(SqlScript))
            {
                Clipboard.SetText(SqlScript);
            }
        }

        private RelayCommand gotFocusConnectionStringCommand;
        public ICommand GotFocusConnectionStringCommand
        {
            get
            {
                if (gotFocusConnectionStringCommand == null) gotFocusConnectionStringCommand = new RelayCommand(param => this.GotFocusConnectionString());
                return gotFocusConnectionStringCommand;
            }
        }
        private void GotFocusConnectionString()
        {
            if (ConnectionString.Trim() == conString)
                ConnectionString = "";
        }

        private RelayCommand lostFocusConnectionStringCommand;
        public ICommand LostFocusConnectionStringCommand
        {
            get
            {
                if (lostFocusConnectionStringCommand == null) lostFocusConnectionStringCommand = new RelayCommand(param => this.LostFocusConnectionString());
                return lostFocusConnectionStringCommand;
            }
        }
        private void LostFocusConnectionString()
        {
            if (ConnectionString.Trim() == "")
                ConnectionString = conString;
        }

        #endregion

        #region SP Generation

        private void GenerateSps()
        {
            StringBuilder sb = new StringBuilder(1000);
            //model.RefreshSettings();
            foreach (TreeViewNode tblNode in rootNode.Children)
            {
                if (tblNode.IsChecked ?? true)
                {
                    sb.AppendLine(GenerateSPForSingleTable(tblNode));
                }
            }
            SqlScript = sb.ToString();
        }

        private string GenerateSPForSingleTable(TreeViewNode tblNode)
        {
            var sb = new StringBuilder();
            foreach (TreeViewNode childNode in tblNode.Children)
            {
                var selectedFields = GetSelectedFields(childNode);
                var whereClauseSelectedFields = GetWhereClauseFields(childNode);

                if (childNode.IsChecked ?? true)
                {
                    try
                    {
                        var sp = model.GenerateSp((DBTableInfo)tblNode.Tag, childNode.Name, selectedFields, whereClauseSelectedFields);
                        sb.AppendLine(sp.Script);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            return sb.ToString();
        }

        private List<DBTableColumnInfo> GetSelectedFields(TreeViewNode tableChildNode)
        {
            var selectedFields = new List<DBTableColumnInfo>();
            foreach (TreeViewNode fieldNode in tableChildNode.Children)
            {
                if ((fieldNode.IsChecked ?? true) && fieldNode.Name != Constants.whereConditionTreeNodeText)
                {
                    selectedFields.Add((DBTableColumnInfo)fieldNode.Tag);
                }
            }

            return selectedFields;
        }

        private List<DBTableColumnInfo> GetWhereClauseFields(TreeViewNode tableChildNode)
        {
            var selectedFields = new List<DBTableColumnInfo>();
            var whereClauseNode = GetChildNodeByText(tableChildNode, Constants.whereConditionTreeNodeText);
            if (whereClauseNode == null)
                return selectedFields;

            foreach (TreeViewNode fieldNode in whereClauseNode.Children)
            {
                if (fieldNode.IsChecked ?? true)
                {
                    selectedFields.Add((DBTableColumnInfo)fieldNode.Tag);
                }
            }

            return selectedFields;
        }

        private TreeViewNode GetChildNodeByText(TreeViewNode parentNode, string childNodeText)
        {
            var childNode = parentNode.Children.OfType<TreeViewNode>()
                          .FirstOrDefault(node => node.Name.Equals(childNodeText));

            return childNode;
        }

        #endregion

        #region PopulateTree
        private void PopulateTree(IEnumerable<DBTableInfo> sqlTableList, TreeViewWithCheckBox treeView1)
        {
            TreeViewNode root = new TreeViewNode(Constants.rootTreeNodeText, null);
            foreach (var tbl in sqlTableList)
            {
                root.Children.Add(CreateTableNode(tbl, root));
            }
            root.IsNodeExpanded = true;
            treeView1.DataContext = new List<TreeViewNode> { root };
            rootNode = root;
        }

        private TreeViewNode CreateTableNode(DBTableInfo sqlTableInfo, TreeViewNode parent)
        {
            string tableDisplayName = sqlTableInfo.Schema == "dbo"
                ? sqlTableInfo.TableName
                : sqlTableInfo.Schema + "." + sqlTableInfo.TableName;
            TreeViewNode tblNode = new TreeViewNode(tableDisplayName, parent);
            tblNode.Tag = sqlTableInfo;

            TreeViewNode insertSp = new TreeViewNode(Constants.insertTreeNodeText, tblNode);
            AddColumnNodes(insertSp, sqlTableInfo.Columns.Where(x => !x.Exclude), true);
            tblNode.Children.Add(insertSp);


            TreeViewNode deleteSp = new TreeViewNode(Constants.deleteTreeNodeText, tblNode);
            TreeViewNode whereDelCondition = new TreeViewNode(Constants.whereConditionTreeNodeText, deleteSp);
            deleteSp.Children.Add(whereDelCondition);
            tblNode.Children.Add(deleteSp);
            AddColumnNodes(whereDelCondition, sqlTableInfo.Columns.Where(x => x.IsPrimaryKey).ToList(), true);
            
            var updateCols = sqlTableInfo.Columns.Where(x => !x.Exclude).ToArray();
            var updateWhereCols = sqlTableInfo.Columns.Where(x => x.IsPrimaryKey).ToArray();
            if (updateCols.Any() && updateCols.All(x => !updateWhereCols.Contains(x)))
            {
                TreeViewNode updateSp = new TreeViewNode(Constants.updateTreeNodeText, tblNode);
                
                AddColumnNodes(updateSp, updateCols, true);
                tblNode.Children.Add(updateSp);
                TreeViewNode whereUpdateCondition = new TreeViewNode(Constants.whereConditionTreeNodeText, updateSp);
                updateSp.Children.Add(whereUpdateCondition);
                AddColumnNodes(whereUpdateCondition, updateWhereCols, true);
            }
            AddColumnNodes(deleteSp, new List<DBTableColumnInfo>(), true);

            var selectColumns = new DBTableColumnInfo[sqlTableInfo.Columns.Count];
            TreeViewNode selectSp = new TreeViewNode(Constants.selectTreeNodeText, tblNode);
            sqlTableInfo.Columns.CopyTo(selectColumns, 0);
            Array.ForEach(selectColumns, x =>
            {
                x.Exclude = false;
            });
            AddColumnNodes(selectSp, selectColumns, true);
            tblNode.Children.Add(selectSp);

            var selectOneColumns = new DBTableColumnInfo[sqlTableInfo.Columns.Count];
            var selectOneWhereCols = sqlTableInfo.Columns.Where(x => x.IsPrimaryKey).ToArray();
            sqlTableInfo.Columns.CopyTo(selectOneColumns, 0);
            Array.ForEach(selectColumns, x =>
            {
                x.Exclude = false;
            });
            if (selectOneColumns.Any() &&
                selectOneWhereCols.All(x => selectOneColumns.Any(y => x.ColumnName != y.ColumnName)))
            {
                TreeViewNode selectOneSp = new TreeViewNode(Constants.selectOneTreeNodeText, tblNode);
                AddColumnNodes(selectOneSp, selectOneColumns, true);
                tblNode.Children.Add(selectOneSp);
                TreeViewNode whereSelectOneCondition = new TreeViewNode(Constants.whereConditionTreeNodeText,
                    selectOneSp);
                selectOneSp.Children.Add(whereSelectOneCondition);
                AddColumnNodes(whereSelectOneCondition, selectOneWhereCols, true);
            }

            return tblNode;
        }

        private TreeViewNode AddColumnNodes(TreeViewNode parentNode, IEnumerable<DBTableColumnInfo> columns, bool disableExludeColumn)
        {
            foreach (var colInfo in columns)
            {
                TreeViewNode colNode = new TreeViewNode(GetColumnDispalyName(colInfo), parentNode);
                colNode.Tag = colInfo;
                if (disableExludeColumn)
                {
                    colNode.IsNodeEnabled = !colInfo.Exclude;
                }
                parentNode.Children.Add(colNode);
            }
            return parentNode;
        }

        private string GetColumnDispalyName(DBTableColumnInfo colInfo)
        {
            string displayName = colInfo.ColumnName;
            if (colInfo.Exclude)
            {
                if (colInfo.IsIdentity)
                {
                    displayName += " (IDENTITY)";
                }
                if (colInfo.DataType.ToUpperInvariant() == "TIMESTAMP")
                    displayName += " (TIMESTAMP)";

            }

            return displayName;
        }

        #endregion

    }
}
