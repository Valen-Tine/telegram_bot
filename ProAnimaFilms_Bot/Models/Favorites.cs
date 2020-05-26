using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Requests;

namespace ProAnimaFilms_Bot.Models
{
    public class Favorites
    {
        public int _id { get; set; }
        public List<string> featuredfims { get; set; }
    }
}
