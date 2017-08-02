using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Myriad.Models
{
    public class ActorsViewModels
    {

        public ActorsViewModels()
        {
            this.glist = new List<SelectListItem>();
            List<SelectListItem> genderList = new List<SelectListItem>();
            genderList.Add(new SelectListItem { Text = "Male", Value = "1" });
            genderList.Add(new SelectListItem { Text = "Female", Value = "2" });
            genderList.Add(new SelectListItem { Text = "Others", Value = "3" });

            this.glist = genderList;
        }
        public int ActID { get; set; }
        [Required(ErrorMessage="Actor Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public int Sex { get; set; }

        [DisplayName("Birthday"),DataType(DataType.Date,ErrorMessage="Birthday should be date")]
        public Nullable<System.DateTime> DOB { get; set; }
        [DisplayName("About")]
        public string Bio { get; set; }


        IEnumerable<SelectListItem> glist { get; set; }
        public virtual ICollection<MovieActor> MovieActors { get; set; }
    }
}