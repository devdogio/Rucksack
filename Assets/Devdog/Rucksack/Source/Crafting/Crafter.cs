using System;

namespace Devdog.Rucksack.Crafting
{
    public class Crafter<T>
        where T: IEquatable<T>, IIdentifiable
    {
        private readonly ILogger _logger;
        
        public Crafter(ILogger logger = null)
        {
            _logger = logger ?? new Logger("[CRAFTER] ");
        }
        
        public virtual Result<bool> CanCraft(CraftDetails<T> details)
        {
            _logger.Log("Not implemented...");
            throw new NotImplementedException();
        }

        public virtual Result<CraftResult<T>> Craft(CraftDetails<T> details)
        {
            _logger.Log("Not implemented...");
            throw new NotImplementedException();
        }
    }
}