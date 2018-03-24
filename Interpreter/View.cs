using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Interpreter
{
    class View
    {
        public Label num;
        public CheckBox chD, chE;
        public TextBox[] tb;
        
        public View()
        {
            num = new Label();
            chD = new CheckBox();
            chE = new CheckBox();

            tb = new[] {
                new TextBox(),
                new TextBox(),
                new TextBox(),
                new TextBox()
            };
            
            Init(num);
            Init(chE, "Выполнить");
            Init(chD, "Удалить");

            tb[3].IsReadOnly = true;
            foreach (var tbox in tb)
            {
                Init(tbox);
            }
        }
        public void Init(Label l)
        {
            l.VerticalAlignment = VerticalAlignment.Center;
            l.HorizontalAlignment = HorizontalAlignment.Center;
            l.FontSize = 14;
        }
        public void Init(TextBox c)
        {
            c.BorderBrush = Brushes.Black;
            c.Margin = new Thickness(3);
            c.HorizontalContentAlignment = HorizontalAlignment.Center;
            c.VerticalContentAlignment = VerticalAlignment.Center;
            c.TextChanged += HandleChar;
        }
        public void Init(Control c, string hint)
        {
            c.ToolTip = hint;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.HorizontalAlignment = HorizontalAlignment.Center;
        }
        public void HandleChar(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && !System.Text.RegularExpressions.Regex.IsMatch(tb.Text, "^[0-9]")) 
            {
                tb.Text = "";
            }
        }
    }
}
