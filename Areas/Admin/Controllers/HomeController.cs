using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace test.Areas.Admin.Controllers;
//add Area Admin
[Area("Admin")]
// [Authorize]
public class HomeController : Controller
{
    private readonly Context dbs;
    private readonly string salt = "S@lt?";

    private readonly IWebHostEnvironment _env;
    public HomeController(Context _db, IWebHostEnvironment env)
    {
        _env = env;
        dbs = _db;
    }


    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Category()
    {
        ViewBag.Cats = dbs.Categories_tbl.OrderByDescending(x => x.Id).ToList();
        return View("ViewCat");
    }

    public IActionResult AddCat()
    {
        ViewBag.Cats = dbs.Categories_tbl.Where(x=>x.ParentId==0).OrderByDescending(x => x.Id).ToList();
        return View("AddCat");
    }

    [HttpGet]
    public IActionResult EditCat(int id)
    {
        var cat = dbs.Categories_tbl.Find(id);
        ViewBag.Cat = cat;
        ViewBag.Cats = dbs.Categories_tbl.Where(x => x.Id != id && x.ParentId == 0).OrderByDescending(x => x.Id).ToList();
        return View("EditCat", cat);
    }

    [HttpPost]
    public IActionResult EditCat(int id , int? ParentId, string CatName, int CatCode)
    {
        var cat = dbs.Categories_tbl.Find(id);
        //edit
        cat.CatName = CatName;
        cat.ParentId = (int)ParentId;
        cat.CatCode = CatCode;

        dbs.Categories_tbl.Update(cat);
        dbs.SaveChanges();

        return RedirectToAction("Category");
    }


    [HttpPost]
    public IActionResult AddCat(int ParentId, string CatName, int CatCode)
    {
        dbs.Categories_tbl.Add(new Category
        {
            ParentId = ParentId,
            ParentName = ParentId == 0 ? "دسته اصلی" : dbs.Categories_tbl.Find(ParentId).CatName,
            CatName = CatName,
            CatCode = (int)CatCode,
            CreateDateTime = DateTime.Now
        });
        dbs.SaveChanges();
        return RedirectToAction("Category");
    }

    [HttpGet]
    public IActionResult DeleteCat(int id)
    {
        var cat = dbs.Categories_tbl.Find(id);
        dbs.Categories_tbl.Remove(cat);
        dbs.SaveChanges();
        return RedirectToAction("Category");
    }

    public IActionResult ReportSeen()
    {
        var cats = dbs.Categories_tbl.Where(x => x.ParentId == 0).OrderByDescending(x => x.Id).ToList(); 
        ViewBag.Categories = cats;

        var notif = cats.Select(x=> dbs.FileCats_tbl.Where(y=>y.Cat.ParentId == x.Id && y.isRead == false).Count()).ToList();
        ViewBag.notif = notif;

        return View();
    }
    public IActionResult SubReportSeen(int Id)
    {
        var cats = dbs.Categories_tbl.Where(x => x.ParentId == Id).OrderByDescending(x => x.Id).ToList();
        ViewBag.Categories = cats;

        var notif = cats.Select(x=> dbs.FileCats_tbl.Where(y=>y.CatId == x.Id && y.isRead == false).Count()).ToList();
        ViewBag.notif = notif;

        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(DtodUser user)
    {
        string PathSave;

        Users check =
            dbs
                .Users_tbl
                .FirstOrDefault(x =>
                    x.Username == user.Username ||
                    x.NatinalCode == user.NatinalCode ||
                    x.Phone == user.Phone);
        if (check != null)
        {
            if (check.Username == user.Username.ToLower())
            {
                ViewBag.Error = ("کاربر وارد شده تکراری است");
                return View();
            }
            else if (check.NatinalCode == user.NatinalCode)
            {
                ViewBag.Error = ("کد ملی وارد شده تکراری است");
                return View();
            }
            else if (check.Phone == user.Phone)
            {
                ViewBag.Error = ("شماره تلفن وارد شده  تکراری است");
                return View();
            }
            else
            {
                ViewBag.Error = "مشکلی پیش امده است ، با پشتیبانی تماس بگیرید";
                return View();
            }
        }
        else
        {
            string FileExtension = Path.GetExtension(user.Profile.FileName);
            var NewFileName =
                String.Concat(Guid.NewGuid().ToString(), FileExtension);
            var path = $"{_env.WebRootPath}\\uploads\\{NewFileName}";
            PathSave = $"\\uploads\\{NewFileName}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await user.Profile.CopyToAsync(stream);
            }

            var NewUser =
                new Users
                {
                    Username = user.Username.ToLower(),
                    Password =
                        BCrypt
                            .Net
                            .BCrypt
                            .HashPassword(user.Password +
                            salt +
                            user.Username.ToLower()),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Addres = user.Addres,
                    NatinalCode = user.NatinalCode,
                    PerconalCode = user.PerconalCode,
                    Profile = PathSave,
                    CreateDateTime = DateTime.UtcNow,
                    Token = "null",
                    isActive = true
                };
            dbs.Users_tbl.Add(NewUser);
            dbs.SaveChanges();

            ViewBag.Result = "ثبت نام با موفقیت انجام شد ";
            return RedirectToAction("GetUsers");
        }
    }
    public IActionResult GetUsers()
    {
        ViewBag.Users = dbs.Users_tbl.Where(x => x.Username != "admin").OrderByDescending(x => x.Id).ToList();
        return View("GetUsers");
    }

    public IActionResult UserSetting(int Id)
    {
        Users Check = dbs.Users_tbl.Find(Id);
        ViewBag.User = Check;
        return View("UserSetting");
    }

    [HttpPost]
    public async Task<IActionResult> UserSettingAsync(string Addres, string FirstName, string LastName, string Phone, string PerconalCode, string NatinalCode, IFormFile Profile, int Id)
    {
        var BaseUser = dbs.Users_tbl.Find(Id);
        BaseUser.Addres = Addres;
        BaseUser.FirstName = FirstName;
        BaseUser.LastName = LastName;
        BaseUser.Phone = Phone;
        BaseUser.PerconalCode = PerconalCode;
        BaseUser.NatinalCode = NatinalCode;
        var PathSave = BaseUser.Profile;
        if (Profile != null)
        {
            string FileExtension = Path.GetExtension(Profile.FileName);
            var NewFileName = System.String.Concat(Guid.NewGuid().ToString(), FileExtension);
            var path = $"{_env.WebRootPath}\\uploads\\{NewFileName}";
            PathSave = $"\\uploads\\{NewFileName}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await Profile.CopyToAsync(stream);
            }
        }
        BaseUser.Profile = PathSave;
        dbs.Users_tbl.Update(BaseUser);
        dbs.SaveChanges();


        //update Claim Profile
        var Identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name,BaseUser.FirstName+" "+BaseUser.LastName),
            new Claim(ClaimTypes.NameIdentifier,BaseUser.Id.ToString()),
            new Claim("Profile",BaseUser.Profile),
            new Claim("Username", BaseUser.Username)
        }, CookieAuthenticationDefaults.AuthenticationScheme);




        var princpal = new ClaimsPrincipal(Identity);

        var properties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddMonths(1),
            IsPersistent = true
        };

        HttpContext.SignInAsync(princpal, properties);
        return RedirectToAction("GetUsers");
    }

    public IActionResult UserStatus(int id){
        var check = dbs.Users_tbl.Find(id);
        check.isActive = !check.isActive;
        dbs.SaveChanges();
        return RedirectToAction("GetUsers");
    }

    [HttpGet]
    public IActionResult ProfileUser()
    {
        var UserId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var UserLogCheck = Log.AllUserLog(dbs, User);
        ViewBag.dataUserLog = UserLogCheck;

        Users Check = dbs.Users_tbl.Find(UserId);
        ViewBag.data = Check;

        return View();
    }

    public IActionResult GetCategory()
    {
        ViewBag.Users = dbs.Users_tbl.Where(x => x.Username != "admin").Include(x => x.Categories).ThenInclude(x=>x.Cat).OrderByDescending(x => x.Id).ToList();
        return View();
    }
    public IActionResult DelCategories(int id)
    {
        dbs.UserCats_tbl.Remove(dbs.UserCats_tbl.Find(id));
        dbs.SaveChanges();
        return RedirectToAction("GetCategory");
    }
    [HttpGet]
    public IActionResult ChildCategories(int id)
    {
        ViewBag.Categories = dbs.Categories_tbl.Where(x => x.ParentId == 0).OrderByDescending(x => x.Id).ToList();
        ViewBag.Id = id;
        return View("Categories");
    }
    [HttpGet]
    public IActionResult Categories(int id, int parentId)
    {
        ViewBag.Categories = dbs.Categories_tbl.Where(x => x.ParentId == parentId).OrderByDescending(x => x.Id).ToList();
        ViewBag.Id = id;
        ViewBag.parentId = parentId;
        return View();
    }
    [HttpGet]
    public IActionResult AddCategories(int id, int catId , int type)
    {
        var categoryCheck = dbs.Categories_tbl.Find(catId);
        if(categoryCheck.ParentId==0){
            foreach(var item in dbs.Categories_tbl.Where(x=>x.ParentId == categoryCheck.Id)){
                if (!dbs.UserCats_tbl.Any(x => x.CatId == item.Id && x.UserId == id))
                {
                    dbs.UserCats_tbl.Add(new UserCats
                    {
                        CatId = (int)item.Id,
                        UserId = id,
                        type = type,
                        CreateDateTime = DateTime.UtcNow
                    });
                    dbs.SaveChanges();
                }
            }
        }
        else{
            
            if (!dbs.UserCats_tbl.Any(x => x.CatId == catId && x.UserId == id))
            {
                dbs.UserCats_tbl.Add(new UserCats
                {
                    CatId = catId,
                    UserId = id,
                    CreateDateTime = DateTime.UtcNow,
                    type = type
                });
                dbs.SaveChanges();
            }
        }
        return RedirectToAction("GetCategory");
    }

    [HttpGet]
    public IActionResult view(int id){

        var cats= dbs.Categories_tbl.Find(id);
        ViewBag.Cat = cats; 
        
        List<FileCat> datas = dbs.FileCats_tbl.Where(x => x.CatId == id).Include(x => x.Files).Include(x => x.SenderUser).OrderByDescending(x => x.Id).ToList();
        ViewBag.Data = datas;

        foreach(FileCat item in datas){
            if(!item.isRead){
                item.isRead = true;
                dbs.FileCats_tbl.Update(item);
                dbs.SaveChanges();
            }
        }
        
        return View();
    }
}