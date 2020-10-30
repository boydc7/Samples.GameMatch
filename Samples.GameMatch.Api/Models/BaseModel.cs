using System;
using System.ComponentModel.DataAnnotations;

namespace Samples.GameMatch.Api
{
    public abstract class BaseModel : IHasGuidId, IEquatable<BaseModel>
    {
        [Required]
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public bool Equals(BaseModel other)
            => other != null && other.Id == Id;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is BaseModel bmo && Equals(bmo);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
