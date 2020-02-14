using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChatAssistant.MessageBox
{
    /// <summary>
    /// Interaction logic for ChatMessageListControl.xaml
    /// </summary>
    public partial class ChatMessageListControl : UserControl
    {
        public ChatMessageListControl()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += ((sender, e) =>
            {
                if (MessageList.VerticalOffset == MessageList.ScrollableHeight)
                {
                    MessageList.ScrollToEnd();
                }
            });
            timer.Start();
        }

        /// <summary>
        /// Add message to the message list
        /// </summary>
        /// <param name="text">the new message content</param>
        /// <param name="isFromUser">true if the message is from the user and false if it's API response message</param>
        public void AddMessage(string text, bool isFromUser)
        {
            var messageList = (ObservableCollection<ChatMessageListItem>)Messages.ItemsSource;

            //Don't send a blank message
            if (string.IsNullOrEmpty(text))
                return;

            //Check if message list is empty
            if(messageList == null)
            {
                messageList = new ObservableCollection<ChatMessageListItem>();
            }

            var message = new ChatMessageListItem
            {
                Message = text,
                TimeStamp = DateTimeOffset.Now,
                SentByUser = isFromUser
            };

            //Add message to the list
            messageList.Add(message);
        }
    }
}
