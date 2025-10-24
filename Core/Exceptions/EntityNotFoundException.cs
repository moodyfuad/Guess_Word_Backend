using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, object key)
            : base($"Entity \"{entityName}\" with key {key} was not found.")
        {
        }
    }
}
