using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager1.Models
{
    public class Task
    {
        [Key] private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string FileName { get; set; }
        
        public int? CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}