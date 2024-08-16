public class FileCat : Parent
{
    public int SenderUserId { get; set; }
    public Users SenderUser { get; set; }
    public int CatId { get; set; }
    public Category Cat { get; set; }
    public List<Files> Files { get; set; }
    public bool isRead { get; set; }
}