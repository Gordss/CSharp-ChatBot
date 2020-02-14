using System.Net.Http;
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
            APICallAsync(e);
        }

        /// <summary>
        /// Add message by button clicked function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            MessageContainer.AddMessage(InputField.Text, true);
            APICallAsync(InputField.Text);
            InputField.Text = string.Empty;
        }

        /// <summary>
        /// Calls API to get reponse message for the user
        /// </summary>
        /// <param name="userRequestMessage">user request</param>
        public async void APICallAsync(string userRequestMessage)
        {
            HttpClient client = new HttpClient();
            //WebAPITest.Program.Main(new string[] { });
            string url = "https://localhost:44363/api/call/";
            url += userRequestMessage;
            HttpResponseMessage message = await client.GetAsync(url);
            if (!message.IsSuccessStatusCode)
            {
                System.Windows.MessageBox.Show($"Request failed with status code {message.StatusCode}");
            }
            else
            {
                MessageContainer.AddMessage((await message.Content.ReadAsStringAsync()).ToString(), false);
            }
        }
    }
}
