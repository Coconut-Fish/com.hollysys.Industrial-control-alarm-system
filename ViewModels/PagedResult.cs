﻿namespace Server.ViewModels
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = null!;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

}
