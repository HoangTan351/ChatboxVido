using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatboxVido.Models
{
    public class ProfileZalo
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Avatars
        {
            public string _240 { get; set; }
            public string _120 { get; set; }
        }

        public class ZaloProfileInfo
        {
            public string avatar { get; set; }
            public Avatars avatars { get; set; }
            public int user_gender { get; set; }
            public string user_id { get; set; }
            public string user_id_by_app { get; set; }
            public string display_name { get; set; }
            public int birth_date { get; set; }
            public TagsAndNotesInfo tags_and_notes_info { get; set; }
        }

        public class ZaloGetProfileResult
        {
            public ZaloProfileInfo data { get; set; }
            public int error { get; set; }
            public string message { get; set; }
        }

        public class TagsAndNotesInfo
        {
            public List<object> notes { get; set; }
            public List<object> tag_names { get; set; }
        }


    }
}
