using System.Security.Claims;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly Context db;
    private readonly IWebHostEnvironment _env;
    public HomeController(Context _db, IWebHostEnvironment env)
    {
        _env = env;
        db = _db;
    }
    public IActionResult Index()
    {
        // ViewBag.Categories = db.Users_tbl.Include(x=>x.Categories).ThenInclude(x=>x.Cat).Where(x=> x.Id == Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier))).Select(x=>x.Categories.Select(y=>y.Cat)).ToList()[0];
        ViewBag.Categories = db.Users_tbl
            .Include(x => x.Categories)
                .ThenInclude(x => x.Cat)
            .Where(x => x.Id == Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            .Select(x => x.Categories.Select(y => y.Cat))
            .ToList()[0]
            .Select(x => db.Categories_tbl.Find(x.ParentId))
            .GroupBy(x => x.Id)
            .Select(x => x.First())
            .ToList();

        return View();
    }
    public IActionResult IndexSub(int id)
    {
        ViewBag.Categories = db.Users_tbl
            .Include(x => x.Categories)
                .ThenInclude(x => x.Cat)
            .Where(x => x.Id == Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            .Select(x => x.Categories.Select(y => y.Cat))
            .ToList()[0]
            .Where(x => x.ParentId == id)
            .ToList();

        return View();
    }

    public IActionResult view(int id, int page = 1)
    {
        var UserCatCheck = db.UserCats_tbl.FirstOrDefault(x => x.CatId == id && x.UserId == Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        if (UserCatCheck == null)
        {
            return Ok("شما دسترسی ندارید"); // تکمیلی
        }
        var CatCheck = db.Categories_tbl.Find(UserCatCheck.CatId);
        ViewBag.Cat = CatCheck;

        List<FileCat> datas;

        switch (UserCatCheck.type)
        {
            case 1:
                ViewBag.CanUpload = false;
                datas = db.FileCats_tbl.Where(x => x.CatId == id).Include(x => x.Files).Include(x => x.SenderUser).OrderByDescending(x => x.Id).ToList();
                ViewBag.Data = datas;
                break;
            case 2:
                ViewBag.CanUpload = true;
                datas = db.FileCats_tbl.Where(x => x.CatId == id && x.SenderUserId == Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)) && DateTime.UtcNow.AddDays(-1) < x.CreateDateTime).Include(x => x.Files).Include(x => x.SenderUser).OrderByDescending(x => x.Id).ToList();
                ViewBag.Data = datas;
                break;
            case 3:
                ViewBag.CanUpload = true;
                datas = db.FileCats_tbl.Where(x => x.CatId == id).Include(x => x.Files).Include(x => x.SenderUser).OrderByDescending(x => x.Id).ToList();
                ViewBag.Data = datas;
                break;
            default:
                return Ok("شما دسترسی ندارید");
        }

        ViewBag.DataCount = (int)Math.Ceiling((double)datas.Count / 10);

        var datasChose = datas.Skip((page - 1) * 10).Take(10).ToList();
        ViewBag.Data = datasChose;

        ViewBag.page = page;

        foreach (var item in datasChose)
        {
            if(item.propStatus == 2){
                item.propStatus = 3;
                db.FileCats_tbl.Update(item);
                db.SaveChanges();
            }
        }
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> AddFileAsync(int id, ICollection<IFormFile> NewFiles)
    {
        var UserCatCheck = db.UserCats_tbl.FirstOrDefault(x => x.CatId == id && x.UserId == Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)) && (x.type == 2 || x.type == 3));
        if (UserCatCheck == null)
        {
            return Ok("شما دسترسی ندارید"); // تکمیلی
        }
        else
        {

            var newFileCat = new FileCat
            {
                CatId = id,
                SenderUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                CreateDateTime = DateTime.UtcNow,
                isRead = false,
                propStatus = 1,
                propText = "empty"
            };

            db.FileCats_tbl.Add(newFileCat);
            db.SaveChanges();

            foreach (var NewFile in NewFiles)
            {
                string FileExtension = Path.GetExtension(NewFile.FileName);
                var NewFileName = System.String.Concat(Guid.NewGuid().ToString(), FileExtension);
                var path = $"{_env.WebRootPath}\\uploads\\{NewFileName}";
                var PathSave = $"\\uploads\\{NewFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await NewFile.CopyToAsync(stream);
                }

                db.Files_tbl.Add(new Files
                {
                    CreateDateTime = DateTime.UtcNow,
                    FileName = NewFile.FileName,
                    FilePath = PathSave,
                    FileType = FileExtension,
                    FileCatId = (int)newFileCat.Id
                });
                db.SaveChanges();
            }
            return RedirectToAction("view", "home", new { id });
        }
    }

    [HttpGet]
    public IActionResult DeleteFile(int id, int FileId)
    {
        var UserCatCheck = db.UserCats_tbl.FirstOrDefault(x => x.CatId == id && x.UserId == Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)) && (x.type == 2 || x.type == 3));
        var check = db.FileCats_tbl.Find(FileId);

        if (UserCatCheck == null)
        {
            return Ok("شما دسترسی ندارید"); // تکمیلی
        }
        else if (check.SenderUserId != UserCatCheck.UserId)
        {
            return Ok("پیام برای شما نیستش"); // تکمیلی
        }
        else if (check.CreateDateTime < DateTime.UtcNow.AddDays(-1))
        {
            return Ok("مهلت حذف پیام به اتمام رسیده است"); // تکمیلی
        }
        else
        {
            db.FileCats_tbl.Remove(check);
            db.SaveChanges();

            return RedirectToAction("view", new { id });
        }
    }

}
