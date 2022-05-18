using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ChatboxVido.Models
{
    public class ContactCrisp
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string ImageSource { get; set; }
        public string ContactId { get; set; }
        public string LastMessage { get; set; }
        public string Website_Id { get; set; }
        public string Session_Id { get; set; }
        public string Content { get; set; }
        public string AppId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string EventName { get; set; }
        public string MsgText { get; set; }
        public string MsgId { get; set; }
        public string Timestamp { get; set; }
        public string EventnameFrom { get; set; }

        public int src { get; set; }
        public long time { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public string message_id { get; set; }
        public string from_id { get; set; }
        public string to_id { get; set; }
        public string from_display_name { get; set; }
        public string from_avatar { get; set; }
        public string to_display_name { get; set; }
        public string to_avatar { get; set; }
    }
}
