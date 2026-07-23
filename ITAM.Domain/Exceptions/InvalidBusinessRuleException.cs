using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Exceptions
{
    public class InvalidBusinessRuleException : DomainException
    {
        public InvalidBusinessRuleException()
            : base() { }

        public InvalidBusinessRuleException(string message)
            : base(message) { }

        public InvalidBusinessRuleException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
