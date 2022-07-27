using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> Details(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(u=>u.ID==id);


            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypelist = _unitOfWork.CoverType.GetALL();
            return View(objCoverTypelist);
        }

        
        [HttpGet]
        public IActionResult UpSert(int? id)
        {
            ProductVM productVM = new()
            {

                Product = new(),
                CategoryList = _unitOfWork.Category.GetALL().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetALL().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString()
                }),
            };

            if (id==null || id==0)
            {
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                //create product
                return View(productVM);
            }
            else
            {
                //Update product
            }
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpSert(ProductVM obj,IFormFile? file)
        {
            //if (obj.Name == obj.Name.ToString())
            //{
            //    ModelState.AddModelError("Name", "The Display Order cannot exactly match the name ");
            //}
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName= Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\producs");
                    var extenon =Path.GetExtension(file.FileName);
                    using (var fileStreams =new FileStream(Path.Combine(uploads, filename + extenon), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + fileName + extenon;
                }
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"]= "CoverType Updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var coverTypeFromDbFirst = _unitOfWork.CoverType.GetFirstOrDefault(u => u.ID == id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (coverTypeFromDbFirst == null)
            {
                return NotFound();
            }
            return View(coverTypeFromDbFirst);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.CoverType.GetFirstOrDefault(u=>u.ID ==id);
            if(obj==null)
            {
                return NotFound();
            }

            _unitOfWork.CoverType.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "CoverType Deleted successfully";
            return RedirectToAction("Index");
            
            
        }

    }
}
