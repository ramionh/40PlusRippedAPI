using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace _40PlusRipped.Core.Models
{
    public class HealthLevel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int PhysicalActivityLevel { get; set; } // 1-10
        public int NutritionQuality { get; set; } // 1-10
        public int SleepQuality { get; set; } // 1-10
        public int StressLevel { get; set; } // 1-10
        public int OverallHealth { get; set; } // 1-10
        public DateTime RecordedDate { get; set; }
        public string Notes { get; set; }
    }
}