using System.Windows.Controls;
using System.Windows.Input;
using TreeViewControl.ViewModels;

namespace TreeViewControl.Views
{
    /// <summary>
    /// Interaction logic for TreeViewWithCheckBox.xaml
    /// </summary>
    public partial class TreeViewWithCheckBox : UserControl
    {
        public TreeViewWithCheckBox()
        {
            InitializeComponent();
        }

        public void BindCommand()
        {
            TreeViewNode root = this.tree.Items[0] as TreeViewNode;

            base.CommandBindings.Add(
                new CommandBinding(
                    ApplicationCommands.Undo,
                    (sender, e) => // Execute
                    {
                        e.Handled = true;
                        root.IsChecked = false;
                        this.tree.Focus();
                    },
                    (sender, e) => // CanExecute
                    {
                        e.Handled = true;
                        e.CanExecute = (root.IsChecked != false);
                    }));

            this.tree.Focus();
            
        }

        public TreeView GetTree()
        {
           return this.tree;
        }
        
    }
}
