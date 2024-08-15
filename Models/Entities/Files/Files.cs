public class Files : Parent
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FileType { get; set; }
    public int FileCatId { get; set; }
    public FileCat FileCat { get; set; }
}