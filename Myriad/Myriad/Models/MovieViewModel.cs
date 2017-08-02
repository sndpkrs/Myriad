using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Myriad.Models;
using System.ComponentModel.DataAnnotations;

namespace Myriad.Models
{
    public class MovieViewModel
    {
        MyriadDbEntities db = new MyriadDbEntities();
        

        [Key]
        public int MovID { get; set; }

        [DisplayName("Movie"),Required]
        public string Name { get; set; }

        [DisplayName("Date of Release"), Required, DataType(DataType.Date, ErrorMessage = "Release Date should be a date")]
        public Nullable<System.DateTime> ReleaseDate { get; set; }
        public string Plot { get; set; }
        public string Poster { get; set; }

        [DisplayName("Producer Name")]
        [Required]
        public Nullable<int> ProID { get; set; }
        
        [DisplayName("Producer1 Name")]
        public Producer Producer { get; set; }
        [DisplayName("Actors")]
        public List<CheckActorsModel> ActorsList { get; set; }
    }
}