using System;
using System.Collections.ObjectModel;

namespace ChatAssistant.MessageBox
{
    /// <summary>
    /// The chat message list as class representation
    /// </summary>
    public class ChatMessageList
    {
        /// <summary>
        /// List of chat messages
        /// </summary>
        public ObservableCollection<ChatMessageListItem> Items { get; set; }
    }
}
