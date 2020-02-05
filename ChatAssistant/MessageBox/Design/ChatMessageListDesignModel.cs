using System;
using System.Collections.ObjectModel;

namespace ChatAssistant.MessageBox.Design
{
    public class ChatMessageListDesignModel : ChatMessageList
    {
        #region Singleton

        /// <summary>
        /// A signle instance of the design model
        /// </summary>
        public static ChatMessageListDesignModel Instance => new ChatMessageListDesignModel();

        #endregion

        #region Contructor

        /// <summary>
        /// Default constuctor
        /// </summary>

        public ChatMessageListDesignModel()
        {
            Items = new ObservableCollection<ChatMessageListItem>
            {
                new ChatMessageListItem
                {
                    Message = "give me the oee.performance of m45",
                    TimeStamp = DateTimeOffset.Now,
                    SentByUser = true
                },
                new ChatMessageListItem
                {
                    Message = "The performance of M45 is 100!",
                    TimeStamp = DateTimeOffset.Now,
                    SentByUser = false
                },
            };
        }

        #endregion
    }
}
