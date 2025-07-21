﻿using Microsoft.EntityFrameworkCore;

namespace mvcFirstApp.ViewModels
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }  // current page
        public int TotalPages { get; private set; } // total pages

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count(); // total records
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); // current page records
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }

}
