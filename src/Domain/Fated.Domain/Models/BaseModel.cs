using System;

namespace Fated.Domain.Models
{
    public class BaseModel<TId>
    {
        public TId Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }

    public class BaseModel : BaseModel<long>
    {

    }

}
