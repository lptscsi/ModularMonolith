﻿namespace Shop.Emails.Implementation
{
    internal class EmailOptions
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FromName { get; set; }
    }
}
