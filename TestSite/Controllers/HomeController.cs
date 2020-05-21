using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSite.Data;
using TestSite.Data.FileManager;
using TestSite.Data.Repository;
using TestSite.Models;
using TestSite.Models.Comments;
using TestSite.ViewModels;

namespace TestSite.Controllers
{
    public class HomeController : Controller
    {
        private IRepository _repository;
        private IFileManager _fileManager;

        public HomeController(IRepository repository, IFileManager fileManager)
        {
            _repository = repository;
            _fileManager = fileManager;
        }

        public IActionResult Index(int pageNumber, string category, string search)
        {
            if (pageNumber < 1)
                return RedirectToAction("Index", new { pageNumber = 1, category });

            //var vm = new IndexPageViewModel
            //{
            //    PageNumber = pageNumber,
            //    Posts = string.IsNullOrEmpty(category) ?
            //    _repository.GetAllPosts(pageNumber)
            //    : _repository.GetAllPosts(category)
            //};

            var vm = _repository.GetAllPosts(pageNumber, category, search);

            return View(vm);
        }

        public IActionResult Post(int id)
        {
            var post = _repository.GetPost(id);
            
            return View(post);
        }

        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image)
        { 
            var mime = image.Substring(image.LastIndexOf('.') + 1);
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Post", new { id = vm.PostId });

            var post = _repository.GetPost(vm.PostId);

            if (vm.MainCommentId == 0)
            {
                post.Comments ??= new List<MainComment>();

                post.Comments.Add(new MainComment
                {
                    Message = vm.Message,
                    Creator = vm.Creator,
                    Created = DateTime.Now
                });

                //_repository.UpdatePost(post);
            }
            else
            {
                var subComment = new SubComment
                {
                    Message = vm.Message,
                    Creator = vm.Creator,
                    Created = DateTime.Now,
                    MainCommentId = vm.MainCommentId
                };
                _repository.AddSubComment(subComment);
            }

            await _repository.SaveChangesAsync();

            return RedirectToAction("Post", new { id = vm.PostId });
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View(new PostViewModel());
            else
            {
                var post = _repository.GetPost((int)id);
                return View(new PostViewModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    CurrentImage = post.Image,
                    Description = post.Description,
                    Tags = post.Tags,
                    Category = post.Category
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            //var post = new Post
            //{
            //    Id = vm.Id,
            //    Title = vm.Title,
            //    Body = vm.Body,
            //    //Image = await _fileManager.SaveImage(vm.Image)
            //};

            var post = new Post
            {
                Id = vm.Id,
                Title = vm.Title,
                Body = vm.Body,
                Description = vm.Description,
                Tags = vm.Tags,
                Category = vm.Category
            };

            if (vm.Image == null)
                //post.Image = $"~/content/blog/question_mark.jpg";
            post.Image = vm.CurrentImage;
            else
            {
                if (!string.IsNullOrEmpty(vm.CurrentImage))
                    _fileManager.RemoveImage(vm.CurrentImage);

                post.Image = await _fileManager.SaveImage(vm.Image);
            }

            if (post.Id > 0)
                _repository.UpdatePost(post);
            else
                _repository.AddPost(post);

            if (await _repository.SaveChangesAsync())
                return RedirectToAction("Index");
            else return View(post);
        }
    }
}
