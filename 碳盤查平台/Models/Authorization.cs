namespace 碳盤查平台.Models
{
    public class Authorization
    {
        public int Id { get; set; }
        public int GroupsId { get; set; }
        public int FumtionId { get; set; }

        //Navigation Property
        public virtual Groups Groups { get; set; }
        public ICollection<Funtion> Funtions { get; set; }
    }
}
