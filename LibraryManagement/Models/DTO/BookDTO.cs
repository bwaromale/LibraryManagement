﻿using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class BookDTO
    {
        public string Title { get; set; }
        public string CoverType { get; set; }
        public string NoOfPages { get; set; }
        public string ForewardBy { get; set; }
        public string ISBN { get; set; }
        public double Price { get; set; }
        public bool Available { get; set; }
        public int TotalCopies { get; set; }
        public int BorrowedCopies { get; set; }
    }
}
