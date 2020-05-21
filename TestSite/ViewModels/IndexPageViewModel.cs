using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSite.Models;

namespace TestSite.ViewModels
{
    public class IndexPageViewModel
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public bool CanGoNextPage { get; set; }
        public string Category { get; set; }
        public string Search { get; set; }
        public IEnumerable<Post> Posts { get; set; }
    }
}
