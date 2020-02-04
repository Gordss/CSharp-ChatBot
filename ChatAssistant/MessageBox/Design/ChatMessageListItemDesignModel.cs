using System;

namespace ChatAssistant.MessageBox.Design
{
    public class ChatMessageListItemDesignModel : ChatMessageListItem
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static ChatMessageListItemDesignModel Instance => new ChatMessageListItemDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ChatMessageListItemDesignModel()
        {
            Message = "give me the oee.performance of m45";
            TimeStamp = DateTimeOffset.Now;
            SentByUser = true;
        }
        #endregion
    }
}
