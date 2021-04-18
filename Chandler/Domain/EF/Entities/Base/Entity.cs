using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.EF.Entities.Base
{
    public class Entity
    {
        protected Entity() { }

        [Key]
        public Guid Id { get; private set; }
    }
}
