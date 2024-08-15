 public class Category: Parent
{
    public string CatName { get; set; }

    public int ParentId { get; set; }
    public string ParentName { get; set; }
    public int CatCode { get; set; }
    public bool Status { get; set; }

    public ICollection<UserCats> Subs { get; set; }
}