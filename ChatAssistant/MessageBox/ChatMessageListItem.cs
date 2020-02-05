using System;

namespace ChatAssistant.MessageBox
{
    /// <summary>
    /// each chat message representation
    /// </summary>
    public class ChatMessageListItem
    {
        /// <summary>
        /// Message content
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Message timestamp
        /// </summary>
        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// True if this message was send by the user
        /// </summary>
        public bool SentByUser { get; set; }
    }
}
