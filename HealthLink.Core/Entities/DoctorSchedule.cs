using HealthLink.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthLink.Core.Entities
{
    public class DoctorSchedule : BaseEntity
    {
        public Guid DoctorId { get; private set; }
        public virtual Doctor Doctor { get; set; }

        public DayOfWeek DayOfWeek { get; private set; }         // ✅ Monday
        public TimeSpan StartTime { get; private set; }          // ✅ 9:00 AM
        public TimeSpan EndTime { get; private set; }            // ✅ 5:00 PM
        public bool IsAvailable { get; private set; } = true;    // ✅ Available this day
        public int SlotDurationMinutes { get; private set; } = 30; // ✅ 30-min appointments

        // Lunch break
        public TimeSpan? BreakStartTime { get; private set; }    // ✅ 12:00 PM
        public TimeSpan? BreakEndTime { get; private set; }      // ✅ 1:00 PM




    }
}
