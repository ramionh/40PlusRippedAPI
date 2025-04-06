using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _40PlusRipped.Core.Models
{
    public class UserVitals
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public decimal BMI { get; set; }
        public int Age { get; set; }
        public DateTime RecordedDate { get; set; }
        public string Notes { get; set; }
    }
}
