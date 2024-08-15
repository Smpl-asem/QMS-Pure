public class UserCats : Parent
{
    public int UserId { get; set; }
    public Users User { get; set; }
    public int CatId { get; set; }
    public Category Cat { get; set; }
    public int type { get; set; } // 1 => Read | 2 => Write | 3 => Both
}