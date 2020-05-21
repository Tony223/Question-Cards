using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSite.Data.FileManager;
using TestSite.Data.Repository;
using TestSite.Models;
using TestSite.ViewModels;

namespace TestSite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private IRepository _repository;
        private IFileManager _fileManager;

        public PanelController(IRepository repository, IFileManager fileManager)
        {
            _repository = repository;
            _fileManager = fileManager;
        }

        public IActionResult Index()
        {
            var posts = _repository.GetAllPosts();

            return View(posts);
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

        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {
            _repository.RemovePost(id);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}