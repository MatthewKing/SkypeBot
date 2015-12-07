using System;
using System.Collections.Generic;
using Dapper;

namespace SkypeBot
{
    public class MessageReader
    {
        private readonly SkypeDatabase database;
        private Nullable<int> messageId;

        public MessageReader(SkypeDatabase database)
        {
            this.database = database;
            this.messageId = null;
        }

        public IEnumerable<Message> GetUnreadMessages()
        {
            if (!this.messageId.HasValue)
            {
                this.messageId = this.GetCurrentMessageId();
            }

            var messages = this.GetMessages(this.messageId.Value);

            this.messageId = this.GetCurrentMessageId();

            return messages;
        }

        private int GetCurrentMessageId()
        {
            var sql = "select max([id]) from [Messages]";

            using (var connection = this.database.GetConnection())
            {
                return Convert.ToInt32(connection.ExecuteScalar(sql));
            }
        }

        private IEnumerable<Message> GetMessages(int startingMessageId)
        {
            var sql = "select datetime([timestamp], 'unixepoch') as [Timestamp], [chatname] as [ChatName], [author] as [Sender], [from_dispname] as [SenderDisplayName], [body_xml] as [Text] from [Messages] where [id] > @StartingMessageId;";
            var param = new
            {
                StartingMessageId = startingMessageId,
            };

            using (var connection = this.database.GetConnection())
            {
                return connection.Query<Message>(sql, param);
            }
        }
    }
}
