using System;
using System.Collections.Generic;
using System.Text;

namespace ProAnimaFilms_Bot.Models
{
    class SearchTVshows
    {
        public int page { get; set; }
        public int total_results { get; set; }
        public int total_pages { get; set; }
        public List<TVshowsResults> results { get; set; }
    }
    public class TVshowsResults
    {
        public double popularity { get; set; }
        public int vote_count { get; set; }
        public string poster_path { get; set; }
        public int id { get; set; }
        public string backdrop_path { get; set; }
        public string original_language { get; set; }
        public List<int> genre_ids { get; set; }
        public string overview { get; set; }
        public string release_date { get; set; }
        public string first_air_date { get; set; }
        public List<string> origin_country { get; set; }
        public string name { get; set; }
        public string original_name { get; set; }
    }
    
}
