using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spam
{
    internal class UserCategories
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
    }
}
