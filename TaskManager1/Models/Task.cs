using System.ComponentModel.DataAnnotations;

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
    }
}