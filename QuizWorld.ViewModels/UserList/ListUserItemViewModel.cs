﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.UserList
{
    /// <summary>
    /// Represents a user in a list of users.
    /// </summary>
    public class ListUserItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// A collection of roles. The role "User" is
        /// removed from the list if the user has any other roles.
        /// </summary>
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}
