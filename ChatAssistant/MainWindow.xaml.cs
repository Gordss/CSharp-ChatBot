using System.Windows;


namespace ChatAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        /// <summary>
        /// Add message function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">the message content</param>
        private void InputBox_TextSubmitted(object sender, string e)
        {
            MessageContainer.AddMessage(e.ToString(), true);
        }

        /// <summary>
        /// Add message by button clicked function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            MessageContainer.AddMessage(InputField.Text, true);
            InputField.Text = string.Empty;
        }
    }
}
