﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public ApproveUserStatus Approved { get; set; }
        public UserRoles UserRole { get; set; }
    }
    public enum ApproveUserStatus
    {
        Pending,
        Approved,
        Disapproved
    }
}
