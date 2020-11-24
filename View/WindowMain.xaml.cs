using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PipePressureDrop.View
{
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            // 对于NET45，当UpdateSourceTrigger设为PropertyChanged时，默认无法输入小数
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
            InitializeComponent();
        }

        public void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }
    }
}
