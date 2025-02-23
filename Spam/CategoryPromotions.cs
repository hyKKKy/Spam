using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spam
{
    internal class CategoryPromotions
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? PromotionId { get; set; }
    }
}
