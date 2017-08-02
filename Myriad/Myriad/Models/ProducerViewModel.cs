using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Myriad.Models
{
    public class ProducerViewModel
    {
        public int ProID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int? Sex { get; set; }

        [DisplayName("Birthday"), DataType(DataType.Date, ErrorMessage = "Birthday should be date")]
        public Nullable<System.DateTime> DOB { get; set; }
        [DisplayName("About")]
        public string Bio { get; set; }
    }
}