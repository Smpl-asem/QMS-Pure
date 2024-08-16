public class FileCat : Parent
{
    public int SenderUserId { get; set; }
    public Users SenderUser { get; set; }
    public int CatId { get; set; }
    public Category Cat { get; set; }
    public List<Files> Files { get; set; }
    public bool isRead { get; set; }
    public int propStatus { get; set; } // 1) empty | 2) write | 3) readed
    public string propText { get; set; }
}