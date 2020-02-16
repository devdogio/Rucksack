using System;

namespace Devdog.Rucksack.Items
{
    public class CooldownTimer
    {
        public DateTime? startTime;
        public TimeSpan? cooldownTime;

        public bool isInCooldown
        {
            get
            {
                if (startTime == null || cooldownTime == null)
                {
                    return false;
                }
                
                return startTime.Value.Add(cooldownTime.Value) < DateTime.Now;
            }
        }
    }
}