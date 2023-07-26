using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.UserList
{
    /// <summary>
    /// A catalogue of users
    /// </summary>
    public class ListUsersViewModel
    {
        public int Total { get; set; }
        public IEnumerable<ListUserItemViewModel> Users { get; set; } = Enumerable.Empty<ListUserItemViewModel>();
    }
}
