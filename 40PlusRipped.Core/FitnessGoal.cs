using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _40PlusRipped.Core.Models
{
    public enum GoalType
    {
        WeightLoss,
        MuscleGain,
        Endurance,
        Overall
    }

    public class FitnessGoal
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public GoalType GoalType { get; set; }
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Notes { get; set; }
    }
}