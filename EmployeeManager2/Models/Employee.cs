using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager2.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
            ErrorMessage = "Invalid Email Format")]
        [Display(Name = "Office Email")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }

        public DateTime? ReimburseDate { get; set; }
        public DateTime? ReceiptDate { get; set; }

        public string PhotoPath { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal ReceiptAmount { get; set; }



    }
}
